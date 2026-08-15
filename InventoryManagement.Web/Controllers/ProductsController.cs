using InventoryManagement.Web.Models;
using InventoryManagement.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManagement.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductsController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        // GET: /Products
        public IActionResult Index()
        {
            var products = _productRepository.GetAll();
            return View(products);
        }

        // GET: /Products/Details/5
        public IActionResult Details(int id)
        {
            var product = _productRepository.GetById(id);
            if (product is null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: /Products/Create
        [HttpGet]
        public IActionResult Create()
        {
            var model = new ProductFormViewModel
            {
                CategoryOptions = BuildCategoryOptions()
            };
            return View(model);
        }

        // POST: /Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.CategoryOptions = BuildCategoryOptions(model.CategoryId);
                return View(model);
            }

            if (_productRepository.SkuExists(model.Sku))
            {
                ModelState.AddModelError(nameof(model.Sku), "Bu SKU değeri zaten kullanılıyor.");
                model.CategoryOptions = BuildCategoryOptions(model.CategoryId);
                return View(model);
            }

            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Sku = model.Sku,
                Price = model.Price,
                CategoryId = model.CategoryId
            };

            _productRepository.Add(product);

            TempData["SuccessMessage"] = $"'{product.Name}' ürünü oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Products/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _productRepository.GetById(id);
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
                CategoryOptions = BuildCategoryOptions(product.CategoryId)
            };

            ViewBag.ProductId = id;
            return View(model);
        }

        // POST: /Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ProductFormViewModel model)
        {
            ViewBag.ProductId = id;

            if (!ModelState.IsValid)
            {
                model.CategoryOptions = BuildCategoryOptions(model.CategoryId);
                return View(model);
            }

            var product = _productRepository.GetById(id);
            if (product is null)
            {
                return NotFound();
            }

            if (_productRepository.SkuExists(model.Sku, id))
            {
                ModelState.AddModelError(nameof(model.Sku), "Bu SKU değeri zaten kullanılıyor.");
                model.CategoryOptions = BuildCategoryOptions(model.CategoryId);
                return View(model);
            }

            product.Name = model.Name;
            product.Description = model.Description;
            product.Sku = model.Sku;
            product.Price = model.Price;
            product.CategoryId = model.CategoryId;

            _productRepository.Update(product);

            TempData["SuccessMessage"] = $"'{product.Name}' ürünü güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Products/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var product = _productRepository.GetById(id);
            if (product is null)
            {
                return NotFound();
            }

            _productRepository.Delete(product);
            TempData["SuccessMessage"] = "Ürün silindi.";

            return RedirectToAction(nameof(Index));
        }

        // Kategori dropdown listesini hazırlayan yardımcı (helper) metot.
        private List<SelectListItem> BuildCategoryOptions(int? selectedId = null)
        {
            var categories = _categoryRepository.GetAll();
            return categories
                .Select(c => new SelectListItem(c.Name, c.Id.ToString(), c.Id == selectedId))
                .ToList();
        }
    }
}
