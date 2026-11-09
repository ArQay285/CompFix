using CompFix.Models;
using CompFix.Services;
using CompFix.ViewModels;
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

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager,ControlBaseDate<Product> product)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _productRepo = product;
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound("Такого айді нету");
            }
            var product = await _productRepo.GetByIdAsync(id.Value);
            if(product == null)
            {
                return NotFound("Товар не знайдено");
            }

            return View(product);
        }
        public async Task<IActionResult> UserInterface([FromServices] ControlBaseDate<Category> categories,
            string? search,
            int? categoryId,
            int? condition,
            bool? IsAvaivle,
            bool? HasDiscount,
            string? stockStatus,
            decimal? MinPrice,
            decimal? MaxPrice,
            string? SortBy){
            
                var allproduct = _productRepo.GetQueryable().Include(p => p.Category).Where(p => p.IsAvailable && p.Quantity > 0).AsQueryable();
                // 2. Текстовий пошук за назвою товару
                if (!string.IsNullOrWhiteSpace(search))
                {
                    allproduct = allproduct.Where(p => p.Name.Contains(search));
                }
                if (categoryId.HasValue && categoryId.Value > 0)
                {
                    allproduct = allproduct.Where(p => p.CategoryId == categoryId.Value);
                }
                if (HasDiscount == true)
                {
                    allproduct = allproduct.Where(p => p.Discount > 0);
                }
                if (stockStatus == "low")
                {
                    allproduct = allproduct.Where(p => p.Quantity > 0 && p.Quantity <= 3);
                }
                else if (stockStatus == "out")
                {
                    allproduct = allproduct.Where(p => p.Quantity == 0);
                }
                if (IsAvaivle == true)
                {
                    allproduct = allproduct.Where(p => p.IsAvailable && p.Quantity > 0);
                }
                if (condition.HasValue && condition.Value > 0)
                {
                    allproduct = allproduct.Where(p => p.Condition == (ItemCondition)condition);
                }
                if (MinPrice.HasValue)
                {
                    allproduct = allproduct.Where(p => p.Price >= MinPrice);
                }
                if (MaxPrice.HasValue)
                {
                    allproduct = allproduct.Where(p => p.Price <= MaxPrice.Value);
                }
                allproduct = SortBy switch
                {
                    "price_increase" => allproduct.OrderBy(p => p.Price),
                    "price_reduc" => allproduct.OrderByDescending(p => p.Price),
                    "date_old" => allproduct.OrderBy(p => p.CreatedAt),
                    _ => allproduct.OrderByDescending(p => p.CreatedAt)
                };
                var catageries = await categories.GetAllAsync();
                ViewBag.Categories = new SelectList(catageries, "id", "Name", categoryId);
                ViewBag.CurrentSearch = search;
                ViewBag.CurrentSort = SortBy;
                ViewBag.CurrentCondition = condition;
                ViewBag.MinPrice = MinPrice;
                ViewBag.MaxPrice = MaxPrice;
                ViewBag.HasDiscount = HasDiscount;
                ViewBag.IsAvailable = IsAvaivle;
                ViewBag.StockStatus = stockStatus;
                var listproduct = await allproduct.ToListAsync();
                return View(listproduct);
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
            return RedirectToAction("UserInterface", "Account");
        }
    }
}
