# 📘 Documento de Arquitetura

## Projeto: Studio Mil Ideias

Autor: Leonardo Rodrigues\
Stack: .NET 8 + React / Next.js

------------------------------------------------------------------------

# 1. Visão Geral

## 1.1 Objetivo

Criar uma plataforma de e-commerce de produtos digitais, com:

-   Catálogo de produtos
-   Carrinho de compras
-   Checkout com pagamento online
-   Autenticação de usuários
-   Painel administrativo
-   Entrega digital pós-compra

------------------------------------------------------------------------

# 2. Arquitetura Geral

Frontend (React / Next.js)\
↓\
Backend API (.NET 8 Web API)\
↓\
Banco de Dados (PostgreSQL)\
↓\
Serviços externos (Stripe, Email, Storage)

Arquitetura baseada em Clean Architecture simplificada.

------------------------------------------------------------------------

# 3. Estrutura do Repositório

    studio-mil-ideias
    │
    ├── src
    │   ├── backend
    │   │   ├── StudioMilIdeias.Api
    │   │   ├── StudioMilIdeias.Application
    │   │   ├── StudioMilIdeias.Domain
    │   │   ├── StudioMilIdeias.Infrastructure
    │   │
    │   ├── frontend
    │       ├── studio-web
    │
    ├── docker
    ├── docs
    └── README.md

------------------------------------------------------------------------

# 4. Backend (.NET 8)

## Camadas

### Domain

-   Entidades
-   Regras de negócio
-   Interfaces

### Application

-   DTOs
-   Casos de uso
-   Serviços
-   Validações

### Infrastructure

-   EF Core
-   Banco de dados
-   JWT
-   Stripe
-   Email

### API

-   Controllers
-   Middlewares
-   Configuração

------------------------------------------------------------------------

# 5. Modelo de Domínio

## User

-   Id (Guid)
-   Name
-   Email
-   PasswordHash
-   Role (Admin / Customer)
-   CreatedAt

## Product

-   Id
-   Name
-   Description
-   Price
-   Slug
-   IsActive
-   CategoryId
-   DigitalFileUrl
-   CreatedAt

## Category

-   Id
-   Name
-   Slug

## Cart

-   Id
-   UserId

## CartItem

-   Id
-   CartId
-   ProductId
-   Quantity
-   UnitPrice

## Order

-   Id
-   UserId
-   TotalAmount
-   Status (Pending / Paid / Cancelled)
-   PaymentProvider
-   PaymentId
-   CreatedAt

## OrderItem

-   Id
-   OrderId
-   ProductId
-   Quantity
-   UnitPrice

------------------------------------------------------------------------

# 6. Banco de Dados

-   PostgreSQL
-   EF Core Code First
-   Migrations versionadas
-   Seed inicial com Admin padrão

------------------------------------------------------------------------

# 7. Autenticação

-   JWT
-   BCrypt para senha
-   Rotas protegidas por Role

------------------------------------------------------------------------

# 8. Pagamentos

-   Stripe
-   Endpoint /checkout cria sessão
-   Webhook confirma pagamento
-   Atualiza Order para Paid

------------------------------------------------------------------------

# 9. Entrega Digital

-   Arquivos armazenados em bucket privado
-   Backend gera URL temporária segura após pagamento

------------------------------------------------------------------------

# 10. Frontend

Recomendação: Next.js para melhor SEO.

Estrutura sugerida:

    /pages
      /login
      /register
      /products
      /product/[slug]
      /cart
      /checkout
      /orders
      /admin

------------------------------------------------------------------------

# 11. APIs Principais

## Auth

POST /auth/register\
POST /auth/login

## Products

GET /products\
GET /products/{id}\
POST /products\
PUT /products/{id}\
DELETE /products/{id}

## Cart

GET /cart\
POST /cart/items\
DELETE /cart/items/{id}

## Checkout

POST /checkout\
POST /webhook/stripe

## Orders

GET /orders\
GET /orders/{id}

------------------------------------------------------------------------

# 12. Infraestrutura

Inicial: - Docker Compose (API + PostgreSQL) - GitHub Actions CI

Produção: - Azure / AWS - CDN - HTTPS obrigatório

------------------------------------------------------------------------

# 13. Roadmap

Fase 1 -- Auth + Estrutura\
Fase 2 -- CRUD Produto\
Fase 3 -- Carrinho\
Fase 4 -- Checkout Stripe\
Fase 5 -- Entrega Digital\
Fase 6 -- Admin Panel

------------------------------------------------------------------------

# 14. Estratégia

Começar com MVP funcional: 1. Auth 2. Produto 3. Checkout 4. Download
protegido

Escalar depois.
