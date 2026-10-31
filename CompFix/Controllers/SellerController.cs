using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CompFix.Models;
using CompFix.Services;
namespace CompFix.Controllers
{
    public class SellerController : Controller
    {

        private readonly ControlBaseDate<Product>? productRepo;
        public SellerController(ControlBaseDate<Product> productRepo)
        {
            this.productRepo = productRepo;
        }
        // 2. Метод для тестового додавання товару
        [HttpGet]
        public async Task<IActionResult> TestAdd()
        {
            var testProduct = new Product
            {
                Name = "Тестовий Ноутбук",
                Description = "Перевірка бази даних",
                Price = 25000,
                Quantity = 5,
                Condition = ItemCondition.New
            };

            await productRepo.AddAsync(testProduct);
            return Ok("Товар успішно додано в базу MS SQL Server!");
        }
            [HttpGet]
        // 3. Сторінка зі списком усіх товарів
        public async Task<IActionResult> Index()
        {
            var products = await productRepo.GetAllAsync();
            //return View(products);
            // ТИМЧАСОВО: повертаємо чисті дані з бази замість HTML-сторінки
            return Json(products);
        }
        // 4. Відображення порожньої форми для додавання
        [HttpGet]
        public IActionResult AddProduct()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                await productRepo.AddAsync(product);
                return RedirectToAction(nameof(Index));
            }

            return View(product);
            
        }

        
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await productRepo.RemoveAsync(id);
            return RedirectToAction(nameof(Index));
        }


    }
}
