# Backend — Desafio Mottu

API REST em **.NET 10 / ASP.NET Core** para o desafio de aluguel de motos.
O contrato segue o Swagger oficial (campos em `snake_case`, identificadores
como `string`, erros como `{ "mensagem": "..." }`).

> Para a visão geral do projeto (frontend + docker compose + arquitetura),
> veja o [README na raiz](../README.md).

## Stack & libs

| Categoria          | Lib                                                                                  |
|--------------------|---------------------------------------------------------------------------------------|
| Runtime            | .NET 10 / C# 13                                                                       |
| ORM                | EF Core 10 + Npgsql                                                                   |
| Mensageria         | RabbitMQ.Client + `BackgroundService`                                                 |
| Validação          | FluentValidation                                                                      |
| Result/CQRS        | FluentResults + MediatR                                                               |
| Auth               | JWT Bearer + BCrypt.Net-Next                                                           |
| Documentação       | Swashbuckle (Swagger UI)                                                              |
| Testes             | xUnit, FluentAssertions, Moq, NetArchTest, Bogus, `Xunit.SkippableFact`, `Mvc.Testing` |

## Padrões de projeto aplicados

- **Clean Architecture leve** com 4 projetos (Domain → Application →
  Infrastructure → WebApi).
- **CQRS-lite com MediatR** — cada caso de uso é um `Command` + `Handler`.
- **Result pattern** (`FluentResults`) para erros tipados; `400/404` no
  controller a partir de `Result.Fail(...)`.
- **Repository pattern** — interfaces em `Job.Application/Repositories`,
  implementações em `Job.Infrastructure/Repositories`.
- **Validation** isolada por comando (`FluentValidation`).
- **Outbox simplificado** — handler publica `MotoCreatedEvent`; consumer
  filtra `Year == 2024` e persiste em `MotoNotifications`.
- **Auto-geração de identificadores** — todos os `Identifier` são opcionais;
  quando o cliente não envia, o handler gera `Guid.NewGuid().ToString("N")`.

## Como executar

### Via Docker (recomendado)

```bash
# este diretório (apenas backend + Postgres + RabbitMQ)
docker compose up -d --build
```

| Serviço      | URL                                       |
|--------------|-------------------------------------------|
| API          | http://localhost:5001/swagger             |
| PostgreSQL   | localhost:5432 (`postgres` / `postgres`)  |
| RabbitMQ UI  | http://localhost:15672 (`guest`/`guest`)  |

> Para subir junto com o front-end, use o `docker-compose.yml` da **raiz** do
> repositório.

### Local (sem Docker para a API)

```bash
docker compose up -d postgres rabbitmq      # apenas dependências
dotnet run --project src/Job.WebApi         # http://localhost:5050
```

A API roda automaticamente as migrations no boot e cria/atualiza o schema.
Imagens da CNH são salvas em volume Docker (`uploads-data`) e expostas em
`/uploads/<arquivo>`.

## Configurações principais

`appsettings.json` (sobrescrevíveis via env, formato `Section__Key`):

```jsonc
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=postgres;Port=5432;User Id=postgres;Password=postgres;Database=job;"
  },
  "Jwt":      { "Secret": "<segredo-de-32-ou-mais-chars>" },
  "Storage":  { "RootPath": "uploads", "PublicBaseUrl": "/uploads" },
  "RabbitMq": {
    "HostName": "rabbitmq",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest",
    "MotoCreatedExchange": "moto.created",
    "MotoCreatedQueue":    "moto.created.year-2024"
  },
  "Cors": { "AllowedOrigins": [ "http://localhost:4200" ] }
}
```

Em produção, sobrescreva via env: `Jwt__Secret`, `RabbitMq__Password`,
`ConnectionStrings__DefaultConnection`, `Cors__AllowedOrigins__0`, etc.

## Endpoints (resumo)

Todos em `snake_case`. Erros: `{ "mensagem": "..." }`.

### `/motos` (admin)

| Método | Rota                  | Descrição                                                                |
|--------|-----------------------|--------------------------------------------------------------------------|
| POST   | `/motos`              | Cadastra moto (`identificador` opcional — gerado se omitido)             |
| GET    | `/motos?placa=`       | Lista motos (filtro opcional por placa)                                  |
| GET    | `/motos/{id}`         | Busca moto por identificador                                             |
| PUT    | `/motos/{id}/placa`   | Atualiza apenas a placa                                                  |
| DELETE | `/motos/{id}`         | Remove moto sem locações                                                 |

