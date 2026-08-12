# 🏷️ Versioning Automatique des Images Docker

## 📋 Vue d'ensemble

Le pipeline CI/CD utilise automatiquement le **SHA du commit GitHub** comme version de l'image Docker, ce qui assure :

- ✅ Traçabilité complète (chaque version = commit spécifique)
- ✅ Reproductibilité (rollback facile)
- ✅ Pas de conflit de versions
- ✅ Tag `latest` pour la dernière version

## 🔄 Flux de Versioning

```
Push vers GitHub
    ↓
SHA du commit: abc1234567890def...
    ↓
Image Docker tags:
  - bankaccount-kube:abc1234 (SHA court - 7 caractères)
  - bankaccount-kube:latest
  - bankaccount-kube:v1 (optionnel)
    ↓
Deployment YAML mis à jour dynamiquement
    ↓
kubectl apply -f bankaccount-deployment-updated.yaml
```

## 📦 Tags de l'Image Docker

Chaque build Docker reçoit **3 tags**:

| Tag | Description |
|-----|-------------|
| `bankaccount-kube:abc1234` | SHA court du commit (7 caractères) - **VERSION PRINCIPALE** |
| `bankaccount-kube:latest` | Dernière version |
| `bankaccount-kube:v1` | Version sémantique (optionnel) |

### Exemple

```bash
# Si le commit SHA est: f8a9c1e2d3b4567890abcdef123456789

docker images
# REPOSITORY              TAG        IMAGE ID
# bankaccount-kube        f8a9c1e    f8a9c1e2d3b4
# bankaccount-kube        latest     f8a9c1e2d3b4
# bankaccount-kube        v1         f8a9c1e2d3b4
```

## 🔍 Voir quelle version est déployée

```bash
# Voir l'image utilisée dans le déploiement
kubectl get deployment bankaccount-kube-deployment -o jsonpath='{.spec.template.spec.containers[0].image}'

# Affichage:
# bankaccount-kube:f8a9c1e

# Voir l'historique des déploiements
kubectl rollout history deployment/bankaccount-kube-deployment

# Voir les images disponibles localement
docker images | grep bankaccount-kube
```

## 🔄 Rollback Facile

### Option 1: Rollback via Kubernetes

```bash
# Voir l'historique
kubectl rollout history deployment/bankaccount-kube-deployment

# Revision 1: bankaccount-kube:abc1234
# Revision 2: bankaccount-kube:def5678
# Revision 3: bankaccount-kube:ghi9012

# Revenir à la revision précédente
kubectl rollout undo deployment/bankaccount-kube-deployment

# Revenir à une revision spécifique
kubectl rollout undo deployment/bankaccount-kube-deployment --to-revision=1
```

### Option 2: Rollback via Git + Pipeline

```bash
# Revenir au commit précédent
git revert <commit-sha>
git push

# GitHub Actions exécute le pipeline
# → Nouvelle image Docker avec le nouveau SHA
# → Déploiement automatique
```

## 📊 Exemple Complet du Pipeline

### Étape 5: Build Docker
```yaml
- name: Build Docker Image
  run: |
    IMAGE_TAG=${{ github.sha }}
    SHORT_SHA=${IMAGE_TAG:0:7}
    
    docker build -t bankaccount-kube:$SHORT_SHA \
                 -t bankaccount-kube:latest \
                 -t bankaccount-kube:v1 .
    
    echo "IMAGE_TAG=$SHORT_SHA" >> $GITHUB_ENV
```

**Résultat:**
```
IMAGE_TAG = abc1234  (7 premiers caractères du SHA)
```

### Étape 8: Update Deployment YAML
```yaml
- name: Update Deployment Image and Deploy
  run: |
    cp bankaccount-deployment.yaml bankaccount-deployment-updated.yaml
    
    # Remplacer "image: bankaccount-kube:*" par la nouvelle version
    sed -i "s|image: bankaccount-kube:.*|image: bankaccount-kube:abc1234|g" \
        bankaccount-deployment-updated.yaml
    
    kubectl apply -f bankaccount-deployment-updated.yaml
```

