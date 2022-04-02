#!/bin/sh
docker exec -it $(docker ps -q -l) /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $DBPASS -d master -i /var/opt/mssql/data/createdb.sql
