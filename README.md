# GamingJourney
![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet)
![C#](https://img.shields.io/badge/C%23-239120?logo=csharp&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?logo=microsoftsqlserver&logoColor=white)
![Azure](https://img.shields.io/badge/Azure-0078D4?logo=microsoftazure&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ED?logo=docker&logoColor=white)
![JWT](https://img.shields.io/badge/JWT-black?logo=jsonwebtokens)

O **GamingJourney** é uma API REST robusta desenvolvida para o gerenciamento de bibliotecas pessoais de jogos. A aplicação permite que usuários, após se cadastrarem e fazerem um login, cataloguem seus jogos, atribuam notas, monitorem o status de progresso (Playing, Completed, PlanToPlay, etc.) e filtrem sua coleção de forma personalizada.

## 🚀 Novidades: Deploy e Infraestrutura
A API encontra-se atualmente hospedada no **Azure App Service**, utilizando **Azure SQL Database** como persistência de dados. O projeto utiliza containers **Docker** para garantir a consistência entre os ambientes de desenvolvimento e produção.
[📎Clique aqui para acessar a API (Swagger Live Demo)](https://gamingjourney-cbd2gngwfabvg9gk.canadaeast-01.azurewebsites.net/index.html)

## Tecnologias Utilizadas
* Runtime: .NET 10
* Linguagem: C#
* Framework Web: ASP.NET Core
* ORM: Entity Framework Core
* Banco de Dados: SQL Server
* Autenticação: JWT (JSON Web Token)
* Mapeamento: AutoMapper e Mapeamento Manual
* Documentação: Swagger (OpenAPI)
* Cloud: Azure App Service & Azure SQL

## Arquitetura e Padrões
O projeto foi estruturado seguindo princípios de separação de responsabilidades para facilitar a manutenção e escalabilidade:

### Autenticação e Usuários
* Cadastro de novos usuários com criptografia de senha (BCrypt).
* Autenticação via JWT com suporte a Roles.
* Gerenciamento de perfil de usuário.

### Gerenciamento de Coleção (CRUD)
* Adição de jogos à coleção pessoal com validação de duplicidade.
* Listagem com filtros avançados: Nome, Gênero, Plataforma, Nota e Status.
* Atualização dinâmica de Status e Notas (Escala de 0 a 10).
* Remoção de itens da coleção com proteção de propriedade (um usuário só pode gerenciar sua própria lista).

## Como Executar o Projeto

## Primeiro Acesso
O cadastro de novos usuários requer autenticação de Admin. Para usar a API pela primeira vez, é necessário inserir o primeiro administrador diretamente no banco de dados.
Após rodar as migrations, execute o seguinte SQL no seu SQL Server:

```sql
INSERT INTO Usuarios (Nome, Email, SenhaHash, DtNasc, DtCadastro, Cargo, EmailConfirmado)
VALUES (
    'Admin', 
    'admin@gamingjourney.com', 
    '$2a$12$1D2sQdtdFiOTdOTzRDpKvO8FZkWh6Q6R26vKpX8AjtJ2Im1.8H13K', 
    '1990-01-01', 
    GETDATE(), 
    2, 
    1
)
```
> O hash acima corresponde à senha `password`. Troque imediatamente após o primeiro login usando o endpoint `PUT /api/Usuarios/perfil`.

> O valor `2` no campo `Cargo` corresponde ao enum `Admin`.

### Executando com Docker (recomendado)
1. Clone o repositório
2. Execute:
3. Acesse o Swagger em: http://localhost:8080/swagger
4. Insira o primeiro admin via SQL conforme instruções acima

### Pré-requisitos
* SDK .NET 10 instalado.
* Instância do SQL Server em execução.

### Configuração
1. Clone o repositório.
2. Configure a Connection String no arquivo appsettings.json.
3. Execute as Migrations para criar o banco de dados:
   dotnet ef database update
4. Inicie a aplicação:
   dotnet run
5. Acesse o Swagger em: https://localhost:7064/swagger

## Roadmap de Desenvolvimento

- [X] Implementação de Containerização com Docker.
- [X] Deploy automatizado no Azure (CI/CD via GitHub Actions)
- [ ] Integração com APIs externas (Preço em tempo real via Steam API, etc.)
- [ ] Dashboard com estatísticas da coleção (Média de notas, gêneros frequentes).
- [ ] Vínculo de plataforma específica por registro de usuário.

---
Desenvolvido por Anderson Guedes Ferreira Soares