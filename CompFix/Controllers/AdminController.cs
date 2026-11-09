using CompFix.Models;
using CompFix.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.Mime.MediaTypeNames;
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


        //Видаляє фото яке вже не використвуєтсья з папки
        private void DeleteExistFile(Product existingProduct)
        {
            if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
            {
                string existingFilePath = Path.Combine(_webHostEnvironment.WebRootPath, existingProduct.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(existingFilePath))
                {
                    System.IO.File.Delete(existingFilePath);
                }
            }
        }
        
        [Route("")] // Відповідає адресі /admin
        [HttpGet("allproducts")]
        /// <summary>
        /// Сторінка зі списком усіх товарів
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> AllProducts([FromServices] ControlBaseDate<Category> categories,
            string? search,
            int? categoryId,
            int? condition,
            bool? IsAvailable,
            bool? HasDiscount,
            string? stockStatus,
            decimal?MinPrice,
            decimal? MaxPrice,
            string? SortBy)
        {
            var allproduct = _productRepo.GetQueryable().Include(p => p.Category).AsQueryable();
            // 2. Текстовий пошук за назвою товару
            if (!string.IsNullOrWhiteSpace(search))
            {
                allproduct = allproduct.Where(p => p.Name.Contains(search));
            }
            if(categoryId.HasValue && categoryId.Value > 0)
            {
                allproduct = allproduct.Where(p => p.CategoryId == categoryId.Value);
            }
            if(HasDiscount == true)
            {
                allproduct = allproduct.Where(p => p.Discount > 0);
            }
            if(stockStatus == "low")
            {
                allproduct = allproduct.Where(p => p.Quantity > 0 && p.Quantity <= 3);
            }else if(stockStatus == "out"){
                allproduct = allproduct.Where(p => p.Quantity == 0);
            }
            if(IsAvailable == true)
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
                _          => allproduct.OrderByDescending(p => p.CreatedAt)
            };
            var catageries = await categories.GetAllAsync();
            ViewBag.Categories = new SelectList(catageries, "id", "Name", categoryId);
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentSort = SortBy;
            ViewBag.CurrentCondition = condition;
            ViewBag.MinPrice = MinPrice;
            ViewBag.MaxPrice = MaxPrice;
            ViewBag.HasDiscount = HasDiscount;  
            ViewBag.IsAvailable = IsAvailable;
            ViewBag.StockStatus = stockStatus;
            var listproduct = await allproduct.ToListAsync();
            return View(listproduct);
        }

        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null) return NotFound("Товар не знайдено");

            return Ok(product);
        }

        // 4. Відображення порожньої formи для додавання
        [HttpGet("addproduct")]
        public async  Task<IActionResult> AddProduct([FromServices] ControlBaseDate<Category> categoryRepo)
        {
            var categories = await categoryRepo.GetAllAsync();
            if(categories == null)
            {
                return NotFound("Категорії не знайдено");
            }
            ViewBag.Categories = new SelectList(categories, "id", "Name");
            return View();
        }
        [HttpPost("addproductAsync")]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(5 * 1024 * 1024)]
        public async Task<IActionResult> AddProductAsync(Product product, IFormFile? image, [FromServices] ControlBaseDate<Category> categoryRepo)
        {
            ModelState.Remove(nameof(product.ImageUrl));
            ModelState.Remove(nameof(product.Category));
            if (ModelState.IsValid)
            {

                if (image != null && image.Length > 0)
                {
                    // 2. Список дозволених розширень для безпеки
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", };
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
                return RedirectToAction(nameof(AllProducts));// тимчасово вертає назад допоки не свторено повніціно сторніка із списком продуктів
            }
            var categories = await categoryRepo.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "id", "Name", product.CategoryId);
            return View(product);

        }


        [HttpGet("editproduct")]
        public async Task<IActionResult> EditProduct(int id, [FromServices] ControlBaseDate<Category> categoryRepo)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null) return NotFound("Товар не знайдено");
            var categories = await categoryRepo.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "id", "Name", product.CategoryId);
            return View(product);
        }
        [HttpPost("editproductAsync")]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(5 * 1024 * 1024)]
        public async Task<IActionResult> EditProductAsync(int id, Product product, IFormFile? image, [FromServices] ControlBaseDate<Category> categoryRepo)
        {
            if (id != product.Id) return NotFound("Товар не знайдено");
            var existingProduct = await _productRepo.GetByIdAsync(id);
            if (existingProduct == null) return NotFound("Товар не знайдено");
            ModelState.Remove(nameof(product.ImageUrl));
            ModelState.Remove(nameof(product.Category));
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
                    DeleteExistFile(existingProduct);
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(image.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                    }


                    existingProduct.ImageUrl = "/images/products/" + uniqueFileName;
                }
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.Quantity = product.Quantity;
                existingProduct.Condition = product.Condition;
                existingProduct.Discount = product.Discount;
                existingProduct.IsAvailable = product.IsAvailable;
                await _productRepo.UpdateAsync(existingProduct);
                return RedirectToAction(nameof(AllProducts));// тимчасово вертає назад допоки не свторено повніціно сторніка із списком продуктів
            }
            var categories = await categoryRepo.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "id", "Name", product.CategoryId);
            return View(product);
        }


        [HttpPost("DeleteAsync/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null) return NotFound("Товар не знайдено");
            DeleteExistFile(product);
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
