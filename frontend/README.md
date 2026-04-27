# Frontend — Desafio Mottu

SPA em **Angular 18** (standalone components + signals) para o desafio
de aluguel de motos. Consome a API .NET 10 do backend.

> Para a visão geral do projeto (backend, mensageria, docker compose),
> veja o [README na raiz](../README.md).

## Stack & libs

| Categoria         | Lib                                                                  |
|-------------------|----------------------------------------------------------------------|
| Framework         | Angular 18 (standalone components, signals)                          |
| Linguagem         | TypeScript estrito (`noImplicitAny: true`)                           |
| Estilo            | Tailwind CSS v3                                                      |
| HTTP              | `provideHttpClient(withInterceptors([authInterceptor]))`             |
| Forms             | `FormsModule` (template-driven)                                      |
| Lint              | ESLint 9 + `angular-eslint` + `typescript-eslint` (`no-explicit-any: error`) |
| Testes            | Karma + Jasmine                                                      |
| Servidor produção | Nginx 1.27 alpine (com reverse proxy `/api` e `/uploads`)            |

## Padrões aplicados

- **Standalone components + signals** — sem NgModules; `signal()` para
  estado local e `computed()` para derivações.
- **Functional auth interceptor** — interceptor lê o token do
  `AuthService.token()` (signal) e injeta `Authorization: Bearer`.
- **Functional guards** — `adminGuard` e `motoboyGuard` baseados em signals.
- **API clients tipados** — interfaces em `core/api/*.ts` (motoboy, moto,
  rental); zero `any`.
- **Reverse-proxy em produção** — `environment.production.ts` usa `/api`
  e o Nginx faz o repasse — assim, zero CORS em prod.

## Como executar

### Docker (junto com backend, recomendado)

```bash
# raiz do repositório
docker compose up -d --build
# abra http://localhost:4200
```

O front-end é buildado em multi-stage (Node 20 → Nginx 1.27 alpine) e
servido junto com o backend numa única `docker network`.

### Local (dev server)

```bash
npm install
npm start          # http://localhost:4200, aponta p/ http://localhost:5050
```

Pré-requisito para o dev server consumir a API: o backend rodando em
`http://localhost:5050` (ex.: `dotnet run --project ../backend/src/Job.WebApi`).

### Build de produção

```bash
npm run build      # gera dist/frontend/browser/
```

`angular.json` faz `fileReplacements` trocando `environment.ts` por
`environment.production.ts` (`apiUrl: '/api'`), de modo que o bundle final
sempre dependa do reverse-proxy do Nginx.

## Lint & testes

```bash
npm run lint
npm test -- --watch=false --browsers=ChromeHeadless
```

## Estrutura

```
frontend/
├── Dockerfile                 # multi-stage Node 20 → Nginx 1.27 alpine
├── nginx.conf                 # SPA fallback + proxy /api e /uploads
├── angular.json
├── tailwind.config.js
└── src/
    ├── app/
    │   ├── core/
    │   │   ├── api/           # MotoboyApi, MotoApi, RentalApi
    │   │   ├── auth.service.ts
    │   │   ├── auth.interceptor.ts
    │   │   └── guards/        # adminGuard, motoboyGuard
    │   └── pages/
    │       ├── admin/         # admin-motos
    │       └── motoboy/       # login, register, dashboard
    ├── environments/
    │   ├── environment.ts             # dev → http://localhost:5050
    │   └── environment.production.ts  # prod → /api
    └── styles.scss
```

## Decisões relevantes

- **Senha do entregador** é exigida no formulário de cadastro
  (`/motoboy/cadastro`), com confirmação de senha. A tela de login não
  pré-preenche nenhuma senha "padrão".
- **Identificador da locação/moto** não é mais pedido ao usuário; quem
  gera é o backend. O dashboard usa o `identificador` retornado pela
  API (`RentalApi.create()` retorna `Observable<Rental>`).
- **Tema visual Mottu** — paleta verde/preto via Tailwind, logo SVG na
  toolbar, favicon próprio e `theme-color` definido em `index.html`.
# Frontend

This project was generated with [Angular CLI](https://github.com/angular/angular-cli) version 18.2.21.

## Development server

Run `ng serve` for a dev server. Navigate to `http://localhost:4200/`. The application will automatically reload if you change any of the source files.

## Code scaffolding

Run `ng generate component component-name` to generate a new component. You can also use `ng generate directive|pipe|service|class|guard|interface|enum|module`.

## Build

Run `ng build` to build the project. The build artifacts will be stored in the `dist/` directory.

## Running unit tests

Run `ng test` to execute the unit tests via [Karma](https://karma-runner.github.io).

## Running end-to-end tests

Run `ng e2e` to execute the end-to-end tests via a platform of your choice. To use this command, you need to first add a package that implements end-to-end testing capabilities.

## Further help

To get more help on the Angular CLI use `ng help` or go check out the [Angular CLI Overview and Command Reference](https://angular.dev/tools/cli) page.
