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
    mssql/server

Подключение

* 127.0.0.1 (localhost:1433 не принял)
* user: sa
* password: XXXXXXXX


Создание базы. Русская коллация в контейнере недоступна (санкции?)

    CREATE DATABASE testbackend COLLATE Yakut_100_CI_AS_SC_UTF8;
    CREATE LOGIN backend WITH PASSWORD = 'HeadStart2IsBackEnd';

	CREATE TABLE Client (
		id INT PRIMARY KEY,
		Name VARCHAR(100) NOT NULL,
		BirthDate DATE NOT NULL,
		Inn VARCHAR(12) NOT NULL,
		PhoneNumber VARCHAR(14) NOT NULL,
		Email VARCHAR(60)
	) 

	CREATE TABLE [Order] (
		id INT PRIMARY KEY,
		CreatedOn DATE NOT NULL,
		Status INT NOT NULL
		CONSTRAINT DefaultToDo DEFAULT 0
	) 

	CREATE TABLE ClientOrder (
		client_id INT NOT NULL,
		order_id INT NOT NULL,
		CONSTRAINT FKClient FOREIGN KEY (client_id) 
		REFERENCES Client(id)
		ON DELETE CASCADE,
		CONSTRAINT FKOreder FOREIGN KEY (order_id) 
		REFERENCES [Order](id)
		ON DELETE CASCADE
	) 



## TODO
* ~~Пройти туториал для minimal API~~  <br />среда, 30 марта 2022 г. 11:52:10 (+09)
* Понять, как мне лучше: сопрягать EF с существующей базой, или создавать из кода
* Реализовать подключение к базе из .NET 6
* Адаптировать решение под более старые версии фреймворка
* ...

### Tutorial Minimal API
[Туториал в оф документации](https://docs.microsoft.com/en-us/aspnet/core/tutorials/min-web-api?view=aspnetcore-6.0&tabs=visual-studio-code) относится к .NET 6. VS2019 на mac не работает с .NET 6! Есть отзличия от старых версий: убрали startup.cs, из Program.cs выкинули очень много, нет `namespace`, `public class Program` и т.д. Сразу `var builder = WebApplication.CreateBuilder(args);`. Туториал демонстрирует минимальные API без контроллеров на примере EF с базой в памяти (`UseInMemoryDatabase`).

Для установки EF: из папки проекта

    dotnet add package Microsoft.EntityFrameworkCore.InMemory --prerelease
    dotnet add package Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore --prerelease

Вторая команда не прошла, `--prerelease` требует .NET 7😳. без этого ключа встала.

