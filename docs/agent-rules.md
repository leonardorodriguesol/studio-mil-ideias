# Agent Rules -- Studio Mil Ideias

Este documento define as regras globais obrigatórias para todos os
agentes do projeto.

------------------------------------------------------------------------

# 1. Fonte de Verdade

Todos os agentes DEVEM seguir obrigatoriamente:

/docs/architecture/StudioMilIdeias_Arquitetura_v1.md

Nenhum agente pode alterar arquitetura, estrutura de camadas ou modelo
de domínio sem atualização formal do documento principal.

------------------------------------------------------------------------

# 2. Separação de Responsabilidades

Cada agente só pode atuar dentro do seu escopo:

-   Backend Agent → /src/backend
-   Frontend Agent → /src/frontend
-   Database Agent → EF Core / Migrations
-   Payment Agent → Integrações Stripe
-   DevOps Agent → Docker / CI/CD
-   QA Agent → Testes
-   Orchestrator → Planejamento

É proibido modificar código fora do escopo definido.

------------------------------------------------------------------------

# 3. Padrões de Código

Backend: - Clean Architecture obrigatória - Sem lógica de negócio em
Controllers - Services na camada Application - Regras de negócio no
Domain - Dependency Injection obrigatória

Frontend: - Componentização obrigatória - Separação de pages e
components - Estado global via Zustand (ou equivalente definido) -
Nenhuma regra crítica de negócio no frontend

Banco: - Toda alteração de entidade exige migration - Índices devem ser
considerados para performance

------------------------------------------------------------------------

# 4. Convenção de Commits

Padrão obrigatório:

feat(auth): descrição feat(products): descrição fix(cart): descrição
refactor(domain): descrição test(auth): descrição chore(devops):
descrição

Cada agente deve gerar commits organizados por feature.

------------------------------------------------------------------------

# 5. Ordem de Execução

Fluxo padrão de implementação:

1.  @orchestrator planeja
2.  @backend implementa base
3.  @database gera migration
4.  @frontend integra
5.  @payment integra pagamento (se necessário)
6.  @qa cria testes
7.  @devops valida ambiente

Nenhuma feature deve pular etapas.

------------------------------------------------------------------------

# 6. Segurança Obrigatória

-   Senhas com hash seguro (BCrypt ou superior)
-   JWT configurado corretamente
-   Validação de entrada com validações explícitas
-   Webhooks devem validar assinatura
-   HTTPS obrigatório em produção

------------------------------------------------------------------------

# 7. Proibição de Código Experimental

Nenhum agente deve: - Criar código temporário - Inserir comentários TODO
sem justificativa - Criar lógica duplicada - Ignorar regras de
arquitetura

------------------------------------------------------------------------

# 8. Evolução da Arquitetura

Se uma mudança estrutural for necessária:

1.  @orchestrator deve propor alteração
2.  Documento de arquitetura deve ser atualizado
3.  Somente depois os agentes podem implementar

------------------------------------------------------------------------

Este documento é vinculante para todos os agentes.
