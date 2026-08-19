using InventoryManagement.Web.Models;
using InventoryManagement.Web.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Web.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ICategoryRepository categoryRepository, ILogger<CategoriesController> logger)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        // GET: /Categories
        public IActionResult Index()
        {
            var categories = _categoryRepository.GetAll();
            return View(categories);
        }

        // GET: /Categories/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CategoryFormViewModel());
        }

        // POST: /Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (_categoryRepository.NameExists(model.Name))
            {
                _logger.LogWarning("Kategori eklenemedi çünkü '{KategoriAdi}' ismi zaten kullanılıyor.", model.Name);
                ModelState.AddModelError(nameof(model.Name), "Bu isimde bir kategori zaten var.");
                return View(model);
            }

            var category = new Category
            {
                Name = model.Name,
                Description = model.Description
            };

            _categoryRepository.Add(category);

            _logger.LogInformation("Yeni kategori oluşturuldu. Id: {KategoriId}, Ad: {KategoriAdi}", category.Id, category.Name);

            TempData["SuccessMessage"] = $"'{category.Name}' kategorisi oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Categories/Edit/id
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _categoryRepository.GetById(id);
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

        // POST: /Categories/Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, CategoryFormViewModel model)
        {
            ViewBag.CategoryId = id;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var category = _categoryRepository.GetById(id);
            if (category is null)
            {
                _logger.LogWarning("Güncellenmek istenen kategori bulunamadı. Id: {KategoriId}", id);
                return NotFound();
            }

            if (_categoryRepository.NameExists(model.Name, id))
            {
                _logger.LogWarning("Kategori güncellenemedi çünkü '{KategoriAdi}' ismi zaten kullanılıyor.", model.Name);
                ModelState.AddModelError(nameof(model.Name), "Bu isimde bir kategori zaten var.");
                return View(model);
            }

            category.Name = model.Name;
            category.Description = model.Description;

            _categoryRepository.Update(category);

            _logger.LogInformation("Kategori güncellendi. Id: {KategoriId}, Ad: {KategoriAdi}", category.Id, category.Name);

            TempData["SuccessMessage"] = $"'{category.Name}' kategorisi güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Categories/Delete/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var category = _categoryRepository.GetById(id);
            if (category is null)
            {
                _logger.LogWarning("Silinmek istenen kategori bulunamadı. Id: {KategoriId}", id);
                return NotFound();
            }

            try
            {
                _categoryRepository.Delete(category);
                _logger.LogInformation("Kategori silindi. Id: {KategoriId}, Ad: {KategoriAdi}", category.Id, category.Name);
                TempData["SuccessMessage"] = "Kategori silindi.";
            }
            catch (Exception ex)
            {
                // Kategoriye bağlı ürün varsa veritabanı silmeye izin vermez (Restrict).
                _logger.LogError(ex, "Kategori silinirken hata oluştu. Id: {KategoriId}", id);
                TempData["ErrorMessage"] = "Bu kategoriye bağlı ürünler olduğu için silinemedi.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
