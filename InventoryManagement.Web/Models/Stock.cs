using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Web.Models
{
    // Veritabanındaki "Stocks" tablosunu temsil eden entity sınıfı.
    // Her ürünün en fazla bir stok kaydı olur.
    public class Stock
    {
        public int Id { get; set; }

        [Display(Name = "Ürün")]
        public int ProductId { get; set; }

        public Product? Product { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stok miktarı negatif olamaz.")]
        [Display(Name = "Miktar")]
        public int Quantity { get; set; }

        // Durum, controller tarafında miktara bakılarak otomatik hesaplanır.
        [Display(Name = "Durum")]
        public StockStatus Status { get; set; } = StockStatus.OutOfStock;

        [Display(Name = "Son Güncelleme")]
        public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;
    }
}
