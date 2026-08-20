using InventoryManagement.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Web.Data
{
    // Entity Framework Core veritabanı bağlantı sınıfımız.
    // Program.cs içinde DI (Dependency Injection) ile kaydediliyor.
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Her DbSet, veritabanındaki bir tabloyu temsil eder.
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Stock> Stocks => Set<Stock>();
        public DbSet<Admin> Admins => Set<Admin>();

        // Tablo ilişkilerini ve kısıtlarını burada Fluent API ile belirtiyoruz.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Kategori adı benzersiz (unique) olsun istiyoruz.
            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Name)
                .IsUnique();

            // Ürün SKU değeri de benzersiz olmalı.
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Sku)
                .IsUnique();

            // Kategori - Ürün ilişkisi: bir kategoriye bağlı ürünler silinemesin (Restrict).
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ürün - Stok ilişkisi: ürün silinince stok kaydı da silinsin (Cascade).
            modelBuilder.Entity<Stock>()
                .HasOne(s => s.Product)
                .WithOne(p => p.Stock)
                .HasForeignKey<Stock>(s => s.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Bir ürünün sadece bir stok kaydı olabilsin diye ProductId'yi de unique yapıyoruz.
            modelBuilder.Entity<Stock>()
                .HasIndex(s => s.ProductId)
                .IsUnique();

            // Kullanıcı adı benzersiz olsun.
            modelBuilder.Entity<Admin>()
                .HasIndex(a => a.Username)
                .IsUnique();
        }
    }
}
