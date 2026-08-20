using InventoryManagement.Web.Data;
using InventoryManagement.Web.Logging;
using InventoryManagement.Web.Middlewares;
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

            // Kendi yazdığımız dosya logger'ını da ekliyoruz.
            // Bu sayede loglar hem konsola (varsayılan) hem de Logs klasöründeki dosyaya yazılacak.
            builder.Logging.AddProvider(new DosyayaYazanLoggerProvider("Logs"));

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

            // Genel hata yönetimi middleware'ini boru hattının en başına ekliyoruz
            // ki kendisinden sonraki tüm middleware ve controller'lardaki
            // yakalanmayan hataları görebilsin.
            app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

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
