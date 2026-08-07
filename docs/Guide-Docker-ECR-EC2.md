# Guide Docker, Amazon ECR et EC2 — BankAccountServices

Ce guide explique le déploiement de l'API .NET 8 depuis le code source jusqu'à son exécution sur une instance EC2.

## 1. Comprendre les éléments

| Élément | Rôle |
|---|---|
| Code source | Les fichiers C# de l'application |
| Dockerfile | La recette utilisée pour fabriquer l'image |
| Image Docker | Paquet immuable contenant l'application et son environnement d'exécution |
| Conteneur | Instance en cours d'exécution d'une image |
| Amazon ECR | Registre privé dans lequel les images sont stockées |
| Amazon EC2 | Serveur virtuel qui télécharge et exécute l'image |

ECR ne lance pas l'application. Il conserve l'image :

```text
Code + Dockerfile
        |
        | docker build
        v
Image locale bankaccount-api:1.0.0
        |
        | docker tag + docker push
        v
Amazon ECR
        |
        | docker pull
        v
Instance EC2
        |
        | docker run
        v
Conteneur ASP.NET Core
```

## 2. Fichiers utilisés dans ce projet

### `Dockerfile`

Le `Dockerfile` comporte deux étapes afin de produire une image finale plus petite.

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY BankAccountServices.csproj ./
RUN dotnet restore BankAccountServices.csproj

COPY . ./
RUN dotnet publish BankAccountServices.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

COPY --from=build --chown=app:app /app/publish ./
RUN mkdir -p /app/Logs && chown -R app:app /app/Logs

USER app
ENTRYPOINT ["dotnet", "BankAccountServices.dll"]
```

Explication :

- `FROM ...sdk:8.0 AS build` : utilise le SDK .NET pour compiler.
- `WORKDIR /src` : définit le dossier courant dans l'image.
- `COPY ...csproj` puis `dotnet restore` : restaure les packages NuGet. Cette séparation améliore le cache Docker.
- `COPY . ./` : copie le code source autorisé dans l'image de construction.
- `dotnet publish -c Release` : compile et publie l'API en mode production.
- `FROM ...aspnet:8.0 AS runtime` : démarre une nouvelle image ne contenant que le runtime ASP.NET.
- `ASPNETCORE_URLS=http://+:5000` : l'API écoute sur le port 5000 du conteneur.
- `EXPOSE 5000` : documente le port utilisé ; cela ne publie pas le port sur EC2.
- `COPY --from=build` : récupère seulement le résultat compilé.
- `USER app` : exécute l'API avec un utilisateur non-root.
- `ENTRYPOINT` : commande lancée au démarrage du conteneur.

### `.dockerignore`

Ce fichier exclut du contexte de construction les fichiers inutiles comme `bin/`, `obj/`, `.git/`, `Logs/` et `.vs/`. Le build est plus rapide et évite d'inclure accidentellement des fichiers locaux.

### `BankAccountServices.csproj`

Il décrit la cible `.NET 8` et les dépendances NuGet. Il est utilisé par `dotnet restore` et `dotnet publish` pendant le build.

### `appsettings.json` et variables d'environnement

L'image contient la configuration non secrète. Les valeurs de production et les secrets doivent être injectés au lancement. ASP.NET Core transforme `__` en `:` :

```text
ConnectionStrings__DBConnection -> ConnectionStrings:DBConnection
Jwt__Key                       -> Jwt:Key
S3__BucketName                 -> S3:BucketName
```

Ne placez pas de mot de passe RDS, clé JWT ou clé AWS dans le `Dockerfile` ou dans l'image.

## 3. Pré-requis sur le poste de développement

- Docker Desktop démarré ;
- AWS CLI installé ;
- accès à un compte AWS ;
- région ECR choisie, ici `us-east-1` ;
- architecture de l'EC2 connue, ici Linux AMD64.

Vérification dans PowerShell :

```powershell
docker version
docker info
aws --version
aws sts get-caller-identity
```

