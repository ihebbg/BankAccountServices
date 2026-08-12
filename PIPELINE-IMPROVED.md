# 🚀 Pipeline CI/CD Amélioré - Déploiement Automatique

## 📝 Nouveautés

Le pipeline GitHub Actions a été amélioré pour :

1. ✅ **Charger l'image Docker** en local
2. ✅ **Mettre à jour le YAML** automatiquement
3. ✅ **Détecteur le cluster Kubernetes**
4. ✅ **Déployer automatiquement** si Kubernetes est disponible
5. ✅ **Fournir des instructions** si Kubernetes n'est pas disponible

---

## 📊 Nouveau Flux du Pipeline

```
Push vers GitHub
    ↓
1. Checkout Code
2. Setup .NET 8.0
3. Restore packages
4. Build .NET
5. Build Docker Image (avec SHA)
6. Save Docker Image (.tar)
7. Upload Artifact
    ↓
8. Load Docker Image (optionnel)
9. Update Deployment YAML (image: SHA)
10. Configure Kubernetes Access
11. Deploy to Kubernetes
    ├─ Si Kubernetes disponible → Déploie
    └─ Sinon → Instructions pour usage local
12. Check Status & Logs
```

---

## 🎯 Nouvelles Étapes

### **Étape 8️⃣ : Load Docker Image**

```yaml
- name: Load Docker Image for Local Testing
  run: |
    docker load -i bankaccount-image.tar
    docker images | grep bankaccount-kube || true
  continue-on-error: true
```

**Qu'il fait:**
- Charge l'image Docker en format tar
- La rend disponible localement
- Affiche l'image si elle existe

---

### **Étape 9️⃣ : Update Deployment Image**

```yaml
- name: Update Deployment Image
  run: |
    cp bankaccount-deployment.yaml bankaccount-deployment-updated.yaml
    sed -i "s|image: bankaccount-kube:.*|image: bankaccount-kube:${{ env.IMAGE_TAG }}|g" \
      bankaccount-deployment-updated.yaml
```

**Qu'il fait:**
- Copie le fichier YAML original
- **Remplace automatiquement** `v1` par le SHA du commit
- Prépare le fichier pour le déploiement

---

### **Étape 🔟 : Configure Kubernetes Access**

```yaml
- name: Configure Kubernetes Access
  run: |
    if ! command -v kubectl &> /dev/null; then
      echo "kubectl n'est pas disponible"
      exit 0
    fi
    
    echo "=== Contexte Kubernetes actuel ==="
    kubectl config current-context || echo "Aucun contexte"
```

**Qu'il fait:**
- Vérifie si kubectl est disponible
- Affiche le contexte Kubernetes actuel
- Détecte les contextes disponibles

---

### **Étape 1️⃣1️⃣ : Deploy to Kubernetes**

```yaml
- name: Deploy to Kubernetes
  run: |
    if kubectl cluster-info &> /dev/null; then
      echo "✓ Cluster Kubernetes détecté"
      kubectl apply -f bankaccount-deployment-updated.yaml
      kubectl rollout status deployment/bankaccount-kube-deployment --timeout=5m
    else
      echo "⚠️  Aucun cluster disponible"
      echo "Téléchargez l'artifact et chargez-le localement"
    fi
```

**Qu'il fait:**
- Teste la connexion Kubernetes
- **Si le cluster est disponible** → Déploie automatiquement
- **Sinon** → Fournit des instructions
- Attends que les pods soient prêts

---

## 🔄 Trois Scénarios Possibles

### **Scénario 1: Déploiement sur Docker Desktop (Local Kubernetes)**

```
GitHub Actions → Détecte Docker Desktop Kubernetes
    ↓
kubectl apply -f deployment-updated.yaml
    ↓
✅ Déploie automatiquement sur Docker Desktop
    ↓
Application accessible à http://localhost:30080
```

### **Scénario 2: Déploiement sur un Cluster Kubernetes Distant**

```
GitHub Actions → Détecte le cluster (via kubeconfig secret)
    ↓
kubectl apply -f deployment-updated.yaml
    ↓
✅ Déploie automatiquement sur le cluster
    ↓
Application déployée en prod
```

### **Scénario 3: Aucun Kubernetes (Utiliser Localement)**

```
GitHub Actions → Aucun cluster détecté
    ↓
Fournit les instructions
    ↓
Vous téléchargez l'artifact
    ↓
docker load -i docker-image/bankaccount-image.tar
kubectl apply -f bankaccount-deployment.yaml
    ↓
✅ Déploiement manuel local
```

---

## 📋 Comment Utiliser

### **Option 1: Déploiement Automatique (Docker Desktop)**

**Prérequis:**
- Docker Desktop avec Kubernetes activé
- Kubernetes accessible en local

**Flux:**
```bash
Push → GitHub Actions → Déploie automatiquement → App en prod ✓
```

---

### **Option 2: Télécharger l'Artifact (Manuel)**

**Prérequis:**
- Docker Desktop avec Kubernetes activé

**Flux:**

1. **Aller sur GitHub Actions**
   - Your Repo → Actions → Dernier workflow
   
2. **Télécharger l'artifact**
   - Cliquer sur "docker-image"
   - Extraire le fichier

