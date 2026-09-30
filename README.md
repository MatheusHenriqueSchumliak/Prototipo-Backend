# PrototipoBackEnd

API REST da plataforma de curadoria e divulgação de artesãos e artesanatos. O
backend foi refatorado em .NET 8 com separação entre domínio, aplicação,
infraestrutura e apresentação, utilizando MongoDB para persistência, Amazon S3
para imagens e JWT para autenticação.

## Visão geral

A API centraliza informações de pessoas, usuários, artesãos e artesanatos para
que o frontend possa consultar, cadastrar e atualizar os dados da plataforma.
Entre os recursos disponíveis estão:

- autenticação de usuários por login e token JWT;
- cadastro, consulta, atualização e exclusão de usuários;
- cadastro e consulta de artesãos com filtros;
- cadastro e consulta de artesanatos relacionados a um artesão;
- upload de imagens de artesãos e artesanatos para o Amazon S3;
- documentação interativa dos endpoints por Swagger/OpenAPI;
- soft delete e controle de status das entidades no domínio.

## Arquitetura da solução

A solução `PrototipoBackEnd.sln` é organizada nas seguintes camadas:

```text
PrototipoBackEnd.API
├── Controllers/
├── Extensions/
├── WebApplicationExtensions/
└── Program.cs

PrototipoBackEnd.Application
├── Common/
├── Dtos/
├── Interfaces/
└── Services/

PrototipoBackEnd.Domain
├── Entities/
├── Enumerables/
├── Interfaces/
└── ValueObjects/

PrototipoBackEnd.Infrastructure
├── Configurations/
├── Context/
├── IoC/
├── Mapping/
├── Repository/
├── Security/
└── Services/
```

### `PrototipoBackEnd.API`

Camada de apresentação HTTP. Contém os controllers, o pipeline da aplicação,
configurações de CORS, Swagger, autenticação e autorização.

### `PrototipoBackEnd.Application`

Camada de casos de uso. Contém os DTOs, contratos de aplicação e serviços que
orquestram as operações de usuários, artesãos, artesanatos, autenticação e
pessoas.

### `PrototipoBackEnd.Domain`

Núcleo do sistema, independente da infraestrutura. Contém as entidades
`Pessoa`, `Usuario`, `Artesao` e `Artesanato`, a classe base das entidades,
value objects, enums e interfaces de repositórios e serviços.

Os relacionamentos principais são:

```text
Pessoa
├── Usuario
└── Artesao
    └── Artesanato
```

As entidades usam identificadores string baseados em GUID e possuem os campos
de controle definidos em `EntityBase`, como `DataCriacao`,
`DataAtualizacao`, `DataRemocao` e `IsAtivo`.

### `PrototipoBackEnd.Infrastructure`

Implementa os contratos definidos no domínio e na aplicação:

- `MongoDbContext` e repositórios genéricos/específicos;
- configuração e criação do cliente MongoDB;
- `AmazonS3Service` para operações com arquivos;
- `TokenService` para geração de tokens JWT;
- registro das dependências por injeção de dependência.

### `PrototipoBackEnd.Tests`

Reúne testes automatizados:

- testes unitários dos serviços;
- testes de integração dos serviços e repositórios em memória.

## Tecnologias

- .NET 8 e ASP.NET Core;
- MongoDB.Driver 3.4;
- Amazon S3 por meio do AWSSDK.S3;
- JWT Bearer;
- Swagger/OpenAPI com Swashbuckle;
- BCrypt.Net-Next para hash de senhas;
- xUnit e Moq para testes.

## Pré-requisitos

- .NET SDK 8;
- MongoDB local ou MongoDB Atlas;
- bucket do Amazon S3, caso os endpoints de imagem sejam utilizados;
- credenciais AWS configuradas de forma segura.

## Configuração

Não versione credenciais reais, chaves JWT ou strings de conexão. Prefira
variáveis de ambiente, `dotnet user-secrets` ou um arquivo de configuração
local fora do controle de versão.

Exemplo de configuração mínima:

```json
{
  "MongoDBSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "RefactorCuradoriaArtesanato"
  },
  "AWS": {
    "Region": "us-east-1",
    "BucketName": "seu-bucket"
  },
  "Jwt": {
    "Key": "uma-chave-secreta-com-pelo-menos-32-caracteres",
    "Issuer": "Administrador",
    "Audience": "prototipo-TCC2024"
  }
}
```

### Atenção ao nome da seção do MongoDB

O registro de dependências da infraestrutura faz binding da seção
`MongoDBSettings` (com `DB` em maiúsculas). Portanto, essa é a nomenclatura
esperada para a configuração consumida por `MongoDbSettings`.

O arquivo `PrototipoBackEnd.API/appsettings.json` da branch ainda utiliza
`MongoDbSettings` em alguns pontos. Ao executar a API, alinhe a chave do
arquivo de configuração com `MongoDBSettings` ou ajuste o binding antes de
utilizar o MongoDB.

As propriedades esperadas são:

| Seção | Propriedades |
| --- | --- |
| `MongoDBSettings` | `ConnectionString`, `DatabaseName` |
| `AWS` | `AccessKey`, `SecretKey`, `Region`, `BucketName` |
| `Jwt` | `Key`, `Issuer`, `Audience` |

