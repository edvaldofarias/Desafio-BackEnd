# Introdução

Projetos desenvolvidos durante o processo seletivo, com o objetivo específico de criar uma aplicação 
abrangente e funcional para o gerenciamento eficiente de aluguel de motocicletas. 
Estes projetos não apenas demonstram habilidades técnicas, mas também abordam desafios 
práticos do mundo real no setor de aluguel de veículos, incorporando funcionalidades 
como cadastro de clientes, controle de estoque de motocicletas, agendamento de aluguéis, 
e geração de relatórios financeiros.

## Introdução para execução

### Variáveis de ambiente

Ajustar as variáveis de ambiente `ASPNETCORE_ENVIRONMENT` e `DOTNET_ENVIRONMENT` com o valor `Development` no sistema operacional.

No Windows, para criar a variável de ambiente para o usuário conectado, ou alterar o valor, com CMD:

```bash
setx ASPNETCORE_ENVIRONMENT "Development"
setx DOTNET_ENVIRONMENT "Development"
```

No MacOS, para criar as variáveis de ambiente permanentemente, é necessário editar o arquivo `~/.bash_profile` como root e acrescentar as seguintes linhas no final do arquivo:

```bash
export ASPNETCORE_ENVIRONMENT="Development"
export DOTNET_ENVIRONMENT="Development"
```

### Postman Collection

Para testar a API, importe o arquivo `job.postman_collection.json` no Postman.

### Ferramentas necessárias

- Docker/Docker Compose

Para executar o projeto, basta clonar o repositório e executar o comando `docker compose up -d` para subir o container do banco de dados e da aplicação.
Abra o navegador e acesse `http://localhost:5001/swagger/index.html` para visualizar a aplicação.

Ao executar o comando `docker compose up -d` o banco de dados será populado com dados de teste.

### Autenticação ADMIN

Para acessar a aplicação como administrador, utilize o seguinte usuário:

``` json
{
  "email": "job@job.com",
  "password": "mudar@123"
}
```
Para autenticar como motoboy e necessário fazer o cadastro e utilizar o CNPJ no login.

## Características do projeto

- .NET 8
- Banco de dados PostgreSQL
- CQS (Command Query Separation)
- MediatR para implementação do padrão CQS.
- ORM: **EntityFramework Core**
- Framework de Testes: **XUnit**
- Framework de Assertions: **FluentAssertions**
- Framework de Mock: **Moq**
- Code Analyzer: **Microsoft.CodeAnalysis.NetAnalyzers**
- Projeto para testes de Unidade
- Projeto para testes de Integração
- Tratamento de Warning como Erro.
- Dockerfile para a aplicação e para o banco de dados.
- Docker Compose para subir a aplicação e o banco de dados.
- [BCrypt.NET - NEXT](https://github.com/BcryptNet/bcrypt.net) para criptografia de senha.
- Swagger para documentação da API.
- [Fluent Result](https://github.com/altmann/FluentResults) para padronização de retorno da API.
- Postman Collection para testes de API.
