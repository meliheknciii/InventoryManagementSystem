using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Web.Logging
{
    // ASP.NET Core, loglama sistemine yeni bir "sağlayıcı" (provider) eklememizi
    // istediğimizde bu arayüzü (ILoggerProvider) uygulamamızı bekler.
    public class DosyayaYazanLoggerProvider : ILoggerProvider
    {
        private readonly string _dosyaYolu;

        // Her controller/sınıf için ayrı ayrı logger nesnesi oluşturmak yerine
        // aynı isimdekileri tekrar tekrar üretmemek adına burada saklıyoruz.
        private readonly ConcurrentDictionary<string, DosyayaYazanLogger> _loggerlar = new();

        public DosyayaYazanLoggerProvider(string klasorAdi = "Logs")
        {
            // Log dosyalarını proje içindeki "Logs" klasörüne, gün bazlı isimlendirerek yazıyoruz.
            var klasorYolu = Path.Combine(Directory.GetCurrentDirectory(), klasorAdi);

            // Klasör yoksa oluştur.
            if (!Directory.Exists(klasorYolu))
            {
                Directory.CreateDirectory(klasorYolu);
            }

            var dosyaAdi = $"log-{DateTime.Now:yyyy-MM-dd}.txt";
            _dosyaYolu = Path.Combine(klasorYolu, dosyaAdi);
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggerlar.GetOrAdd(categoryName, isim => new DosyayaYazanLogger(isim, _dosyaYolu));
        }

        public void Dispose()
        {
            _loggerlar.Clear();
        }
    }
}
