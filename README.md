# Ingressinhos
Projeto da faculdade de compras de ingresso

## PostgreSQL em Docker

Para desenvolvimento com replica real de leitura:

```powershell
docker compose -f .\docker-compose.postgres.yml up -d
```

Portas do host:

- `15432` -> PostgreSQL primary
- `15433` -> PostgreSQL replica
- `15434` -> PostgreSQL compartilhado de `Auth` e `Payment`

Connections de desenvolvimento:

- `DefaultConnection` -> `localhost:15432`
- `ReadConnection` -> `localhost:15433`
- `AuthConnection` -> `localhost:15434`
- `PaymentConnection` -> `localhost:15434`

### Migrations

Use migrations somente no contexto principal:

```powershell
Add-Migration NomeDaMigration -Context AppDbContext
Update-Database -Context AppDbContext
```

Nao use o `ReadAppDbContext` para `Add-Migration` nem para `Update-Database`. A replica recebe schema e dados do primary pela replicacao do PostgreSQL.

O banco compartilhado de `Auth` e `Payment` nao participa da replica do Ingressinhos. Ele sobe separado no mesmo compose, na mesma porta do host (`15434`), com dois databases:

- `IngressinhosAuthDb`
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

Credenciais padrao do compose:

- usuario: `admin`
- senha: `1234`

Uso atual no projeto:

- `Ingressinhos.Worker` consome mensagens de pagamento pelo RabbitMQ
- `Payment.Api` e `Payment.Worker` tambem estao configurados para usar o mesmo broker

Configuracao local padrao:

- `Messaging:Provider = RabbitMq` usa o broker real
- `Messaging:Provider = File` usa a mensageria mock por arquivos
- `Messaging:BasePath = ..\message-bus` define a pasta usada pela mensageria mock

Validacao rapida:

- abrir `http://localhost:15672`
- entrar com `admin / 1234`
- confirmar se o broker esta online antes de subir os workers