```json
POST /motos
{ "ano": 2024, "modelo": "Mottu Sport", "placa": "CDX-0101" }
```

### `/entregadores`

| Método | Rota                              | Descrição                                       |
|--------|-----------------------------------|-------------------------------------------------|
| POST   | `/entregadores`                   | Cadastro (exige `senha` ≥ 6 chars)              |
| POST   | `/entregadores/{id}/cnh`          | Atualiza imagem da CNH (base64 png/bmp)         |
| POST   | `/entregadores/authentication`    | Login do entregador → JWT                       |

```json
POST /entregadores
{
  "nome": "João Entregador",
  "cnpj": "12345678000195",
  "senha": "minha-senha-forte",
  "data_nascimento": "1990-01-01",
  "numero_cnh": "77058710884",
  "tipo_cnh": "A"
}
```

### `/locacao`

| Método | Rota                          | Descrição                                  |
|--------|-------------------------------|--------------------------------------------|
| POST   | `/locacao`                    | Aluga moto (planos 7 / 15 / 30 / 45 / 50)  |
| GET    | `/locacao/{id}`               | Consulta locação                           |
| PUT    | `/locacao/{id}/devolucao`     | Informa devolução, calcula multa/total     |

Regras:

- Diárias: 7 = R$ 30 · 15 = R$ 28 · 30 = R$ 22 · 45 = R$ 20 · 50 = R$ 18.
- Devolução **antes** da previsão → cobra dias usados + multa (20% no plano
  de 7, 40% nos demais sobre as diárias não usadas).
- Devolução **depois** da previsão → cobra todas as diárias do plano +
  R$ 50,00 por dia adicional.
- Apenas entregador com CNH `A` ou `A+B` pode alugar.

### `/manager`

| Método | Rota                      | Descrição           |
|--------|---------------------------|---------------------|
| POST   | `/manager/authentication` | Login admin (JWT)   |

Credenciais semeadas: `job@job.com` / `mudar@123`.

## Mensageria

Ao cadastrar uma moto a API publica `MotoCreatedEvent` na exchange
`moto.created`. O `MotoNotificationConsumer` (BackgroundService) consome a
fila `moto.created.year-2024`, filtra eventos cujo `Year == 2024` e persiste
a notificação na tabela `MotoNotifications`.

## Testes

```bash
dotnet test Job.slnx --nologo
```

- `Job.UnitTests` — handlers e regras puras.
- `Job.ArchitectureTest` — fronteiras Domain/Application/Infrastructure/WebApi.
- `Job.IntegrationTest` — `WebApplicationFactory` + `SkippableFact` (sobe se
  houver Postgres + RabbitMQ; senão, pula graciosamente).
- `Job.CommonsTest` — fakers Bogus reutilizáveis.

## Estrutura

```
backend/
├── docker-compose.yml          # API + Postgres + RabbitMQ
├── Job.slnx                    # solution moderna .NET 10
├── src/
│   ├── Job.Domain              # entidades, enums, regras puras
│   ├── Job.Application         # commands, validations, services (handlers)
│   ├── Job.Infrastructure      # EF Core, repositórios, RabbitMQ, storage
│   └── Job.WebApi              # controllers, Program.cs, middlewares, Dockerfile
└── test/
    ├── Job.UnitTests
    ├── Job.ArchitectureTest
    ├── Job.IntegrationTest
    └── Job.CommonsTest
```
# Desafio Backend — Mottu

Projeto desenvolvido durante o processo seletivo, implementando uma API REST
para o gerenciamento de aluguel de motos por entregadores. A solução segue o
contrato definido no Swagger oficial do desafio (campos `snake_case`,
identificadores externos como `string` e respostas de erro no formato
`{ "mensagem": "..." }`).

## Stack

- .NET 8 / C# 12
- PostgreSQL + EF Core (Npgsql)
- RabbitMQ 3.13 (publisher + consumer hospedado em `BackgroundService`)
- MediatR + FluentResults + FluentValidation
- BCrypt.NET, Swashbuckle (Swagger)
- xUnit, FluentAssertions, Moq, NetArchTest, Bogus
- Docker / Docker Compose

