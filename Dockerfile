FROM mcr.microsoft.com/dotnet/core/sdk:2.1 AS buildenv
WORKDIR /src
COPY *.sln .
COPY clientsorders/*.csproj ./clientsorders/
COPY SeedDB/*.csproj ./SeedDB/
RUN dotnet restore

FROM buildenv AS publish
WORKDIR /src
COPY clientsorders/. ./clientsorders
WORKDIR /src/clientsorders
RUN dotnet build
RUN dotnet publish -c release -o /app/clientsorders --no-restore

WORKDIR /src
COPY SeedDB/. ./SeedDB
WORKDIR /src/SeedDB
RUN dotnet build
RUN dotnet publish -c release -o /app/SeedDB --no-restore

FROM mcr.microsoft.com/dotnet/core/aspnet:2.1 AS final
WORKDIR /app
COPY --from=publish /app/clientsorders .
COPY --from=publish /app/SeedDB .

# ENTRYPOINT ["dotnet", "clientsorders.dll"]
