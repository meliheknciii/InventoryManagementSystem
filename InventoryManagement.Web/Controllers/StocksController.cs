using InventoryManagement.Web.Models;
using InventoryManagement.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManagement.Web.Controllers
{
    public class StocksController : Controller
    {
        private readonly IStockApiClient _stockApiClient;
        private readonly IProductApiClient _productApiClient;
        private readonly ILogger<StocksController> _logger;

        public StocksController(
            IStockApiClient stockApiClient,
            IProductApiClient productApiClient,
            ILogger<StocksController> logger)
        {
            _stockApiClient = stockApiClient;
            _productApiClient = productApiClient;
            _logger = logger;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            try
            {
                var stocks = await _stockApiClient.GetAllAsync(cancellationToken);
                return View(stocks);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "Stoklar alınırken API'ye ulaşılamadı.");
                TempData["ErrorMessage"] = "Backend API'ye ulaşılamadı. Lütfen API'nin çalıştığından emin olun.";
                return View(new List<StockViewModel>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            var model = new StockFormViewModel
            {
                ProductOptions = await BuildProductOptionsAsync(cancellationToken)
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StockFormViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                model.ProductOptions = await BuildProductOptionsAsync(cancellationToken, model.ProductId);
                return View(model);
            }

            try
            {
                await _stockApiClient.CreateAsync(model, cancellationToken);
                TempData["SuccessMessage"] = "Stok kaydı oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }
            catch (ApiException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                model.ProductOptions = await BuildProductOptionsAsync(cancellationToken, model.ProductId);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var stock = await _stockApiClient.GetByIdAsync(id, cancellationToken);
            if (stock is null)
            {
                return NotFound();
            }

            var model = new StockFormViewModel
            {
                ProductId = stock.ProductId,
                Quantity = stock.Quantity,
                IsEdit = true,
                ProductOptions = await BuildProductOptionsAsync(cancellationToken, stock.ProductId)
            };

            ViewBag.StockId = id;
            ViewBag.ProductName = stock.ProductName;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StockFormViewModel model, CancellationToken cancellationToken)
        {
            model.IsEdit = true;

            if (!ModelState.IsValid)
            {
                model.ProductOptions = await BuildProductOptionsAsync(cancellationToken, model.ProductId);
                ViewBag.StockId = id;
                return View(model);
            }

            try
            {
                await _stockApiClient.UpdateAsync(id, model, cancellationToken);
                TempData["SuccessMessage"] = "Stok kaydı güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            catch (ApiException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                model.ProductOptions = await BuildProductOptionsAsync(cancellationToken, model.ProductId);
                ViewBag.StockId = id;
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            try
            {
                await _stockApiClient.DeleteAsync(id, cancellationToken);
                TempData["SuccessMessage"] = "Stok kaydı silindi.";
            }
            catch (ApiException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<List<SelectListItem>> BuildProductOptionsAsync(CancellationToken cancellationToken, int? selectedId = null)
        {
            var products = await _productApiClient.GetAllAsync(cancellationToken);
            return products
                .Select(p => new SelectListItem($"{p.Name} ({p.Sku})", p.Id.ToString(), p.Id == selectedId))
                .ToList();
        }
    }
}
