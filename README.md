![build](https://github.com/saaska/dotnet-web/actions/workflows/build.yml/badge.svg)
![docker build](https://github.com/saaska/dotnet-web/actions/workflows/dockerbuild.yml/badge.svg)

# dotnet-web

Корневая папка содержит проекты: 
* Приложение "Клиенты и Заказы"
* Пустые папки для БД
* Скрипт bash для запуска БД

Проект запущен на платформе Heroku по адресу https://albank-dotnetweb.herokuapp.com/


## Версия .NET

По требованиям задания, версия 2.1.

## База данных

Приложение может работать с БД PostgreSQL в docker-контейнере. Для удобства есть скрипт  
[run-postgres-docker.sh](./run-postgres-docker.sh), запускающий сервер БД  Postgres
на нестандартном порту 54321 локальной машины. 

## Приложение "Клиенты и Заказы"

Основной проект [clientsorders](./clientsorders) запускается при локальной сборке 
по умолчанию на портах 5000 (HTTP) и 5001 (HTTPS). Если выставлена переменная среды
PORT, запускается только HTTP сервер на указанном в ней порту.

    cd clientsorders
    dotnet run

Настройки подключения к БД должны быть указаны в переменной среды DATABASE_URL, 
по умолчанию 

    postgres://postgres:InsideContainer2022@localhost:54321/backend

Это подходит для сервера, запущенного скриптом [run-postgres-docker.sh](./run-postgres-docker.sh)
на той же машине.

Проект Написан с использованием Razor Pages, имеет Swagger-страничку по адресу
`/swagger` для просмотра API.


## Docker Compose

С помощью GitHub Actions после коммита автоматически собирается и в случае
успеха публикуется на DockerHub docker-контейнер `zaazp/dotnetweb`. 
В репозитории имеется файл [docker-compose.yml](./docker-compose.yml),
с помощью которого можно запускать и БД, и приложение однй строкой 

    docker compose up

Но в нем не работают сертификаты разработчика,
поэтому сайт будет доступен только по HTTP [http://localhost:5000](http://localhost:5000).


[*Дневник разработки*](./diary.md)
