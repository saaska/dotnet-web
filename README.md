![docker build](https://github.com/saaska/dotnet-web/actions/workflows/dockerbuild.yml/badge.svg)

# dotnet-web

Корневая папка содержит проекты: 
* Приложение "Клиенты и Заказы"
* Вспомогательная программа - генератор данных
* Пустые папки для БД
* Скрипт bash для запуска БД

## Версия .NET

По требованиям задания, версия 2.1.

## База данных

Приложение работает с MS SQL в docker-контейнере. Перед запуском приложения 
следует установить произвольный пароль администратора БД в переменной среды `DBPASS` 
и запустить сервер БД командным файлом [run-sqlserver-docker.sh](./run-sqlserver-docker.sh)
Этот скрипт запускает сервер на нестандартном порту 54321 локальной машины. 

Теперь нужно установить переменную среды `DBCONN` для строки подключения приложений к БД.
В юниксах

    export DBCONN="Server=127.0.0.1,54321;User Id=sa;Password=$DBPASS"

или выполнить скрипт `source setdbconn-local.sh`.

Есть вспомогательный проект для заполнения базы данных тестовыми данными,
SeedDB. Его нужно скомпилировать и запустить, предварительну установив `DBCONN`.

    cd SeedDB
    dotnet run

## Приложение "Клиенты и Заказы"

Основной проект [clientsorders](./clientsorders) запускается на порту 5001.

    cd clientsorders
    dotnet run

Также берет строку подключения из `DBCONN`.

Написан с использованием Razor Pages, имеет Swagger-страничку по адресу
`/swagger` для просмотра API.


## Docker Compose

С помощью GitHub Actions после коммита автоматически собирается и в случае
успеха публикуется на DockerHub docker-контейнер `zaazp/dotnetweb`. 
В репозитории имеется файл [docker-compose.yml](./docker-compose.yml),
с помощью которого можно запускать и БД, и приложение однй строкой без
возни с переменными среды. Но в нем не работают сертификаты разработчика,
поэтому сайт будет доступен только по HTTP [http://localhost:5000](http://localhost:5000), 
и Swagger работать не будет.


[*Дневник разработки*](./diary.md)
