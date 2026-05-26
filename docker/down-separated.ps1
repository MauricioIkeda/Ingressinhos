$ErrorActionPreference = "Stop"

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")

Push-Location $repoRoot
try {
    docker compose -f .\docker-compose.workers.yml down
    docker compose -f .\docker-compose.apis.yml down
    docker compose -f .\docker-compose.rabbitmq.yml down
    docker compose -f .\docker-compose.mongo.yml down
    docker compose -f .\docker-compose.postgres.yml down
}
finally {
    Pop-Location
}

