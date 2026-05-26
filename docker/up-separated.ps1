$ErrorActionPreference = "Stop"

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")

Push-Location $repoRoot
try {
    $networkExists = docker network ls --format "{{.Name}}" | Where-Object { $_ -eq "ingressinhos-network" }
    if (-not $networkExists) {
        docker network create ingressinhos-network
    }

    docker compose -f .\docker-compose.postgres.yml up -d
    docker compose -f .\docker-compose.mongo.yml up -d
    docker compose -f .\docker-compose.rabbitmq.yml up -d
    docker compose -f .\docker-compose.apis.yml up -d --build
    docker compose -f .\docker-compose.workers.yml up -d --build
}
finally {
    Pop-Location
}
