# Script PowerShell pour créer les secrets Kubernetes

param(
    [string]$ConnectionString
)

Write-Host "╔════════════════════════════════════════════╗" -ForegroundColor Green
Write-Host "║  Configuration des Secrets Kubernetes      ║" -ForegroundColor Green
Write-Host "╚════════════════════════════════════════════╝" -ForegroundColor Green

# Vérifier que kubectl est disponible
try {
    kubectl version --client | Out-Null
} catch {
    Write-Host "✗ kubectl n'est pas disponible" -ForegroundColor Red
    exit 1
}

# Afficher le contexte actuel
Write-Host "`nContexte Kubernetes actuel:" -ForegroundColor Yellow
kubectl config current-context

# Créer le secret pour la base de données
Write-Host "`nCréation du secret pour la base de données..." -ForegroundColor Yellow

if (-not $ConnectionString) {
    $ConnectionString = Read-Host "Entrez la chaîne de connexion SQL Server"
}

if ([string]::IsNullOrEmpty($ConnectionString)) {
    Write-Host "✗ La chaîne de connexion ne peut pas être vide" -ForegroundColor Red
    exit 1
}

# Créer le secret
kubectl create secret generic bankaccount-db `
  --from-literal=connection-string="$ConnectionString" `
  --dry-run=client -o yaml | kubectl apply -f -

if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Secret 'bankaccount-db' créé avec succès" -ForegroundColor Green
} else {
    Write-Host "✗ Erreur lors de la création du secret" -ForegroundColor Red
    exit 1
}

# Vérifier les secrets créés
Write-Host "`nSecrets actuels:" -ForegroundColor Yellow
kubectl get secrets

Write-Host "`n✓ Configuration des secrets terminée" -ForegroundColor Green
Write-Host "`nProchaines étapes:" -ForegroundColor Yellow
Write-Host "1. Déployer l'application: kubectl apply -f bankaccount-deployment.yaml" -ForegroundColor Cyan
Write-Host "2. Vérifier le statut: kubectl get pods -l app=bankaccount" -ForegroundColor Cyan
Write-Host "3. Voir les logs: kubectl logs -l app=bankaccount -f" -ForegroundColor Cyan
