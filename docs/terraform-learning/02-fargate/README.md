# Apprendre ECS/Fargate avec BankAccountServices

Ce laboratoire déploie l'API ASP.NET Core dans **ECS sur Fargate**. ECS orchestre les conteneurs ; Fargate fournit leur calcul sans serveur EC2 à administrer.

## Ce que tu vas construire

```text
Internet -> ALB:80 -> ECS service -> tâche Fargate:5000
                              |-> CloudWatch Logs
                              |-> S3 via le task role
                              |-> MySQL/RDS via la chaîne de connexion secrète
ECR -> image Docker ----------^
Secrets Manager -> DB + JWT --^
```

Le lab crée un VPC, deux sous-réseaux publics, un ALB, ECR, ECS, les rôles IAM et CloudWatch. Il **ne crée ni la base MySQL ni le bucket S3**. Les tâches ont une IP publique afin d'éviter le coût d'une NAT Gateway ; leur groupe de sécurité n'accepte toutefois le port 5000 que depuis l'ALB. En production, préfère des tâches en sous-réseaux privés avec NAT ou VPC endpoints, HTTPS/ACM et une base RDS privée.

## Les notions à retenir

- Une **task definition** est le patron versionné : image, CPU, mémoire, variables, secrets, logs et rôles.
- Une **task** est une instance en cours d'exécution de ce patron.
- Un **service** maintient le nombre voulu de tâches et orchestre les remplacements.
- L'**execution role** permet à l'agent ECS de lire ECR, écrire les logs et injecter les secrets.
- Le **task role** donne à l'application ses propres droits AWS, ici l'accès minimal au bucket S3.
- Avec `awsvpc`, chaque tâche reçoit une interface réseau ; le target group de l'ALB cible donc des **IP**.

## Prérequis

- AWS CLI authentifiée : `aws sts get-caller-identity`
- Docker Desktop démarré
- Terraform installé
- un bucket S3 existant
- une base MySQL joignable depuis ce VPC (pour RDS, autoriser le groupe de sécurité ECS sur le port 3306)

Les ressources AWS sont facturées, notamment l'ALB, Fargate, CloudWatch et Secrets Manager. Détruis le lab lorsque tu as fini.

## 1. Créer les secrets hors de Terraform

PowerShell :

```powershell
$AwsRegion = "eu-west-3"
aws secretsmanager create-secret --region $AwsRegion --name bankaccount/lab/db --secret-string 'server=HOST;port=3306;database=bankaccountdb;user=USER;password=PASSWORD'
aws secretsmanager create-secret --region $AwsRegion --name bankaccount/lab/jwt --generate-secret-string '{"PasswordLength":48,"ExcludePunctuation":true}'
```

Copie les deux ARN retournés. Cette approche évite de placer les valeurs secrètes dans Git et dans l'état Terraform. Le secret DB doit contenir la **chaîne complète**, pas un objet JSON.

## 2. Initialiser et vérifier l'infrastructure

Depuis ce dossier :

```powershell
Copy-Item terraform.tfvars.example terraform.tfvars
# Éditer terraform.tfvars avec les ARN, le bucket et la région.
terraform init
terraform fmt -check
terraform validate
terraform plan
terraform apply
```

Au premier passage, conserve `desired_count = 0` : ECR est vide, donc aucune tâche ne peut encore démarrer.

## 3. Construire et pousser l'image

Depuis la racine du dépôt :

```powershell
$AwsRegion = "eu-west-3"
$LabDir = "docs/terraform-learning/02-fargate"
$Repository = terraform -chdir=$LabDir output -raw ecr_repository_url
$Registry = $Repository.Split('/')[0]
$ImageTag = (git rev-parse --short HEAD)

aws ecr get-login-password --region $AwsRegion | docker login --username AWS --password-stdin $Registry
docker build --platform linux/amd64 --tag "${Repository}:${ImageTag}" .
docker push "${Repository}:${ImageTag}"
```

## 4. Démarrer le service Fargate

Dans `terraform.tfvars`, mets le tag réellement poussé dans `image_tag` et passe `desired_count` à `1`, puis :

```powershell
terraform apply
$Url = terraform output -raw application_url
Invoke-RestMethod "$Url/health"
```

Observe ensuite ECS > Clusters > service > Tasks, le target group de l'ALB et CloudWatch Logs. Pour suivre les événements sans console :

```powershell
aws ecs describe-services --region $AwsRegion --cluster bankaccount-lab --services bankaccount-lab-api --query 'services[0].events[0:5]'
aws logs tail /ecs/bankaccount-lab --region $AwsRegion --follow
```

## 5. Faire une nouvelle version

Après une modification, construis et pousse un **nouveau tag immuable**, remplace `image_tag`, puis lance `terraform apply`. ECS crée une nouvelle révision de task definition et effectue un rolling deployment.

## Diagnostic guidé

- `CannotPullContainerError` : tag absent dans ECR ou problème de route/rôle d'exécution.
- `ResourceInitializationError` : ARN de secret incorrect ou execution role sans droit.
- target `unhealthy` : vérifie `/health`, le port 5000 et les logs CloudWatch.
- erreur MySQL : la route, le DNS ou le security group DB n'autorise pas la tâche ECS.
- erreur S3 `AccessDenied` : bucket/préfixe incohérent avec le task role, ou bucket policy plus restrictive.

## Nettoyage

Le dépôt ECR du lab est configuré avec `force_delete = true`, donc ses images seront également supprimées :

```powershell
terraform destroy
aws secretsmanager delete-secret --region $AwsRegion --secret-id bankaccount/lab/db --recovery-window-in-days 7
aws secretsmanager delete-secret --region $AwsRegion --secret-id bankaccount/lab/jwt --recovery-window-in-days 7
```

Ne supprime pas le bucket ou la base existante : ils ne sont pas gérés par ce stack.
