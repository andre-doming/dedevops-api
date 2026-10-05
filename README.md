# dedevops-api

API REST de laboratório do **DEDEVOPS**, construída em **.NET 10 / ASP.NET Core (Minimal API)**.

## Objetivo

Ser a primeira aplicação real do pipeline de CI/CD do laboratório. Ela é propositalmente simples
(sem banco de dados, cache ou mensageria) e será, nas próximas etapas:

1. testada localmente;
2. validada pelo GitHub Actions;
3. empacotada em Docker;
4. publicada no GitHub Container Registry (GHCR);
5. implantada via Argo CD a partir do repositório separado `dedevops-gitops`;
6. integrada aos Smoke Tests, Regression Tests e Argo Rollouts.

## Estrutura

```
dedevops-api/
├── src/
│   └── Dedevops.Api/                 # API (Minimal API)
│       ├── Dedevops.Api.csproj
│       ├── Program.cs                # Pipeline HTTP e endpoints
│       ├── ApplicationOptions.cs     # Configuração "Application" (nome, versão, descrição)
│       ├── Responses.cs              # Contratos de resposta (records)
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       └── Properties/launchSettings.json
├── tests/
│   └── Dedevops.Api.Tests/           # Testes de integração (xUnit + WebApplicationFactory)
│       ├── Dedevops.Api.Tests.csproj
│       └── ApiEndpointsTests.cs
├── Dedevops.Api.slnx                 # Solução (formato .slnx)
├── Dockerfile                        # Build multi-stage para produção
├── .dockerignore
├── .gitignore
├── .gitattributes
└── README.md
```

## Pré-requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0) (`dotnet --version` deve retornar `10.x`)
- [Docker](https://docs.docker.com/get-docker/) (para build/execução do container)
- `curl` (para os exemplos)

## Porta utilizada

A aplicação escuta na porta **8080**, tanto localmente (`dotnet run`, via `launchSettings.json`)
quanto dentro do container (via `ASPNETCORE_HTTP_PORTS=8080`).

## Executar localmente

```bash
dotnet run --project src/Dedevops.Api
```

A API ficará disponível em `http://localhost:8080`.

Para sobrescrever a porta sem alterar arquivos:

```bash
dotnet run --project src/Dedevops.Api --urls http://localhost:5000
```

## Executar os testes

Na raiz do repositório:

```bash
dotnet test
```

## Construir a imagem Docker

```bash
docker build -t dedevops-api:local .
```

## Executar o container

```bash
docker run --rm -d --name dedevops-api-test -p 8080:8080 dedevops-api:local
curl http://localhost:8080/api/health
docker stop dedevops-api-test
```

O container roda como usuário **não-root** (`app`, UID 1654), fornecido pela imagem oficial
`mcr.microsoft.com/dotnet/aspnet:10.0`.

## Endpoints

| Método | Rota          | Descrição                                   |
|--------|---------------|---------------------------------------------|
| GET    | `/api/health` | Health check da aplicação                   |
| GET    | `/api/info`   | Informações da aplicação e do ambiente      |

Rotas inexistentes retornam **HTTP 404** com corpo JSON no formato
[Problem Details (RFC 9457)](https://www.rfc-editor.org/rfc/rfc9457) (`application/problem+json`).
Erros não tratados retornam **HTTP 500** no mesmo formato, sem expor stack trace em produção.

### Exemplos com curl

```bash
curl -i http://localhost:8080/api/health
```

```json
{
  "status": "ok",
  "application": "dedevops-api",
  "version": "1.0.0"
}
```

```bash
curl -i http://localhost:8080/api/info
```

```json
{
  "application": "dedevops-api",
  "version": "1.0.0",
  "description": "Aplicação de laboratório do DEDEVOPS para validação do pipeline de CI/CD.",
  "environment": "Production",
  "framework": ".NET 10.0.x",
  "hostname": "a1b2c3d4e5f6",
  "timestampUtc": "2026-10-05T13:00:00.0000000+00:00"
}
```

```bash
curl -i http://localhost:8080/api/does-not-exist
```

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Not Found",
  "status": 404
}
```

## Configuração

Os valores da seção `Application` podem ser sobrescritos por variáveis de ambiente
(útil para o pipeline injetar a versão da imagem, por exemplo):

| Variável                   | Padrão         |
|----------------------------|----------------|
| `Application__Name`        | `dedevops-api` |
| `Application__Version`     | `1.0.0`        |
| `Application__Description` | (texto padrão) |
| `ASPNETCORE_ENVIRONMENT`   | `Production` no container |

Nenhum secret é necessário ou armazenado neste repositório.
