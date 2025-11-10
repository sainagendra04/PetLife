using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace PetLife.Models.DBContext
{
    public class PetLifeDBContext : DbContext
    {
        public PetLifeDBContext(DbContextOptions<PetLifeDBContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<PetFood> PetFoods { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PetCategory> PetCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //one-to-one User-->Customer
            modelBuilder.Entity<User>()
                .HasOne(x=>x.Customer)
                .WithOne(x => x.User)
                .HasForeignKey<Customer>(x => x.UserId);

            //one-to-many Customer-->Order
            modelBuilder.Entity<Customer>()
                .HasMany(x=>x.Orders)
                .WithOne(x => x.Customer)
                .HasForeignKey(x => x.CustomerId);
            //one-to-many Order-->OrderItem
            modelBuilder.Entity<Order>()
                .HasMany(x => x.OrderItems)
                .WithOne(x => x.Order)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            //one-to-one Order-->Payment
            modelBuilder.Entity<Order>()
                .HasOne(x => x.Payment)
                .WithOne(x => x.Order)
                .HasForeignKey<Payment>(x => x.OrderId);
            //one-to-many PetCategory-->Pets
            modelBuilder.Entity<PetCategory>()
                .HasMany(x=>x.Pets)
                .WithOne(x=>x.Category)
                .HasForeignKey(x => x.PetCategoryId);
            //one-to-many PetCategory-->PetFoods
            modelBuilder.Entity<PetCategory>()
                .HasMany(x => x.PetFood)
                .WithOne(x => x.Category)
                .HasForeignKey(x => x.PetCategoryId);
        }
    }
}
