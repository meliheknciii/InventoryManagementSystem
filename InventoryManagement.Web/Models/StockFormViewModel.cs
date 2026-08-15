using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManagement.Web.Models
{
    // Stok Ekle / Düzenle formlarında kullanılan model.
    public class StockFormViewModel
    {
        [Display(Name = "Ürün")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir ürün seçilmelidir.")]
        public int ProductId { get; set; }

        [Display(Name = "Miktar")]
        [Range(0, int.MaxValue, ErrorMessage = "Stok miktarı negatif olamaz.")]
        public int Quantity { get; set; }

        // Formda ürün seçim kutusunu (dropdown) doldurmak için kullanılıyor.
        public List<SelectListItem> ProductOptions { get; set; } = new List<SelectListItem>();

        // Düzenleme ekranında true olur, bu durumda ürün seçimi kilitlenir
        // çünkü bir ürünün sadece bir stok kaydı olabilir.
        public bool IsEdit { get; set; }
    }
}
