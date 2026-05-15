# 🗂️ IPManager — Gerenciador de Projetos e IPs

Sistema web para cadastro e gerenciamento de projetos com seus respectivos endereços IP, desenvolvido com ASP.NET Core MVC (.NET 8) e PostgreSQL.

---

## 🚀 Funcionalidades

- ✅ Cadastro e login de usuários com autenticação via Cookie
- ✅ Senhas protegidas com hash **BCrypt**
- ✅ Cadastro de projetos com nome, IP e tipo de IP
- ✅ Listagem de projetos com filtros por nome, IP e tipo de IP
- ✅ Edição e exclusão de projetos (somente pelo dono)
- ✅ Rotas protegidas por `[Authorize]`

---

## 🛠️ Tecnologias Utilizadas

| Tecnologia | Versão |
|---|---|
| .NET | 8.0 |
| ASP.NET Core MVC | 8.0 |
| Entity Framework Core + Npgsql | 8.0 |
| PostgreSQL | — |
| BCrypt.Net-Next | 4.1.0 |
| DotNetEnv | 3.2.0 |

---

## 📁 Estrutura do Projeto

```
projeto/
├── Controllers/
│   ├── ContasController.cs    # Login, cadastro e logout
│   └── ProjetosController.cs  # CRUD de projetos
├── Data/
│   └── AppDbContext.cs        # Contexto do EF Core
├── Models/
│   ├── Projeto.cs             # Entidade Projeto
│   └── Usuario.cs             # Entidade Usuário
├── ViewModels/
│   └── ProjetosViewModel.cs   # ViewModel da tela de projetos
├── Views/
│   ├── Contas/                # Views de login e cadastro
│   └── Projetos/              # View principal (listagem/filtros)
├── wwwroot/                   # Arquivos estáticos (CSS, JS)
├── Program.cs                 # Configuração da aplicação
└── .env                       # Variáveis de ambiente (não versionar!)
```

---

## ⚙️ Configuração e Execução

### 1. Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/)

### 2. Clone o repositório

```bash
git clone https://github.com/wGabriel007/projeto.git
cd projeto
```

### 3. Configure o arquivo `.env`

Crie um arquivo `.env` na raiz do projeto com o seguinte conteúdo:

```env
DB_HOST=localhost
DB_PORT=5432
DB_NAME=nome_do_banco
DB_USER=seu_usuario
DB_PASSWORD=sua_senha
```

> ⚠️ **Nunca versione o arquivo `.env`!** Adicione-o ao `.gitignore`.

### 4. Aplique as migrations

```bash
dotnet ef database update
```

### 5. Execute o projeto

```bash
dotnet run
```

Acesse em: `https://localhost:{porta}`

---

## 🔐 Rotas Principais

| Método | Rota | Descrição |
|---|---|---|
| GET | `/Contas/Login` | Tela de login |
| POST | `/Contas/Login` | Autenticar usuário |
| GET | `/Contas/Cadastro` | Tela de cadastro |
| POST | `/Contas/Cadastro` | Registrar novo usuário |
| POST | `/Contas/Logout` | Encerrar sessão |
| GET | `/Projetos` | Listar projetos (autenticado) |
| POST | `/Projetos/Cadastrar` | Criar novo projeto |
| POST | `/Projetos/Editar` | Editar projeto existente |
| POST | `/Projetos/Excluir/{id}` | Excluir projeto |

---

## 🗄️ Modelo de Dados

```
┌─────────────────────┐          ┌──────────────────────┐
│       USUARIOS      │          │       PROJETOS       │
├─────────────────────┤          ├──────────────────────┤
│ id          INT  PK │◄────┐    │ id          INT  PK  │
│ nome        TEXT    │     │    │ nome        TEXT     │
│ email       TEXT    │     │    │ ip          TEXT     │
│ senha       TEXT    │     └────│ usuario_id  INT  FK  │
└─────────────────────┘          │ tipo_ip     TEXT     │
                                 └──────────────────────┘
```

---

## 📄 Licença

Este projeto está sob a licença MIT.
