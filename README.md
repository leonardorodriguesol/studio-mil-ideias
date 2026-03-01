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
2. Adicionar `docker-compose` para API + PostgreSQL.

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

```bash
cd src/backend/StudioMilIdeias.Api
dotnet run
```

Health check disponivel em `GET /health`.
