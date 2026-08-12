# 🚀 Pipeline CI/CD - BankAccount Services

Ce document explique le pipeline CI/CD automatisé pour construire et déployer l'application BankAccount sur Kubernetes.

## 📋 Architecture

```
Push vers GitHub
    ↓
GitHub Actions
    ├─ Checkout code
    ├─ Build .NET
    ├─ Build Docker Image
    └─ Deploy to Kubernetes (Docker Desktop)
```

## 🔧 Configuration Requise

### Prérequis Locaux
- ✅ Docker Desktop (avec Kubernetes activé)
- ✅ .NET 8.0 SDK
- ✅ kubectl
- ✅ Git

### Vérification de l'installation

```powershell
# PowerShell
docker version
dotnet --version
kubectl version --client
git --version
```

```bash
# Bash/Linux/Mac
docker version
dotnet --version
kubectl version --client
git --version
```

## 🚀 Déploiement Local

### Option 1: Script PowerShell (Recommandé pour Windows)

```powershell
# Déploiement standard
.\deploy-local.ps1

# Avec nettoyage du déploiement précédent
.\deploy-local.ps1 -Clean

# Avec affichage des logs en direct
.\deploy-local.ps1 -Logs

# Combiné
.\deploy-local.ps1 -Clean -Logs
```

### Option 2: Script Bash (Linux/Mac)

```bash
# Rendre le script exécutable
chmod +x deploy-local.sh

# Exécuter le déploiement
./deploy-local.sh
```

### Option 3: Commandes Manuelles

```bash
# 1. Compiler le projet
dotnet build -c Release

# 2. Construire l'image Docker
docker build -t bankaccount-kube:v1 .

# 3. Vérifier que l'image est créée
docker images | grep bankaccount-kube

# 4. Déployer sur Kubernetes
kubectl apply -f bankaccount-deployment.yaml

# 5. Vérifier le statut
kubectl get deployments
kubectl get pods -l app=bankaccount
kubectl get services
```

## 📊 Vérification du Déploiement

### Voir les pods en cours d'exécution
```bash
kubectl get pods -l app=bankaccount
```

### Voir les logs de l'application
```bash
kubectl logs -l app=bankaccount -f
```

### Accéder à l'application
- **URL**: http://localhost:30080
- **Port**: 30080 (configuré dans le service Kubernetes)

### Décrire le déploiement
```bash
kubectl describe deployment bankaccount-kube-deployment
```

### Vérifier les erreurs
```bash
kubectl get events --sort-by='.lastTimestamp'
```

## 🧹 Nettoyage

### Supprimer le déploiement
```bash
kubectl delete -f bankaccount-deployment.yaml
```

### Supprimer l'image Docker locale
```bash
docker rmi bankaccount-kube:v1
```

### Nettoyage complet (PowerShell)
```powershell
.\deploy-local.ps1 -Clean
```

## 🔄 Pipeline GitHub Actions

Le workflow `.github/workflows/ci-cd.yml` s'exécute automatiquement sur:
- Push vers `main`
- Push vers `develop`

### Étapes du Pipeline

1. **Checkout** - Récupère le code
2. **Setup .NET** - Configure .NET 8.0
3. **Restore** - Restaure les packages NuGet
4. **Build** - Compile le projet
5. **Test** - Exécute les tests (optionnel)
6. **Docker Build** - Construit l'image Docker
7. **Save Image** - Sauvegarde l'image en artifact
8. **Deploy** - Déploie sur Kubernetes (Docker Desktop)
9. **Verify** - Vérifie le statut du déploiement
10. **Logs** - Affiche les informations de déploiement

## 📦 Artifacts GitHub

L'image Docker est sauvegardée comme artifact pendant 1 jour :
- **Nom**: `docker-image`
- **Contenu**: `bankaccount-image.tar`
- **Rétention**: 1 jour

### Utiliser l'artifact

```bash
# Télécharger depuis GitHub Actions
# 1. Allez sur le run du workflow
# 2. Téléchargez l'artifact
# 3. Charger l'image localement
docker load -i docker-image/bankaccount-image.tar
```

## ⚙️ Configuration Kubernetes

### Fichier de déploiement: `bankaccount-deployment.yaml`

**Ressources déployées:**
- Deployment (2 replicas)
- Service (NodePort 30080)

**Env vars utilisées:**
- `ConnectionStrings__DBConnection` - Récupérée du secret `bankaccount-db`

### Créer le secret pour la base de données

```bash
kubectl create secret generic bankaccount-db \
  --from-literal=connection-string='YOUR_CONNECTION_STRING'
```

## 🐛 Dépannage

### L'image Docker n'est pas trouvée par Kubernetes

**Cause**: L'image n'a pas été chargée dans Docker Desktop

**Solution**:
```bash
# Vérifier les images disponibles
docker images | grep bankaccount-kube

# Reconstruire si nécessaire
docker build -t bankaccount-kube:v1 .
```

### Les pods restent en "Pending"

```bash
# Vérifier les événements
kubectl describe pod <POD_NAME>

# Voir l'état de tous les ressources
kubectl get all
```

### Erreur de connexion à la base de données

```bash
# Vérifier que le secret existe
kubectl get secrets
kubectl describe secret bankaccount-db

# Tester la connexion depuis le pod
kubectl exec -it <POD_NAME> -- dotnet run
```

### Erreur 30080 non accessible

```bash
# Vérifier que le service est créé
kubectl get services

# Tester la connectivité
kubectl port-forward svc/bankaccount-service 8080:8080
# Accès via http://localhost:8080
```

## 📝 Variables d'Environnement

Les variables d'environnement pour l'application sont définies dans le deployment Kubernetes.

### Configuration actuelle:
```yaml
env:
  - name: ConnectionStrings__DBConnection
    valueFrom:
      secretKeyRef:
        name: bankaccount-db
        key: connection-string
```

Pour ajouter d'autres variables:
1. Éditez `bankaccount-deployment.yaml`
2. Ajoutez les nouvelles variables dans la section `env`
3. Déployez: `kubectl apply -f bankaccount-deployment.yaml`

## 🔐 Sécurité

### Bonnes pratiques

✅ **À faire:**
- Utiliser des secrets Kubernetes pour les données sensibles
- Ne pas versionner les secrets en Git
- Utiliser `imagePullPolicy: IfNotPresent` pour les images locales

❌ **À éviter:**
- Stocker les mots de passe en clair dans les fichiers YAML
- Pousser les images sur un registre public sans sécurité
- Utiliser `imagePullPolicy: Always` en développement local

## 📚 Ressources Utiles

- [Kubernetes Documentation](https://kubernetes.io/docs/)
- [Docker Documentation](https://docs.docker.com/)
- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [.NET Documentation](https://learn.microsoft.com/en-us/dotnet/)

## 💡 Prochaines Étapes

1. **Enregistre Docker Hub** - Si tu veux pousser les images vers un registre
2. **Ajoute des tests** - Améliore la couverture de tests
3. **Configuration Pro** - Utilise un registre privé (ECR, Azure ACR, etc.)
4. **Monitoring** - Intégre Prometheus + Grafana
5. **Logs centralisés** - Utilise ELK ou CloudWatch

---

**Besoin d'aide?** Consulte les logs ou execute le script en mode verbose.
