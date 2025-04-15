# 🐇 Importer RabbitMQ

Aplicação .NET distribuída para **upload e processamento assíncrono de dados** via arquivos.  
Utiliza **Docker**, **RabbitMQ**, **PostgreSQL** e **Entity Framework Core** para oferecer uma solução escalável e moderna de importação de dados.

## 🧩 Tecnologias Utilizadas

- [.NET 8](https://dotnet.microsoft.com/)
- PostgreSQL + Docker
- RabbitMQ
- Entity Framework Core (Migrations)
- Console App + API REST (Web API)
- Docker Compose
- Dapper (opcional, se usado no projeto)

---

## 📦 Estrutura do Projeto

```bash
importer-rabbitmq/
├── api-upload/        # Projeto Web API (.NET)
├── console-importer/  # Projeto Console Consumer (.NET)
├── shared/            # Projeto compartilhado (DbContext, Models, etc)
├── docker-compose.yml
└── README.md

🚀 Como Executar o Projeto
1. Clone o repositório

git clone https://github.com/seu-usuario/importer-rabbitmq.git
cd importer-rabbitmq

2. Suba os containers com Docker

docker-compose up -d

Este comando sobe os serviços:

🐘 PostgreSQL (porta 5432)

🐇 RabbitMQ Management (porta 15672)

RabbitMQ Broker (porta 5672)

Você pode acessar o painel do RabbitMQ em:
📍 http://localhost:15672
Login: guest | Senha: guest
