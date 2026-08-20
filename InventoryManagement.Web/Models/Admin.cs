using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Web.Models
{
    // Bu sınıf veritabanındaki "Admins" tablosunu temsil eder.
    // Sisteme giriş yapabilecek yönetici kullanıcılarını tutar.
    public class Admin
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        // Şifre asla düz metin olarak saklanmaz; PasswordHasher ile hash'lenmiş hali tutulur.
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
