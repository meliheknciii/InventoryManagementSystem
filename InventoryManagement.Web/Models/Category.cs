using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Web.Models
{
    // Bu sınıf veritabanındaki "Categories" tablosunu temsil eder.
    // Entity Framework Core bu sınıfa bakarak tabloyu otomatik oluşturur.
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori adı zorunludur.")]
        [StringLength(100, ErrorMessage = "Kategori adı en fazla 100 karakter olabilir.")]
        [Display(Name = "Kategori Adı")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        // Bir kategoriye ait ürünlerin listesi. Navigation property olarak adlandırılır.
        public List<Product> Products { get; set; } = new List<Product>();
    }
}
