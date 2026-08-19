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
    }
}
