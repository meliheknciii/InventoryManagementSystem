using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManagement.Web.Models
{
    /// <summary>
    /// Bound model used by the Create/Edit product forms.
    /// </summary>
    public class ProductFormViewModel
    {
        [Display(Name = "Ürün Adı")]
        [Required(ErrorMessage = "Ürün adı zorunludur.")]
        [StringLength(200, ErrorMessage = "Ürün adı en fazla 200 karakter olabilir.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Açıklama")]
        [StringLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir.")]
        public string? Description { get; set; }

        [Display(Name = "SKU")]
        [Required(ErrorMessage = "SKU zorunludur.")]
        [StringLength(50, ErrorMessage = "SKU en fazla 50 karakter olabilir.")]
        public string Sku { get; set; } = string.Empty;

        [Display(Name = "Fiyat")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat sıfırdan büyük olmalıdır.")]
        public decimal Price { get; set; }

        [Display(Name = "Kategori")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir kategori seçilmelidir.")]
        public int CategoryId { get; set; }

        /// <summary>
        /// Populated by the controller for rendering the category dropdown; not sent to the API.
        /// </summary>
        public List<SelectListItem> CategoryOptions { get; set; } = new();
    }
}
