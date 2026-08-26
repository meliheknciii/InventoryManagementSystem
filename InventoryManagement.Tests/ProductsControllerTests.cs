using InventoryManagement.Tests.Fakes;
using InventoryManagement.Web.Controllers;
using InventoryManagement.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace InventoryManagement.Tests
{
    public class ProductsControllerTests
    {
        private ProductsController CreateController(out FakeProductRepository productRepo, out FakeCategoryRepository categoryRepo)
        {
            productRepo = new FakeProductRepository();
            categoryRepo = new FakeCategoryRepository();
            var controller = new ProductsController(productRepo, categoryRepo, NullLogger<ProductsController>.Instance);
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), new FakeTempDataProvider());
            return controller;
        }

        [Fact]
        public void TestUrunEkleme()
        {
            var controller = CreateController(out var productRepo, out var categoryRepo);
            categoryRepo.Add(new Category { Name = "Kirtasiye" });

            var model = new ProductFormViewModel
            {
                Name = "Tukenmez Kalem",
                Sku = "TK-100",
                Price = 15,
                CategoryId = 1
            };

            var result = controller.Create(model);

            Assert.IsType<RedirectToActionResult>(result);
            var eklenenUrun = productRepo.GetById(1);
            Assert.NotNull(eklenenUrun);
            Assert.Equal("Tukenmez Kalem", eklenenUrun!.Name);
        }

        [Fact]
        public void SkuTekrarKontrolu()
        {
            var controller = CreateController(out var productRepo, out var categoryRepo);
            categoryRepo.Add(new Category { Name = "Kirtasiye" });
            productRepo.Add(new Product { Name = "Silgi", Sku = "SKU-1", Price = 3, CategoryId = 1 });

            // ayni SKU ile ikinci bir urun eklemeye calisiyoruz, hata beklenir
            var model = new ProductFormViewModel { Name = "Silgi 2", Sku = "SKU-1", Price = 4, CategoryId = 1 };
            var result = controller.Create(model);

            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public void UrunBulunamadiDetay()
        {
            var controller = CreateController(out var productRepo, out var categoryRepo);

            // 42 numarali urun yok
            var result = controller.Details(42);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
