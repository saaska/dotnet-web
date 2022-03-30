using System.Collections.Generic;
using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace dotnet_web.Models
{
    public enum AStatus { ToDo, InProgress, Done }

    public class Order
    {
        public int Id { get; set; }

        [MaxLength(100), Required]
        public string Name { get; set; }

        [DataType(DataType.Date), Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public AStatus Status { get; set; }
    }

    public class Client
    {
        private List<Order> orders;

        public int Id { get; set; }

        [MaxLength(100), Required]
        public string Name { get; set; }

        [DataType(DataType.Date), Required]
        public DateTime BirthDate { get; set; }

        [MinLength(10), MaxLength(12), Required]
        public string Inn { get; set; }

        [MaxLength(14), Required]
        public string PhoneNumber { get; set; }

        [MaxLength(60)]
        public string Email { get; set; }

        List<Order> Orders { get; set;}
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
