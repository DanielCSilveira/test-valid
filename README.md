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


## SETUP
### KeyCloak
Setup necessário para execução local
- **Realm** - crie um novo realm chamado valid
- **Client** - Crie um cliente chamado valid-api
-- configure Clients>valid-api>Client Scopes>valid_api_dedicated>add mapper (By configuration):
Name: Valid Client Audience
Mapper Type: Audience
Included Client Audience: valid-api (Seu Client ID)
Add to Access Token: ON (Ligado)
Add to ID Token: ON (Ligado)


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
- [x] Configurar JWT Bearer com Keycloak
- [x] Implementar endpoints Clientes (CRUD)
- [x] Implementar endpoints Pedidos (POST + publicar RabbitMQ)
- [x] Implementar endpoint PUT para status via procedure
- [x] Testes unitários (mínimo 10)
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

### PLUS - Publish the project
- [ ] Publish the Base Infra - compose infra
- [ ] Publish API
- [ ] Publish Front
- [ ] Publish Worker
 
