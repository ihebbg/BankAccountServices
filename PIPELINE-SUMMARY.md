# 📦 Pipeline CI/CD - Récapitulatif des Fichiers Créés

## 📝 Fichiers Créés

### 1. **Pipeline GitHub Actions**
   - **Fichier**: `.github/workflows/ci-cd.yml`
   - **Description**: Workflow automatisé qui s'exécute à chaque push
   - **Branchages**: `main` et `develop`
   - **Actions**: Build .NET → Build Docker → Deploy Kubernetes

### 2. **Scripts de Déploiement Local**
   
   #### PowerShell (Windows) - RECOMMANDÉ
   - **Fichier**: `deploy-local.ps1`
   - **Utilisation**:
     ```powershell
     .\deploy-local.ps1                    # Déploiement standard
     .\deploy-local.ps1 -Clean             # Nettoie et redéploie
     .\deploy-local.ps1 -Logs              # Affiche les logs
     .\deploy-local.ps1 -Clean -Logs       # Combiné
     ```
   
   #### Bash (Linux/Mac)
   - **Fichier**: `deploy-local.sh`
   - **Utilisation**:
     ```bash
     chmod +x deploy-local.sh
     ./deploy-local.sh
     ```

### 3. **Makefile PowerShell**
   - **Fichier**: `Makefile.ps1`
   - **Description**: Centralise les commandes courantes
   - **Utilisation**:
     ```powershell
     .\Makefile.ps1 help              # Affiche l'aide
     .\Makefile.ps1 build             # Compile
     .\Makefile.ps1 docker-build      # Construit Docker
     .\Makefile.ps1 deploy            # Déploie
     .\Makefile.ps1 deploy-clean      # Redéploie
     .\Makefile.ps1 logs              # Voir les logs
     .\Makefile.ps1 status            # Statut
     .\Makefile.ps1 all               # Build + Docker + Deploy
     ```

### 4. **Configuration des Secrets**
   
   #### PowerShell
   - **Fichier**: `create-secrets.ps1`
   - **Description**: Crée les secrets Kubernetes pour la base de données
   - **Utilisation**:
     ```powershell
     .\create-secrets.ps1
     # Ou avec la chaîne de connexion en paramètre
     .\create-secrets.ps1 -ConnectionString "Server=localhost;Database=BankAccount;..."
     ```
   
   #### Bash
   - **Fichier**: `create-secrets.sh`
   - **Description**: Version Bash du script de secrets
   - **Utilisation**:
     ```bash
     chmod +x create-secrets.sh
     ./create-secrets.sh
     ```

### 5. **Documentation**
   - **Fichier**: `CI-CD-README.md`
   - **Description**: Documentation complète du pipeline
   - **Contient**:
     - Architecture du pipeline
     - Configuration requise
     - Instructions de déploiement
     - Commandes de vérification
     - Guide de dépannage
     - Bonnes pratiques de sécurité

## 🚀 Démarrage Rapide

### Étape 1: Initialiser les secrets (Une seule fois)
```powershell
.\create-secrets.ps1
# Entrez votre chaîne de connexion SQL Server
```

### Étape 2: Déployer l'application
```powershell
# Option A: Utiliser le script complet
.\deploy-local.ps1

# Option B: Utiliser le Makefile
.\Makefile.ps1 all

# Option C: Commandes manuelles
dotnet build -c Release
docker build -t bankaccount-kube:v1 .
kubectl apply -f bankaccount-deployment.yaml
```

### Étape 3: Vérifier le déploiement
```powershell
.\Makefile.ps1 status
```

### Étape 4: Accéder à l'application
- **URL**: http://localhost:30080
- **Logs**: `.\Makefile.ps1 logs`
- **Supprimer**: `.\Makefile.ps1 clean`

## 📊 Flux de Déploiement

```
LOCAL DEVELOPMENT
├─ .\deploy-local.ps1          (Script complet)
├─ .\Makefile.ps1 all          (Commandes modulables)
└─ Commandes manuelles

CI/CD AUTOMATISÉ (GitHub)
├─ Push vers main/develop
├─ GitHub Actions exécute ci-cd.yml
├─ Build Docker
├─ Sauvegarde image en artifact
└─ Déploie sur Kubernetes (Docker Desktop)
```

## ⚙️ Fichiers Modifiés

- **`bankaccount-deployment.yaml`**: `imagePullPolicy` changé de `Never` à `IfNotPresent`
- **`.github/workflows/ci-cd.yml`**: Pipeline créé (nouveau fichier)

## 🔒 Sécurité - À Faire Avant la Production

- [ ] Générer des secrets robustes
- [ ] Utiliser un registre Docker privé (ECR, ACR, etc.)
- [ ] Configurer les ImagePullSecrets dans Kubernetes
- [ ] Ajouter les limites de ressources (CPU/Mémoire)
- [ ] Implémenter les Health Checks (Liveness/Readiness probes)
- [ ] Configurer les NetworkPolicies
- [ ] Utiliser les Ingress au lieu de NodePort

## 📋 Commandes Utiles

### Déploiement
```powershell
# Build et déploie tout
.\deploy-local.ps1

# Redéploie (nettoie en premier)
.\deploy-local.ps1 -Clean

# Voir les logs
.\Makefile.ps1 logs

# Vérifier l'état
.\Makefile.ps1 status
```

### Kubernetes
```bash
# Lister les ressources
kubectl get all
kubectl get pods
kubectl get deployments
kubectl get services

# Décrire une ressource
kubectl describe pod <POD_NAME>
kubectl describe deployment bankaccount-kube-deployment

# Logs
kubectl logs -l app=bankaccount -f

# Port forward
kubectl port-forward svc/bankaccount-service 8080:8080
```

### Docker
```bash
# Lister les images
docker images | grep bankaccount-kube

# Inspecter l'image
docker inspect bankaccount-kube:v1

# Supprimer l'image
docker rmi bankaccount-kube:v1
```

## 🐛 Dépannage Rapide

| Problème | Solution |
|----------|----------|
| Image non trouvée | `docker build -t bankaccount-kube:v1 .` |
| Pod en Pending | `kubectl describe pod <NAME>` |
| Connection BD échouée | `.\create-secrets.ps1` (vérifier la chaîne) |
| Port 30080 non accessible | `kubectl port-forward svc/bankaccount-service 8080:8080` |
| Erreur Docker Desktop | Activer Kubernetes dans Docker Desktop |

## ✅ Checklist Avant de Commencer

- [ ] Docker Desktop installé et Kubernetes activé
- [ ] .NET 8.0 SDK installé
- [ ] kubectl configuré et connecté à Docker Desktop
- [ ] Git configuré
- [ ] Clône le repo et accède au dossier racine

## 📚 Prochaines Améliorations

1. **Déploiement en Production**: Utiliser ECR + ECS/EKS
2. **Tests Automatisés**: Ajouter les étapes de test dans le CI/CD
3. **Monitoring**: Intégrer Prometheus + Grafana
4. **Logs Centralisés**: ELK Stack ou CloudWatch
5. **Security Scanning**: Trivy pour scanner les images Docker
6. **ArgoCD**: GitOps pour le déploiement

---

## 📞 Support

Pour plus d'informations, consultez `CI-CD-README.md` ou les logs des workflows.

**Dernière mise à jour**: 2026-08-12
