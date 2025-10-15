using Microsoft.EntityFrameworkCore;
using WebMobileStore.Models.Entity;

namespace WebMobileStore.Models.Data
{
    public class MobileStoreContext : DbContext
    {
        public MobileStoreContext(DbContextOptions<MobileStoreContext> options) : base(options) { }

        public DbSet<Users> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Carts> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Reviews> Reviews { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== Đặt tên bảng =====
            modelBuilder.Entity<Users>().ToTable("Users");
            modelBuilder.Entity<Address>().ToTable("Addresses");
            modelBuilder.Entity<Carts>().ToTable("Carts");
            modelBuilder.Entity<CartItem>().ToTable("CartItems");
            modelBuilder.Entity<Products>().ToTable("Products");
            modelBuilder.Entity<ProductVariant>().ToTable("ProductVariants");
            modelBuilder.Entity<Categories>().ToTable("Categories");
            modelBuilder.Entity<Brand>().ToTable("Brands");
            modelBuilder.Entity<Orders>().ToTable("Orders");
            modelBuilder.Entity<OrderDetail>().ToTable("OrderDetails");
            modelBuilder.Entity<Payment>().ToTable("Payments");
            modelBuilder.Entity<Reviews>().ToTable("Reviews");
            modelBuilder.Entity<ProductImage>().ToTable("ProductImages");

            // ===== Quan hệ 1-1 =====
            modelBuilder.Entity<Users>()
                .HasOne(u => u.Address)
                .WithOne(a => a.Users)
                .HasForeignKey<Address>(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Users>()
                .HasOne(u => u.Carts)
                .WithOne(c => c.User)
                .HasForeignKey<Carts>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Orders>()
                .HasOne(o => o.Payment)
                .WithOne(p => p.Orders)
                .HasForeignKey<Payment>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== Quan hệ 1-N =====

            // Categories -> Brands
            modelBuilder.Entity<Brand>()
                .HasOne(b => b.Category)
                .WithMany(c => c.Brands)
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Products>()
            .HasOne(p => p.Brand)
            .WithMany(b => b.Products)
            .HasForeignKey(p => p.BrandId)
            .OnDelete(DeleteBehavior.NoAction);


            // Products -> ProductVariants
            modelBuilder.Entity<ProductVariant>()
                .HasOne(pv => pv.Products)
                .WithMany(p => p.ProductVariants)
                .HasForeignKey(pv => pv.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Products -> ProductImages
            modelBuilder.Entity<ProductImage>()
                .HasOne(pi => pi.Products)
                .WithMany(p => p.ProductImages)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Carts -> CartItems
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Carts)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            // Orders -> OrderDetails
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Orders)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Users -> Orders
            modelBuilder.Entity<Orders>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== Tắt cascade để tránh lỗi =====
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.ProductVariant)
                .WithMany(pv => pv.CartItems)
                .HasForeignKey(ci => ci.ProductVariantId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.ProductVariant)
                .WithMany(pv => pv.OrderDetails)
                .HasForeignKey(od => od.ProductVariantId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Reviews>()
                .HasOne(r => r.Users)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Reviews>()
                .HasOne(r => r.Products)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Reviews>()
                .HasOne(r => r.Orders)
                .WithMany(o => o.reviews)
                .HasForeignKey(r => r.OrderId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
