#!/bin/bash

set -e

until dotnet SeedDB.dll; do
>&2 echo "Жду старта SQL Server..."
sleep 2
done

dotnet clientsorders.dll