3. **Charger l'image localement**
   ```powershell
   docker load -i docker-image/bankaccount-image.tar
   docker images | grep bankaccount-kube
   ```

4. **Déployer**
   ```powershell
   kubectl apply -f bankaccount-deployment.yaml
   kubectl get pods
   ```

---

### **Option 3: Configurer un Cluster Distant**

Pour que le pipeline déploie sur un cluster distant (AWS EKS, GKE, etc.) :

1. **Ajouter le kubeconfig en secret GitHub**
   ```bash
   # Dans GitHub Repo Settings → Secrets and Variables → New Repository Secret
   Name: KUBECONFIG_BASE64
   Value: (base64 du kubeconfig)
   ```

2. **Modifier le pipeline** (ajouter avant l'étape 10):
   ```yaml
   - name: Setup Kubernetes Config
     run: |
       mkdir -p $HOME/.kube
       echo "${{ secrets.KUBECONFIG_BASE64 }}" | base64 -d > $HOME/.kube/config
       chmod 600 $HOME/.kube/config
   ```

3. **Le pipeline détectera le cluster et déploiera automatiquement** ✓

---

## 📊 Visualiser le Pipeline

```
┌──────────────────────────────────────────────────────────┐
│  Étape 1-7: Build & Package                              │
│  ✓ Compile (.NET)                                        │
│  ✓ Build Docker Image (7 premiers chars du SHA)          │
│  ✓ Save as Artifact                                      │
└────────┬─────────────────────────────────────────────────┘
         │
         ▼
┌──────────────────────────────────────────────────────────┐
│  Étape 8-9: Prepare Deployment                           │
│  ✓ Load Docker Image                                     │
│  ✓ Update YAML (v1 → SHA)                                │
└────────┬─────────────────────────────────────────────────┘
         │
         ▼
┌──────────────────────────────────────────────────────────┐
│  Étape 10: Detect Kubernetes                             │
│  ├─ kubectl available?                                   │
│  └─ Cluster accessible?                                  │
└────────┬─────────────────────────────────────────────────┘
         │
         ▼
    ┌────────────────────────┐
    │ Kubernetes Disponible? │
    └────┬──────────────┬────┘
         │              │
      OUI │              │ NON
         ▼              ▼
    ┌────────────┐  ┌─────────────────────┐
    │  DÉPLOIE   │  │  Fournir Instructions│
    │    AUTO    │  │  pour usage local    │
    └────────────┘  └─────────────────────┘
         │              │
         └──────┬───────┘
                ▼
    ┌──────────────────────┐
    │  Afficher Logs/Info  │
    └──────────────────────┘
```

---

## ✅ Avantages du Nouveau Pipeline

| Feature | Avant | Après |
|---------|-------|-------|
| **Versioning** | Statique (v1) | ✅ Dynamique (SHA) |
| **Déploiement** | Manuel | ✅ Automatique (si K8s) |
| **Détection** | Aucune | ✅ Détecte Kubernetes |
| **Instructions** | Aucune | ✅ Guide l'utilisateur |
| **Docker Load** | Absent | ✅ Charge l'image |
| **Artifact** | Oui | ✅ Toujours disponible |
| **Logs** | Limités | ✅ Détaillés |

---

## 🔧 Cas d'Usage

### **Dev Local (Docker Desktop)**

```bash
# Push vers GitHub
git push

# GitHub Actions:
# 1. Build Docker Image: bankaccount-kube:abc1234
# 2. Détecte Docker Desktop Kubernetes
# 3. Déploie automatiquement
# 4. App accessible à localhost:30080

# Résultat: ✅ Changements visibles immédiatement
```

### **Intégration Continue (Cluster Distant)**

```bash
# Push vers GitHub
git push

# GitHub Actions:
# 1. Build Docker Image: bankaccount-kube:abc1234
# 2. Détecte le cluster distant (via kubeconfig secret)
# 3. Déploie automatiquement en prod
# 4. App en prod immédiatement

# Résultat: ✅ CI/CD fully automated
```

### **Pas de Kubernetes**

```bash
# Push vers GitHub
git push

# GitHub Actions:
# 1. Build Docker Image
# 2. Aucun Kubernetes détecté
# 3. Fournit instructions
# 4. Artifact disponible au téléchargement

# Vous:
# docker load -i artifact.tar
# kubectl apply -f deployment.yaml

# Résultat: ✅ Déploiement semi-automatique
```

---

## 📞 Dépannage

### **"kubectl n'est pas disponible"**

→ C'est normal sur GitHub Actions, l'image est sauvegardée en artifact

### **"Aucun cluster Kubernetes détecté"**

→ Vérifiez que Docker Desktop Kubernetes est activé

### **Erreur de connexion Kubernetes**

→ Le kubeconfig n'est pas configuré comme secret GitHub

---

## 🎯 Prochaines Étapes

1. **Push le code**
   ```bash
   git add .
   git commit -m "chore: improve CI/CD pipeline"
   git push
   ```

2. **Voir le pipeline en action**
   - GitHub → Actions → Dernier workflow
   - Regarder les étapes s'exécuter

3. **Vérifier le déploiement**
   - http://localhost:30080 (local)
   - Ou le cluster distant

---

**Pipeline amélioré et prêt! 🚀**
