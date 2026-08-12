#!/bin/bash
# Script pour créer les secrets Kubernetes nécessaires

# Couleurs pour l'affichage
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

echo -e "${GREEN}╔════════════════════════════════════════════╗${NC}"
echo -e "${GREEN}║  Configuration des Secrets Kubernetes      ║${NC}"
echo -e "${GREEN}╚════════════════════════════════════════════╝${NC}"

# Vérifier que kubectl est disponible
if ! command -v kubectl &> /dev/null; then
    echo -e "${RED}✗ kubectl n'est pas installé${NC}"
    exit 1
fi

# Afficher le contexte actuel
echo -e "\n${YELLOW}Contexte Kubernetes actuel:${NC}"
kubectl config current-context

# Créer le secret pour la base de données
echo -e "\n${YELLOW}Création du secret pour la base de données...${NC}"

# Option 1: Saisir la chaîne de connexion interactivement
read -p "Entrez la chaîne de connexion SQL Server: " CONNECTION_STRING

if [ -z "$CONNECTION_STRING" ]; then
    echo -e "${RED}✗ La chaîne de connexion ne peut pas être vide${NC}"
    exit 1
fi

# Créer le secret
kubectl create secret generic bankaccount-db \
  --from-literal=connection-string="$CONNECTION_STRING" \
  --dry-run=client -o yaml | kubectl apply -f -

if [ $? -eq 0 ]; then
    echo -e "${GREEN}✓ Secret 'bankaccount-db' créé avec succès${NC}"
else
    echo -e "${RED}✗ Erreur lors de la création du secret${NC}"
    exit 1
fi

# Vérifier les secrets créés
echo -e "\n${YELLOW}Secrets actuels:${NC}"
kubectl get secrets

echo -e "\n${GREEN}✓ Configuration des secrets terminée${NC}"
echo -e "\n${YELLOW}Prochaines étapes:${NC}"
echo "1. Déployer l'application: kubectl apply -f bankaccount-deployment.yaml"
echo "2. Vérifier le statut: kubectl get pods -l app=bankaccount"
echo "3. Voir les logs: kubectl logs -l app=bankaccount -f"
