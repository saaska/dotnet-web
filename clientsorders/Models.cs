using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

using Bogus;

namespace ClientsOrders.Models
{
    public enum Status { ToDo, InProgress, Done }


    // Основа для Клиента, без навигационного свойства Заказы
    public class ClientBase
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Date of Birth")]
        [Column(TypeName = "Date")]
        [DataType(DataType.Date)]
        [Required]
        public DateTime BirthDate { get; set; }

        [Display(Name = "INN")]
        [MinLength(10), MaxLength(12)]
        [Required]
        [RegularExpression(@"^(\d{10}|\d{12})$", ErrorMessage="Please enter 10 or 12 digits")]
        public string Inn { get; set; }
        
        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        [MinLength(6), MaxLength(14)]
        [Required]
        [RegularExpression(@"[0-9\-() \+]{6,14}", ErrorMessage="Please enter from 6 to 14 digits")]
        public string PhoneNumber { get; set; }

        [MaxLength(60)]
        public string Email { get; set; }

        public  ClientBase(){}

        // "стирающий" конструктор, отбрасыващий Заказы
        public ClientBase(Client c) : this() {
            Id = c.Id; 
            Name = c.Name; 
            BirthDate = c.BirthDate; 
            Inn = c.Inn; 
            PhoneNumber = c.PhoneNumber; 
            Email = c.Email; 
        }
    }

    
    public class Client : ClientBase
    {
        virtual public IList<Order> Orders { get; } = new List<Order>();
    }


    // Заказ, без навигационного свойства Клиент, только ClientId
    public class OrderBase
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Created On")]
        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public Status Status { get; set; }

        [Display(Name = "Client")]
        public int ClientId { get; set; }

        public OrderBase() {}

        // "стирающий" конструктор, отбрасыващий ссылку на Клиента
        public OrderBase(Order o) : this() 
        {
            Id = o.Id;
            Name = o.Name;
            CreatedOn = o.CreatedOn;
            Status = o.Status;
            ClientId = o.ClientId;
        }
    }


    public class Order : OrderBase
    {
        [ForeignKey("ClientId")]
        public Client Client { get; set; }
    }


    // используется на сайте, для показа в заказе имени клиента вместо его Id
    public class OrderDto : OrderBase
    {
        [Display(Name = "Client")]
        public string ClientName { get; set; }
    }

    // используется в api, содержит массив id заказов клиента
    public class ClientDetailDto : ClientBase
    {
        virtual public IList<int> Orders { get; set; } = new List<int>();

        public ClientDetailDto() {}

        public ClientDetailDto(Client c) {
            Id = c.Id;
            Name = c.Name;
            BirthDate = c.BirthDate;
            Inn = c.Inn;
            PhoneNumber = c.PhoneNumber;
            Email = c.Email;
            Orders = (from o in c.Orders select o.Id).ToList();
        }
    }


    // основной класс для подключения к БД
    public class MyDbContext : DbContext
    {
        public string DBSERVER { get; }
        
        static Dictionary<string, Dictionary<string, string>> dbopt =
            new Dictionary<string, Dictionary<string, string>>
            {
                ["PGSQL"] = new Dictionary<string, string>
                {
                    ["defdate"] = "NOW()",
                    ["locale"] = "ENCODING 'UTF8'"
                },
                ["MSSQL"] = new Dictionary<string, string>
                {
                    ["defdate"] = "CONVERT(datetime2(0),GETDATE())",
                    ["locale"] = "COLLATE Yakut_100_CI_AS_SC_UTF8"
                }
            };

        public MyDbContext(DbContextOptions<MyDbContext> options, string _DBSERVER="PGSQL")
            : base(options)
        {
            DBSERVER = _DBSERVER;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Автозаполнению текущего времени в заказе
            modelBuilder.Entity<Order>()
                .Property(order => order.CreatedOn)
                .HasDefaultValueSql(dbopt[DBSERVER]["defdate"]);

            if (DBSERVER == "PGSQL") {
                modelBuilder.Entity<Client>()
                    .Property(p => p.Id).UseNpgsqlIdentityByDefaultColumn();
                modelBuilder.Entity<Order>()
                    .Property(p => p.Id).UseNpgsqlIdentityByDefaultColumn();
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
            }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Order> Orders { get; set; }
    }


    // используется для генерации данных для новой пустой базы
    public class DbSeedingContext : MyDbContext
    {
        public DbSeedingContext(DbContextOptions<MyDbContext> options, string _DBSERVER="PGSQL")
            : base(options, _DBSERVER)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // родить столько клиентов...
            const int clientCount = 500;

            // каждый из которых имеет одно из таких вариантов количества заказов
            var orderCounts = new int[] { 0, 1, 3, 4, 5, 8, 14, 45 };

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

            Console.WriteLine("Start seeding the DB...");
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
    
}
