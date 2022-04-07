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
RUN dotnet publish -c debug -o /app/clientsorders --no-restore

WORKDIR /src
COPY SeedDB/. ./SeedDB
WORKDIR /src/SeedDB
RUN dotnet publish -c debug -o /app/SeedDB --no-restore

FROM postgres AS FINAL
WORKDIR /app
RUN apt update && apt install -y wget
RUN wget https://packages.microsoft.com/config/debian/11/packages-microsoft-prod.deb \
    -O packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && rm packages-microsoft-prod.deb
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
RUN apt update \
    && apt install -y aspnetcore-runtime-2.1 \
    && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/clientsorders .
COPY --from=publish /app/SeedDB .

COPY run.sh .
RUN chmod +x ./run.sh

WORKDIR /app
EXPOSE 80
ENV POSTGRES_PASSWORD=InsideContainer2022
ENV DBCONN=Host=localhost;Port=5432;Username=postgres;Password=$POSTGRES_PASSWORD
ENV ASPNETCORE_URLS=http://+:80

STOPSIGNAL SIGINT
ENTRYPOINT [ "./run.sh" ]
