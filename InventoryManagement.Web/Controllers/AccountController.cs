using System.Security.Claims;
using InventoryManagement.Web.Data;
using InventoryManagement.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Web.Controllers
{
    // Admin giriş/çıkış işlemlerini yöneten controller.
    // [AllowAnonymous] sayesinde global authorize filtresine takılmadan
    // Login sayfasına giriş yapmamış kullanıcılar da erişebilir.
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IPasswordHasher<Admin> _passwordHasher;
        private readonly ILogger<AccountController> _logger;

        public AccountController(AppDbContext dbContext, IPasswordHasher<Admin> passwordHasher, ILogger<AccountController> logger)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var admin = _dbContext.Admins.FirstOrDefault(a => a.Username == model.Username);
            if (admin is null)
            {
                _logger.LogWarning("Giriş denemesi başarısız oldu, kullanıcı bulunamadı: {KullaniciAdi}", model.Username);
                ModelState.AddModelError(string.Empty, "Kullanıcı adı veya şifre hatalı.");
                return View(model);
            }

            var verifyResult = _passwordHasher.VerifyHashedPassword(admin, admin.PasswordHash, model.Password);
            if (verifyResult == PasswordVerificationResult.Failed)
            {
                _logger.LogWarning("Giriş denemesi başarısız oldu, şifre hatalı: {KullaniciAdi}", model.Username);
                ModelState.AddModelError(string.Empty, "Kullanıcı adı veya şifre hatalı.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, admin.Username),
                new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            _logger.LogInformation("Admin girişi yapıldı: {KullaniciAdi}", admin.Username);

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("Admin çıkışı yapıldı.");
            return RedirectToAction(nameof(Login));
        }
    }
}
