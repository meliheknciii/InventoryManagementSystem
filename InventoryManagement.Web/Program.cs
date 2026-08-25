using InventoryManagement.Web.Data;
using InventoryManagement.Web.Logging;
using InventoryManagement.Web.Middlewares;
using InventoryManagement.Web.Models;
using InventoryManagement.Web.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // MVC için Controller ve View desteğini ekliyoruz.
            // Global filtre ile, [AllowAnonymous] işaretlenmemiş tüm action'lar
            // giriş yapmış (admin) kullanıcı gerektirir.
            builder.Services.AddControllersWithViews(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });

            // Admin girişi için cookie tabanlı authentication ekliyoruz.
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/Login";
                    options.ExpireTimeSpan = TimeSpan.FromHours(8);
                    options.SlidingExpiration = true;
                });

            // Admin şifrelerini güvenli şekilde hash'lemek/doğrulamak için kullanıyoruz.
            builder.Services.AddScoped<IPasswordHasher<Admin>, PasswordHasher<Admin>>();

            // Kendi yazdığımız dosya logger'ını da ekliyoruz.
            // Bu sayede loglar hem konsola (varsayılan) hem de Logs klasöründeki dosyaya yazılacak.
            builder.Logging.AddProvider(new DosyayaYazanLoggerProvider("Logs"));

            // Veritabanı bağlantı bilgisini appsettings.json'dan okuyoruz.
            // ÜRETİM (production) ortamında bu değer appsettings.json yerine ortam
            // değişkeni (ConnectionStrings__DefaultConnection) veya secret manager
            // üzerinden verilmelidir; hassas bilgi kaynak koda commit edilmemelidir.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' bulunamadı.");

            // EF Core DbContext'i DI container'a kaydediyoruz.
            // EnableRetryOnFailure: geçici (transient) SQL Server bağlantı hatalarında
            // işlemi otomatik olarak birkaç kez yeniden dener.
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure()));

            // Repository'leri DI container'a kaydediyoruz.
            // AddScoped: her HTTP isteğinde yeni bir örnek (instance) oluşturulur.
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IStockRepository, StockRepository>();

            var app = builder.Build();

            // Genel hata yönetimi middleware'ini boru hattının en başına ekliyoruz
            // ki kendisinden sonraki tüm middleware ve controller'lardaki
            // yakalanmayan hataları görebilsin. Merkezi loglama ve /Home/Error'a
            // yönlendirme tek noktadan bu middleware ile yapılır.
            app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

            // HTTP pipeline (istek/cevap boru hattı) yapılandırması.
            // Not: UseExceptionHandler bilinçli olarak eklenmedi. Eklenirse özel
            // GlobalExceptionHandlingMiddleware'den önce (iç katmanda) hatayı yakalar
            // ve production'da merkezi loglama devre dışı kalırdı.
            if (!app.Environment.IsDevelopment())
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Uygulama ilk defa çalıştığında bekleyen migration'ları uyguluyoruz
            // ve veritabanında hiç admin yoksa varsayılan bir admin kullanıcısı oluşturuyoruz.
            // Kullanıcı adı: admin, şifre: Admin123!
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.Migrate();

                var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<Admin>>();

                if (!dbContext.Admins.Any())
                {
                    var defaultAdmin = new Admin { Username = "admin" };
                    defaultAdmin.PasswordHash = passwordHasher.HashPassword(defaultAdmin, "Admin123!");
                    dbContext.Admins.Add(defaultAdmin);
                    dbContext.SaveChanges();
                }
            }

            app.Run();
        }
    }
}
