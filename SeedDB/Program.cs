using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

using Bogus;

using ClientsOrders.Models;

namespace SeedDB
{   
    public class Program
    {
        const string DBSERVER = "PGSQL";  
        static Dictionary<string, Dictionary<string, string>> dbopt =
            new Dictionary<string, Dictionary<string, string>>
            {
                ["PGSQL"] = new Dictionary<string, string> {
                    ["defdate"] = "NOW()",
                    ["locale"] = "ENCODING 'UTF8'"
                },
                ["MSSQL"] = new Dictionary<string, string>
                {
                    ["defdate"] = "CONVERT(datetime2(0),GETDATE())",
                    ["locale"] = "COLLATE Yakut_100_CI_AS_SC_UTF8"
                }
            };

        public class DataSeedingContext : DbContext
        {

            public DbSet<Client> Clients { get; set; }
            public DbSet<Order> Orders { get; set; }

            // #region DefineLoggerFactory
            // public static readonly LoggerFactory MyLoggerFactory
            //     = new LoggerFactory(new[] {new ConsoleLoggerProvider(((category, level) => true), true)});
            // #endregion

            // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            // => optionsBuilder.UseLoggerFactory(MyLoggerFactory);

            public DataSeedingContext(DbContextOptions<DataSeedingContext> options)
                : base(options)
            {
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                // Автозаполнению текущего времени в заказе
                modelBuilder.Entity<Order>()
                    .Property(order => order.CreatedOn)
                    .HasDefaultValueSql(dbopt[DBSERVER]["defdate"]);

                // Identity для Postgres
                if (DBSERVER == "PGSQL") {
                    modelBuilder.Entity<Client>()
                        .Property(p => p.Id)
                        .UseNpgsqlIdentityByDefaultColumn();
                    modelBuilder.Entity<Order>()
                        .Property(p => p.Id)
                        .UseNpgsqlIdentityByDefaultColumn();
                }

                // Индексы
                modelBuilder.Entity<Client>()
                    .HasIndex(client => new { client.Name, client.Id });
                modelBuilder.Entity<Order>()
                    .HasIndex(order => new { order.Name, order.Id });
                modelBuilder.Entity<Order>()
                    .HasIndex(order => new { order.CreatedOn, order.Id });
                modelBuilder.Entity<Order>()
                    .HasIndex(order => new { order.ClientId, order.CreatedOn, order.Id });
                modelBuilder.Entity<Order>()
                    .HasIndex(order => new { order.ClientId, order.Name, order.Id });

                // родить столько клиентов...
                const int clientCount = 1000;

                // каждый из которых имеет одно из таких вариантов количества заказов
                var orderCounts = new int[] { 0, 1, 3, 4, 5, 8, 14, 55 };

                Faker f = new Faker();
                Randomizer.Seed = new Random(42);

                List<Client> BogusClients = new List<Client>();
                List<Order> BogusOrders = new List<Order>();

                int totalOrders = 0;
                DateTime person_t0 = new DateTime(1920, 1, 1, 0, 0, 0),
                         person_t1 = new DateTime(2000, 12, 31, 0, 0, 0),
                         company_t0 = new DateTime(1720, 1, 1, 0, 0, 0),
                         company_t1 = new DateTime(2021, 1, 1, 0, 0, 0),
                         order_t0 = new DateTime(2021, 1, 1, 0, 0, 0),
                         order_t1 = new DateTime(2022, 3, 31, 0, 0, 0);  

                for (int i = 0; i < clientCount; i++)
                {
                    bool isPerson = f.Random.Bool();
                    String name = isPerson ? f.Name.FullName() : f.Company.CompanyName(),
                           inn = isPerson ? new Randomizer().Replace("############") :
                                             new Randomizer().Replace("##########");
                    DateTime bd = isPerson ? f.Date.Between(person_t0, person_t1) :
                                             f.Date.Between(company_t0, company_t1);
                    var client = new Client
                    {
                        Id = i + 1,
                        Name = name,
                        BirthDate = bd,
                        Email = f.Internet.Email(),
                        Inn = inn,
                        PhoneNumber = f.Phone.PhoneNumberFormat()
                    };
                    BogusClients.Add(client);

                    var howManyOrders = f.PickRandom<int>(orderCounts);
                    for (int j = 0; j < howManyOrders; j++)
                    {
                        DateTime dt = f.Date.Between(order_t0, order_t1);
                        dt = dt.AddMilliseconds(-dt.Millisecond);
                        BogusOrders.Add(new Order()
                        {
                            Id = totalOrders + 1,
                            ClientId = client.Id, //i + 1,
                            Name = f.Commerce.ProductName(),
                            CreatedOn = dt,
                            Status = f.PickRandom<Status>(Status.InProgress, Status.Done, Status.ToDo)
                        });
                        totalOrders++;
                    }
                }

                modelBuilder.Entity<Client>().HasData(BogusClients.ToArray());
                modelBuilder.Entity<Order>().HasData(BogusOrders.ToArray());
            }
        }

        public static void Main(string[] args)
        {
            var dbconn = Environment.GetEnvironmentVariable("DBCONN");
            if (dbconn == null)
            {
                Console.WriteLine("ERROR: Set database connection string in the DBCONN environment variable. Do not use database name, BACKEND will be used.");
                Environment.Exit(1);
            }

            var dbCtx = new DbContext(
                DBSERVER == "MSSQL" ?
                    new DbContextOptionsBuilder<DbContext>()
                    .UseSqlServer(dbconn)
                    .Options
                :
                    new DbContextOptionsBuilder<DbContext>()
                    .UseNpgsql(dbconn)
                    .Options
            );
            dbCtx.Database.ExecuteSqlCommand($"DROP DATABASE IF EXISTS backend; "+
                $"CREATE DATABASE backend {dbopt[DBSERVER]["locale"]};");

            dbconn += (dbconn.TrimEnd().EndsWith(';') ? "" : ";") + $"Database=backend";
            var seedingCtx = new DataSeedingContext(
                DBSERVER == "MSSQL" ?
                    new DbContextOptionsBuilder<DataSeedingContext>()
                    .UseSqlServer(dbconn)
                    .Options
                :
                    new DbContextOptionsBuilder<DataSeedingContext>()
                    .UseNpgsql(dbconn)
                    .Options
            );

            seedingCtx.Database.EnsureCreated();

            int numClients = seedingCtx.Clients.Count(), 
                numOrders = seedingCtx.Orders.Count();

            Console.WriteLine("The database backend now has "+
                              $"{numClients} clients and "+
                              $"{numOrders} orders.");

            if (DBSERVER == "PGSQL") {
                seedingCtx.Database.ExecuteSqlCommand($"ALTER TABLE \"Orders\" ALTER COLUMN \"Id\" RESTART WITH {numOrders + 1};"
                + $"ALTER TABLE \"Clients\" ALTER COLUMN \"Id\" RESTART WITH {numClients + 1};");
            }

            Console.WriteLine("Done");
        }

    }
}
