using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using Bogus;

using ClientsOrders.Models;

namespace SeedDB
{
    public class Program
    {
        public class DataSeedingContext : DbContext
        {
            public DbSet<Client> Clients { get; set; }
            public DbSet<Order> Orders { get; set; }

            public DataSeedingContext(DbContextOptions<DataSeedingContext> options)
                : base(options)
            {
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                // Автозаполнению текущего времени в заказе
                modelBuilder.Entity<Order>()
                    .Property(order => order.CreatedOn)
                    .HasDefaultValueSql("CONVERT(datetime2(0),GETDATE())");

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
                            ClientId = i + 1,
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

        const string DATABASE_NAME = "backend";

        public static void Main(string[] args)
        {
            var dbpass = Environment.GetEnvironmentVariable("DBPASS");
            if (dbpass == null)
            {
                Console.WriteLine("ERROR: Set database password in the DBPASS environment variable.");
                Environment.Exit(1);
            }

            var dbCtx = new DbContext(
                new DbContextOptionsBuilder<DbContext>()
                .UseSqlServer($"Server=127.0.0.1,54321;User Id=sa;Password={dbpass}")
                .Options
            );
            dbCtx.Database.ExecuteSqlCommand("DROP DATABASE IF EXISTS backend;"+
                $"CREATE DATABASE {DATABASE_NAME} COLLATE Yakut_100_CI_AS_SC_UTF8;");

            var seedingCtx = new DataSeedingContext(
                new DbContextOptionsBuilder<DataSeedingContext>()
                .UseSqlServer($"Server=127.0.0.1,54321;User Id=sa;Password={dbpass};Database={DATABASE_NAME}")
                .Options
            );

            seedingCtx.Database.EnsureCreated();

            Console.WriteLine($"The database <{DATABASE_NAME}> now has "+
                              $"{seedingCtx.Clients.Count()} clients and "+
                              $"{seedingCtx.Orders.Count()} orders.");

            Console.WriteLine("Done");
        }

    }
}
