# Script de déploiement local pour Docker Desktop (PowerShell)

param(
    [switch]$Clean,
    [switch]$Logs
)

Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "BankAccount - CI/CD Local Deployment" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan

# Vérifier que Docker Desktop est lancé
Write-Host "`n✓ Vérification de Docker..." -ForegroundColor Green
try {
    docker version | Out-Null
} catch {
    Write-Host "✗ Docker n'est pas disponible" -ForegroundColor Red
    exit 1
}

# Vérifier que Kubectl est disponible
Write-Host "✓ Vérification de Kubectl..." -ForegroundColor Green
try {
    kubectl version --client | Out-Null
} catch {
    Write-Host "✗ Kubectl n'est pas disponible" -ForegroundColor Red
    exit 1
}

# Afficher le contexte Kubernetes actuel
Write-Host "`nContexte Kubernetes actuel:" -ForegroundColor Yellow
kubectl config current-context

if ($Clean) {
    Write-Host "`n==========================================" -ForegroundColor Cyan
    Write-Host "Suppression du déploiement existant" -ForegroundColor Cyan
    Write-Host "==========================================" -ForegroundColor Cyan
    kubectl delete -f bankaccount-deployment.yaml --ignore-not-found=true
    docker rmi bankaccount-kube:v1 --force 2>$null || $null
    Start-Sleep -Seconds 2
}

Write-Host "`n==========================================" -ForegroundColor Cyan
Write-Host "Étape 1: Compiler le projet .NET" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
dotnet build -c Release --nologo

Write-Host "`n==========================================" -ForegroundColor Cyan
Write-Host "Étape 2: Construire l'image Docker" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
docker build -t bankaccount-kube:v1 .

Write-Host "`n==========================================" -ForegroundColor Cyan
Write-Host "Étape 3: Vérifier les images Docker" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
docker images | Select-String "bankaccount-kube"

Write-Host "`n==========================================" -ForegroundColor Cyan
Write-Host "Étape 4: Appliquer le déploiement Kubernetes" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
kubectl apply -f bankaccount-deployment.yaml

Write-Host "`n==========================================" -ForegroundColor Cyan
Write-Host "Étape 5: Vérifier le statut du déploiement" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
Start-Sleep -Seconds 2
kubectl get deployments -l app=bankaccount
Write-Host ""
kubectl get pods -l app=bankaccount
Write-Host ""
kubectl get services bankaccount-service

if ($Logs) {
    Write-Host "`n==========================================" -ForegroundColor Cyan
    Write-Host "Affichage des logs" -ForegroundColor Cyan
    Write-Host "==========================================" -ForegroundColor Cyan
    kubectl logs -l app=bankaccount -f
}

Write-Host "`n==========================================" -ForegroundColor Green
Write-Host "✓ Déploiement terminé avec succès!" -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Pour accéder à l'application:" -ForegroundColor Yellow
Write-Host "  URL: http://localhost:30080" -ForegroundColor White
Write-Host ""
Write-Host "Pour voir les logs:" -ForegroundColor Yellow
Write-Host "  kubectl logs -l app=bankaccount -f" -ForegroundColor White
Write-Host ""
Write-Host "Pour nettoyer:" -ForegroundColor Yellow
Write-Host "  .\deploy-local.ps1 -Clean" -ForegroundColor White
Write-Host ""
Write-Host "Pour afficher les logs après déploiement:" -ForegroundColor Yellow
Write-Host "  .\deploy-local.ps1 -Logs" -ForegroundColor White
