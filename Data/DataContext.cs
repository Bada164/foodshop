using foodshop.Models;
using Microsoft.EntityFrameworkCore;

namespace foodshop.Data
{
    public class DataContext : DbContext
    {
        private readonly IConfiguration _config;

        public DataContext(IConfiguration config)
        {
            _config = config;
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        public DbSet<Favorite> Favorites { get; set; }

        public DbSet<Addon> Addons { get; set; }
        public DbSet<CartItemAddon> CartItemAddons { get; set; }
        public DbSet<OrderItemAddon> OrderItemAddons { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //optionsBuilder.UseNpgsql(_config.GetConnectionString("DefaultConnection"), optionsBuilder => optionsBuilder.EnableRetryOnFailure());
                optionsBuilder
            .UseNpgsql(_config.GetConnectionString("DefaultConnection"), optionsBuilder => optionsBuilder.EnableRetryOnFailure())
            .UseSnakeCaseNamingConvention();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>().ToTable("users").HasKey(u => u.Id);
            modelBuilder.Entity<Category>().ToTable("categories").HasKey(u => u.Id);
            modelBuilder.Entity<Product>().ToTable("products").HasKey(u => u.Id);
            modelBuilder.Entity<Order>().ToTable("orders").HasKey(u => u.Id);
            modelBuilder.Entity<OrderItem>().ToTable("orderItems").HasKey(u => u.Id);

            modelBuilder.Entity<Cart>().ToTable("carts");
            modelBuilder.Entity<CartItem>().ToTable("cart_items");


            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId);

            modelBuilder.Entity<Category>().ToTable("categories");


            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

            modelBuilder.Entity<Favorite>().ToTable("favorites");

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId);

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Product)
                .WithMany()
                .HasForeignKey(f => f.ProductId);


            modelBuilder.Entity<Favorite>()
                .HasIndex(f => new { f.UserId, f.ProductId });
            //.IsUnique();

            modelBuilder.Entity<Addon>().ToTable("addons");
            modelBuilder.Entity<CartItemAddon>().ToTable("cart_item_addons");
            modelBuilder.Entity<OrderItemAddon>().ToTable("order_item_addons");

            // CartItemAddon kapcsolatok
            modelBuilder.Entity<CartItemAddon>()
                .HasOne(cia => cia.CartItem)
                .WithMany(ci => ci.Addons)
                .HasForeignKey(cia => cia.CartItemId);

            modelBuilder.Entity<CartItemAddon>()
                .HasOne(cia => cia.Addon)
                .WithMany()
                .HasForeignKey(cia => cia.AddonId);

            // OrderItemAddon kapcsolat
            modelBuilder.Entity<OrderItemAddon>()
                .HasOne(oia => oia.OrderItem)
                .WithMany(oi => oi.Addons)
                .HasForeignKey(oia => oia.OrderItemId);
        }
    }
}