As credenciais AWS podem ser fornecidas pelo mecanismo padrão de credenciais
da AWS. Se `AccessKey` e `SecretKey` forem utilizadas na configuração da
aplicação, elas devem ser mantidas apenas em ambiente local ou seguro.

## Executando localmente

Na raiz do repositório:

```bash
dotnet restore
dotnet build
dotnet run --project PrototipoBackEnd.API
```

Com os perfis atuais de desenvolvimento, a API pode ser acessada em:

- HTTP: `http://localhost:5064`;
- HTTPS: `https://localhost:7058`.

O `launchSettings.json` abre o Swagger em `/swagger` quando a aplicação é
executada por um perfil que habilita o navegador.

## Swagger

Em ambiente de desenvolvimento, a documentação está disponível em:

```text
https://localhost:7058/swagger
```

O Swagger também pode ser habilitado em outros ambientes com:

```json
{
  "EnableSwaggerInProduction": true
}
```

Para endpoints protegidos:

1. faça login em `POST /Auth/login`;
2. copie o token retornado;
3. clique em **Authorize** no Swagger;
4. informe `Bearer <token>`.

## Endpoints

As rotas usam o nome do controller como prefixo.

### Autenticação

| Método | Rota | Descrição |
| --- | --- | --- |
| `POST` | `/Auth/login` | Autentica o usuário e retorna o token JWT |

O login recebe `LoginRequestDto` no corpo da requisição.

### Usuários

| Método | Rota | Descrição |
| --- | --- | --- |
| `GET` | `/Usuario/BuscarTodos` | Lista os usuários |
| `GET` | `/Usuario/BuscarPorId/{id}` | Busca um usuário por ID |
| `POST` | `/Usuario/Adicionar` | Cadastra um usuário |
| `PUT` | `/Usuario/Atualizar/{id}` | Atualiza um usuário |
| `DELETE` | `/Usuario/Excluir/{id}` | Inativa/exclui um usuário |

Os endpoints de criação e atualização recebem `UsuarioDto` via
`multipart/form-data`.

### Artesãos

| Método | Rota | Descrição |
| --- | --- | --- |
| `GET` | `/Artesao/BuscarTodos` | Lista artesãos e aceita filtros |
| `GET` | `/Artesao/BuscarPorId/{id}` | Busca um artesão por ID |
| `POST` | `/Artesao/Adicionar` | Cadastra um artesão com imagem |
| `PUT` | `/Artesao/Atualizar/{id}` | Atualiza um artesão e, opcionalmente, sua imagem |
| `DELETE` | `/Artesao/Excluir/{id}` | Inativa/exclui um artesão |

Os filtros de `BuscarTodos` são enviados por query string:

```text
nome
nichoAtuacao
receberEncomendas
enviaEncomendas
```

Os endpoints de criação e atualização recebem `ArtesaoDto` via
`multipart/form-data`. O campo do arquivo é `imagem`.

### Artesanatos

| Método | Rota | Descrição |
| --- | --- | --- |
| `GET` | `/Artesanato/BuscarTodos` | Lista os artesanatos |
| `GET` | `/Artesanato/BuscarPorId/{id}` | Busca um artesanato por ID |
| `GET` | `/Artesanato/PorArtesao/{artesaoId}` | Busca um artesanato pelo artesão |
| `GET` | `/Artesanato/TodosPorArtesao/{artesaoId}` | Lista todos os artesanatos de um artesão |
| `POST` | `/Artesanato/Adicionar` | Cadastra um artesanato com imagens |
| `PUT` | `/Artesanato/Atualizar/{id}` | Atualiza um artesanato |
| `DELETE` | `/Artesanato/Excluir/{id}` | Inativa/exclui um artesanato |

O cadastro recebe `ArtesanatoDto` e uma lista de arquivos no campo `imagem`,
por meio de `multipart/form-data`.

## Autenticação e autorização

A API valida tokens usando JWT Bearer e verifica:

- assinatura da chave configurada em `Jwt:Key`;
- emissor (`Jwt:Issuer`);
- audiência (`Jwt:Audience`);
- validade do token, sem tolerância adicional de tempo.

Existe a policy `Administrador`, baseada na role `Administrador`, para uso em
endpoints que exigirem autorização por perfil.

## CORS

A policy `Frontend` permite:

- origens `http://localhost` e `https://localhost`;
- subdomínios `*.vercel.app` que contenham `prototipo-frontend` no domínio.

São permitidos qualquer método e cabeçalho, com credenciais habilitadas.

## Testes

Para executar todos os testes:

```bash
dotnet test
```

Para executar apenas os testes do projeto:

```bash
dotnet test PrototipoBackEnd.Tests/PrototipoBackEnd.Tests.csproj
```

Os testes ficam organizados em `Unit` e `Integration`.

## Ambiente publicado

Quando o ambiente Azure estiver disponível, a API publicada pode ser acessada
em:

```text
https://prototipocuradoriaapi.azurewebsites.net
```

Rotas úteis:

- `/` — verificação simples de disponibilidade;
- `/swagger` — documentação, quando habilitada no ambiente.

O endereço e a disponibilidade do ambiente publicado podem variar conforme o
deploy atual da branch.
