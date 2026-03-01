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
│   │   └── .gitkeep
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

1. Criar a solucao .NET em `src/backend` com os projetos:
   - `StudioMilIdeias.Api`
   - `StudioMilIdeias.Application`
   - `StudioMilIdeias.Domain`
   - `StudioMilIdeias.Infrastructure`
2. Inicializar app Next.js em `src/frontend/studio-web`.
3. Adicionar `docker-compose` para API + PostgreSQL.
