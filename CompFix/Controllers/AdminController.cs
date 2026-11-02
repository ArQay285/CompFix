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
        public AdminController(ControlBaseDate<Product> productRepo)
        {
            _productRepo = productRepo;
        }

        [Route("")] // Відповідає адресі /admin
        [Route("index")]
        public async Task<IActionResult> Index()
        {
            await Task.CompletedTask;
            return View();
        }

        //[HttpGet("allproducts")]
        //// 3. Сторінка зі списком усіх товарів
        //public async Task<IActionResult> AllProducts()
        //{
        //    var products = await productRepo.GetAllAsync();
        //    return View(products);

        //}

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
        [HttpPost]
        public async Task<IActionResult> AddProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                await _productRepo.AddAsync(product);
                return RedirectToAction(nameof(Index));
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
        [HttpPost]
        public async Task<IActionResult> EditProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                await _productRepo.UpdateAsync(product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null) return NotFound("Товар не знайдено");
            await _productRepo.RemoveAsync(id);
            return RedirectToAction(nameof(Index));
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
            return RedirectToAction(nameof(Index));
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
            return RedirectToAction(nameof(Index));
        }



    }
}
