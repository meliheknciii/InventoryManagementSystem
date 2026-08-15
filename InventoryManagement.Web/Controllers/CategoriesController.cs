using InventoryManagement.Web.Models;
using InventoryManagement.Web.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Web.Controllers
{
    public class CategoriesController : Controller
    {
        // Constructor Injection: ihtiyacımız olan repository'yi
        // Program.cs içinde DI container'a kaydettik, ASP.NET Core bize burada otomatik veriyor.
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
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
                ModelState.AddModelError(nameof(model.Name), "Bu isimde bir kategori zaten var.");
                return View(model);
            }

            var category = new Category
            {
                Name = model.Name,
                Description = model.Description
            };

            _categoryRepository.Add(category);

            TempData["SuccessMessage"] = $"'{category.Name}' kategorisi oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Categories/Edit/5
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

        // POST: /Categories/Edit/5
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
                return NotFound();
            }

            if (_categoryRepository.NameExists(model.Name, id))
            {
                ModelState.AddModelError(nameof(model.Name), "Bu isimde bir kategori zaten var.");
                return View(model);
            }

            category.Name = model.Name;
            category.Description = model.Description;

            _categoryRepository.Update(category);

            TempData["SuccessMessage"] = $"'{category.Name}' kategorisi güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Categories/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var category = _categoryRepository.GetById(id);
            if (category is null)
            {
                return NotFound();
            }

            try
            {
                _categoryRepository.Delete(category);
                TempData["SuccessMessage"] = "Kategori silindi.";
            }
            catch (Exception)
            {
                // Kategoriye bağlı ürün varsa veritabanı silmeye izin vermez (Restrict).
                TempData["ErrorMessage"] = "Bu kategoriye bağlı ürünler olduğu için silinemedi.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