`get-caller-identity` affiche le compte et l'identité AWS actuellement utilisés.

## 4. Construire l'image

Depuis la racine du projet :

```powershell
cd C:\Users\ihebb\source\repos\BankAccountServices
docker build --platform linux/amd64 -t bankaccount-api:1.0.0 .
```

Signification :

- `docker build` : fabrique une image à partir du `Dockerfile` ;
- `--platform linux/amd64` : produit une image compatible avec une EC2 x86_64 ;
- `-t bankaccount-api:1.0.0` : définit le nom et la version locale ;
- `.` : utilise le dossier courant comme contexte de build.

Vérifier l'image :

```powershell
docker image ls bankaccount-api
docker image inspect bankaccount-api:1.0.0
```

## 5. Tester l'image localement

Pour un démarrage simple :

```powershell
docker run -d --name bankaccount-api-test -p 5000:5000 bankaccount-api:1.0.0
```

- `-d` : exécute en arrière-plan ;
- `--name` : attribue un nom au conteneur ;
- `-p 5000:5000` : relie le port 5000 du PC au port 5000 du conteneur ;
- le dernier argument est l'image utilisée.

Contrôler le résultat :

```powershell
docker ps
docker logs --tail 100 bankaccount-api-test
Invoke-RestMethod http://localhost:5000/health
docker stop bankaccount-api-test
docker rm bankaccount-api-test
```

Si les routes utilisent MySQL, JWT ou S3, fournir aussi les variables correspondantes avec `--env-file`.

## 6. Créer le repository ECR

Cette opération est généralement réalisée une seule fois :

```powershell
aws ecr create-repository `
  --repository-name bankaccount-ecr `
  --region us-east-1 `
  --image-tag-mutability IMMUTABLE `
  --image-scanning-configuration scanOnPush=true
```

- `IMMUTABLE` interdit d'écraser un tag déjà publié ;
- `scanOnPush=true` lance une analyse de vulnérabilités après le push.

Récupérer l'adresse du repository :

```powershell
aws ecr describe-repositories `
  --repository-names bankaccount-ecr `
  --region us-east-1 `
  --query "repositories[0].repositoryUri" `
  --output text
```

Elle ressemble à :

```text
652197205619.dkr.ecr.us-east-1.amazonaws.com/bankaccount-ecr
```

Remplacez l'identifiant de compte des commandes suivantes par le vôtre.

## 7. Autorisations IAM pour le push

L'identité utilisée sur le poste ou dans la CI/CD doit être autorisée à envoyer des images. Elle a notamment besoin de :

```text
ecr:GetAuthorizationToken
ecr:BatchCheckLayerAvailability
ecr:InitiateLayerUpload
ecr:UploadLayerPart
ecr:CompleteLayerUpload
ecr:PutImage
```

Pour un poste humain, utilisez de préférence AWS IAM Identity Center/SSO. Pour GitHub Actions, utilisez un rôle assumé par OIDC. Évitez les access keys permanentes lorsque c'est possible.

## 8. Se connecter à ECR, taguer et pousser

Connexion de Docker au registre :

```powershell
aws ecr get-login-password --region us-east-1 |
docker login `
  --username AWS `
  --password-stdin `
  652197205619.dkr.ecr.us-east-1.amazonaws.com
```

AWS fournit un jeton temporaire à `docker login`. `--password-stdin` évite de placer ce jeton directement dans la commande.

Ajouter à l'image locale un tag contenant l'adresse ECR :

```powershell
docker tag `
  bankaccount-api:1.0.0 `
  652197205619.dkr.ecr.us-east-1.amazonaws.com/bankaccount-ecr:1.0.0
```

`docker tag` ne copie et ne reconstruit pas l'image : il lui ajoute un autre nom.

Envoyer l'image :

```powershell
docker push 652197205619.dkr.ecr.us-east-1.amazonaws.com/bankaccount-ecr:1.0.0
```

Vérifier dans ECR :

```powershell
aws ecr describe-images `
  --repository-name bankaccount-ecr `
  --region us-east-1
