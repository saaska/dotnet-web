#!/bin/bash

# выход по любой ошибке кроме как в цикле until
set -e

# запуск Postgres
/usr/local/bin/docker-entrypoint.sh postgres&

# населяем базу, так как сервер стартует медленно, повторяем 
# попытки с паузой 2с, пока не получится
until dotnet SeedDB.dll; do
>&2 echo "Жду старта Postgres..."
sleep 2
done

# запуск основного приложения
exec dotnet clientsorders.dll $*
