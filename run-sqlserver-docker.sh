#!/bin/sh
docker run -e 'ACCEPT_EULA=Y' -e MSSQL_SA_PASSWORD=$DBPASS -p 54321:1433 -v $(pwd)/db/data:/var/opt/mssql/data -v $(pwd)/db/log:/var/opt/mssql/log -v $(pwd)/db/secrets:/var/opt/mssql/secrets -d mcr.microsoft.com/mssql/server
