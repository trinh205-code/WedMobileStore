using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using WebMobileStore.Models.Entity;

namespace WebMobileStore.Models.Data
{
    public class MobileStoreContext : DbContext 
    {
        public MobileStoreContext(DbContextOptions<MobileStoreContext> options) : base(options)
        {}

        public DbSet<Users> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Carts> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<CategoryGroup> CategoryGroups { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Reviews> Reviews { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // === Tên bảng ===
            modelBuilder.Entity<Users>().ToTable("Users");
            modelBuilder.Entity<Address>().ToTable("Addresses");
            modelBuilder.Entity<Carts>().ToTable("Carts");
            modelBuilder.Entity<CartItem>().ToTable("CartItems");
            modelBuilder.Entity<Products>().ToTable("Products");
            modelBuilder.Entity<Categories>().ToTable("Categories");
            modelBuilder.Entity<CategoryGroup>().ToTable("CategoryGroups");
            modelBuilder.Entity<Orders>().ToTable("Orders");
            modelBuilder.Entity<OrderDetail>().ToTable("OrderDetails");
            modelBuilder.Entity<Payment>().ToTable("Payments");
            modelBuilder.Entity<Reviews>().ToTable("Reviews");
            modelBuilder.Entity<ProductImage>().ToTable("ProductImages");


            // Users -> Address (1-1)
            modelBuilder.Entity<Users>()
                .HasOne(u => u.Address)
                .WithOne(a => a.Users)
                .HasForeignKey<Address>(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Users -> Carts (1-1)
            modelBuilder.Entity<Users>()
                .HasOne(u => u.Carts)
                .WithOne(c => c.User)
                .HasForeignKey<Carts>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Orders -> Payment (1-1)
            modelBuilder.Entity<Orders>()
                .HasOne(o => o.Payment)
                .WithOne(p => p.Orders)
                .HasForeignKey<Payment>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Reviews relationships - TẮT CASCADE DELETE để tránh multiple cascade paths
            modelBuilder.Entity<Reviews>()
                .HasOne(r => r.Users)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.NoAction); // ← Quan trọng

            modelBuilder.Entity<Reviews>()
                .HasOne(r => r.Products)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.NoAction); // ← Quan trọng

            modelBuilder.Entity<Reviews>()
                .HasOne(r => r.Orders)
                .WithMany(o => o.reviews)
                .HasForeignKey(r => r.OrderId)
                .OnDelete(DeleteBehavior.NoAction); // ← Quan trọng

            // CartItem relationships - FIX foreign key naming
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Carts)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Products)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ProductsId)
                .OnDelete(DeleteBehavior.NoAction); // Tránh cascade conflict

            // OrderDetail relationships - FIX foreign key naming
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Orders)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Products)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.NoAction); // Tránh cascade conflict

            // ProductImage relationships - FIX foreign key naming
            modelBuilder.Entity<ProductImage>()
                .HasOne(pi => pi.Products)
                .WithMany(p => p.ProductImages)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

        }

    }
}
