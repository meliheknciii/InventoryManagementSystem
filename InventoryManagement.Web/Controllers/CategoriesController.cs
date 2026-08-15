using InventoryManagement.Web.Models;
using InventoryManagement.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Web.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoryApiClient _categoryApiClient;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ICategoryApiClient categoryApiClient, ILogger<CategoriesController> logger)
        {
            _categoryApiClient = categoryApiClient;
            _logger = logger;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            try
            {
                var categories = await _categoryApiClient.GetAllAsync(cancellationToken);
                return View(categories);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "Kategoriler alınırken API'ye ulaşılamadı.");
                TempData["ErrorMessage"] = "Backend API'ye ulaşılamadı. Lütfen API'nin çalıştığından emin olun.";
                return View(new List<CategoryViewModel>());
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CategoryFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryFormViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _categoryApiClient.CreateAsync(model, cancellationToken);
                TempData["SuccessMessage"] = $"'{model.Name}' kategorisi oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }
            catch (ApiException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var category = await _categoryApiClient.GetByIdAsync(id, cancellationToken);
            if (category is null)
            {
                return NotFound();
            }

            var model = new CategoryFormViewModel
            {
                Name = category.Name,
                Description = category.Description
            };

            ViewBag.CategoryId = id;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryFormViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CategoryId = id;
                return View(model);
            }

            try
            {
                await _categoryApiClient.UpdateAsync(id, model, cancellationToken);
                TempData["SuccessMessage"] = $"'{model.Name}' kategorisi güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            catch (ApiException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.CategoryId = id;
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            try
            {
                await _categoryApiClient.DeleteAsync(id, cancellationToken);
                TempData["SuccessMessage"] = "Kategori silindi.";
            }
            catch (ApiException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
