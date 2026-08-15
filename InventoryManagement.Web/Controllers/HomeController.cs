using System.Diagnostics;
using InventoryManagement.Web.Models;
using InventoryManagement.Web.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IStockRepository _stockRepository;

        public HomeController(
            ICategoryRepository categoryRepository,
            IProductRepository productRepository,
            IStockRepository stockRepository)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _stockRepository = stockRepository;
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
