namespace InventoryManagement.Web.Models
{
    // Bir ürünün stok durumunu ifade eden enum.
    // Miktara göre otomatik hesaplanır, kullanıcı elle seçmez.
    public enum StockStatus
    {
        OutOfStock = 0, // Stok tükendi
        LowStock = 1,   // Az stok kaldı
        InStock = 2     // Stokta var
    }
}
