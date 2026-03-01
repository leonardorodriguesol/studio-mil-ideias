# Studio Mil Ideias

Plataforma de e-commerce para produtos digitais.

## Stack

- Backend: .NET 8 Web API (Clean Architecture simplificada)
- Frontend: React com recomendacao de Next.js
- Banco de dados: PostgreSQL
- Integracoes previstas: Stripe, Email, Storage

## Estrutura inicial

```text
studio-mil-ideias
├── docker
│   └── docker-compose.yml
├── docs
├── src
│   ├── backend
│   │   ├── StudioMilIdeias.sln
│   │   ├── StudioMilIdeias.Api
│   │   ├── StudioMilIdeias.Application
│   │   ├── StudioMilIdeias.Domain
│   │   └── StudioMilIdeias.Infrastructure
│   └── frontend
│       └── .gitkeep
└── README.md
```

## Diretrizes do projeto

- Fonte de verdade da arquitetura:
  - `docs/architecture/StudioMilIdeias_Arquitetura_v1.md`
- Regras globais dos agentes:
  - `docs/agent-rules.md`

## Roadmap (alto nivel)

1. Auth + estrutura base
2. CRUD de produtos
3. Carrinho
4. Checkout com Stripe
5. Entrega digital pos-compra
6. Painel administrativo

## Proximos passos sugeridos

1. Inicializar app Next.js em `src/frontend/studio-web`.
2. Expandir cobertura de testes automatizados (unitarios/integracao) para aumentar o valor da pipeline.

## CI (GitHub Actions)

- Workflow: `.github/workflows/backend-ci.yml`
- Executa em `push`/`pull_request` para `main` (quando houver mudancas em `src/backend`):
  - `dotnet restore`
  - `dotnet build` (Release)
  - `dotnet test`
- Em `pull_request`, roda tambem `Dependency Review` para revisar riscos de dependencias.

## Backend setup atual

- Solucao: `src/backend/StudioMilIdeias.sln`
- Projetos:
  - `StudioMilIdeias.Api` (ASP.NET Core Web API)
  - `StudioMilIdeias.Application` (Class Library)
  - `StudioMilIdeias.Domain` (Class Library)
  - `StudioMilIdeias.Infrastructure` (Class Library)
- Referencias entre camadas:
  - `Application -> Domain`
  - `Infrastructure -> Application + Domain`
  - `Api -> Application + Infrastructure`

### Rodar backend localmente

1. Subir PostgreSQL com Docker:

```bash
docker compose -f docker/docker-compose.yml up -d
```

2. Iniciar a API:

```bash
cd src/backend/StudioMilIdeias.Api
dotnet run
```

Health check disponivel em `GET /health`.

Endpoints iniciais:
- `POST /auth/register`
- `POST /auth/login`
- `GET /products`
- `GET /products/{id}`
- `POST /products` (Admin)
- `PUT /products/{id}` (Admin)
- `DELETE /products/{id}` (Admin)
- `GET /cart` (Auth)
- `POST /cart/items` (Auth)
- `DELETE /cart/items/{id}` (Auth)
- `POST /checkout` (Auth)
- `POST /webhook/stripe` (Webhook Stripe)
- `GET /orders` (Auth)
- `GET /orders/{id}` (Auth)
- `GET /orders/{orderId}/items/{productId}/download-link` (Auth)
- `GET /downloads/{token}` (Link temporario)

### Swagger + JWT

- Acesse `/swagger` com a API rodando.
- Gere um token em `POST /auth/login`.
- Clique em `Authorize` e informe: `Bearer {seu_token}`.

### Stripe (Checkout)

- Configure em `appsettings.json`:
  - `Stripe:SecretKey`
  - `Stripe:WebhookSecret`
  - `Stripe:Currency` (ex.: `brl`)
- Fluxo:
  - `POST /checkout` cria sessao de pagamento no Stripe.
  - `POST /webhook/stripe` confirma pagamento e marca pedido como `Paid`.

### Entrega digital

- Configure em `appsettings.json`:
  - `DigitalDelivery:TokenSecret`
  - `DigitalDelivery:TokenTtlMinutes`
- Fluxo:
  - Usuario autenticado solicita `GET /orders/{orderId}/items/{productId}/download-link`.
  - API gera token temporario assinado.
  - `GET /downloads/{token}` resolve o recurso digital apenas para pedido pago (retornando a URL do recurso).
