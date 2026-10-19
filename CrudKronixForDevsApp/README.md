# CRUD Kronix for Devs

Aplicação web para gerenciamento de medicamentos e anexos. O frontend foi
desenvolvido com React e Vite em sua ultima versão. E consome uma API HTTP para autenticação e
operações de dados.

## Pré-requisitos

- **Node.js 20.19 ou superior**, ou **22.12 ou superior**.
- **npm**, instalado junto com o Node.js.
- A API do projeto em execução. Por padrão, o frontend espera encontrá-la em
  `http://localhost:5295`.

Para conferir as versões instaladas:

```bash
node --version
npm --version
```

## Como executar em ambiente de desenvolvimento

### 1. Abra a pasta do projeto no terminal

Se ainda não tiver o código, clone o repositório usando o endereço fornecido
pelo responsável pelo projeto. Depois, entre na pasta clonada:

```bash
cd caminho/para/CrudKronixForDevsApp
```

### 2. Instale as dependências

Na pasta que contém o `package.json`, execute:

```bash
npm ci
```

Esse comando instala as versões registradas em `package-lock.json`.

### 3. Inicie a API

Inicie o backend separadamente e confirme que ele está acessível em
`http://localhost:5295`. O frontend envia as chamadas para `/api`, e o proxy do
Vite as encaminha para esse endereço.

Se a API estiver em outra origem, crie um arquivo `.env.local` na raiz do
projeto e informe a origem do backend **sem** acrescentar `/api`. Por exemplo:

```dotenv
VITE_API_PROXY_TARGET=http://localhost:5000
```

O arquivo `.env.local` é local da sua máquina. Reinicie o servidor do Vite
depois de alterar essa configuração.

Como alternativa, para apontar o Axios diretamente para a API (sem usar o
proxy do Vite), defina `VITE_API_BASE_URL` com a URL base completa, incluindo
`/api` se esse for o prefixo usado pelo backend:

```dotenv
VITE_API_BASE_URL=http://localhost:5295/api
```

Não coloque senhas, tokens ou outros segredos em variáveis `VITE_*`: esses
valores podem ser incluídos no código disponibilizado ao navegador.

### 4. Inicie o frontend

Em outro terminal, ainda na pasta do projeto, execute:

```bash
npm run dev
```

Abra no navegador o endereço mostrado no terminal. Normalmente é
`http://localhost:5173`.

### 5. Entre na aplicação

Faça login com um email e uma senha aceitos pela API. O endpoint usado para
autenticação é `/api/auth/login`; a resposta deve incluir um token de acesso
(`token`, `accessToken` ou `jwt`, diretamente ou dentro de `data`). Sem uma API
em execução e credenciais válidas, não será possível acessar as telas
protegidas.

## Comandos disponíveis

```bash
npm run dev      # inicia o servidor local de desenvolvimento
npm run lint     # verifica problemas de lint no projeto
npm run build    # gera a versão de produção em dist/
npm run preview  # serve localmente a versão gerada pelo build
```

Para conferir a versão de produção localmente, gere primeiro o build e depois
inicie o preview:

```bash
npm run build
npm run preview
```

## Funcionalidades

- Login integrado à API.
- Consulta, cadastro, edição, exclusão, filtro e ordenação de medicamentos.
- Envio, consulta e exclusão de anexos. Cada arquivo é enviado
  individualmente em `multipart/form-data`.
- Área de geração de arquivos reservada para uma implementação futura.

## Estrutura da aplicação

- `src/App.jsx`: autenticação, navegação e estado compartilhado.
- `src/components/`: componentes reutilizáveis, formulário e tabela.
- `src/pages/`: telas de Medicamentos, Anexos e Geração de arquivos.
- `src/services/api.js`: configuração do Axios e chamadas à API.
- `src/utils/`: formatação de datas, validade do token e tratamento de erros.
