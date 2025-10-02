# Valid - Test Técnico

Este repositório contém a POC para do teste técnico de um sistema WinForms legado em uma arquitetura com **.NET 8, PostgreSQL, RabbitMQ, Keycloak (OIDC)** e **React**.

---

## Stack Utilizada
- **.NET 8** (API REST + Worker)
- **PostgreSQL 15** (com procedures)
- **RabbitMQ** (mensageria)
- **Keycloak** (OIDC/JWT)
- **React + Vite** (frontend)
- **Docker Compose** (orquestração)
- **GitHub Actions** (CI/CD)

---

## Estrutura do Projeto


## ✅ Checklist do Projeto - Desafio Técnico

### Estrutura e setup
- [x] Criar solução .NET 8
- [x] Criar projetos: API, Worker, LegacySimulator
- [x] Criar projeto React + Vite + TypeScript
- [x] Criar Docker Compose com:
  - [x] PostgreSQL
  - [x] RabbitMQ
  - [x] Keycloak
  - [x] API
  - [x] Worker
  - [x] Frontend

### Banco de dados
- [x] Criar tabelas: Clientes, Pedidos
- [x] Criar procedures
- [x] Scripts de seed/teste

### API (.NET 8)
- [ ] Configurar JWT Bearer com Keycloak
- [ ] Implementar endpoints Clientes (CRUD)
- [ ] Implementar endpoints Pedidos (POST + publicar RabbitMQ)
- [ ] Implementar endpoint PUT para status via procedure
- [ ] Testes unitários (mínimo 10)
- [ ] Testes de integração (mínimo 5)
- [ ] Healthcheck e Swagger

### Worker
- [ ] Configurar consumo de fila RabbitMQ (pedidos)
- [ ] Processamento de mensagens
- [ ] Logs e tratamento de erros
- [ ] Testes unitários/integrados do Worker

### Frontend (React)
- [ ] Configurar login OIDC com Keycloak
- [ ] Criar páginas Clientes e Pedidos (CRUD)
- [ ] Integração com API
- [ ] Roteamento com React Router
- [ ] Layout básico e validações

### Testes & Insomnia/Postman
- [ ] Criar coleção para teste da API
- [ ] Testar endpoints protegidos com JWT

### Documentação
- [ ] ADRs (Architecture Decision Records)
- [ ] Diagramas (C4, sequence, fluxo de pedidos)
- [ ] README completo com instruções de setup

### CI/CD
- [ ] Pipeline GitHub Actions (build, testes, lint)
- [ ] Deploy/local docker dev