```

## 9. Rôle IAM de l'instance EC2

L'EC2 doit avoir un rôle IAM avec la policy gérée :

```text
AmazonEC2ContainerRegistryReadOnly
```

La relation de confiance du rôle autorise le service `ec2.amazonaws.com` à l'assumer. Attachez ensuite ce rôle à l'instance :

```text
EC2 > Instances > sélectionner l'instance
Actions > Security > Modify IAM role
```

Le rôle fournit automatiquement des identifiants temporaires à l'EC2. Ne lancez pas `aws configure` sur le serveur de production et n'y copiez pas des clés permanentes.

Si l'API accède aussi à S3, ajoutez au même rôle une policy limitée au bucket et aux actions réellement nécessaires. Les permissions ECR servent à télécharger l'image ; les permissions S3 servent à l'application pendant son exécution.

## 10. Préparer EC2

Sur une EC2 Ubuntu :

```bash
sudo apt update
sudo apt install -y docker.io awscli
sudo systemctl enable --now docker
sudo usermod -aG docker ubuntu
```

Après `usermod`, déconnectez-vous puis reconnectez-vous. Vérifiez ensuite :

```bash
docker version
aws sts get-caller-identity
```

L'ARN retourné doit contenir le nom du rôle attaché à EC2.

Le Security Group de l'EC2 doit autoriser au minimum :

- SSH 22 depuis votre adresse IP d'administration ;
- HTTP 80 depuis les clients attendus ;
- HTTPS 443 seulement si TLS est réellement configuré.

## 11. Pull de l'image sur EC2

```bash
aws ecr get-login-password --region us-east-1 |
docker login \
  --username AWS \
  --password-stdin \
  652197205619.dkr.ecr.us-east-1.amazonaws.com

docker pull \
  652197205619.dkr.ecr.us-east-1.amazonaws.com/bankaccount-ecr:1.0.0

docker image ls
```

`docker pull` télécharge les couches absentes. Les couches déjà présentes sont réutilisées.

## 12. Configuration de production

Créer `/etc/bankaccount.env` sur EC2 :

```text
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:5000
ConnectionStrings__DBConnection=Server=ENDPOINT_RDS;Port=3306;Database=bankaccountdb;User=bankaccount_app;Password=SECRET;SslMode=Required;
Jwt__Key=CLE_JWT_LONGUE
Jwt__Issuer=BankAccountServices
Jwt__Audience=BankAccountClient
S3__BucketName=bucketbankaccount
S3__Region=us-east-1
S3__Prefix=uploads
S3__PreSignedUrlExpirationMinutes=15
```

Protéger le fichier :

```bash
sudo chmod 600 /etc/bankaccount.env
```

Pour un environnement plus mature, utilisez AWS Secrets Manager ou Systems Manager Parameter Store au lieu d'un fichier contenant les secrets.

## 13. Exécuter le conteneur sur EC2

```bash
docker run -d \
  --name bankaccount-api \
  --restart unless-stopped \
  -p 80:5000 \
  --env-file /etc/bankaccount.env \
  652197205619.dkr.ecr.us-east-1.amazonaws.com/bankaccount-ecr:1.0.0
```

- `--restart unless-stopped` : redémarre le conteneur après un redémarrage de l'EC2, sauf arrêt manuel ;
- `-p 80:5000` : `http://IP_EC2:80` est transmis au port 5000 du conteneur ;
- `--env-file` : injecte la configuration sans la placer dans l'image.

Vérifier :

```bash
docker ps
docker port bankaccount-api
docker logs --tail 100 bankaccount-api
curl http://127.0.0.1/health
```

Le chemin de la requête est :

```text
Client -> Security Group EC2 -> port 80 EC2 -> port 5000 conteneur -> API ASP.NET
```

