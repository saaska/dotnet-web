using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace dotnet_web.Models
{
    public enum Status { ToDo, InProgress, Done }


    public class Client
    {
        private List<Order> orders;

        public int Id { get; set; }

        [MaxLength(100)]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Date of Birth")]
        [Column(TypeName = "Date")]
        [Required]
        public DateTime BirthDate { get; set; }

        [Display(Name = "INN")]
        [MinLength(10), MaxLength(12)]
        [Required]
        public string Inn { get; set; }
        
        [Display(Name = "Phone Number")]
        [MaxLength(14)]
        [Required]
        public string PhoneNumber { get; set; }

        [MaxLength(60)]
        public string Email { get; set; }

        public IList<Order> Orders { get; } = new List<Order>();
    }


    public class Order
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("name=ConnectionStrings:SQLServerDocker");
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            GregorianCalendar gc = new GregorianCalendar();
            modelBuilder.Entity<Client>().HasData(
                new Client()
                {
                    Id = 1,
                    Name = "Поликарп Петров",
                    BirthDate = new DateTime(1970, 1, 13, gc),
                    Inn = "140123456789",
                    PhoneNumber = "+79245551122"
                },
                new Client()
                {
                    Id = 2,
                    Name = "Юникс Мультиксович Линукс",
                    BirthDate = new DateTime(1970, 1, 1, gc),
                    Inn = "1234567890",
                    PhoneNumber = "+14151234567",
                    Email = "nokia@bell-labs.com"
                },
                new Client()
                {
                    Id = 3,
                    Name = "Шерлок Холмс",
                    BirthDate = new DateTime(1854, 1, 6, gc),
                    Inn = "1234567890",
                    PhoneNumber = "+4402072243688",
                    Email = "sherlock@holmes.co.uk"
                }
            );
            modelBuilder.Entity<Order>().HasData(
                new Order()
                {
                    Id = 1,
                    Name = "Драконьи шкуры",
                    CreatedOn = new DateTime(2022, 3, 30, 17, 53, 0),
                    ClientId = 1,
                    Status = Status.Done
                },
                new Order()
                {
                    Id = 2,
                    Name = "Автозапчасти",
                    CreatedOn = new DateTime(2022, 3, 31, 10, 0, 0),
                    ClientId = 1,
                    Status = Status.InProgress
                },
                new Order()
                {
                    Id = 3,
                    Name = "8\" Inch Floppy Disks",
                    CreatedOn = new DateTime(2021, 2, 1, 10, 05, 34),
                    ClientId = 2,
                    Status = Status.ToDo
                },
                new Order()
                {
                    Id = 4,
                    Name = "Bell Labs Technical Reports 1970-1974",
                    CreatedOn = new DateTime(2019, 12, 31, 18, 30, 0),
                    ClientId = 2,
                    Status = Status.ToDo
                }
            );
        }
    }
}
