using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManagement.Web.Models
{
    // Ürünler listeleme sayfasında arama ve filtreleme durumunu taşıyan model.
    public class ProductListViewModel
    {
        // Filtrelenmiş ürün listesi
        public List<Product> Products { get; set; } = new List<Product>();

        // Arama kutusuna girilen metin (Ürün adı veya SKU'da aranır)
        [Display(Name = "Ara")]
        public string? SearchTerm { get; set; }

        // Seçili kategori filtresi (null ise tüm kategoriler)
        [Display(Name = "Kategori")]
        public int? CategoryId { get; set; }

        // Kategori filtre dropdown'ını doldurmak için kullanılır.
        public List<SelectListItem> CategoryOptions { get; set; } = new List<SelectListItem>();

        // --- Sayfalama (Pagination) için eklenen alanlar ---

        // Şu an hangi sayfadayız (1'den başlar)
        public int PageNumber { get; set; } = 1;

        // Bir sayfada kaç ürün gösterileceği
        public int PageSize { get; set; } = 5;

        // Filtrelere uyan toplam ürün sayısı
        public int TotalCount { get; set; }

        // Toplam kaç sayfa olduğunu buradan hesaplıyoruz.
        // Örnek: 12 ürün, sayfa boyutu 5 ise -> 12 / 5 = 2.4 -> yukarı yuvarlayınca 3 sayfa eder.
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}
