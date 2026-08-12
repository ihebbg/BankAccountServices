#!/usr/bin/env pwsh
# Makefile PowerShell pour les commandes courantes

param(
    [Parameter(Position = 0)]
    [ValidateSet('help', 'build', 'docker-build', 'deploy', 'deploy-clean', 'logs', 'status', 'delete', 'clean', 'all')]
    [string]$Command = 'help'
)

function Show-Help {
    Write-Host @"
╔════════════════════════════════════════════════════════════════╗
║         BankAccount Services - Pipeline Commands               ║
╚════════════════════════════════════════════════════════════════╝

Usage: .\Makefile.ps1 [command]

Commands:
  help              Affiche cette aide
  build             Compile le projet .NET
  docker-build      Construit l'image Docker
  deploy            Déploie sur Kubernetes (Docker Desktop)
  deploy-clean      Nettoie et redéploie
  logs              Affiche les logs de l'application
  status            Affiche le statut du déploiement
  delete            Supprime le déploiement Kubernetes
  clean             Supprime tout (déploiement + image Docker)

  all               Build + Docker + Deploy (équivalent à: build docker-build deploy)

Examples:
  .\Makefile.ps1 help
  .\Makefile.ps1 build
  .\Makefile.ps1 all
  .\Makefile.ps1 logs -Follow

"@ -ForegroundColor Cyan
}

function Invoke-Build {
    Write-Host "`n▶ Compilation du projet .NET..." -ForegroundColor Green
    dotnet build -c Release --nologo
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Compilation réussie" -ForegroundColor Green
    }
    else {
        Write-Host "✗ Erreur de compilation" -ForegroundColor Red
        exit 1
    }
}

function Invoke-DockerBuild {
    Write-Host "`n▶ Construction de l'image Docker..." -ForegroundColor Green
    docker build -t bankaccount-kube:v1 .
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Image Docker créée avec succès" -ForegroundColor Green
        docker images | Select-String "bankaccount-kube"
    }
    else {
        Write-Host "✗ Erreur lors de la construction Docker" -ForegroundColor Red
        exit 1
    }
}

function Invoke-Deploy {
    Write-Host "`n▶ Déploiement sur Kubernetes..." -ForegroundColor Green
    kubectl apply -f bankaccount-deployment.yaml
    Start-Sleep -Seconds 2
    Invoke-Status
}

function Invoke-DeployClean {
    Write-Host "`n▶ Nettoyage du déploiement précédent..." -ForegroundColor Yellow
    kubectl delete -f bankaccount-deployment.yaml --ignore-not-found=true 2>$null
    docker rmi bankaccount-kube:v1 --force 2>$null || $null
    Start-Sleep -Seconds 2
    Write-Host "✓ Nettoyage terminé" -ForegroundColor Green
    Invoke-Deploy
}

function Invoke-Logs {
    Write-Host "`n▶ Affichage des logs..." -ForegroundColor Green
    kubectl logs -l app=bankaccount -f
}

function Invoke-Status {
    Write-Host "`n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
    Write-Host "Déploiements:" -ForegroundColor Yellow
    kubectl get deployments -l app=bankaccount
    Write-Host "`nPods:" -ForegroundColor Yellow
    kubectl get pods -l app=bankaccount
    Write-Host "`nServices:" -ForegroundColor Yellow
    kubectl get services
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━`n" -ForegroundColor Cyan
}

function Invoke-Delete {
    Write-Host "`n▶ Suppression du déploiement Kubernetes..." -ForegroundColor Yellow
    kubectl delete -f bankaccount-deployment.yaml
    Write-Host "✓ Déploiement supprimé" -ForegroundColor Green
}

function Invoke-Clean {
    Write-Host "`n▶ Nettoyage complet..." -ForegroundColor Yellow
    kubectl delete -f bankaccount-deployment.yaml --ignore-not-found=true 2>$null
    docker rmi bankaccount-kube:v1 --force 2>$null || $null
    Write-Host "✓ Nettoyage complet terminé" -ForegroundColor Green
}


function Invoke-All {
    Invoke-Build
    Invoke-DockerBuild
    Invoke-Deploy
}

# Routage des commandes
switch ($Command) {
    'help' { Show-Help }
    'build' { Invoke-Build }
    'docker-build' { Invoke-DockerBuild }
    'deploy' { Invoke-Deploy }
    'deploy-clean' { Invoke-DeployClean }
    'logs' { Invoke-Logs }
    'status' { Invoke-Status }
    'delete' { Invoke-Delete }
    'clean' { Invoke-Clean }
    'all' { Invoke-All }
    default { Show-Help }
}
