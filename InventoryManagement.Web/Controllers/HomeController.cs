using InventoryManagement.Web.Models;
using InventoryManagement.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace InventoryManagement.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICategoryApiClient _categoryApiClient;
        private readonly IProductApiClient _productApiClient;
        private readonly IStockApiClient _stockApiClient;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            ICategoryApiClient categoryApiClient,
            IProductApiClient productApiClient,
            IStockApiClient stockApiClient,
            ILogger<HomeController> logger)
        {
            _categoryApiClient = categoryApiClient;
            _productApiClient = productApiClient;
            _stockApiClient = stockApiClient;
            _logger = logger;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var dashboard = new DashboardViewModel();

            try
            {
                var categories = await _categoryApiClient.GetAllAsync(cancellationToken);
                var products = await _productApiClient.GetAllAsync(cancellationToken);
                var stocks = await _stockApiClient.GetAllAsync(cancellationToken);

                dashboard.CategoryCount = categories.Count;
                dashboard.ProductCount = products.Count;
                dashboard.LowStockCount = stocks.Count(s => s.Status == StockStatus.LowStock);
                dashboard.OutOfStockCount = stocks.Count(s => s.Status == StockStatus.OutOfStock);
                dashboard.RecentProducts = products
                    .OrderByDescending(p => p.Id)
                    .Take(5)
                    .ToList();
                dashboard.CriticalStocks = stocks
                    .Where(s => s.Status is StockStatus.LowStock or StockStatus.OutOfStock)
                    .OrderBy(s => s.Quantity)
                    .Take(5)
                    .ToList();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "Backend API'ye ulaşılamadı.");
                dashboard.ApiUnavailable = true;
            }

            return View(dashboard);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
