using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Swagger;
using System.Text.RegularExpressions;

using ClientsOrders.Models;

namespace ClientsOrders
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            ConfigureDatabase(services);

            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1",
                    new Info {
                        Title = "Clients and Orders API",
                        Version = "v1",
                        Description = "Простой экспорт ASP.NET Core Web API в Swagger"
                    });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Clients and Orders API V1");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseMvc();
        }
        
        private string DBSERVER {get; set;}
        private const string DATABASE_NAME = "backend";
        
        // словарь опций БД.
        // начинающиеся с заглавной буквы образуют строку подключения
        // со строчных - различия для PostgreSQL и MS SQL Server 
        private Dictionary<string, string> dbOptions {get; set;}

        // строка подключения к БД, строится из dbOptions
        private string dbconn{
            get => string.Join(";", from k in dbOptions where Char.IsUpper(k.Key[0]) 
                                    select $"{k.Key}={k.Value}");
        }

        public void ConfigureDatabase(IServiceCollection services)
        {
            ParseDbUrl();
            ProcessDbOptions();
            Console.WriteLine("Parsed connection string is: " + dbconn);

            if (!dbOptions.ContainsKey("Database")) 
            {
                // База не указана: Создаем базу на сервере
                Console.WriteLine($"URL contains no database name. Сreating database \"{DATABASE_NAME}\"...");
                var dbCtx = new DbContext(
                    DatabaseOptions<DbContext>(DBSERVER, dbconn)
                );
                dbCtx.Database.ExecuteSqlCommand($"DROP DATABASE IF EXISTS {DATABASE_NAME};" +
                    $"CREATE DATABASE {DATABASE_NAME} {dbOptions["locale"]};");
                dbCtx.Dispose();
            
                dbOptions["Database"] = DATABASE_NAME;
                Console.WriteLine("New connection string: " + dbconn);
            }    

            // создаем данные в таблицах            
            var dbSeed = new DbSeedingContext(
                DatabaseOptions<MyDbContext>(DBSERVER, dbconn), DBSERVER
            );
            dbSeed.Database.EnsureCreated();
            int numClients = dbSeed.Clients.Count(), 
                numOrders = dbSeed.Orders.Count();

            Console.WriteLine($"The database \"{DATABASE_NAME}\" contains "+
                              $"{numClients} clients and {numOrders} orders.");

            // вручную увеличиваем начальное значение столбцов Id для Postgres
            if (DBSERVER == "PGSQL") {                    
                string idquery = $"ALTER TABLE \"Orders\" ALTER COLUMN \"Id\" RESTART WITH {numOrders+1};";
                idquery += $"ALTER TABLE \"Clients\" ALTER COLUMN \"Id\" RESTART WITH {numClients + 1};";
                dbSeed.Database.ExecuteSqlCommand(idquery);
            }
            dbSeed.Dispose();

            // создаем основной контекст БД для работы и добавляем к контейнеру DI
            if (DBSERVER == "MSSQL")
                services.AddDbContext<MyDbContext>(opt => opt.UseSqlServer(dbconn));
            else
                services.AddDbContext<MyDbContext>(opt => opt.UseNpgsql(dbconn));
        }        

        private void ParseDbUrl(){
            // парсит URL в стиле Heroku из переменной среды
            // и заполняет словарь dbOptions
            var dbUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            if (dbUrl == null)
            {
                Console.WriteLine("Assuming default database URL postgres://postgres:InsideContainer2022@localhost:54321\n" +
                    "If this is not correct, set the DATABASE_URL environment variable as \n" +
                    "    servertype://[user[:password]@]host[:port][/database]");
                dbUrl = "postgres://postgres:InsideContainer2022@localhost:54321";
            }
            else
            {
                Console.WriteLine("Using database URL " + dbUrl);
            }
            
            var dbUrlRe = new Regex(
                @"(?<Scheme>\w+)://((?<Username>[^:]+)(:(?<Password>[^@]+))?@)?" + 
                @"(?<Host>[A-za-z0-9.\-]+)(:(?<Port>\d+))?(/(?<Database>[^/?]+))?",
                RegexOptions.Compiled);
            var match = dbUrlRe.Match(dbUrl);

            var keys = dbUrlRe.GetGroupNames();
            dbOptions = new Dictionary<string, string>();
            foreach (var key in keys)
                if (Char.IsUpper(key[0]) && match.Groups[key].Value != "")
                    dbOptions[key] = match.Groups[key].Value;

        }

        private static void RenameKey(Dictionary<string, string> D, string Old, string New)
        {
            if (D.ContainsKey(Old))
            {
                D[New] = D[Old];
                D.Remove(Old);
            }
        }
        
         private void ProcessDbOptions(){
            DBSERVER = dbOptions["Scheme"] == "postgres" ? "PGSQL" : "MSSQL";
            dbOptions.Remove("Scheme");

            // части SQL, разные для Postgres / MSSQL
            switch (DBSERVER)
            {
                case "MSSQL": 
                    dbOptions["defdate"] = "CONVERT(datetime2(0),GETDATE())";
                    dbOptions["locale"] = "COLLATE Yakut_100_CI_AS_SC_UTF8";
                    // Позаботиться о разнице ключей в строке подключения            
                    RenameKey(dbOptions, "Username", "User Id");
                    RenameKey(dbOptions, "Host", "Server");
                    if (dbOptions.ContainsKey("Port"))
                    {
                        dbOptions["Server"] += "," + dbOptions["Port"];
                        dbOptions.Remove("Port");
                    }
                    break;
                case "PGSQL":
                    dbOptions["defdate"] = "NOW()";
                    dbOptions["locale"] = "ENCODING 'UTF8'";
                    
                    // Heroku Postgres requires SSL on db connections,
                    // https://stackoverflow.com/questions/44161509/message28000-no-pg-hba-conf-entry-for-host-xx-xxx-xxx-xxxx-user-user
                    if (dbOptions["Host"].EndsWith(".com"))
                    {
                        dbOptions["Sslmode"] = "Require"; 
                        dbOptions["Trust Server Certificate"] = "true";

                    } 
                    break;
            }
        }

        private static DbContextOptions<DBC> DatabaseOptions<DBC>(string DbServer, string ConnectionString) 
        where DBC : DbContext
        {
            var builder = new DbContextOptionsBuilder<DBC>();
            if (DbServer == "MSSQL")
                return builder.UseSqlServer(ConnectionString).Options;
            else
                return builder.UseNpgsql(ConnectionString).Options;
        }
    }
}
