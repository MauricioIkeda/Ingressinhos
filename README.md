# Ingressinhos
Projeto da faculdade de compras de ingresso

## Ambiente completo em Docker

Para subir bancos, RabbitMQ, MongoDB, APIs e workers:

Lembre-se, você precisa colocar as senhas, e portas, no geral precisa de um arquivo .env por causa de um parceiro nosso.

```powershell
docker compose up -d --build
```

Para parar o ambiente:

```powershell
docker compose down
```

APIs expostas no host:

- `http://localhost:5254` -> SentinelAuth API
- `http://localhost:5202` -> Ingressinhos API
- `http://localhost:5071` -> Payment API
- `http://localhost:5110` -> Generic API

Servicos de apoio:

- `http://localhost:15672` -> painel do RabbitMQ (credenciais do `.env`)
- `localhost:27017` -> MongoDB
- `localhost:15432` -> PostgreSQL primary do Ingressinhos
- `localhost:15433` -> PostgreSQL replica do Ingressinhos
- `localhost:15434` -> PostgreSQL compartilhado de SentinelAuth e Payment

O compose completo nao aplica migrations automaticamente. Depois que os bancos subirem, aplique as migrations normalmente para cada contexto antes de usar as APIs.

### Ambiente separado por grupos no Docker Desktop

Para aparecer separado em grupos, use os compose files por contexto. Se ainda nao existir, crie a rede compartilhada uma vez:

```powershell
docker network inspect ingressinhos-network *> $null; if ($LASTEXITCODE -ne 0) { docker network create ingressinhos-network }
```

Para subir tudo separado em grupos com um comando:

```powershell
.\docker\up-separated.ps1
```

Para parar os grupos separados:

```powershell
.\docker\down-separated.ps1
```

Ou suba manualmente nesta ordem:

```powershell
docker compose -f .\docker-compose.postgres.yml up -d
docker compose -f .\docker-compose.mongo.yml up -d
docker compose -f .\docker-compose.rabbitmq.yml up -d
docker compose -f .\docker-compose.apis.yml up -d --build
docker compose -f .\docker-compose.workers.yml up -d --build
```

No Docker Desktop eles aparecem como:

- `ingressinhos-bancos-postgres`
- `ingressinhos-mongo`
- `ingressinhos-mensageria`
- `ingressinhos-apis`
- `ingressinhos-workers`

Se voce ja subiu o compose completo antes, pare ele primeiro para evitar conflito de nomes de containers:

```powershell
docker compose down
```

## PostgreSQL em Docker

Para desenvolvimento com replica real de leitura:

```powershell
docker compose -f .\docker-compose.postgres.yml up -d
```

Portas do host:

- `15432` -> PostgreSQL primary
- `15433` -> PostgreSQL replica
- `15434` -> PostgreSQL compartilhado de `SentinelAuth` e `Payment`

Connections de desenvolvimento:

- `DefaultConnection` -> `localhost:15432`
- `ReadConnection` -> `localhost:15433`
- `SentinelAuth DefaultConnection` -> `localhost:15434`
- `PaymentConnection` -> `localhost:15434`

### Migrations

Use migrations somente no contexto principal:

```powershell
Add-Migration NomeDaMigration -Context AppDbContext
Update-Database -Context AppDbContext
```

Nao use o `ReadAppDbContext` para `Add-Migration` nem para `Update-Database`. A replica recebe schema e dados do primary pela replicacao do PostgreSQL.

O banco compartilhado de `SentinelAuth` e `Payment` nao participa da replica do Ingressinhos. Ele sobe separado no mesmo compose, na mesma porta do host (`15434`), com dois databases:

- `SentinelAuthDb`
- `PaymentDb`

### Validacao rapida

Depois de subir os containers e aplicar a migration no primary:

- leituras da API devem usar a `ReadConnection`
- comandos e worker continuam usando a `DefaultConnection`
- escritas diretas na replica devem falhar por ela ser somente leitura

## Mensageria

Para subir o RabbitMQ do projeto:

```powershell
docker compose -f .\docker-compose.rabbitmq.yml up -d
```

Portas do host:

- `5672` -> broker AMQP
- `15672` -> painel web do RabbitMQ

Credenciais do compose:

- usuario: valor de `RABBITMQ_USER` no `.env`
- senha: valor de `RABBITMQ_PASSWORD` no `.env`

Uso atual no projeto:

- `Ingressinhos.Worker` consome mensagens de pagamento pelo RabbitMQ
- `Payment.Api` e `Payment.Worker` tambem estao configurados para usar o mesmo broker

Configuracao local padrao:

- `Messaging:Provider = RabbitMq` usa o broker real
- `Messaging:Provider = File` usa a mensageria mock por arquivos
- `Messaging:BasePath = ..\message-bus` define a pasta usada pela mensageria mock

Validacao rapida:

- abrir `http://localhost:15672`
- entrar com as credenciais `RABBITMQ_USER` / `RABBITMQ_PASSWORD` configuradas no `.env`
- confirmar se o broker esta online antes de subir os workers

## MongoDB para leitura de bilhetes

Para subir o banco de leitura dos bilhetes do cliente:

```powershell
docker compose -f .\docker-compose.mongo.yml up -d
```

Portas do host:

- `27017` -> MongoDB

Configuracao local padrao:

- `Mongo:ConnectionString` -> configure via `.env`, por exemplo `INGRESSINHOS_MONGO_CONNECTION`.
- `Mongo:Database` -> `IngressinhosReadDb`
- `Mongo:TicketCollection` -> `clientTickets`

Uso atual no projeto:

- `Ingressinhos.Worker` publica mensagens internas em `ticket-read-model-sync`
- o mesmo worker consome essa fila e projeta os bilhetes emitidos no MongoDB
- `Ingressinhos.API` usa o MongoDB para `GET /api/issued-tickets/me`
- o Postgres continua sendo a fonte oficial de pedidos, pagamentos e bilhetes emitidos
