# Protótipo API Backend com .NET 8, MongoDB e Amazon S3

Este projeto é uma **API REST** desenvolvida em **.NET 8** (ASP.NET Core). Ela faz parte de uma plataforma de **curadoria e divulgação de artesãos**, permitindo cadastro e consulta de **Usuários**, **Artesãos** e **Artesanatos**, com suporte a **upload de imagens para Amazon S3** e **autenticação via JWT**.

A solução segue uma **arquitetura em camadas**, separando responsabilidades em projetos distintos.

---

## API em produção (Azure)

- **Base URL:** `https://prototipocuradoriaapi.azurewebsites.net`
- **Swagger (UI):** `https://prototipocuradoriaapi.azurewebsites.net/swagger/index.html`
- **Swagger (OpenAPI JSON):** `https://prototipocuradoriaapi.azurewebsites.net/swagger/v1/swagger.json`
- **Status:** `GET /` em `https://prototipocuradoriaapi.azurewebsites.net/`

---

## Contexto e problema

Artesãos e produtores locais, em muitos casos, dependem de redes sociais e divulgação informal para apresentar seus trabalhos. Esse cenário costuma gerar alguns problemas:

- **Baixa encontrabilidade:** é difícil localizar artesãos por critérios objetivos (ex.: nicho de atuação, disponibilidade para encomendas, cidade/região, tipo de produto).
- **Falta de padronização das informações:** descrições, preços, fotos e dados de contato podem ficar incompletos, desatualizados ou espalhados em múltiplos canais.
- **Dificuldade de curadoria e organização:** para iniciativas de curadoria/divulgação (feiras, projetos culturais, catálogos), consolidar e validar dados manualmente é demorado.
- **Gestão de imagens pouco escalável:** armazenar fotos de artesanatos diretamente no servidor pode aumentar custo, consumo de banda e complexidade de infraestrutura.

Essas limitações reduzem o alcance do trabalho dos artesãos e tornam mais difícil construir uma plataforma que organize e divulgue esse conteúdo de forma consistente.

---

## Solução proposta

Esta API REST é o backend de uma plataforma de **curadoria e divulgação de artesãos**, oferecendo uma base centralizada e padronizada para cadastro e consulta de dados.

Principais pontos da solução:

- **Cadastro e consulta de artesãos e artesanatos**, permitindo manter informações organizadas e consistentes.
- **Busca e filtros** para apoiar encontrabilidade (ex.: busca por nome e filtros de atuação/disponibilidade).
- **Autenticação e autorização via JWT**, permitindo controle de acesso por perfis (roles), com suporte a políticas (ex.: Administrador).
- **Armazenamento de imagens no Amazon S3**, tornando o upload/consulta mais escalável e desacoplando mídia do servidor de aplicação.
- **Persistência em MongoDB**, facilitando flexibilidade de modelagem e evolução do domínio.
- **Documentação e testes via Swagger**, facilitando validação e integração com o frontend.

Em resumo, o backend resolve o problema de **centralizar**, **padronizar** e **disponibilizar** os dados necessários para que o frontend consiga apresentar e divulgar artesãos e seus produtos de forma organizada, segura e escalável.

---

## Arquitetura (camadas / projetos)

A solution (`PrototipoBackEnd.sln`) é composta por:

- **PrototipoBackEnd.API (Presentation)**
  - Controllers (endpoints HTTP)
  - Configuração do pipeline (CORS, Swagger, Auth)
  - Autenticação JWT Bearer e Authorization por policy/role

- **PrototipoBackEnd.Application**
  - DTOs (objetos de transporte)
  - Interfaces de serviços (contratos usados pela API)
  - Mapeamentos (AutoMapper)

- **PrototipoBackEnd.Domain**
  - Entidades (ex.: `Usuario`)
  - Enums (ex.: roles/perfis)
  - Interfaces (contratos do domínio / serviços)

- **PrototipoBackEnd.Infrastructure**
  - MongoDB Context
  - Implementações concretas (ex.: `TokenService`, `AmazonS3Service`)
  - Configurações e IoC (injeção de dependência)

- **PrototipoBackEnd.Tests**
  - Estrutura inicial para testes (Moq)

---

## Tecnologias utilizadas

- **.NET 8 / ASP.NET Core**
- **MongoDB** (via `MongoDB.Driver`)
- **JWT (Bearer Tokens)** para autenticação
- **Swagger (OpenAPI)** para documentação e testes
- **AutoMapper** para mapeamento entre entidade e DTO
- **Amazon S3** para armazenamento de arquivos/imagens
- **BCrypt** para hash de senha (dependência presente na Application)

---

## Funcionalidades já implementadas (visão geral)

