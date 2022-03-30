# Software

* macos
* Visual Studio Code
* .NET Core 
* Azure Data Studio


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