**Fichier avant:**
```yaml
image: bankaccount-kube:v1
```

**Fichier après (auto-généré):**
```yaml
image: bankaccount-kube:abc1234
```

## 🎯 Avantages du Versioning par SHA

### ✅ Traçabilité
Chaque version de l'image = commit exact dans Git

```bash
# Trouver le commit exact d'une version déployée
git log --oneline | grep abc1234
# Output: abc1234 feat: Add new feature
```

### ✅ Reproducibilité
Déployer exactement la même build

```bash
# Redéployer une ancienne version
docker tag bankaccount-kube:abc1234 bankaccount-kube:latest
kubectl set image deployment/bankaccount-kube-deployment \
  bankaccount=bankaccount-kube:latest --record
```

### ✅ Pas de Conflits
Chaque commit = version unique

```bash
# Impossible d'avoir des conflits de versions
docker build -t bankaccount-kube:f8a9c1e2 .
# Succès ✓
```

### ✅ CI/CD Automatisé
Pas de modification manuelle du YAML

```bash
# Avant: Modifier manuellement le YAML
# image: bankaccount-kube:v2

# Après: Automatique via le pipeline
# image: bankaccount-kube:abc1234
```

## 📝 Fichiers Impliqués

### `.github/workflows/ci-cd.yml`
- Crée les tags dynamiques basés sur le SHA
- Sauvegarde le SHA court en variable d'environnement `IMAGE_TAG`
- Remplace le tag dans le YAML avant `kubectl apply`

### `bankaccount-deployment.yaml`
- Tag initial: `image: bankaccount-kube:v1`
- Remplacé automatiquement par: `image: bankaccount-kube:<SHA>`

## 🔧 Cas d'Usage

### Scenario 1: Déploiement Normal
```
Push → Pipeline → Build (sha:abc1234) → Deploy → En prod ✓
```

### Scenario 2: Hotfix Urgent
```
Emergency commit → Push → Pipeline → Build (sha:def5678) → Deploy → Prod (new version)
```

### Scenario 3: Rollback Nécessaire
```
kubectl rollout undo deployment/bankaccount-kube-deployment
→ Revenir à la version précédente (sha:abc1234)
```

### Scenario 4: Vérification de Version
```
kubectl describe pod <pod-name>
→ Voir image: bankaccount-kube:abc1234
→ Trouver le commit exact dans Git
```

## 🚀 Optimisations Possibles

### 1. Tags Sémantiques
```bash
# Utiliser des tags comme: v1.2.3
docker build -t bankaccount-kube:v1.2.3 .
```

### 2. Tags par Branche
```bash
# main branch → v1.2.3
# develop branch → v1.2.3-dev
```

### 3. Tags par Date
```bash
# Format: 20260812-abc1234
docker build -t bankaccount-kube:$(date +%Y%m%d)-abc1234 .
```

### 4. Multiple Registries
```bash
# ECR: 123456789.dkr.ecr.us-east-1.amazonaws.com/bankaccount:abc1234
# Docker Hub: myregistry/bankaccount:abc1234
```

## 📞 Commandes Utiles

```bash
# Voir le SHA actuel
git rev-parse --short HEAD

# Voir la version déployée
kubectl get deployment bankaccount-kube-deployment -o jsonpath='{.spec.template.spec.containers[0].image}'

# Voir tout l'historique
kubectl rollout history deployment/bankaccount-kube-deployment

# Voir les détails d'une revision
kubectl rollout history deployment/bankaccount-kube-deployment --revision=1

# Rollback à la version précédente
kubectl rollout undo deployment/bankaccount-kube-deployment
```

---

**Besoin de modifier le versioning?** Consultez `.github/workflows/ci-cd.yml`

**Dernière mise à jour**: 2026-08-12
