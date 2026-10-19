# CrudKronixForDevs

API REST didática desenvolvida com ASP.NET Core e .NET 10. O projeto demonstra um CRUD de medicamentos utilizando separação em Controllers, Services, Repositories e Entity Framework Core com banco InMemory.

## Principais recursos

- API REST de medicamentos.
- Entity Framework Core com banco de dados InMemory.
- Autenticação própria com JWT Bearer.
- Senhas protegidas com `IPasswordHasher<Usuario>`.
- Usuário inicial criado durante a inicialização da aplicação.
- DTOs para entrada e saída de dados.
- Validações com Data Annotations e regras de negócio nos Services.
- Middleware global para tratamento de exceções.
- OpenAPI e Scalar para documentação e testes manuais.
- Testes unitários com xUnit e Moq.

## Pré-requisitos

Instale os seguintes componentes antes de executar o projeto:

- .NET 10 SDK.
- Visual Studio 2026 com a carga de trabalho **ASP.NET e desenvolvimento Web**, ou um editor compatível.
- Git, caso o projeto seja obtido por um repositório Git.
- PowerShell ou outro terminal compatível com os comandos do .NET CLI.

Confirme a instalação do SDK:

```powershell
dotnet --version
```

A versão exibida deve ser compatível com o target framework `net10.0`.

## Obtendo o projeto

Clone o repositório e acesse a pasta da solução:

```powershell
git clone <URL_DO_REPOSITORIO>
cd CrudKronixForDevs
```

Se o projeto já estiver disponível localmente, apenas abra a pasta:

```text
EXEMPLO: C:\LocalRepository\CrudKronixForDevs\
```

## Restaurando as dependências

Na raiz da solução, execute:

```powershell
dotnet restore .\CrudKronixForDevs.slnx
```

## Configurando o JWT e o usuário de desenvolvimento

A API exige uma chave JWT e as credenciais do usuário inicial. Esses valores devem ser configurados com **User Secrets** ou variáveis de ambiente. Não coloque chaves e senhas reais no Git.

Inicialize o User Secrets para o projeto da API:

```powershell
dotnet user-secrets init --project .\CrudKronixForDevs\CrudKronixForDevs.csproj
```

Configure os valores para desenvolvimento:

```powershell
dotnet user-secrets set "Jwt:Key" "local-development-key-with-at-least-32-characters" --project .\CrudKronixForDevs\CrudKronixForDevs.csproj
dotnet user-secrets set "Jwt:Issuer" "CrudKronixForDevs" --project .\CrudKronixForDevs\CrudKronixForDevs.csproj
dotnet user-secrets set "Jwt:Audience" "CrudKronixForDevs.Client" --project .\CrudKronixForDevs\CrudKronixForDevs.csproj
dotnet user-secrets set "Jwt:ExpirationMinutes" "60" --project .\CrudKronixForDevs\CrudKronixForDevs.csproj
dotnet user-secrets set "SeedUser:Email" "admin@exemplo.com" --project .\CrudKronixForDevs\CrudKronixForDevs.csproj
dotnet user-secrets set "SeedUser:Name" "Administrador" --project .\CrudKronixForDevs\CrudKronixForDevs.csproj
dotnet user-secrets set "SeedUser:Password" "SenhaSegura123!" --project .\CrudKronixForDevs\CrudKronixForDevs.csproj
```

Esses valores são apenas para desenvolvimento local. Em ambientes reais, utilize um gerenciador seguro de segredos como temos no Kronix, e uma chave JWT forte e aleatória.

### Consultando os User Secrets

Para listar as configurações cadastradas para o projeto:

```powershell
dotnet user-secrets list --project .\CrudKronixForDevs\CrudKronixForDevs.csproj
```

O comando pode exibir valores sensíveis diretamente no terminal. Não compartilhe essa saída nem a adicione ao repositório.

### Removendo um User Secret

Para remover uma configuração específica, informe a chave desejada:

```powershell
dotnet user-secrets remove "SeedUser:Password" --project .\CrudKronixForDevs\CrudKronixForDevs.csproj
```

Exemplo para remover a chave JWT:

```powershell
dotnet user-secrets remove "Jwt:Key" --project .\CrudKronixForDevs\CrudKronixForDevs.csproj
```

### Limpando todos os User Secrets

Para apagar todas as configurações locais associadas ao projeto:

```powershell
dotnet user-secrets clear --project .\CrudKronixForDevs\CrudKronixForDevs.csproj
```

Depois de executar `clear`, será necessário configurar novamente a chave JWT e as credenciais do usuário para iniciar a API.

## Compilando a solução

Para compilar a API e o projeto de testes:

```powershell
dotnet build .\CrudKronixForDevs.slnx
```

## Executando a API

Pelo .NET CLI:

```powershell
dotnet run --project .\CrudKronixForDevs\CrudKronixForDevs.csproj
```

Pelo Visual Studio:

1. Abra `CrudKronixForDevs.slnx`.
2. Defina `CrudKronixForDevs` como projeto de inicialização.
3. Escolha o perfil `http` ou `https`.
4. Pressione `F5` ou `Ctrl+F5`.

As URLs padrão são:

- HTTP: `http://localhost:5295`
- HTTPS: `https://localhost:7185`

## Documentação da API

Em ambiente de desenvolvimento, a especificação OpenAPI é disponibilizada em:

