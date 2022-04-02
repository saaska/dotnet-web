#!/bin/sh
rm -rf Migrations
dotnet ef migrations add Initial
dotnet ef database update