### Autenticação e autorização (JWT)
- A API utiliza **JWT Bearer**.
- Tokens são gerados pelo `TokenService` (Infrastructure), contendo claims como **Nome**, **Email** e **Role**.
- Existe policy de autorização para **Administrador**.

### Persistência em MongoDB
- Existe `MongoDbContext` responsável por:
  - Conectar no MongoDB (`ConnectionString`)
  - Selecionar o banco (`DatabaseName`)
  - Obter collections tipadas via `GetCollection<T>()`

### Upload de imagens no Amazon S3
- Existe `AmazonS3Service` com operações:
  - Upload
  - Delete
  - FileExists
  - Download (GetFile)
  - ListFiles

### Documentação com Swagger
- Swagger configurado com suporte a **Bearer JWT**.

### CORS para o Frontend
- Política `"Frontend"` permitindo:
  - `http://localhost*` (dev)
  - subdomínios `*.vercel.app` do projeto (quando contém `prototipo-frontend`)

---

## Endpoints principais

> Base: os controllers usam `[Route("[controller]")]`, então a rota base é o nome do controller.

### Auth
- `POST /Auth/login`  
  Faz login e retorna o resultado (ex.: token), via `IAuthService.Login(LoginRequestDto)`.

### Usuário
- `GET /Usuario/BuscarTodos`
- `GET /Usuario/BuscarPorId/{id}`
- `POST /Usuario/Adicionar` (via `multipart/form-data` com `UsuarioDto`)
- `PUT /Usuario/Atualizar/{id}` (via `multipart/form-data` com `UsuarioDto`)
- `DELETE /Usuario/Excluir/{id}`

### Artesão
- `GET /Artesao/BuscarTodos`
  - filtros por querystring:
    - `nome` (string)
    - `nichoAtuacao` (string)
    - `receberEncomendas` (bool)
    - `enviaEncomendas` (bool)
- `GET /Artesao/BuscarPorId/{id}`
- `POST /Artesao/Adicionar` (via `multipart/form-data` com `ArtesaoDto` + `imagem`)
- `PUT /Artesao/Atualizar/{id}` (via `multipart/form-data` com `ArtesaoDto` + `imagem` opcional)
- `DELETE /Artesao/Excluir/{id}`

### Artesanato
- `GET /Artesanato/BuscarTodos`
- `GET /Artesanato/BuscarPorId/{id}`
- `GET /Artesanato/PorArtesao/{artesaoId}`
- `GET /Artesanato/TodosPorArtesao/{artesaoId}`
- `POST /Artesanato/Adicionar` (via `multipart/form-data` com `ArtesanatoDto` + `imagem[]`)
- `PUT /Artesanato/Atualizar/{id}` (via `multipart/form-data` com `ArtesanatoDto`)
- `DELETE /Artesanato/Excluir/{id}`

---

## Como rodar o projeto (desenvolvimento)

### Pré-requisitos
- **.NET SDK 8**
- Acesso a um **MongoDB** (local ou Atlas)
- Credenciais/config para **AWS S3** (variáveis de ambiente, profile AWS, ou configuração equivalente)

### Executar a API
Dentro da pasta do projeto:

```bash
dotnet restore
dotnet run --project PrototipoBackEnd.API
```

Por padrão, o `launchSettings.json` abre o navegador em `/swagger`.

---

## Configuração (exemplo de appsettings.json)

Crie um arquivo `appsettings.json` (ou configure via variáveis de ambiente) com as chaves usadas no código:

```json
{
  "MongoDbSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "PrototipoBackEnd"
  },
  "Jwt": {
    "Key": "SUA_CHAVE_FORTE_AQUI",
    "Issuer": "PrototipoBackEnd",
    "Audience": "PrototipoBackEnd"
  },
  "AWSSettings": {
    "BucketName": "seu-bucket-aqui"
  },
  "EnableSwaggerInProduction": false
}
```

> Importante:
> - `Jwt:Key` deve ser uma chave forte (não usar valor padrão).
> - O AWS SDK geralmente lê credenciais via variáveis/arquivo de credenciais. Garanta que a aplicação tenha acesso.

---

## Usando o Swagger com JWT

1. Faça login em `POST /Auth/login`
2. Copie o token retornado
3. No Swagger, clique em **Authorize** e cole:
   - `Bearer SEU_TOKEN_AQUI`

---

## Observações / melhorias previstas

- Consolidar a configuração de CORS no pipeline (garantir uso consistente da policy `"Frontend"`).
- Expandir validações (pasta `Validators` existe na Application, mas ainda está em evolução).
- Ampliar cobertura de testes (`PrototipoBackEnd.Tests`).

---

## Próximos passos (roadmap)
- Implementar/validar regras de negócio e validações (FluentValidation ou similar).
- Melhorar documentação dos DTOs e exemplos de request/response.
- Adicionar CI (GitHub Actions) e padronização (format/lint).
