using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Northwind.Application.Auth;
using Northwind.Application.Interfaces;
using Northwind.Domain.Entities;

namespace Northwind.Persistence
{
    public class NorthwindDbContext : IdentityDbContext<ApplicationUser>, INorthwindDbContext
    {
        public NorthwindDbContext(DbContextOptions<NorthwindDbContext> options)
            : base(options)
        {
            //Dùng nếu không migration
            //Database.EnsureDeleted();
            //Database.EnsureCreated();
            //NorthwindInitializer.Initialize(this);
        }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<EmployeeTerritory> EmployeeTerritories { get; set; }

        public DbSet<OrderDetail> OrderDetails { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Region> Region { get; set; }

        public DbSet<Shipper> Shippers { get; set; }

        public DbSet<Supplier> Suppliers { get; set; }

        public DbSet<Territory> Territories { get; set; }

        //public DbSet<User> Users { get; set; }

      
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<EmployeeTerritory>().HasKey(e => new {e.EmployeeId, e.TerritoryId});
            //modelBuilder.Entity<OrderDetail>().HasKey(e => new { e.OrderId, e.ProductId });
            //modelBuilder.Entity<User>().OwnsOne(u => u.AdAccount);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(NorthwindDbContext).Assembly);

            
        }
    }
}