## Como executar

Pré-requisitos: Docker e Docker Compose.

```bash
cd backend
docker compose up -d --build
```

Serviços expostos:

| Serviço      | URL                                       |
|--------------|-------------------------------------------|
| API          | http://localhost:5001/swagger             |
| PostgreSQL   | localhost:5432 (postgres / postgres)      |
| RabbitMQ UI  | http://localhost:15672 (guest / guest)    |

A primeira execução roda automaticamente as migrations. Imagens enviadas para
`/entregadores/{id}/cnh` são salvas em volume Docker e expostas em
`http://localhost:5001/uploads/<arquivo>`.

## Configurações principais

`appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=postgres;Port=5432;User Id=postgres;Password=postgres;Database=job;"
  },
  "Jwt":      { "Secret": "<segredo-de-32-ou-mais-chars>" },
  "Storage":  { "RootPath": "uploads", "PublicBaseUrl": "/uploads" },
  "RabbitMq": {
    "HostName": "rabbitmq",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest",
    "MotoCreatedExchange": "moto.created",
    "MotoCreatedQueue": "moto.created.year-2024"
  }
}
```

Em produção, sobrescreva `Jwt:Secret` e credenciais via variáveis de ambiente
(`Jwt__Secret`, `RabbitMq__Password`, etc.).

## Endpoints (contrato Swagger)

Todos os payloads usam `snake_case`. Erros retornam `400` ou `404` com
`{ "mensagem": "..." }`.

### Motos — `/motos`

| Método | Rota                  | Descrição                         |
|--------|-----------------------|-----------------------------------|
| POST   | `/motos`              | Cadastra moto                     |
| GET    | `/motos?placa=`       | Lista motos (filtro opcional)     |
| GET    | `/motos/{id}`         | Busca moto por identificador      |
| PUT    | `/motos/{id}/placa`   | Atualiza apenas a placa           |
| DELETE | `/motos/{id}`         | Remove moto sem locações          |

```json
POST /motos
{
  "identificador": "moto123",
  "ano": 2024,
  "modelo": "Mottu Sport",
  "placa": "CDX-0101"
}
```

### Entregadores — `/entregadores`

| Método | Rota                            | Descrição                         |
|--------|---------------------------------|-----------------------------------|
| POST   | `/entregadores`                 | Cadastra entregador               |
| POST   | `/entregadores/{id}/cnh`        | Atualiza imagem da CNH (base64)   |

```json
POST /entregadores
{
  "identificador": "entregador123",
  "nome": "João Entregador",
  "cnpj": "12345678000195",
  "data_nascimento": "1990-01-01",
  "numero_cnh": "77058710884",
  "tipo_cnh": "A",        // "A" | "B" | "A+B"
  "imagem_cnh": null       // opcional, base64 png/bmp
}
```

```json
POST /entregadores/{id}/cnh
{ "imagem_cnh": "data:image/png;base64,iVBORw0KGgo..." }
```

### Locação — `/locacao`

| Método | Rota                          | Descrição                                 |
|--------|-------------------------------|-------------------------------------------|
| POST   | `/locacao`                    | Aluga uma moto (planos 7,15,30,45,50)     |
| GET    | `/locacao/{id}`               | Consulta locação por identificador        |
| PUT    | `/locacao/{id}/devolucao`     | Informa devolução e calcula multa/valor   |

```json
POST /locacao
{
  "identificador": "locacao123",
  "entregador_id": "entregador123",
  "moto_id": "moto123",
  "data_inicio": "2026-05-01T00:00:00Z",
  "data_termino": "2026-05-08T00:00:00Z",
  "data_previsao_termino": "2026-05-08T00:00:00Z",
  "plano": 7
}
```

Regras de cálculo:

- Diárias: `7=R$30`, `15=R$28`, `30=R$22`, `45=R$20`, `50=R$18`.
- Devolução **antes** da previsão: cobra dias usados + multa (20% do valor das
  diárias não utilizadas no plano de 7 dias, 40% nos demais).
- Devolução **depois** da previsão: cobra todas as diárias do plano +
  R$50,00 por dia adicional.
- Apenas entregadores com CNH categoria `A` ou `A+B` podem alugar.

### Manager (legado) — `/manager`

