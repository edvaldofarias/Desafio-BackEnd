# Mottu — Desafio Backend (full-stack)

Aplicação completa para o desafio de aluguel de motos da Mottu, composta por:

- **Backend** em **.NET 10 / ASP.NET Core** (API REST + RabbitMQ + Postgres)
- **Frontend** em **Angular 18** (SPA com Tailwind CSS, autenticação JWT)
- Orquestração via **Docker Compose** (Postgres + RabbitMQ + API + SPA atrás de Nginx)

> O contrato de API segue o Swagger oficial do desafio (campos em `snake_case`,
> identificadores como `string`, erros no formato `{ "mensagem": "..." }`).

---

## Sumário

- [Arquitetura](#arquitetura)
- [Stack & libs](#stack--libs)
- [Padrões de projeto](#padrões-de-projeto)
- [Como executar (Docker)](#como-executar-docker)
- [Como executar local (sem Docker)](#como-executar-local-sem-docker)
- [Endpoints](#endpoints)
- [Mensageria](#mensageria)
- [Testes](#testes)
- [Estrutura do repositório](#estrutura-do-repositório)
- [Decisões de design e mudanças relevantes](#decisões-de-design-e-mudanças-relevantes)

---

## Arquitetura

```
                       ┌──────────────────────────────────────────────────┐
                       │                     Browser                       │
                       └───────────────┬──────────────────────────────────┘
                                       │ http://localhost:4200
                                       ▼
                       ┌──────────────────────────────────────────────────┐
   docker network ───► │ frontend (Nginx + Angular build)                 │
   "mottu-network"     │   /            → SPA                              │
                       │   /api/*       → reverse-proxy → core-webapi:8080 │
                       │   /uploads/*   → reverse-proxy → core-webapi:8080 │
                       └───────────────┬──────────────────────────────────┘
                                       │
                                       ▼
                       ┌──────────────────────────────────────────────────┐
                       │ core-webapi (.NET 10)                            │
                       │   Controllers → MediatR Handlers → Repositories  │
                       │   FluentValidation • FluentResults • BCrypt      │
                       │   StaticFiles em /uploads                         │
                       └─────────┬────────────────────────────┬───────────┘
                                 │                            │
                       publish   │                            │ EF Core / Npgsql
                                 ▼                            ▼
                       ┌─────────────────┐         ┌──────────────────────┐
                       │ RabbitMQ 3.13   │         │ Postgres 16          │
                       │  exchange       │         │  database "job"      │
                       │  moto.created   │         │  migrations on boot  │
                       └────────┬────────┘         └──────────────────────┘
                                │ consumer (BackgroundService)
                                ▼
                       MotoNotifications (apenas eventos com Year == 2024)
```

---

## Stack & libs

### Backend (`backend/`)

| Categoria          | Tecnologia / Lib                                    | Por quê                                                                 |
|--------------------|-----------------------------------------------------|------------------------------------------------------------------------|
| Runtime            | **.NET 10 / C# 13**                                 | Versão LTS atual, records + primary constructors                        |
| Web framework      | ASP.NET Core Minimal Hosting + Controllers          | Pipeline simples e separação clara entre rotas e regras                 |
| Persistência       | **EF Core 10** + Npgsql                             | Migrations automáticas, fluent mapping, `DateOnly`/`TimeOnly` nativos   |
| Mensageria         | RabbitMQ.Client + `BackgroundService` consumer       | Atende ao requisito com publisher/consumer desacoplados                 |
| Validação          | **FluentValidation**                                | Regras testáveis e isoladas dos comandos                                |
| Resultado funcional| **FluentResults**                                   | Erros tipados (`Result.Fail("...")`) sem usar exceções como controle    |
| Mediator           | **MediatR**                                         | Cada caso de uso é um `IRequestHandler<TCommand, Result>`               |
| Hash de senha      | **BCrypt.Net-Next**                                 | Algoritmo robusto para senha do entregador                              |
| Auth               | JWT Bearer (`Microsoft.AspNetCore.Authentication.JwtBearer`) | Login admin e entregador                                              |
| Documentação       | Swashbuckle (Swagger UI)                            | Confere contrato com o swagger oficial                                  |
| Testes             | xUnit, FluentAssertions, Moq, NetArchTest, Bogus, `Xunit.SkippableFact`, `Microsoft.AspNetCore.Mvc.Testing` | Unit, arquitetura e integração (com Docker quando disponível) |

### Frontend (`frontend/`)

| Categoria          | Tecnologia / Lib                                    | Por quê                                                                 |
|--------------------|-----------------------------------------------------|------------------------------------------------------------------------|
| Framework          | **Angular 18** (standalone components, signals)     | API moderna, sem NgModules, change-detection com signals                |
| Linguagem          | **TypeScript estrito** (`noImplicitAny`, `no-explicit-any: error`) | Contrato forte, evita `any` acidental                                |
| Estilização        | **Tailwind CSS v3**                                 | Utility-first, builds rápidos                                           |
| HTTP / Forms       | `provideHttpClient(withInterceptors(...))`, `FormsModule` | DI funcional, interceptor de auth com signals                        |
| Lint               | ESLint 9 + `angular-eslint` + `typescript-eslint`   | Erro em `any`, regras Angular                                           |
| Testes             | Karma + Jasmine                                     | Specs cobrem serviços HTTP, guards e auth-service                       |
| Servidor produção  | **Nginx 1.27 alpine**                               | Serve a SPA + reverse-proxy `/api/*` e `/uploads/*` (zero CORS)         |

### Infraestrutura

- **PostgreSQL 16** — banco principal
- **RabbitMQ 3.13** (management) — fila `moto.created.year-2024`
- **Docker / Docker Compose** — orquestração

---

## Padrões de projeto

- **Clean Architecture leve** com 4 projetos: `Job.Domain` (entidades + regras puras),
  `Job.Application` (commands, validations, services/handlers, contratos de
  repositório), `Job.Infrastructure` (EF Core, repositórios concretos, storage,
  mensageria) e `Job.WebApi` (controllers, middlewares, composition root).
- **CQRS-lite com MediatR** — cada operação é um `Command`/`Handler`. Não há
  separação física de leitura porque os endpoints de consulta são simples.
- **Result pattern (`FluentResults`)** — handlers retornam `Result` e os
  controllers traduzem para `200/201/400/404`. Exceções são reservadas para
  falhas verdadeiramente inesperadas.
- **Repository pattern** — interfaces em `Job.Application/Repositories`,
  implementações em `Job.Infrastructure/Repositories`. Facilita mocks no unit
  test e mantém a Application livre de EF Core.
- **Validation isolada** com FluentValidation por comando
  (`CreateMotoboyValidation`, `CreateMotoValidation`, `CreateRentalValidation`).
- **Outbox simplificado** — após `CreateAsync` o handler publica
  `MotoCreatedEvent` no RabbitMQ; o consumer (`BackgroundService`) persiste em
  `MotoNotifications` para auditoria.
- **Faker pattern (Bogus)** em `Job.CommonsTest` — fakers reaproveitados por
  unit e integration tests (`CreateMotoboyCommandFaker.Default()`,
  `.Invalid()`, etc.).
- **Reverse proxy no Nginx** — o front consome `/api/*`, evitando CORS em
  produção (em dev, CORS é configurável via `Cors__AllowedOrigins__0`).
- **Auto-geração de identificadores** — `Identifier` é opcional em todos os
  comandos (`Moto`, `Motoboy`, `Rental`); quando ausente, o handler usa
  `Guid.NewGuid().ToString("N")`. O usuário final nunca precisa pensar nisso.

---

## Como executar (Docker)

Pré-requisito: **Docker Desktop** (ou Docker + Compose v2).

```bash
# na raiz do repositório
docker compose up -d --build
```

Aguarde os healthchecks (≈ 30-60 s na primeira execução) e abra:

| URL                                | O que é                                   |
|-----------------------------------|-------------------------------------------|
| http://localhost:4200             | Front-end (SPA Angular + proxy `/api`)    |
| http://localhost:5001/swagger     | Swagger da API direto no backend          |
| http://localhost:15672            | RabbitMQ Management (`guest` / `guest`)   |
| `localhost:5432`                  | Postgres (`postgres` / `postgres`, db `job`) |

Comandos úteis:

```bash
docker compose logs -f core-webapi    # logs da API
docker compose logs -f frontend       # logs do nginx
docker compose down                   # derruba mantendo volumes
docker compose down -v                # derruba e apaga dados
```

A API roda automaticamente as migrations no boot. Imagens da CNH enviadas
em `POST /entregadores/{id}/cnh` são gravadas no volume `uploads-data` e
ficam acessíveis via `http://localhost:4200/uploads/<arquivo>`.

### Credenciais padrão

| Perfil           | Login                       | Senha                  |
|------------------|-----------------------------|------------------------|
| Admin (manager)  | `job@job.com`               | `mudar@123`            |
| Entregador       | criado no fluxo `/cadastro` | definida pelo usuário  |

---

## Como executar local (sem Docker)

```bash
# 1) dependências de infra
docker compose up -d postgres rabbitmq

# 2) backend
cd backend
dotnet run --project src/Job.WebApi    # http://localhost:5050

# 3) frontend (em outro terminal)
cd frontend
npm install
npm start                              # http://localhost:4200
```

O `frontend/src/environments/environment.ts` aponta para
`http://localhost:5050` em dev. Em build de produção, o
`environment.production.ts` é usado e aponta para `/api` (proxy do Nginx).

---

## Endpoints

Todos em `snake_case`. Erros: `400`/`404` com `{ "mensagem": "..." }`.

### `/motos` (admin)

| Método | Rota                  | Descrição                                                                   |
|--------|-----------------------|-----------------------------------------------------------------------------|
| POST   | `/motos`              | Cadastra moto (`identificador` opcional — gerado pelo backend se omitido)   |
| GET    | `/motos?placa=`       | Lista motos (filtro opcional)                                               |
| GET    | `/motos/{id}`         | Busca por identificador                                                     |
| PUT    | `/motos/{id}/placa`   | Atualiza apenas a placa                                                     |
| DELETE | `/motos/{id}`         | Remove moto sem locações                                                    |

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

| Método | Rota                          | Descrição                                 |
|--------|-------------------------------|-------------------------------------------|
| POST   | `/locacao`                    | Aluga moto (planos 7 / 15 / 30 / 45 / 50) |
| GET    | `/locacao/{id}`               | Consulta locação                          |
| PUT    | `/locacao/{id}/devolucao`     | Informa devolução, calcula multa/total    |

Regras de cálculo:

- Diárias: 7 = R$ 30 · 15 = R$ 28 · 30 = R$ 22 · 45 = R$ 20 · 50 = R$ 18.
- Devolução **antes** da previsão → cobra dias usados + multa
  (20% das diárias não usadas no plano de 7, 40% nos demais).
- Devolução **depois** da previsão → cobra todas as diárias do plano
  + R$ 50 por dia adicional.
- Apenas entregador com CNH `A` ou `A+B` pode alugar.

### `/manager`

| Método | Rota                      | Descrição          |
|--------|---------------------------|--------------------|
| POST   | `/manager/authentication` | Login admin (JWT)  |

---

## Mensageria

`MotoService` publica `MotoCreatedEvent` na exchange `moto.created` ao
cadastrar uma moto. O `MotoNotificationConsumer` (BackgroundService)
consome a fila `moto.created.year-2024`, filtra `Year == 2024` e grava
em `MotoNotifications` para auditoria/consulta posterior.

Configurável via `appsettings.json` ou variáveis de ambiente
(`RabbitMq__HostName`, `RabbitMq__Port`, `RabbitMq__UserName`, …).

---

## Testes

Backend:

```bash
cd backend
dotnet test Job.slnx --nologo
```

- **Job.UnitTests** — handlers e regras puras (Moq + FluentAssertions).
- **Job.ArchitectureTest** — NetArchTest garante as fronteiras entre
  Domain / Application / Infrastructure / WebApi.
- **Job.IntegrationTest** — `WebApplicationFactory` + `SkippableFact`.
  Sobe quando há Postgres/RabbitMQ disponível; caso contrário pula
  graciosamente.
- **Job.CommonsTest** — fakers Bogus reutilizáveis.

Frontend:

```bash
cd frontend
npm run lint
npm test -- --watch=false --browsers=ChromeHeadless
```

---

## Estrutura do repositório

```
Desafio-BackEnd/
├── docker-compose.yml          # ← orquestra TUDO (postgres + rabbit + api + front)
├── README.md                   # este arquivo
├── backend/
│   ├── docker-compose.yml      # somente backend + dependências
│   ├── Job.slnx                # solution moderna .NET 10
│   ├── src/
│   │   ├── Job.Domain          # entidades, enums, regras puras
│   │   ├── Job.Application     # commands, validations, services (handlers), contratos
│   │   ├── Job.Infrastructure  # EF Core, repositórios, RabbitMQ, storage local
│   │   └── Job.WebApi          # controllers, Program.cs, middlewares, Dockerfile
│   └── test/
│       ├── Job.UnitTests
│       ├── Job.ArchitectureTest
│       ├── Job.IntegrationTest
│       └── Job.CommonsTest
└── frontend/
    ├── Dockerfile              # multi-stage Node 20 → Nginx 1.27 alpine
    ├── nginx.conf              # SPA fallback + proxy /api e /uploads
    ├── angular.json
    ├── tailwind.config.js
    └── src/
        ├── app/
        │   ├── core/           # ApiClients (motoboy, moto, rental), AuthService, guards, interceptor
        │   └── pages/          # admin, motoboy (login, register, dashboard)
        ├── environments/
        │   ├── environment.ts             # dev → http://localhost:5050
        │   └── environment.production.ts  # prod → /api
        └── styles.scss
```

---

## Decisões de design e mudanças relevantes

Resumo das mudanças mais importantes feitas nesta entrega (além de
implementar o desafio descrito originalmente):

1. **Migração para .NET 10 + `Job.slnx`**
   - `global.json`, `TargetFramework=net10.0` em todos os projetos, troca da
     solution `.sln` por `slnx` (formato XML moderno).

2. **Frontend Angular 18 do zero**
   - Standalone components, signals, `provideHttpClient`, `FormsModule`,
     auth interceptor com signal, guards `adminGuard` e `motoboyGuard`.
   - Tailwind v3 (`postcss.config.js`, `tailwind.config.js`).
   - Specs Karma+Jasmine cobrindo serviços HTTP, guards e auth.
   - ESLint 9 com `@typescript-eslint/no-explicit-any: error` e
     `noImplicitAny: true` no `tsconfig`.

3. **Identificadores opcionais nos comandos**
   - `CreateMotoCommand`, `CreateMotoboyCommand`, `CreateRentalCommand`
     possuem `Identifier? = null` ao final do construtor.
   - As validations correspondentes deixaram de exigir o campo.
   - Os handlers (`MotoService`, `MotoboyService`, `RentalService`) geram
     `Guid.NewGuid().ToString("N")` quando o cliente omite.
   - Por quê: `identificador` era detalhe interno exposto sem necessidade
     no cadastro do usuário final. O Swagger continua aceitando o campo,
     mantendo retrocompatibilidade.

4. **Senha obrigatória do entregador**
   - Campo `senha` (≥ 6 chars) adicionado em `CreateMotoboyCommand`.
   - `MotoboyService.Handle` faz `BCrypt.HashPassword(request.Password, …)`.
   - Removida a constante `DefaultPasswordSeed = "motoboy-default-password"`
     que mascarava a falta de input do usuário.
   - Tela de cadastro do front passou a exigir senha + confirmação.
   - Tela de login não pré-preenche mais nenhuma senha "default".

5. **Docker do front + reverse proxy**
   - Novo `frontend/Dockerfile` (multi-stage Node 20 → Nginx alpine).
   - `frontend/nginx.conf` faz fallback SPA + proxy de `/api/*` e
     `/uploads/*` para `core-webapi:8080`. Resultado: zero CORS em
     produção.
   - `frontend/src/environments/environment.production.ts` aponta para
     `/api`. `angular.json` faz `fileReplacements` no build prod.

6. **`docker-compose.yml` raiz**
   - Sobe Postgres 16, RabbitMQ 3.13, API e front-end Nginx em uma única
     network.
   - Health-checks em Postgres e RabbitMQ + `depends_on: condition: service_healthy`.
   - Volumes nomeados (`postgres-data`, `rabbitmq-data`, `uploads-data`)
     para persistência entre restarts.
   - Configuração via env (`ConnectionStrings__DefaultConnection`,
     `RabbitMq__*`, `Cors__AllowedOrigins__*`).

7. **CORS configurável**
   - `Program.cs` lê `Cors:AllowedOrigins` do `IConfiguration` (fallback
     `http://localhost:4200`) — assim dev local, CI e produção convivem
     sem alterações de código.

8. **Fakers atualizados** (`Job.CommonsTest`)
   - `CreateMotoboyCommandFaker.Default()` agora gera `Password` e
     `Identifier` aleatórios; `.Invalid()` zera a senha para validar a
     regra nova.
   - Reordenação de parâmetros nos fakers de Moto e Rental.
# Desafio backend Mottu.
Seja muito bem-vindo ao desafio backend da Mottu, obrigado pelo interesse em fazer parte do nosso time e ajudar a melhorar a vida de milhares de pessoas.

## Instruções
- O desafio é válido para diversos níveis, portanto não se preocupe se não conseguir resolver por completo.
- A aplicação só será avaliada se estiver rodando, se necessário crie um passo a passo para isso.
- Faça um clone do repositório em seu git pessoal para iniciar o desenvolvimento e não cite nada relacionado a Mottu.
- Após teste realizado, favor encaminha-lo via Link abaixo:
Link: [Formulário - Mottu - Desafio Backend](https://forms.office.com/r/25yMPCax5S)

## Requisitos não funcionais 
- A aplicação deverá ser construida com .Net utilizando C#.
- Utilizar apenas os seguintes bancos de dados (Postgress, MongoDB)
    - Não utilizar PL/pgSQL
- Escolha o sistema de mensageria de sua preferencia( RabbitMq, Sqs/Sns , Kafka, Gooogle Pub/Sub ou qualquer outro)

## Aplicação a ser desenvolvida
Seu objetivo é criar uma aplicação para gerenciar aluguel de motos e entregadores. Quando um entregador estiver registrado e com uma locação ativa poderá também efetuar entregas de pedidos disponíveis na plataforma.

Iremos executar um teste de integração para validar os cenários de uso. Por isso, sua aplicação deve seguir exatamente as especificações de API`s Rest do nosso Swager: request, response e status code.
Garanta que os atributos dos JSON`s e estão de acordo com o Swagger abaixo.

Swagger de referência:
https://app.swaggerhub.com/apis-docs/Mottu/mottu_desafio_backend/1.0.0

### Casos de uso
- Eu como usuário admin quero cadastrar uma nova moto.
  - Os dados obrigatórios da moto são Identificador, Ano, Modelo e Placa
  - A placa é um dado único e não pode se repetir.
  - Quando a moto for cadastrada a aplicação deverá gerar um evento de moto cadastrada
    - A notificação deverá ser publicada por mensageria.
    - Criar um consumidor para notificar quando o ano da moto for "2024"
    - Assim que a mensagem for recebida, deverá ser armazenada no banco de dados para consulta futura.
- Eu como usuário admin quero consultar as motos existentes na plataforma e conseguir filtrar pela placa.
- Eu como usuário admin quero modificar uma moto alterando apenas sua placa que foi cadastrado indevidamente
- Eu como usuário admin quero remover uma moto que foi cadastrado incorretamente, desde que não tenha registro de locações.
- Eu como usuário entregador quero me cadastrar na plataforma para alugar motos.
    - Os dados do entregador são( identificador, nome, cnpj, data de nascimento, número da CNHh, tipo da CNH, imagemCNH)
    - Os tipos de cnh válidos são A, B ou ambas A+B.
    - O cnpj é único e não pode se repetir.
    - O número da CNH é único e não pode se repetir.
- Eu como entregador quero enviar a foto de minha cnh para atualizar meu cadastro.
    - O formato do arquivo deve ser png ou bmp.
    - A foto não poderá ser armazenada no banco de dados, você pode utilizar um serviço de storage( disco local, amazon s3, minIO ou outros).
- Eu como entregador quero alugar uma moto por um período.
    - Os planos disponíveis para locação são:
        - 7 dias com um custo de R$30,00 por dia
        - 15 dias com um custo de R$28,00 por dia
        - 30 dias com um custo de R$22,00 por dia
        - 45 dias com um custo de R$20,00 por dia
        - 50 dias com um custo de R$18,00 por dia
    - A locação obrigatóriamente tem que ter uma data de inicio e uma data de término e outra data de previsão de término.
    - O inicio da locação obrigatóriamente é o primeiro dia após a data de criação.
    - Somente entregadores habilitados na categoria A podem efetuar uma locação
- Eu como entregador quero informar a data que irei devolver a moto e consultar o valor total da locação.
    - Quando a data informada for inferior a data prevista do término, será cobrado o valor das diárias e uma multa adicional
        - Para plano de 7 dias o valor da multa é de 20% sobre o valor das diárias não efetivadas.
        - Para plano de 15 dias o valor da multa é de 40% sobre o valor das diárias não efetivadas.
    - Quando a data informada for superior a data prevista do término, será cobrado um valor adicional de R$50,00 por diária adicional.
    

## Diferenciais 🚀

- Testes unitários
- Testes de integração
- EntityFramework e/ou Dapper
- Docker e Docker Compose
- Design Patterns
- Documentação
- Tratamento de erros
- Arquitetura e modelagem de dados
- Código escrito em língua inglesa
- Código limpo e organizado
- Logs bem estruturados
- Seguir convenções utilizadas pela comunidade