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
* ~~Адаптировать решение под более старые версии фреймворка:~~ сделано для 5
* ~~Понять, как мне лучше: сопрягать EF с существующей базой, или создавать из кода:~~ Code First
* ~~Реализовать подключение к базе из .NET Core 2.1~~
* ~~Сгенерить шаблоны WebAPI~~ <br />четверг, 31 марта 2022 г. 10:26:21 (+09)
* ~~Понять, что происходит с навигационными свойствами~~: ok <br />четверг, 31 марта 2022 г. 12:16:15 (+09)
* ~~Доработать шаблоны на вставку, отображение связанных частей~~: в API кроме id, ничего не нужно
* ~~Попробовать Postman, попробовать подключить Swagger~~<br/> Mon Apr  4 15:12:06 +09 2022
* ~~Понять, на чем делать фронт~~: Razor <br />четверг, 31 марта 2022 г. 17:03:12 (+09)
* ~~Сгенерить шаблоны Razor Pages, посмотреть чего не хватает~~: глюки с fk, enum. пятница,  <br />1 апреля 2022 г. 11:11:40 (+09)
* ~~Исправить страницы формы~~ <br />пятница,  1 апреля 2022 г. 17:57:17 (+09)
* ~~Нагенерить много данных~~ <br/>Sat Apr  2 17:27:51 +09 2022
* ~~Добавить поиск и/или фильтрацию для заказов ~~ <br /> Sun Apr  3 16:09:35 +09 2022
* ~~Сделать пагинацию для заказов ~~ <br /> Sun Apr  3 16:09:35 +09 2022
* ~~Добавить поиск и/или фильтрацию для клиентов~~ Sun Apr  3 16:36:31 +09 2022
* ~~Сделать пагинацию для клиентов~~ Sun Apr  3 16:36:31 +09 2022
* ~~Добавить заказы клиента на `Clients/Details`~~ Mon Apr  4 14:03:25 +09 2022
* Сделать валидацию
* Посмотреть насчет тестов
* Посмотреть насчет локализации
* Развернуть на Heroku


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

### Подключение к базе

Строка подключения базы пишется в словаре в appsettings.json, и доступен из кода, например, при указании параметров типа `"name=ConnectionStrings:<ключ словаря строк подключения>"` в опциях. 

### Шаблоны Web API 
Файлы контроллеров для Web API при создании из шаблона в Visual Studio пустые, возвращают типа `return new string[] { "value1", "value2" };`. Устанавливаются пакеты `Microsoft.VisualStudio.Web.CodeGeneration.Design`, `Microsoft.EntityFrameworkCore.Design` (уже был установлен) и инструмент для скаффолдинга:

    dotnet tool install --global dotnet-aspnet-codegenerator --version 2.1.11

После этого можно по моделям породить контроллеры апи:

    dotnet aspnet-codegenerator controller -name ClientsController -async -api -m Client -dc SqlServerDbContext -outDir Controllers

    dotnet aspnet-codegenerator controller -name OrdersController -async -api -m Order -dc SqlServerDbContext -outDir Controllers

Дурацкий совет: не забыть остановить проект, если он запущен, перед выполнением этих команд.

Создаются методы `GetXs`, `GetX/id`, `PutX/id`, `PostX`, `DeleteX/id`. Весь CRUD работает. Поля в json конвертятся в JS camel case: `BirthDate` в C# -> `birthDate` в JSON. Список orders в клиентах при этом пуст [], и ссылка client в заказах null, хотя FK ClientId в табличках выставлен правильно. ???

### Понять, что происходит с навигационными свойствами

null в ссылках это нормально. Для загрузки связанных объектов надо вызывать методы типа `_context.Orders.orders.Include(o => o.Client)`. Если нужны только какие-то поля связанного объекта, например, если для заказа от клиента только имя, можно вместо сырых Entity использовать DTO (Data Transfer Object) которые служат как прокси, скрывают или переопределяют какие-то свойства. 

### Понять, на чем делать фронт

Можно прицепить React, но из скаффолдинга студии для .Net Core 2.1 он старый, тянет миллиард пакетов в node_modules, из них куча устарели, короче, возни ради 2 крудов многовато. Razor Pages – разумный вариант.

### Сгенерить шаблоны Razor Pages, посмотреть чего не хватает 

Скаффолдятся, но из VS2019 проблемы. Он пытается поставить тулзу для генерации, с новым ключом --ignore-failed-sources, которого не понимает установщик для версии 2.1. Надо помочь вручную из командной строки без этого ключа.

    dotnet tool install --global dotnet-aspnet-codegenerator --version 2.1.10