Le Security Group RDS doit autoriser le port MySQL 3306 avec comme source le Security Group de l'EC2, pas tout Internet.

## 14. Déployer une nouvelle version

Sur le poste de développement :

```powershell
docker build --platform linux/amd64 -t bankaccount-api:1.1.0 .
docker tag bankaccount-api:1.1.0 652197205619.dkr.ecr.us-east-1.amazonaws.com/bankaccount-ecr:1.1.0
docker push 652197205619.dkr.ecr.us-east-1.amazonaws.com/bankaccount-ecr:1.1.0
```

Sur EC2 :

```bash
docker pull 652197205619.dkr.ecr.us-east-1.amazonaws.com/bankaccount-ecr:1.1.0
docker stop bankaccount-api
docker rm bankaccount-api
docker run -d \
  --name bankaccount-api \
  --restart unless-stopped \
  -p 80:5000 \
  --env-file /etc/bankaccount.env \
  652197205619.dkr.ecr.us-east-1.amazonaws.com/bankaccount-ecr:1.1.0
docker logs --tail 100 bankaccount-api
curl http://127.0.0.1/health
```

Conservez l'ancien tag dans ECR pour pouvoir revenir rapidement à la version précédente. Utilisez un numéro de version ou le SHA Git, pas seulement `latest`.

## 15. Commandes Docker utiles

| Commande | Utilité |
|---|---|
| `docker build -t nom:tag .` | Construire une image |
| `docker image ls` | Lister les images locales |
| `docker ps` | Lister les conteneurs actifs |
| `docker ps -a` | Lister tous les conteneurs |
| `docker logs -f nom` | Suivre les journaux |
| `docker inspect nom` | Afficher la configuration détaillée |
| `docker stop nom` | Arrêter proprement un conteneur |
| `docker start nom` | Redémarrer un conteneur arrêté |
| `docker rm nom` | Supprimer un conteneur arrêté |
| `docker image rm image:tag` | Supprimer une image locale inutilisée |
| `docker exec -it nom sh` | Ouvrir un shell dans un conteneur actif |

Supprimer un conteneur ne supprime pas l'image ECR. Supprimer une image locale ne supprime pas non plus l'image stockée dans ECR.

## 16. Erreurs fréquentes

### `no basic auth credentials`

Docker n'est plus connecté à ECR, la région est incorrecte ou le registre n'est pas le bon. Relancez `get-login-password | docker login`.

### `AccessDeniedException`

L'identité IAM n'a pas l'action requise, ou la policy vise le mauvais repository. Vérifiez `aws sts get-caller-identity` et les policies attachées.

### `Unable to locate credentials` sur EC2

Le rôle IAM n'est pas attaché à l'instance ou sa relation de confiance EC2 est incorrecte.

### `exec format error`

L'architecture de l'image ne correspond pas à celle de l'EC2. Construisez avec `linux/amd64` pour une EC2 x86_64 ou `linux/arm64` pour une EC2 Graviton.

### Le conteneur s'arrête immédiatement

```bash
docker ps -a
docker logs bankaccount-api
```

La cause est souvent une variable manquante, une connexion RDS impossible ou une erreur au démarrage de l'API.

### L'API fonctionne dans EC2 mais pas depuis Internet

Vérifiez le mapping `80:5000`, le Security Group EC2, l'adresse IP utilisée et un éventuel pare-feu. `EXPOSE 5000` seul ne publie aucun port.

## 17. Résumé à mémoriser

```text
docker build = créer l'image
docker tag   = donner à l'image son adresse ECR
docker push  = envoyer l'image vers ECR
docker pull  = télécharger l'image depuis ECR
docker run   = créer et démarrer un conteneur
```

Pour ce projet :

```text
Développeur/CI avec droit ECR Push
    -> construit et pousse l'image
ECR
    -> stocke les versions
EC2 avec rôle ECR ReadOnly
    -> télécharge l'image
Docker sur EC2
    -> lance l'API avec la configuration de production
```
