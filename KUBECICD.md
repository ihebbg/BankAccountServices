# KubeCI/CD GitHub Actions

Le pipeline GitHub Actions est [`.github/workflows/kubecicd.yml`](.github/workflows/kubecicd.yml).
Il se declenche sur chaque push vers `main`, `master` ou `develop`, et peut aussi
etre lance manuellement depuis l'onglet **Actions** de GitHub.

## Prerequis GitHub Actions

Le cluster Docker Desktop est sur votre PC et n'est pas accessible par les
runners GitHub heberges. Installez donc un runner **self-hosted Windows** dans
le depot : **Settings > Actions > Runners > New self-hosted runner**. Lancez le
runner avec le meme utilisateur Windows que Docker Desktop afin qu'il puisse
acceder a Docker et au contexte Kubernetes `docker-desktop`.

Le runner doit avoir `docker`, `kubectl` et .NET 8 disponibles. Le secret
Kubernetes `bankaccount-db` doit deja exister sur Docker Desktop ; il n'est pas
stocke dans GitHub Actions.

Chaque execution utilise l'image `bankaccount-kube:<commit-sha>`, applique le
manifeste, puis attend que le rollout des deux pods soit termine. Il n'est donc
plus necessaire de modifier le tag dans le YAML.
