# Deploy com Cloudflare Tunnel

Este projeto fica mais simples de operar usando subdominios separados e um unico Nginx local.
No Cloudflare Tunnel, todos os hostnames publicos podem apontar para o mesmo servico local:

```txt
http://localhost:8080
```

O Nginx diferencia cada destino pelo `Host` recebido.

## Subdominios recomendados

Troque `seu-dominio.com.br` pelo dominio real.

```txt
https://ingressinhos.seu-dominio.com.br  -> Ingressinhos API
https://auth.seu-dominio.com.br          -> SentinelAuth Frontend
https://auth-api.seu-dominio.com.br      -> SentinelAuth API
https://payment.seu-dominio.com.br       -> Payment API + mock payment
```

## Variaveis locais

Copie o arquivo de exemplo:

```powershell
Copy-Item .env.example .env
```

Edite o `.env`:

```env
INGRESSINHOS_PUBLIC_URL=https://ingressinhos.seu-dominio.com.br
SENTINEL_AUTH_FRONTEND_PUBLIC_URL=https://auth.seu-dominio.com.br
SENTINEL_AUTH_API_PUBLIC_URL=https://auth-api.seu-dominio.com.br
PAYMENT_PUBLIC_URL=https://payment.seu-dominio.com.br
```

Essas variaveis fazem tres coisas:

- configuram CORS do SentinelAuth API;
- compilam o SentinelAuth Frontend apontando para a API publica correta;
- fazem o Payment API gerar links de mock checkout no dominio fixo.

## Subir ou atualizar os containers

Depois de editar `.env`, rode:

```powershell
docker compose up -d --build sentinel-auth-api sentinel-auth-frontend payment-api nginx-proxy
```

Se quiser reconstruir tudo:

```powershell
docker compose up -d --build
```

## Configuracao no Cloudflare Tunnel

No painel Cloudflare:

1. Abra **Zero Trust**.
2. Va em **Networks > Tunnels**.
3. Clique no tunnel que esta rodando na sua maquina/servidor.
4. Abra **Public Hostnames**.
5. Adicione estes hostnames:

```txt
Subdomain: ingressinhos
Domain: seu-dominio.com.br
Service: http://localhost:8080
```

```txt
Subdomain: auth
Domain: seu-dominio.com.br
Service: http://localhost:8080
```

```txt
Subdomain: auth-api
Domain: seu-dominio.com.br
Service: http://localhost:8080
```

```txt
Subdomain: payment
Domain: seu-dominio.com.br
Service: http://localhost:8080
```

## URLs usadas pelo app Flutter

Para gerar uma build do app apontando para Cloudflare:

```powershell
flutter build apk --release `
  --dart-define=INGRESSINHOS_API_BASE_URL=https://ingressinhos.seu-dominio.com.br `
  --dart-define=SENTINEL_AUTH_API_BASE_URL=https://auth-api.seu-dominio.com.br `
  --dart-define=SENTINEL_AUTH_FRONTEND_URL=https://auth.seu-dominio.com.br
```

Para debug:

```powershell
flutter run `
  --dart-define=INGRESSINHOS_API_BASE_URL=https://ingressinhos.seu-dominio.com.br `
  --dart-define=SENTINEL_AUTH_API_BASE_URL=https://auth-api.seu-dominio.com.br `
  --dart-define=SENTINEL_AUTH_FRONTEND_URL=https://auth.seu-dominio.com.br
```

Sem `--dart-define`, o app continua usando as URLs locais atuais:

- Android emulator: `10.0.2.2`
- Desktop/iOS/web local: `localhost`

## Checklist de teste

Depois do deploy:

```txt
https://auth.seu-dominio.com.br/admin
https://auth-api.seu-dominio.com.br/api/admin/overview
https://payment.seu-dominio.com.br/mock-payment.html?paymentTransactionId=1
https://ingressinhos.seu-dominio.com.br/scalar
```

O endpoint `/api/admin/overview` pode retornar dados vazios, mas deve responder JSON.
O mock payment com `paymentTransactionId=1` pode mostrar erro de transacao inexistente, mas a pagina deve abrir.
