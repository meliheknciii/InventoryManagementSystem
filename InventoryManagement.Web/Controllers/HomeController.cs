using System.Diagnostics;
using InventoryManagement.Web.Models;
using InventoryManagement.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IStockRepository _stockRepository;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            ICategoryRepository categoryRepository,
            IProductRepository productRepository,
            IStockRepository stockRepository,
            ILogger<HomeController> logger)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _stockRepository = stockRepository;
            _logger = logger;
        }

        // GET: /  (Ana sayfa / Panel)
        public IActionResult Index()
        {
            var categories = _categoryRepository.GetAll();
            var products = _productRepository.GetAll();
            var stocks = _stockRepository.GetAll();

            var dashboard = new DashboardViewModel
            {
                CategoryCount = categories.Count,
                ProductCount = products.Count,
                LowStockCount = stocks.Count(s => s.Status == StockStatus.LowStock),
                OutOfStockCount = stocks.Count(s => s.Status == StockStatus.OutOfStock),
                RecentProducts = products
                    .OrderByDescending(p => p.Id)
                    .Take(5)
                    .ToList(),
                CriticalStocks = stocks
                    .Where(s => s.Status == StockStatus.LowStock || s.Status == StockStatus.OutOfStock)
                    .OrderBy(s => s.Quantity)
                    .Take(5)
                    .ToList()
            };

            _logger.LogInformation("Ana panel görüntülendi. Kategori: {KategoriSayisi}, Ürün: {UrunSayisi}, Az Stok: {AzStokSayisi}, Tükenen: {TukenenSayisi}",
                dashboard.CategoryCount, dashboard.ProductCount, dashboard.LowStockCount, dashboard.OutOfStockCount);

            return View(dashboard);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // Hata sayfası, oturumu olmayan kullanıcılara da gösterilebilmelidir.
        // Aksi halde global AuthorizeFilter yüzünden hata anında kullanıcı
        // login sayfasına yönlendirilir ve asıl hata gizlenir.
        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            _logger.LogError("Hata sayfası gösterildi. RequestId: {RequestId}", requestId);
            return View(new ErrorViewModel { RequestId = requestId });
        }
    }
}
