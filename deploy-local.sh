#!/bin/bash
# Script de déploiement local pour Docker Desktop

set -e

echo "=========================================="
echo "BankAccount - CI/CD Local Deployment"
echo "=========================================="

# Vérifier que Docker Desktop est lancé
echo "✓ Vérification de Docker..."
if ! command -v docker &> /dev/null; then
    echo "✗ Docker n'est pas installé ou pas dans le PATH"
    exit 1
fi

# Vérifier que Kubectl est disponible
echo "✓ Vérification de Kubectl..."
if ! command -v kubectl &> /dev/null; then
    echo "✗ Kubectl n'est pas installé ou pas dans le PATH"
    exit 1
fi

# Afficher le contexte Kubernetes actuel
echo ""
echo "Contexte Kubernetes actuel:"
kubectl config current-context

echo ""
echo "=========================================="
echo "Étape 1: Compiler le projet .NET"
echo "=========================================="
dotnet build -c Release

echo ""
echo "=========================================="
echo "Étape 2: Construire l'image Docker"
echo "=========================================="
docker build -t bankaccount-kube:v1 .

echo ""
echo "=========================================="
echo "Étape 3: Vérifier les images Docker"
echo "=========================================="
docker images | grep bankaccount-kube || true

echo ""
echo "=========================================="
echo "Étape 4: Appliquer le déploiement Kubernetes"
echo "=========================================="
kubectl apply -f bankaccount-deployment.yaml

echo ""
echo "=========================================="
echo "Étape 5: Vérifier le statut du déploiement"
echo "=========================================="
sleep 2
kubectl get deployments -l app=bankaccount
kubectl get pods -l app=bankaccount
kubectl get services bankaccount-service

echo ""
echo "=========================================="
echo "✓ Déploiement terminé avec succès!"
echo "=========================================="
echo ""
echo "Pour accéder à l'application:"
echo "  URL: http://localhost:30080"
echo ""
echo "Pour voir les logs:"
echo "  kubectl logs -l app=bankaccount -f"
echo ""
echo "Pour supprimer le déploiement:"
echo "  kubectl delete -f bankaccount-deployment.yaml"
