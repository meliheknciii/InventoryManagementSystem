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
    // StocksController için testler. Burada asıl önemli olan stok durumu
    // (OutOfStock/LowStock/InStock) hesaplama mantığı.
    public class StocksControllerTests
    {
        // Her testte sıfırdan controller kurmak için küçük bir yardımcı metot.
        private StocksController CreateController(out FakeStockRepository stockRepo, out FakeProductRepository productRepo)
        {
            stockRepo = new FakeStockRepository();
            productRepo = new FakeProductRepository();
            var controller = new StocksController(stockRepo, productRepo, NullLogger<StocksController>.Instance);

            // controller icinde TempData["SuccessMessage"] kullaniliyor, o yuzden
            // testte de TempData'yi elle hazirlamamiz gerekiyor.
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), new FakeTempDataProvider());

            return controller;
        }

        [Fact]
        public void TestStokEkleme()
        {
            var controller = CreateController(out var stockRepo, out var productRepo);
            productRepo.Add(new Product { Name = "Kalem", Sku = "K1", Price = 5 });

            var model = new StockFormViewModel { ProductId = 1, Quantity = 50 };
            var result = controller.Create(model);

            // basarili ekleme sonrasi Index sayfasina yonlendirme bekliyoruz
            Assert.IsType<RedirectToActionResult>(result);
            var eklenenStok = stockRepo.GetByProductId(1);
            Assert.NotNull(eklenenStok);
            Assert.Equal(StockStatus.InStock, eklenenStok!.Status);
        }

        [Fact]
        public void UrunBulunamadiHatasi()
        {
            var controller = CreateController(out var stockRepo, out var productRepo);

            // 99 numarali bir urun yok, bu yuzden hata beklenir
            var model = new StockFormViewModel { ProductId = 99, Quantity = 10 };
            var result = controller.Create(model);

            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public void AyniUruneIkinciStokEklenemez()
        {
            var controller = CreateController(out var stockRepo, out var productRepo);
            productRepo.Add(new Product { Name = "Defter", Sku = "D1", Price = 10 });
            stockRepo.Add(new Stock { ProductId = 1, Quantity = 20 });

            // ayni urun icin ikinci kez stok eklemeye calisiyoruz
            var model = new StockFormViewModel { ProductId = 1, Quantity = 30 };
            var result = controller.Create(model);

            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }

        [Theory]
        [InlineData(0, StockStatus.OutOfStock)]
        [InlineData(5, StockStatus.LowStock)]
        [InlineData(10, StockStatus.LowStock)]
        [InlineData(50, StockStatus.InStock)]
        public void NegatifVeDusukStokKontrolu(int miktar, StockStatus beklenenDurum)
        {
            var controller = CreateController(out var stockRepo, out var productRepo);
            productRepo.Add(new Product { Name = "Silgi", Sku = "S1", Price = 2 });
            stockRepo.Add(new Stock { ProductId = 1, Quantity = 100, Status = StockStatus.InStock });

            var model = new StockFormViewModel { ProductId = 1, Quantity = miktar };
            controller.Edit(1, model);

            var guncelStok = stockRepo.GetById(1);
            Assert.Equal(beklenenDurum, guncelStok!.Status);
        }

        [Fact]
        public void StokBulunamadiHatasi()
        {
            var controller = CreateController(out var stockRepo, out var productRepo);

            // 5 numarali stok kaydi yok
            var model = new StockFormViewModel { ProductId = 1, Quantity = 10 };
            var result = controller.Edit(5, model);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
