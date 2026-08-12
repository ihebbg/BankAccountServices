# =========================
# BASE : environnement pour exécuter .NET
# =========================

# Image .NET Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# Dossier de travail dans le container
WORKDIR /app

# L'application utilise le port 8080
EXPOSE 8080

# ASP.NET écoute sur le port 8080
ENV ASPNETCORE_URLS=http://+:8080


# =========================
# BUILD : compiler le projet
# =========================

# Image .NET SDK pour compiler
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Dossier de travail
WORKDIR /src

# Copier le projet dans le container
COPY . .

# Restaurer les packages NuGet
RUN dotnet restore

# Compiler le projet
RUN dotnet build -c Release

# Publier l'application
RUN dotnet publish -c Release -o /app/publish


# =========================
# FINAL : image finale
# =========================

# Reprendre l'image Runtime
FROM base AS final

# Dossier de l'application
WORKDIR /app

# Copier l'application publiée depuis BUILD
COPY --from=build /app/publish .

# Lancer l'application
ENTRYPOINT ["dotnet", "BankAccountServices.dll"]