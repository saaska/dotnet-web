#!/bin/sh
docker run -e POSTGRES_PASSWORD=$DBPASS -p 54321:5432 -v $(pwd)/db/data/postgres:/var/lib/postgresql/data -d postgres
