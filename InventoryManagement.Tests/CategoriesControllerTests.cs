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
    public class CategoriesControllerTests
    {
        private CategoriesController CreateController(out FakeCategoryRepository categoryRepo)
        {
            categoryRepo = new FakeCategoryRepository();
            var controller = new CategoriesController(categoryRepo, NullLogger<CategoriesController>.Instance);
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), new FakeTempDataProvider());
            return controller;
        }

        [Fact]
        public void TestKategoriEkleme()
        {
            var controller = CreateController(out var categoryRepo);

            var model = new CategoryFormViewModel { Name = "Elektronik", Description = "Elektronik urunler" };
            var result = controller.Create(model);

            Assert.IsType<RedirectToActionResult>(result);
            var eklenenKategori = categoryRepo.GetById(1);
            Assert.NotNull(eklenenKategori);
            Assert.Equal("Elektronik", eklenenKategori!.Name);
        }

        [Fact]
        public void KategoriIsimTekrarKontrolu()
        {
            var controller = CreateController(out var categoryRepo);
            categoryRepo.Add(new Category { Name = "Elektronik" });

            // ayni isimde ikinci kategori eklenmeye calisiliyor
            var model = new CategoryFormViewModel { Name = "Elektronik" };
            var result = controller.Create(model);

            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public void KategoriBulunamadiHatasi()
        {
            var controller = CreateController(out var categoryRepo);

            // 7 numarali kategori yok
            var model = new CategoryFormViewModel { Name = "Yeni Kategori" };
            var result = controller.Edit(7, model);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
