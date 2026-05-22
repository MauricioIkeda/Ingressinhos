#!/bin/bash
set -e

export PGPASSWORD="${POSTGRESQL_PASSWORD}"

psql -v ON_ERROR_STOP=1 --username "postgres" --dbname "postgres" <<'SQL'
SELECT 'CREATE DATABASE "IngressinhosAuthDb"'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'IngressinhosAuthDb')\gexec

SELECT 'CREATE DATABASE "PaymentDb"'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'PaymentDb')\gexec
SQL
