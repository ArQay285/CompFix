using CompFix.Models;
using CompFix.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
namespace CompFix.Controllers
{
    //Контролер для адміністратора, який дозволяє додавати, редагувати та видаляти товари.
    //TODO: розширити функціонал контролера для роботи з користувачами(коли буде готовий клас Users)
    [Authorize(Roles = "admin")] // Для доступу потрібні права admin
    [Route("admin")] // Для доступу за префіксом /admin
    public class AdminController : Controller
    {

        private readonly ControlBaseDate<Product> _productRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AdminController(ControlBaseDate<Product> productRepo, IWebHostEnvironment webHostEnvironment)
        {
            _productRepo = productRepo;
            _webHostEnvironment = webHostEnvironment;
        }

        [Route("")] // Відповідає адресі /admin
        [Route("index")]
        public async Task<IActionResult> Index()
        {
            await Task.CompletedTask;
            return View();
        }

        [HttpGet("allproducts")]
        /// <summary>
        /// Сторінка зі списком усіх товарів
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> AllProducts()
        {
            var products = await _productRepo.GetAllAsync();
            return View(products);

        }

        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null) return NotFound("Товар не знайдено");

            return Ok(product);
        }

        // 4. Відображення порожньої форми для додавання
        [HttpGet("addproduct")]
        public IActionResult AddProduct()
        {
            return View();
        }
        [HttpPost("addproduct")]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(5 * 1024 * 1024)]
        public async Task<IActionResult> AddProduct(Product product, IFormFile image)
        {
            ModelState.Remove(nameof(product.ImageUrl));
            if (ModelState.IsValid)
            {
                if (image != null && image.Length > 0)
                {
                    // 2. Список дозволених розширень для безпеки
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                    string fileExtension = Path.GetExtension(image.FileName).ToLowerInvariant();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("Image", "Неприпустимий формат зображення. Дозволені формати: .jpg, .jpeg, .png, .webp");
                        return View(product);
                    }
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    // 3. Генеруємо унікальне ім'я файлу (Guid + оригінальна назва)
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(image.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var filestream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(filestream);
                    }
                    product.ImageUrl = "/images/products/" + uniqueFileName;
                }
                await _productRepo.AddAsync(product);
                return RedirectToAction(nameof(AllProducts));
            }

            return View(product);

        }


        [HttpGet("editproduct/{id}")]
        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null) return NotFound("Товар не знайдено");

            return View(product);
        }
        [HttpPost("editproduct/{id}")]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(5 * 1024 * 1024)]
        public async Task<IActionResult> EditProduct(int id, Product product, IFormFile? image)
        {
            if (id != product.Id) return NotFound("Товар не знайдено");
            var existingProduct = await _productRepo.GetByIdAsync(id);
            if (existingProduct == null) return NotFound("Товар не знайдено");
            ModelState.Remove(nameof(product.ImageUrl));
            if (ModelState.IsValid)
            {
                if (image != null && image.Length > 0)
                {

                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                    string fileExtension = Path.GetExtension(image.FileName).ToLowerInvariant();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("Image", "Неприпустимий формат зображення. Дозволені формати: .jpg, .jpeg, .png, .webp");
                        return View(product);
                    }
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
                    {
                        string existingFilePath = Path.Combine(_webHostEnvironment.WebRootPath, existingProduct.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                        if (System.IO.File.Exists(existingFilePath))
                        {
                            System.IO.File.Delete(existingFilePath);
                        }
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(image.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                    }


                    existingProduct.ImageUrl = "/images/products/" + uniqueFileName;
                }
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.Quantity = product.Quantity;
                existingProduct.Condition = product.Condition;
                existingProduct.Discount = product.Discount;
                existingProduct.IsAvailable = product.IsAvailable;
                await _productRepo.UpdateAsync(existingProduct);
                return RedirectToAction(nameof(AllProducts));
            }
            return View(product);
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null) return NotFound("Товар не знайдено");
            await _productRepo.RemoveAsync(id);
            return RedirectToAction(nameof(AllProducts));
        }

        [HttpPost]
        //Для встановлення знижки на товар
        public async Task<IActionResult> Discount(int id, int discount)
        {
            if (discount < 0 || discount > 100)
            {
                return BadRequest("Знижка має бути в межах від 0% до 100%");
            }
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound("Товар не знайдено");
            }

            product.Discount = discount;
            await _productRepo.UpdateAsync(product);
            return RedirectToAction(nameof(AllProducts));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleAvailability(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound("Товар не знайдено");
            }
            product.IsAvailable = !product.IsAvailable;
            await _productRepo.UpdateAsync(product);
            return RedirectToAction(nameof(AllProducts));
        }



    }
}
