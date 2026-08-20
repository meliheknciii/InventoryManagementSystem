using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Web.Models
{
    // Login formu için kullanılan view model.
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        [Display(Name = "Kullanıcı Adı")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; } = string.Empty;

        // Login sonrası hangi sayfaya geri dönüleceğini tutar (örn. yetkisiz erişim sonrası).
        public string? ReturnUrl { get; set; }
    }
}
