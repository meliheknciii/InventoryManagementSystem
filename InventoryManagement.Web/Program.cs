using InventoryManagement.Web.Data;
using InventoryManagement.Web.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // MVC için Controller ve View desteğini ekliyoruz.
            builder.Services.AddControllersWithViews();

            // Veritabanı bağlantı bilgisini appsettings.json'dan okuyoruz.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' bulunamadı.");

            // EF Core DbContext'i DI container'a kaydediyoruz.
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Repository'leri DI container'a kaydediyoruz.
            // AddScoped: her HTTP isteğinde yeni bir örnek (instance) oluşturulur.
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IStockRepository, StockRepository>();

            var app = builder.Build();

            // HTTP pipeline (istek/cevap boru hattı) yapılandırması.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
