# TB2-aws

API REST desenvolvida em .NET 10 para cadastro e gerenciamento de produtos, utilizando PostgreSQL e Entity Framework Core.

## Requisitos

- .NET 10 SDK
- Podman e podman-compose
- PostgreSQL (pode ser executado pelo Compose do projeto)

## Como executar localmente

1. Clone o projeto e entre na pasta:

```bash
git clone <url-do-repositorio>
cd TB2-aws
```

2. Inicie o PostgreSQL:

```bash
podman-compose -f postgres/compose.yaml up -d
```

O banco ficará disponível na porta `5432`. O Adminer poderá ser acessado em <http://localhost:8080>.

3. Configure as variáveis de ambiente:

```bash
cp .env.exemple .env
```

Revise os valores do arquivo `.env`, principalmente `DB_PASSWORD` e `JWT_SECRET`. A chave JWT deve possuir pelo menos 32 caracteres. Defina também `ADMIN_NAME`, `ADMIN_EMAIL` e `ADMIN_PASSWORD` para criar o usuário inicial automaticamente na primeira execução.

4. Restaure as dependências e aplique as migrations:

```bash
dotnet restore
dotnet ef database update
```

5. Execute a API:

```bash
dotnet run
```

A API estará disponível no endereço definido por `SERVER_HOST` e `SERVER_PORT` no `.env`.

Para expor na rede, use `SERVER_HOST=0.0.0.0`; acesse pelo IP da máquina, por exemplo `http://192.168.1.20:5059`.

O Swagger está disponível em `http://<IP_DA_MAQUINA>:<SERVER_PORT>/swagger`. Faça login em `POST /api/auth/login`, copie o token retornado e use o botão **Authorize** para informar esse token.

## Autenticação

O login é feito em:

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "admin@example.com",
  "senha": "sua-senha"
}
```

O cadastro público de usuários não está habilitado. O primeiro usuário é criado somente com as variáveis `ADMIN_*` do `.env`, evitando que qualquer pessoa crie contas pela API.

## Endpoints de produtos

| Método | Endpoint | Descrição |
|---|---|---|
| GET | `/api/produtos` | Lista os produtos |
| GET | `/api/produtos/{id}` | Busca um produto pelo ID |
| POST | `/api/produtos` | Cadastra um produto |
| PUT | `/api/produtos/{id}` | Atualiza um produto |
| DELETE | `/api/produtos/{id}` | Exclui um produto |

Todos os endpoints de produtos exigem um token JWT válido.

O arquivo `.env` contém informações locais e não deve ser enviado para o Git.
