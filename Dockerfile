FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY BankAccountServices.csproj ./
RUN dotnet restore BankAccountServices.csproj

COPY . ./
RUN dotnet publish BankAccountServices.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

COPY --from=build --chown=app:app /app/publish ./
RUN mkdir -p /app/Logs && chown -R app:app /app/Logs

USER app

ENTRYPOINT ["dotnet", "BankAccountServices.dll"]
