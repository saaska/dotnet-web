FROM mcr.microsoft.com/dotnet/core/sdk:2.1 AS buildenv
WORKDIR /src
COPY *.sln .
COPY clientsorders/*.csproj ./clientsorders/
RUN dotnet restore

FROM buildenv AS publish
WORKDIR /src
COPY clientsorders/. ./clientsorders
WORKDIR /src/clientsorders
RUN dotnet publish -c debug -o /app/clientsorders --no-restore

FROM mcr.microsoft.com/dotnet/core/aspnet:2.1.30-stretch-slim AS FINAL
WORKDIR /app
COPY --from=publish /app/clientsorders .
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

STOPSIGNAL SIGINT
ENTRYPOINT [ "dotnet", "clientsorders.dll" ]
