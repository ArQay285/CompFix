using CompFix.ViewModels;
using CompFix.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CompFix.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }


        // Адміни
        [HttpGet("admin/login")]
        public async Task<IActionResult> AdminLogin(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            await Task.CompletedTask;
            return View();
        }

        // Обробка POST-запиту для входу адміністратора
        [HttpPost("admin/login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminLogin(AdminLoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid) {
                return View(model);
            }

            // Шукаємо адміна за логіном
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

        // Прості користувачі
        // GET: /register — сторінка реєстрації
        [HttpGet("register")]
        public async Task<IActionResult> Register()
        {
            await Task.CompletedTask;
            return View();
        }

        // POST: /register — обробка реєстрації
        [HttpPost("register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // Видаляє всі випадкові пробіли на початку та в кінці рядка
            string cleanFirstName = model.FirstName.Trim();
            string cleanLastName = model.LastName.Trim();

            if (string.IsNullOrWhiteSpace(cleanFirstName)) {
                ModelState.AddModelError("FirstName", "Ім'я не може бути порожнім або складатися лише з пробілів.");
            }
            if (string.IsNullOrWhiteSpace(cleanLastName)) {
                ModelState.AddModelError("LastName", "Призвище не може бути порожнім або складатися лише з пробілів.");
            }

            var user = new ApplicationUser {
                UserName = model.Email,
                Email = model.Email,
                FirstName = cleanFirstName,
                LastName = cleanLastName,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded) {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors) {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        // GET: /login — сторінка входу
        [HttpGet("login")]
        public async Task<IActionResult> Login(string? returnUrl = null)
        {
            // Якщо в ReturnUrl вказаний шлях до адмінки (починається з /admin)
            if (!string.IsNullOrEmpty(returnUrl) && returnUrl.StartsWith("/admin", StringComparison.OrdinalIgnoreCase)) {
                // Перенаправляє на адмінську сторінку входу /admin/login з збереженням ReturnUrl
                return RedirectToAction("AdminLogin", "Account", new { returnUrl = returnUrl });
            }

            ViewData["ReturnUrl"] = returnUrl;
            await Task.CompletedTask;
            return View();
        }

        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if(user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user.UserName!, model.Password, model.RememberMe, false);

                // Якщо це admin, розлогінює і показує помилку
                if (await _userManager.IsInRoleAsync(user, "admin")) {
                    await _signInManager.SignOutAsync();
                    ModelState.AddModelError(string.Empty, "Невірний логін або пароль.");
                    return View(model);
                }

                if (result.Succeeded) {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError(string.Empty, "Невірний логін або пароль.");
            return View(model);
        }

        // POST: /logout — універсальний вихід для всіх користувачів
        [HttpPost("logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
