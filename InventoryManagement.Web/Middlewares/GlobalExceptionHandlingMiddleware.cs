using System.Diagnostics;

namespace InventoryManagement.Web.Middlewares
{
    // Uygulama genelinde, controller'larda yakalanmayan (unhandled) tüm hataları
    // burada tek bir yerden yakalayıp logluyoruz. Böylece her controller'a ayrı
    // ayrı try-catch yazmaya gerek kalmıyor.
    
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionHandlingMiddleware> logger,
            IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var requestId = Activity.Current?.Id ?? context.TraceIdentifier;

                _logger.LogError(ex,
                    "Beklenmeyen bir hata yakalandı. RequestId: {RequestId}, Yol: {Yol}",
                    requestId, context.Request.Path);

                
                if (_env.IsDevelopment())
                {
                    throw;
                }

                
                if (context.Response.HasStarted)
                {
                    throw;
                }

                // Yanıtı temizleyip hata sayfasını AYNI istek içinde yeniden çalıştırıyoruz
                // (re-execute). Böylece istemciye 302 yönlendirme yerine gerçek 500 durum
                // kodu ile hata sayfası döner. Bu, kaldırılan UseExceptionHandler'ın
                // yaptığı işin sadeleştirilmiş halidir.
                context.Response.Clear();
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                // Önceki istekten kalan routing/endpoint bilgilerini temizliyoruz ki
                // istek yeni yola (/Home/Error) göre baştan yönlendirilebilsin.
                context.SetEndpoint(null);
                context.Request.RouteValues.Clear();
                context.Request.Path = "/Home/Error";
                context.Request.Method = HttpMethods.Get;

                await _next(context);
            }
        }
    }
}
