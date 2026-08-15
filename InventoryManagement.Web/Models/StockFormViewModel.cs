using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManagement.Web.Models
{
    /// <summary>
    /// Bound model used by the Create/Edit stock forms.
    /// </summary>
    public class StockFormViewModel
    {
        [Display(Name = "Ürün")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir ürün seçilmelidir.")]
        public int ProductId { get; set; }

        [Display(Name = "Miktar")]
        [Range(0, int.MaxValue, ErrorMessage = "Stok miktarı negatif olamaz.")]
        public int Quantity { get; set; }

        /// <summary>
        /// Populated by the controller for rendering the product dropdown; not sent to the API.
        /// </summary>
        public List<SelectListItem> ProductOptions { get; set; } = new();

        /// <summary>
        /// True when editing an existing stock record (disables the product selector,
        /// since a product can only have one stock record).
        /// </summary>
        public bool IsEdit { get; set; }
    }
}
