#!/bin/sh
rm -f /Migrations/*
dotnet ef migrations add Initial
dotnet ef database update
