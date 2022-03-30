using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace dotnet_web.Models
{
    public enum AStatus { ToDo, InProgress, Done }


    public class Client
    {
        private List<Order> orders;

        public int Id { get; set; }

        [MaxLength(100), Required]
        public string Name { get; set; }

        [Column(TypeName = "Date"), Required]
        public DateTime BirthDate { get; set; }

        [MinLength(10), MaxLength(12), Required]
        public string Inn { get; set; }

        [MaxLength(14), Required]
        public string PhoneNumber { get; set; }

        [MaxLength(60)]
        public string Email { get; set; }

        public IList<Order> Orders { get; } = new List<Order>();
    }


    public class Order
    {
        public int Id { get; set; }

        [MaxLength(100), Required]
        public string Name { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public AStatus Status { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; }
    }
    

    public class SqlServerDbContext : DbContext
    {
        public SqlServerDbContext(DbContextOptions<SqlServerDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("name=ConnectionStrings:SQLServerDocker");
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Order> Orders { get; set; }
    }
}
