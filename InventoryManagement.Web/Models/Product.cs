using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagement.Web.Models
{
    // Veritabanındaki "Products" tablosunu temsil eden entity sınıfı.
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ürün adı zorunludur.")]
        [StringLength(200, ErrorMessage = "Ürün adı en fazla 200 karakter olabilir.")]
        [Display(Name = "Ürün Adı")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "SKU zorunludur.")]
        [StringLength(50, ErrorMessage = "SKU en fazla 50 karakter olabilir.")]
        [Display(Name = "SKU")]
        public string Sku { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat sıfırdan büyük olmalıdır.")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Fiyat")]
        public decimal Price { get; set; }

        // Foreign key: bu ürünün hangi kategoriye ait olduğunu tutar.
        [Display(Name = "Kategori")]
        public int CategoryId { get; set; }

        // Navigation property: ürünün ait olduğu kategori nesnesi.
        public Category? Category { get; set; }

        // Bir ürünün bir tane stok kaydı olabilir (1-1 ilişki).
        public Stock? Stock { get; set; }
    }
}
