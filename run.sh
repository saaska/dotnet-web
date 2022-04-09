#!/bin/bash

# выход по любой ошибке кроме как в цикле until
set -e

# запуск Postgres
/usr/local/bin/docker-entrypoint.sh postgres&

echo "Жду старта Postgres..."
sleep 15

# запуск основного приложения
exec dotnet clientsorders.dll $*