| Método | Rota                      | Descrição                       |
|--------|---------------------------|---------------------------------|
| POST   | `/manager/authentication` | Login admin com JWT (uso interno)|

Credenciais semeadas: `job@job.com` / `mudar@123`.

## Mensageria (RabbitMQ)

Ao cadastrar uma moto a API publica `MotoCreatedEvent` na exchange
`moto.created`. O `MotoNotificationConsumer` (BackgroundService) consome a fila
`moto.created.year-2024`, filtra eventos cujo `Year == 2024` e persiste a
notificação na tabela `MotoNotifications`.

## Testes

```bash
cd backend
# unit + arquitetura
dotnet test Job.sln --filter "FullyQualifiedName!~IntegrationTest"

# integração (requer Postgres em localhost:5432)
dotnet test test/Job.IntegrationTest
```

## Estrutura

```
backend/
├── docker-compose.yml          # API + Postgres + RabbitMQ
├── src/
│   ├── Job.Domain              # Entidades, regras puras
│   ├── Job.Application         # Commands, Services, Validations
│   ├── Job.Infrastructure      # EF Core, Repositórios, Storage, Messaging
│   └── Job.WebApi              # Controllers, Program.cs, Middlewares
└── test/
    ├── Job.UnitTests
    ├── Job.ArchitectureTest
    ├── Job.IntegrationTest
    └── Job.CommonsTest         # Fakers compartilhados (Bogus)
```
# Introdução

Projetos desenvolvidos durante o processo seletivo, com o objetivo específico de criar uma aplicação 
abrangente e funcional para o gerenciamento eficiente de aluguel de motocicletas. 
Estes projetos não apenas demonstram habilidades técnicas, mas também abordam desafios 
práticos do mundo real no setor de aluguel de veículos, incorporando funcionalidades 
como cadastro de clientes, controle de estoque de motocicletas, agendamento de aluguéis, 
e geração de relatórios financeiros.

## Introdução para execução

### Variáveis de ambiente

Ajustar as variáveis de ambiente `ASPNETCORE_ENVIRONMENT` e `DOTNET_ENVIRONMENT` com o valor `Development` no sistema operacional.

No Windows, para criar a variável de ambiente para o usuário conectado, ou alterar o valor, com CMD:

```bash
setx ASPNETCORE_ENVIRONMENT "Development"
setx DOTNET_ENVIRONMENT "Development"
```

No MacOS, para criar as variáveis de ambiente permanentemente, é necessário editar o arquivo `~/.bash_profile` como root e acrescentar as seguintes linhas no final do arquivo:

```bash
export ASPNETCORE_ENVIRONMENT="Development"
export DOTNET_ENVIRONMENT="Development"
```

### Postman Collection

Para testar a API, importe o arquivo `job.postman_collection.json` no Postman.

### Ferramentas necessárias

- Docker/Docker Compose

Para executar o projeto, basta clonar o repositório e executar o comando `docker compose up -d` para subir o container do banco de dados e da aplicação.
Abra o navegador e acesse `http://localhost:5001/swagger/index.html` para visualizar a aplicação.

Ao executar o comando `docker compose up -d` o banco de dados será populado com dados de teste.

### Autenticação ADMIN

Para acessar a aplicação como administrador, utilize o seguinte usuário:

``` json
{
  "email": "job@job.com",
  "password": "mudar@123"
}
```
Para autenticar como motoboy e necessário fazer o cadastro e utilizar o CNPJ no login.

## Características do projeto

- .NET 8
- Banco de dados PostgreSQL
- CQS (Command Query Separation)
- MediatR para implementação do padrão CQS.
- ORM: **EntityFramework Core**
- Framework de Testes: **XUnit**
- Framework de Assertions: **FluentAssertions**
- Framework de Mock: **Moq**
- Code Analyzer: **Microsoft.CodeAnalysis.NetAnalyzers**
- Projeto para testes de Unidade
- Projeto para testes de Integração
- Tratamento de Warning como Erro.
- Dockerfile para a aplicação e para o banco de dados.
- Docker Compose para subir a aplicação e o banco de dados.
- [BCrypt.NET - NEXT](https://github.com/BcryptNet/bcrypt.net) para criptografia de senha.
- Swagger para documentação da API.
- [Fluent Result](https://github.com/altmann/FluentResults) para padronização de retorno da API.
- Postman Collection para testes de API.
