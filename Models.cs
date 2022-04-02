using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using Bogus;

namespace dotnet_web.Models
{
    public enum Status { ToDo, InProgress, Done }


    public class Client
    {
        private List<Order> orders;

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
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
        public string Inn { get; set; }
        
        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        [MinLength(6)]
        [MaxLength(14)]
        [Required]
        public string PhoneNumber { get; set; }

        [MaxLength(60)]
        public string Email { get; set; }

        public IList<Order> Orders { get; } = new List<Order>();
    }


    public class Order
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
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

        [ForeignKey("ClientId")]
        public Client Client { get; set; }
    }
    

    public class SqlServerDbContext : DbContext
    {
        public SqlServerDbContext(DbContextOptions<SqlServerDbContext> options)
            : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()
                .Property(order => order.CreatedOn)
                .HasDefaultValueSql("CONVERT(datetime2(0),GETDATE())");

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
                     company_t1 = new DateTime(2022, 1, 1, 0, 0, 0);

            for (int i = 0; i < clientCount; i++)
            {
                bool isPerson = f.Random.Bool();
                String name = isPerson ? f.Name.FullName() : f.Company.CompanyName(),
                       inn  = isPerson ? new Randomizer().Replace("############") :
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
                    DateTime dt = f.Date.Recent();
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
}