```text
http://localhost:5295/openapi/v1.json
```

A interface Scalar(Vulgo novo Swagger) pode ser acessada em:

```text
http://localhost:5295/scalar
```

Caso a aplicação seja executada usando HTTPS, substitua a porta e o protocolo conforme o perfil utilizado.

## Autenticação

O endpoint de login é público:

```http
POST /api/auth/login
Content-Type: application/json
```

Exemplo de corpo:

```json
{
  "email": "admin@exemplo.com",
  "password": "SenhaSegura123!"
}
```

A resposta contém um token JWT. Envie esse token nas chamadas protegidas:

```http
Authorization: Bearer SEU_TOKEN_AQUI
```

Todos os endpoints de medicamentos e anexos exigem autenticação.

## Endpoints principais

### Autenticação

| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/api/auth/login` | Valida as credenciais e gera um JWT |

### Medicamentos

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/api/medicamentos` | Lista os medicamentos |
| `GET` | `/api/medicamentos/{id}` | Consulta um medicamento |
| `POST` | `/api/medicamentos` | Cadastra um medicamento |
| `PUT` | `/api/medicamentos/{id}` | Atualiza um medicamento |
| `DELETE` | `/api/medicamentos/{id}` | Remove um medicamento |

Exemplo de corpo para criação ou atualização:

```json
{
  "nome": "Paracetamol",
  "principioAtivo": "Paracetamol",
  "fabricante": "Fabricante Exemplo",
  "preco": 12.50,
  "quantidadeEstoque": 100,
  "dataValidade": "2030-12-31"
}
```

### Anexos

Os anexos são enviados usando `multipart/form-data` no campo `arquivo`. Todos os endpoints utilizam o JWT Bearer.

| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/api/anexos` | Envia um arquivo |
| `GET` | `/api/anexos` | Lista os metadados e URLs dos arquivos |
| `GET` | `/api/anexos/{id}` | Visualiza PDFs/imagens ou baixa outros tipos |
| `DELETE` | `/api/anexos/{id}` | Exclui um arquivo |

Regras atuais:

- Tamanho máximo de 10 MB por arquivo.
- PDFs, JPEGs e PNGs podem ser visualizados diretamente pelo navegador.
- Outros formatos permitidos são baixados como anexos.
- Extensões executáveis ou potencialmente perigosas são bloqueadas.
- Os arquivos são salvos em `CrudKronixForDevs/Storage/Anexos`.
- O nome físico é gerado pela API; o nome enviado pelo cliente é mantido apenas como metadado.

O front-end pode chamar `GET /api/anexos` e usar a propriedade `url` retornada para exibir ou abrir o arquivo.

## Arquivo de requisições HTTP

O arquivo `CrudKronixForDevs/CrudKronixForDevs.http` contém exemplos de:

- Login.
- Listagem de medicamentos.
- Criação.
- Consulta por identificador.
- Atualização.
- Exclusão.
- Upload de anexos.
- Listagem e exclusão de anexos.

Ele pode ser executado diretamente pelo editor HTTP do Visual Studio ou por uma extensão compatível.

## Executando os testes

Para executar todos os testes unitários:

```powershell
dotnet test .\CrudKronixForDevs.Tests\CrudKronixForDevs.Tests.csproj
```

Os testes utilizam:

- xUnit para a estrutura de testes.
- Moq para simular Repositories e Services.

## Estrutura do projeto

```text
CrudKronixForDevs/
├── Configurations/       Configurações tipadas, como JWT
├── Controllers/          Endpoints HTTP
├── Data/                 DbContext e inicialização do banco
├── DTOs/                 Contratos de entrada e saída
├── Middlewares/          Tratamento global de exceções
├── Models/               Entidades do domínio
├── Repositories/         Acesso ao banco de dados
└── Services/             Regras de negócio

CrudKronixForDevs.Tests/
├── Controllers/          Testes das Controllers
├── Middlewares/          Testes do middleware
└── Services/             Testes dos Services
```

## Banco de dados InMemory

O projeto utiliza o provedor `Microsoft.EntityFrameworkCore.InMemory`. Por isso:

- Os dados são mantidos somente enquanto a aplicação está em execução.
- Usuários e medicamentos são perdidos quando a aplicação é reiniciada.
- Não é necessário instalar SQL Server ou outro banco para executar o exemplo.
- O usuário inicial é recriado quando o banco é reiniciado.

Para um ambiente real, substitua o provedor InMemory por um banco persistente, como SQL Server ou PostgreSQL, e utilize migrations do Entity Framework Core.

## Docker

O projeto possui um `Dockerfile`. Para criar a imagem:

```powershell
docker build -t crudkronixfordevs .\CrudKronixForDevs
```

Para executar o container, forneça as configurações sensíveis por variáveis de ambiente. Exemplo:

```powershell
docker run --rm -p 8080:8080 `
  -e Jwt__Key="local-development-key-with-at-least-32-characters" `
  -e Jwt__Issuer="CrudKronixForDevs" `
  -e Jwt__Audience="CrudKronixForDevs.Client" `
  -e Jwt__ExpirationMinutes="60" `
  -e SeedUser__Email="admin@exemplo.com" `
  -e SeedUser__Name="Administrador" `
  -e SeedUser__Password="SenhaSegura123!" `
  crudkronixfordevs
```

Em produção, não utilize as credenciais do exemplo.
