# Software

* macos
* Visual Studio Code
* .NET Core 
* Azure Data Studio
* Postman


# БД

Докер-контейнер MS SQL Server

    mkdir db
    mkdir db/data
    mkdir db/log
    mkdir db/secrets
    sudo docker pull mcr.microsoft.com/mssql/server
    docker run -e 'ACCEPT_EULA=Y' -e 'MSSQL_SA_PASSWORD=XXXXXXXX' -p 1433:1433 -v $(pwd)/db/data:/var/opt/mssql/data -v $(pwd)/db/log:/var/opt/mssql/log -v $(pwd)/db/secrets:/var/opt/mssql/secrets -d mcr.microsoft.com/mssql/server

Подключение

* 127.0.0.1 (localhost:1433 не принял)
* user: sa
* password: XXXXXXXX


## Создание базы. 

Русская коллация в контейнере недоступна (санкции?) 

    CREATE DATABASE backend COLLATE Yakut_100_CI_AS_SC_UTF8;

## TODO
* ~~Пройти туториал для minimal API~~  <br />среда, 30 марта 2022 г. 11:52:10 (+09)
* ~~Адаптировать решение под более старые версии фреймворка~~ 
* Понять, как мне лучше: сопрягать EF с существующей базой, или создавать из кода
* Реализовать подключение к базе из .NET 6
* ...

### Tutorial Minimal API
[Туториал в оф документации](https://docs.microsoft.com/en-us/aspnet/core/tutorials/min-web-api?view=aspnetcore-6.0&tabs=visual-studio-code) относится к .NET 6. VS2019 на mac не работает с .NET 6! Есть отзличия от старых версий: убрали startup.cs, из Program.cs выкинули очень много, нет `namespace`, `public class Program` и т.д. Сразу `var builder = WebApplication.CreateBuilder(args);`. Туториал демонстрирует минимальные API без контроллеров на примере EF с базой в памяти (`UseInMemoryDatabase`).

Для установки EF: из папки проекта

    dotnet add package Microsoft.EntityFrameworkCore.InMemory --prerelease
    dotnet add package Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore --prerelease

Вторая команда не прошла, `--prerelease` требует .NET 7😳. без этого ключа установилась.

### Адаптировать решение под более старые версии фреймворка
Для 5.0.15 все нормально проходит, пашет даже Swagger.
При установке пакетов версия указывается через ключ -v, например 

    dotnet add package Microsoft.EntityFrameworkCore.Design -v 5.0.15

### Понять, как мне лучше использовать EF
Code-First, поскольку база полуэфемерная в докере. Похоже на Django ORM - описываются модели в коде, после изменений создаются и применяются миграции. После создания Models.cs запустил

    dotnet ef migrations add InitialCreate
    dotnet ef database update

Создается папка Migrations и в ней миграции в виде cs-файлов. Вторая команда применяет их к базе, в том числе создает, если не было, служебную таблицу для миграций `__EFMigrationsHistory`. Можно не применяя к базе, сказать `dotnet ef script` и посмотреть на сгенеренный SQL. Проблема с FK: если написать, как в доке

    public IList<Order> Orders { get; } = new List<Order>();

столбики создаются, но они 

    [ClientId] int NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Orders_Clients_ClientId] FOREIGN KEY ([ClientId]) 
               REFERENCES [Clients] ([Id]) ON DELETE NO ACTION

Приходится добавлять вручную 

    public int ClientId { get; set; }
    public Client Client { get; set; }
