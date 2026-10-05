# syntax=docker/dockerfile:1

# ---------- Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

COPY src/Dedevops.Api/Dedevops.Api.csproj src/Dedevops.Api/
RUN dotnet restore src/Dedevops.Api/Dedevops.Api.csproj

COPY src/ src/
RUN dotnet build src/Dedevops.Api/Dedevops.Api.csproj -c Release --no-restore

RUN dotnet publish src/Dedevops.Api/Dedevops.Api.csproj \
    -c Release \
    --no-build \
    -o /app/publish \
    /p:UseAppHost=false

# ---------- Runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_RUNNING_IN_CONTAINER=true

COPY --from=build /app/publish .

# Non-root user provided by the official .NET images (UID 1654).
USER $APP_UID

EXPOSE 8080

ENTRYPOINT ["dotnet", "Dedevops.Api.dll"]
