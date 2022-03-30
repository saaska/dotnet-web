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

        [DataType(DataType.Date)]
        public DateTime CreatedOn { get; set; }

        public AStatus Status { get; set; }
    }

    public class Client
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [MaxLength(12)]
        public string Inn { get; set; }

        [MaxLength(14)]
        public string PhoneNumber { get; set; }

        [MaxLength(60)]
        public string Email { get; set; }

        List<Order> Orders { get; set; }
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
