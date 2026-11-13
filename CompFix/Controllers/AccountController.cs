using CompFix.Models;
using CompFix.Services;
using CompFix.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CompFix.Controllers
{
    public class AccountController : Controller
    {
        private readonly ControlBaseDate<Product> _productRepo;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ControlBaseDate<Product> product)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _productRepo = product;
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) {
                return NotFound("Такого айді немає");
            }
            var product = await _productRepo.GetByIdAsync(id.Value);
            if (product == null) {
                return NotFound("Товар не знайдено");
            }

            return View(product);
        }
        public async Task<IActionResult> UserInterface([FromServices] ControlBaseDate<Category> categories,
           [FromQuery] ProductFilterParams filter)
        {

            var allproduct = _productRepo.GetQueryable().Include(p => p.Category).Where(p => p.IsAvailable && p.Quantity > 0).AsQueryable();
            // Текстовий пошук за назвою товару
            var listproduct = await allproduct.ApplyProductFilter(filter).ToListAsync();
            var catageries = await categories.GetAllAsync();
            ViewBag.Categories = new SelectList(catageries, "id", "Name", filter.CategoryId);
            ViewBag.CurrentSearch = filter.Search;
            ViewBag.CurrentSort = filter.SortBy;
            ViewBag.CurrentCondition = filter.Condition;
            ViewBag.MinPrice = filter.MinPrice;
            ViewBag.MaxPrice = filter.MaxPrice;
            ViewBag.HasDiscount = filter.HasDiscount;
            ViewBag.IsAvailable = filter.IsAvailable;
            ViewBag.StockStatus = filter.StockStatus;
            return View(listproduct);
        }




        // Адміни
        [HttpGet("admin/login")]
        public async Task<IActionResult> AdminLogin(string? returnUrl = null)
        {
            // Якщо користувач вже авторизований, кидає на головну
            if (User.Identity != null && User.Identity.IsAuthenticated) {
                return RedirectToAction("UserInterface", "Account");
            }

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
                        return RedirectToAction("allproducts", "Admin");
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
            // Якщо користувач вже авторизований, кидає на головну
            if (User.Identity != null && User.Identity.IsAuthenticated) {
                return RedirectToAction("UserInterface", "Account");
            }

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
                ModelState.AddModelError("LastName", "Прізвище не може бути порожнім або складатися лише з пробілів.");
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
                return RedirectToAction("UserInterface", "Account");
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

            // Якщо користувач вже авторизований, кидає на головну
            if (User.Identity != null && User.Identity.IsAuthenticated) {
                return RedirectToAction("UserInterface", "Account");
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

            if (user == null || await _userManager.IsInRoleAsync(user, "admin")) {
                // Не дає увійти через звичайну форму адміну або неіснуючому користувачу
                ModelState.AddModelError(string.Empty, "Невірний логін або пароль.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName!, model.Password, model.RememberMe, false);

                if (result.Succeeded) {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("UserInterface", "Account");
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
            return RedirectToAction("UserInterface", "Account");
        }

        // GET: /settings - сторінка налаштувань
        [Authorize]
        [HttpGet("settings")]
        public async Task<IActionResult> Settings()
        {
            var user = await _userManager.GetUserAsync(User);
            // Якщо користувач не знайдений, перенаправляє на сторінку входу
            if (user == null) {
                return RedirectToAction("Login", "Account");
            }

            // Якщо адмін, кидає на головну
            if (await _userManager.IsInRoleAsync(user, "admin")) {
                return RedirectToAction("UserInterface", "Account");
            }

            // Підставляє поточне ім'я користувача у форму
            var model = new UserSettingsViewModel {
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty
            };

            return View(model);
        }

        // POST: /settings — збереження нового імені та/або пароля
        [Authorize]
        [HttpPost("settings")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Settings(UserSettingsViewModel model)
        {
            string cleanFirstName = model.FirstName?.Trim() ?? string.Empty;
            string cleanLastName = model.LastName?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(cleanFirstName)) {
                ModelState.AddModelError("FirstName", "Ім'я не може бути порожнім або складатися лише з пробілів.");
            }
            if (string.IsNullOrWhiteSpace(cleanLastName)) {
                ModelState.AddModelError("LastName", "Прізвище не може бути порожнім або складатися лише з пробілів.");
            }

            if (!ModelState.IsValid) {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                return RedirectToAction("Login", "Account");
            }

            bool isUpdated = false;
            bool isProfileInfoChanged = false;

            // Чи змінили ім'я
            if (user.FirstName != cleanFirstName) {
                user.FirstName = cleanFirstName;
                isProfileInfoChanged = true;
            }

            // Чи змінили прізвище
            if (user.LastName != cleanLastName) {
                user.LastName = cleanLastName;
                isProfileInfoChanged = true;
            }

            // Якщо зміни в профілі є, оновлює користувача
            if (isProfileInfoChanged) {
                var updateProfileResult = await _userManager.UpdateAsync(user);
                if (!updateProfileResult.Succeeded) {
                    foreach (var error in updateProfileResult.Errors) {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
                isUpdated = true;
            }

            // Зміна пароля (якщо користувач заповнив хоча б одне поле пароля)
            bool wantsToChangePassword = !string.IsNullOrEmpty(model.OldPassword) ||
                                         !string.IsNullOrEmpty(model.NewPassword) ||
                                         !string.IsNullOrEmpty(model.ConfirmNewPassword);

            if (wantsToChangePassword) {
                // Перевіряє, чи ввів користувач старий пароль
                if (string.IsNullOrEmpty(model.OldPassword)) {
                    ModelState.AddModelError(nameof(model.OldPassword), "Введіть поточний пароль для його зміни!");
                    return View(model);
                }

                // Перевіряє, чи ввів користувач новий пароль
                if (string.IsNullOrEmpty(model.NewPassword)) {
                    ModelState.AddModelError(nameof(model.NewPassword), "Введіть новий пароль!");
                    return View(model);
                }

                // Перевіряє, чи правильний старий та новий пароль
                var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                if (!changePasswordResult.Succeeded) {
                    foreach (var error in changePasswordResult.Errors) {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }

                isUpdated = true;
            }

            // Оновлення сесії (кукі) та виведення повідомлення
            if (isUpdated) {
                // Оновлення кукі, щоб користувача не розлогінило після зміни пароля!
                await _signInManager.RefreshSignInAsync(user);
                TempData["StatusMessage"] = "Налаштування профілю успішно збережено!";
            }

            return RedirectToAction("Settings");
        }
    }
}
