using Microsoft.Extensions.Logging;

namespace InventoryManagement.Web.Logging
{
    // Bu sınıf, gelen log mesajını ekrana değil, bir txt dosyasına yazar.
    // ILogger arayüzünü (interface) kendimiz uyguluyoruz (implement ediyoruz).
    public class DosyayaYazanLogger : ILogger
    {
        private readonly string _kategoriAdi; // Logun hangi sınıftan geldiğini tutar (örn: ProductsController)
        private readonly string _dosyaYolu;   // Log dosyasının tam yolu

        // Aynı anda birden fazla istek dosyaya yazmaya çalışabilir.
        // Bu yüzden basit bir kilit (lock) nesnesi kullanıyoruz ki satırlar birbirine karışmasın.
        private static readonly object _kilit = new object();

        public DosyayaYazanLogger(string kategoriAdi, string dosyaYolu)
        {
            _kategoriAdi = kategoriAdi;
            _dosyaYolu = dosyaYolu;
        }

        // Bu proje basit olduğu için scope (kapsam) özelliğini kullanmıyoruz.
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        // Hangi seviyedeki logların yazılacağını burada belirliyoruz.
        // Information ve üzerini (Warning, Error, Critical) dosyaya yazıyoruz.
        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= LogLevel.Information;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            // Log mesajını hazır formatter fonksiyonu ile metne çeviriyoruz.
            var mesaj = formatter(state, exception);

            var satir = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}] {_kategoriAdi} - {mesaj}";

            // Eğer bir hata (exception) varsa onu da alt satıra ekleyelim.
            if (exception is not null)
            {
                satir += Environment.NewLine + exception;
            }

            // Aynı anda tek bir işlemin dosyaya yazmasını sağlıyoruz.
            lock (_kilit)
            {
                File.AppendAllText(_dosyaYolu, satir + Environment.NewLine);
            }
        }
    }
}
