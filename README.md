# ⭐ Star Championship

Aplicação web em **ASP.NET Core MVC** para cadastro de atletas e geração automática de times equilibrados com base em atributos técnicos.

O projeto foi construído em **.NET / C#**, com interface server-side em **Razor (HTML)** e **Bootstrap**, persistência em **MySQL** via **Entity Framework Core + Pomelo**, containerização com **Docker** e preparo para deploy em **Render** conectado a banco **Aiven**.

---

## 📌 Sumário

- [Visão geral](#-visão-geral)
- [Funcionalidades principais](#-funcionalidades-principais)
- [Tecnologias utilizadas](#-tecnologias-utilizadas)
- [Arquitetura da aplicação](#-arquitetura-da-arquitetura)
- [Modelo de dados](#-modelo-de-dados)
- [Interface e experiência visual](#-interface-e-experiência-visual)
- [Execução local (sem Docker)](#-execução-local-sem-docker)
- [Execução com Docker](#-execução-com-docker)
- [Banco de dados MySQL (Aiven)](#-banco-de-dados-mysql-aiven)
- [Deploy no Render](#-deploy-no-render)
- [Migrations e manutenção do banco](#-migrations-e-manutenção-do-banco)
- [Estrutura de pastas](#-estrutura-de-pastas)

---

## 🎯 Visão geral

O **Star Championship** resolve um problema comum em jogos e peladas: montar dois times com níveis parecidos de habilidade.

A aplicação permite:
1. Cadastrar jogadores com foto e atributos.
2. Calcular automaticamente o **Overall** de cada atleta.
3. Selecionar um conjunto de jogadores para o sorteio.
4. Gerar duas equipes com a menor diferença possível de força total.
5. Refinar o balanceamento usando uma margem de diferença personalizada.

Tudo isso em uma interface web responsiva, com navegação simples e foco em produtividade.

---

## 🚀 Funcionalidades principais

### 1) Gestão completa de jogadores (CRUD)
- **Criar** atleta com nome, URL de imagem e atributos técnicos.
- **Listar** jogadores com ranking e destaque de top características.
- **Visualizar detalhes** individuais.
- **Editar** informações.
- **Excluir** registro.

### 2) Cálculo de Overall automático
O `Overall` é calculado como a média dos 8 atributos cadastrados:
- Shoot
- Dribble
- First Touch
- Ball Control
- Defense
- Pass
- Speed
- Strength

### 3) Gerador de times equilibrados
No módulo **Gerador**:
- O usuário seleciona os jogadores;
- Define uma margem de equilíbrio;
- O sistema executa múltiplas tentativas aleatórias para encontrar a melhor divisão;
- Exibe os dois times, somatório por equipe e diferença final.

### 4) Ajuste rápido da margem
Na tela de resultado, é possível “re-sortear” com nova margem sem precisar refazer toda a seleção.

---

## 🧰 Tecnologias utilizadas

### Back-end
- **.NET 8**
- **C#**
- **ASP.NET Core MVC**
- **Razor Views**
- **Dependency Injection** nativa do ASP.NET

### Dados
- **Entity Framework Core 9**
- **Pomelo.EntityFrameworkCore.MySql**
- **MySqlConnector**
- **MySQL**

### Front-end
- **HTML (Razor .cshtml)**
- **Bootstrap** (tema custom + responsividade)
- **Bootstrap Icons**
- **jQuery** e validação unobtrusive

### Infra e deploy
- **Docker** (build/publish multi-stage)
- **Render** (hospedagem da aplicação web)
- **Aiven** (MySQL gerenciado na nuvem)

---

## 🏛️ Arquitetura da aplicação

A solução segue o padrão **MVC**:
- `Controllers/` → fluxo HTTP e regras de entrada/saída.
- `Models/` → entidades e validações.
- `Views/` → páginas Razor renderizadas no servidor.
- `Services/` → regras de negócio e acesso organizado ao contexto.
- `Data/` → `DbContext` e seeding.

---

## 🗃️ Modelo de dados

### Entidade principal: `Player`
Campos principais:
- `Id`, `Name`, `ImageUrl` (opcional).
- Atributos técnicos (0 a 100).
- `Overall` (calculado).

---

## ▶️ Execução local (sem Docker)

### Pré-requisitos
- .NET SDK 8+
- MySQL disponível (local ou remoto)

### 1) Clonar
```bash
git clone <url-do-repositorio>
cd StarChampionship

```

### 2) Configurar connection string

Em `StarChampionship/appsettings.json`, preencha:

```json
"ConnectionStrings": {
  "StarChampionshipContext": "server=SEU_HOST;port=3306;database=SEU_DB;user=SEU_USER;password=SUA_SENHA"
}

```

### 3) Restaurar e executar

```bash
dotnet restore StarChampionship/StarChampionship.csproj
dotnet run --project StarChampionship/StarChampionship.csproj

```

---

## 🐳 Execução com Docker

O repositório já inclui `Dockerfile` multi-stage.

### Build da imagem

```bash
docker build -t starchampionship .

```

### Run do container

```bash
docker run --rm -p 10000:10000 \
  -e ConnectionStrings__StarChampionshipContext="server=SEU_HOST;port=3306;database=SEU_DB;user=SEU_USER;password=SUA_SENHA" \
  starchampionship

```

A aplicação ficará disponível em `http://localhost:10000`.

---

## 🧪 Migrations e manutenção do banco

```bash
# Atualizar banco
dotnet ef database update --project StarChampionship/StarChampionship.csproj

# Criar nova migration
dotnet ef migrations add NomeDaMigration --project StarChampionship/StarChampionship.csproj

```

---

## 📁 Estrutura de pastas

```text
.
├── Dockerfile
├── README.md
└── StarChampionship/
    ├── Controllers/
    ├── Data/
    ├── Migrations/
    ├── Models/
    ├── Services/
    ├── Views/
    ├── wwwroot/
    ├── Program.cs
    ├── appsettings.json
    └── StarChampionship.csproj

```

---


## 👨‍💻 Autor

Projeto **Star Championship**, desenvolvido com foco em organização de times equilibrados e experiência web moderna com .NET + C#.

```
Jhonathan Ferroni.