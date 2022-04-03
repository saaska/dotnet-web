using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using Bogus;

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


    public class SqlServerDbContext : DbContext
    {
        public SqlServerDbContext(DbContextOptions<SqlServerDbContext> options)
            : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Order> Orders { get; set; }
    }
}