При генерации зачем-то пытается тянуть sqlite, хотя находит и принимает выбор DbContext из кода. Тоже с командной строки без ключа --useSqlite:

    dotnet aspnet-codegenerator --configuration "Debug" --project "/Users/pav/experiments/aeb/dotnet-web/dotnet-web.csproj" razorpage --model Client --dataContext SqlServerDbContext  --referenceScriptLibraries  --useDefaultLayout --no-build -outDir "/Users/pav/experiments/aeb/dotnet-web/Pages/Clients" --namespaceName ClientsOrders.Pages.Clients

    dotnet aspnet-codegenerator --configuration "Debug" --project "/Users/pav/experiments/aeb/dotnet-web/dotnet-web.csproj" razorpage --model Order --dataContext SqlServerDbContext  --referenceScriptLibraries  --useDefaultLayout --no-build -outDir "/Users/pav/experiments/aeb/dotnet-web/Pages/Orders" --namespaceName ClientsOrders.Pages.Orders --force 

Razor Pages генерятся. Есть какая-то валидация, не дает пустые поля. С клиентами: в поле по умолчанию для выбора даты какая-то проблема с верт выравниванием, зато есть выпадающий календарь. С заказами: на странице списка заказов и на странице Details для enuma status показывает нормально в списке ToDo/ InProgress/ Done, но на странице редактирования в форме в поле выбора пусто и ничего выбрать невозможно. Самое странное, что в cписке Client, в форме редактирования ClientId, и в обоих отображется, внезапно, ИНН. Надо, видимо, как-то указывать дефолтное представление для entity, типа `class.__str__` в питоне.

# Исправить страницы формы
`<select>`-элементы для ввода статуса и выбора клиента по FK надо населять в коде. 
Есть специальный тип SelectList, который содержит список
элементов SelectListItem, в принципе пар (значение, текст), зачем это нагородили,
непонятно. Генератор шаблонов почему-то запихал в качестве текста туда поле INN.
Со статусом проблема в том что он видимо вообще не умеет в enum, и
конструировать этот Select List надо руками. Зато применил Linq list comp:

    ViewData["Status"] = new SelectList(
                from v in Enum.GetValues(typeof(Status)).Cast<Status>()
                select new SelectListItem(((int)(v)).ToString(), v.ToString()),
                "Text", "Value"
    );

Надо добавить пагинацию и поиск. Вообще, конечно, идеально было бы тащить из API,
но это боль с JS.

# Нагенерить много данных
Есть библиотека `Bogus` - интерфейс к `Faker`. Попытался сгенерить побольше, данные сначла в List
объектов, потом вставлять в базу. Пытался находить максимум из имеющихся и инкрементировать Id,
Валится с ошибкой `Cannot insert explicit value for identity column in table 'Clients' when IDENTITY_INSERT is set to OFF.` Так как Id – int, то null присвоить нельзя, 0 не помогает. Помогает при конструировании вообще не указывать Id: `new Client(Name=..., )`. При этом в отладке видно, что Id=0, но видимо есть какое-то приватное свойство модели, которое показывает, что поле не заполнено. После сохранения Id получает корректные значения из базы. С заказами снова была та же ошибка, даже если не указывал Id, оказалось, не надо заполнять обратное свойство Orders на Client. После этого все сохраняется. 

Перенес в Models, там опять двадцать пять, seeding происходит внутри `OnModelCreating` и там 
используется, кажется, Fluent в стиле `modelBuilder.Entity<Client>().HasData(массив)`, и он наоборот
требует наличия Id 🤯. Вернул итерацию и явное `new Client(Id = i+1...)`. Потер базу, потер старые миграции. Новая генерит пять мегов миграций, решил не коммитить.

    dotnet ef migrations add Initial
    dotnet ef database update

# Сделал пагинацию, поиск и сортировку для заказов
Взял сторонний пагинатор LazZiya.TagHelpers. Не больно мудрый, надо самому добавлять 
`.Skip(pagestart).Take(pagelength)` для датасета, просто дает тег, который добавляет 
бутстраповский пагинатор на страницу. Добавил поиск и сортировку на датасет. Много возни
появляется: три независимых параметра, фильтр, порядок сортировки (на двух столбцах),
страница, все три и в .cs коде, и на .cshtml странице. Надо ничего не потерять при разных 
переходах.

# Сделал пагинацию, поиск и сортировку для списка клиентов
Копипаста практически 1 в 1, разве что Dto у заказов было. Надо бы отрефакторить.

# Закончил страницы
В Razor есть включение шаблонов, слегка отрефакторил список заказов с сортировкой и фильтрацией, 
чтобы можно было использовать и для общего списка заказов. Наладил проброс url для возврата
из crud-форм. Не удержался, повозился с оформлением

# Swagger
Подключил, схема подключения как обычно немного разная для каждой версии .NET. Разбираться не стал,
оставил как было из коробки
