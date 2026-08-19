using InventoryManagement.Web.Models;
using InventoryManagement.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManagement.Web.Controllers
{
    public class StocksController : Controller
    {
        // Stok miktarı bu değerin altına düşünce "Az Stok" olarak işaretlenecek.
        private const int LowStockThreshold = 10;

        private readonly IStockRepository _stockRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<StocksController> _logger;

        public StocksController(IStockRepository stockRepository, IProductRepository productRepository, ILogger<StocksController> logger)
        {
            _stockRepository = stockRepository;
            _productRepository = productRepository;
            _logger = logger;
        }

        // GET: /Stocks
        public IActionResult Index()
        {
            var stocks = _stockRepository.GetAll();
            return View(stocks);
        }

        // GET: /Stocks/Create
        [HttpGet]
        public IActionResult Create()
        {
            var model = new StockFormViewModel
            {
                ProductOptions = BuildProductOptions()
            };
            return View(model);
        }

        // POST: /Stocks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(StockFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ProductOptions = BuildProductOptions(model.ProductId);
                return View(model);
            }

            var product = _productRepository.GetById(model.ProductId);
            if (product is null)
            {
                _logger.LogWarning("Stok eklenemedi çünkü seçilen ürün bulunamadı. ÜrünId: {UrunId}", model.ProductId);
                ModelState.AddModelError(nameof(model.ProductId), "Seçilen ürün bulunamadı.");
                model.ProductOptions = BuildProductOptions(model.ProductId);
                return View(model);
            }

            var existingStock = _stockRepository.GetByProductId(model.ProductId);
            if (existingStock is not null)
            {
                _logger.LogWarning("Stok eklenemedi çünkü ÜrünId: {UrunId} için zaten stok kaydı var.", model.ProductId);
                ModelState.AddModelError(nameof(model.ProductId), "Bu ürünün zaten bir stok kaydı var.");
                model.ProductOptions = BuildProductOptions(model.ProductId);
                return View(model);
            }

            var stock = new Stock
            {
                ProductId = model.ProductId,
                Quantity = model.Quantity,
                Status = CalculateStatus(model.Quantity),
                LastUpdatedUtc = DateTime.UtcNow
            };

            _stockRepository.Add(stock);

            _logger.LogInformation("Yeni stok kaydı oluşturuldu. StokId: {StokId}, ÜrünId: {UrunId}, Miktar: {Miktar}", stock.Id, stock.ProductId, stock.Quantity);

            TempData["SuccessMessage"] = "Stok kaydı oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Stocks/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var stock = _stockRepository.GetById(id);
            if (stock is null)
            {
                _logger.LogWarning("Düzenlenmek istenen stok kaydı bulunamadı. StokId: {StokId}", id);
                return NotFound();
            }

            var model = new StockFormViewModel
            {
                ProductId = stock.ProductId,
                Quantity = stock.Quantity,
                IsEdit = true,
                ProductOptions = BuildProductOptions(stock.ProductId)
            };

            ViewBag.StockId = id;
            ViewBag.ProductName = stock.Product?.Name;
            return View(model);
        }

        // POST: /Stocks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, StockFormViewModel model)
        {
            model.IsEdit = true;
            ViewBag.StockId = id;

            if (!ModelState.IsValid)
            {
                model.ProductOptions = BuildProductOptions(model.ProductId);
                return View(model);
            }

            var stock = _stockRepository.GetById(id);
            if (stock is null)
            {
                _logger.LogWarning("Güncellenmek istenen stok kaydı bulunamadı. StokId: {StokId}", id);
                return NotFound();
            }

            // Ürün bilgisi değiştirilmiyor, sadece miktar güncelleniyor.
            stock.Quantity = model.Quantity;
            stock.Status = CalculateStatus(model.Quantity);
            stock.LastUpdatedUtc = DateTime.UtcNow;

            _stockRepository.Update(stock);

            _logger.LogInformation("Stok kaydı güncellendi. StokId: {StokId}, Yeni Miktar: {Miktar}", stock.Id, stock.Quantity);

            // Stok azaldıysa veya tükendiyse bunu ayrıca uyarı olarak loglayalım.
            if (stock.Status == StockStatus.LowStock)
            {
                _logger.LogWarning("Stok azaldı! StokId: {StokId}, Miktar: {Miktar}", stock.Id, stock.Quantity);
            }
            else if (stock.Status == StockStatus.OutOfStock)
            {
                _logger.LogWarning("Stok tükendi! StokId: {StokId}", stock.Id);
            }

            TempData["SuccessMessage"] = "Stok kaydı güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Stocks/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var stock = _stockRepository.GetById(id);
            if (stock is null)
            {
                _logger.LogWarning("Silinmek istenen stok kaydı bulunamadı. StokId: {StokId}", id);
                return NotFound();
            }

            _stockRepository.Delete(stock);
            _logger.LogInformation("Stok kaydı silindi. StokId: {StokId}", id);
            TempData["SuccessMessage"] = "Stok kaydı silindi.";

            return RedirectToAction(nameof(Index));
        }

        // Miktara göre stok durumunu hesaplayan yardımcı metot.
        private static StockStatus CalculateStatus(int quantity)
        {
            if (quantity <= 0)
            {
                return StockStatus.OutOfStock;
            }

            if (quantity <= LowStockThreshold)
            {
                return StockStatus.LowStock;
            }

            return StockStatus.InStock;
        }

        // Ürün dropdown listesini hazırlayan yardımcı metot.
        private List<SelectListItem> BuildProductOptions(int? selectedId = null)
        {
            var products = _productRepository.GetAll();
            return products
                .Select(p => new SelectListItem($"{p.Name} ({p.Sku})", p.Id.ToString(), p.Id == selectedId))
                .ToList();
        }
    }
}
