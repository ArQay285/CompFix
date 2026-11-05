using CompFix.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CompFix.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet("admin/login")]
        public async Task<IActionResult> AdminLogin(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            await Task.CompletedTask;
            return View();
        }

        // Обробка POST-запиту для входу адміністратор
        [HttpPost("admin/login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminLogin(AdminLoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid) {
                return View(model);
            }

            // Шукаємо користувача за логіном
            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user != null) {

                // Перевіряємо, чи є користувач адміністратором
                if (await _userManager.IsInRoleAsync(user, "Admin")) {

                    // Пробуємо виконати вхід
                    var result = await _signInManager.PasswordSignInAsync(user.UserName!, model.Password, model.RememberMe, false);

                    if (result.Succeeded) {
                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) {
                            return Redirect(returnUrl);
                        }
                        return RedirectToAction("Index", "Admin");
                    }

                }
                else {
                    ModelState.AddModelError(string.Empty, "Цей акаунт не має прав адміністратора");
                    return View(model);
                }
            }

            ModelState.AddModelError(string.Empty, "Невірний логін або пароль");
            return View(model);
        }

        // Вихід користувача з системи
        [HttpPost("admin/logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminLogout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
