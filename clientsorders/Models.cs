using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace ClientsOrders.Models
{
    public enum Status { ToDo, InProgress, Done }


    public class Client
    {
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

        virtual public IList<Order> Orders { get; } = new List<Order>();
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


    public class OrderDto
    {
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
        public string ClientName { get; set; }
    }


    public class MyDbContext : DbContext
    {
        const string DBSERVER = "PGSQL";
        
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

        public MyDbContext(DbContextOptions<MyDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Автозаполнению текущего времени в заказе
            modelBuilder.Entity<Order>()
                .Property(order => order.CreatedOn)
                .HasDefaultValueSql(dbopt[DBSERVER]["defdate"]);

            if (DBSERVER == "PGSQL") {
                modelBuilder.Entity<Client>()
                    .Property(p => p.Id)
                    .UseNpgsqlIdentityByDefaultColumn();
                modelBuilder.Entity<Order>()
                    .Property(p => p.Id)
                    .ValueGeneratedOnAdd();
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
}
