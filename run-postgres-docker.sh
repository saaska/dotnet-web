#!/bin/sh
docker run -e POSTGRES_PASSWORD=InsideContainer2022 -p 54321:5432 -v $(pwd)/db/data/postgres:/var/lib/postgresql/data -d postgres
export DATABASE_URL="postgres://postgres:InsideContainer2022@localhost:54321/backend"
