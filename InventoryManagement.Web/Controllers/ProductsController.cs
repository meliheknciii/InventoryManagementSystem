using InventoryManagement.Web.Models;
using InventoryManagement.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManagement.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductApiClient _productApiClient;
        private readonly ICategoryApiClient _categoryApiClient;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(
            IProductApiClient productApiClient,
            ICategoryApiClient categoryApiClient,
            ILogger<ProductsController> logger)
        {
            _productApiClient = productApiClient;
            _categoryApiClient = categoryApiClient;
            _logger = logger;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            try
            {
                var products = await _productApiClient.GetAllAsync(cancellationToken);
                return View(products);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "Ürünler alınırken API'ye ulaşılamadı.");
                TempData["ErrorMessage"] = "Backend API'ye ulaşılamadı. Lütfen API'nin çalıştığından emin olun.";
                return View(new List<ProductViewModel>());
            }
        }

        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
        {
            var product = await _productApiClient.GetByIdAsync(id, cancellationToken);
            return product is null ? NotFound() : View(product);
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            var model = new ProductFormViewModel
            {
                CategoryOptions = await BuildCategoryOptionsAsync(cancellationToken)
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductFormViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                model.CategoryOptions = await BuildCategoryOptionsAsync(cancellationToken, model.CategoryId);
                return View(model);
            }

            try
            {
                await _productApiClient.CreateAsync(model, cancellationToken);
                TempData["SuccessMessage"] = $"'{model.Name}' ürünü oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }
            catch (ApiException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                model.CategoryOptions = await BuildCategoryOptionsAsync(cancellationToken, model.CategoryId);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var product = await _productApiClient.GetByIdAsync(id, cancellationToken);
            if (product is null)
            {
                return NotFound();
            }

            var model = new ProductFormViewModel
            {
                Name = product.Name,
                Description = product.Description,
                Sku = product.Sku,
                Price = product.Price,
                CategoryId = product.CategoryId,
                CategoryOptions = await BuildCategoryOptionsAsync(cancellationToken, product.CategoryId)
            };

            ViewBag.ProductId = id;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductFormViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                model.CategoryOptions = await BuildCategoryOptionsAsync(cancellationToken, model.CategoryId);
                ViewBag.ProductId = id;
                return View(model);
            }

            try
            {
                await _productApiClient.UpdateAsync(id, model, cancellationToken);
                TempData["SuccessMessage"] = $"'{model.Name}' ürünü güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            catch (ApiException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                model.CategoryOptions = await BuildCategoryOptionsAsync(cancellationToken, model.CategoryId);
                ViewBag.ProductId = id;
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            try
            {
                await _productApiClient.DeleteAsync(id, cancellationToken);
                TempData["SuccessMessage"] = "Ürün silindi.";
            }
            catch (ApiException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<List<SelectListItem>> BuildCategoryOptionsAsync(CancellationToken cancellationToken, int? selectedId = null)
        {
            var categories = await _categoryApiClient.GetAllAsync(cancellationToken);
            return categories
                .Select(c => new SelectListItem(c.Name, c.Id.ToString(), c.Id == selectedId))
                .ToList();
        }
    }
}
