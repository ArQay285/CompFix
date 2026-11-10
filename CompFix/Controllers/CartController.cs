using System.Security.Claims;
using CompFix.Models;
using CompFix.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CompFix.Controllers
{
    //Контролер, який приймає команди від кнопок на сайті ("Додати в кошик", "Видалити", "Оформити замовлення").
    public class CartController : Controller
    {
        private readonly AppDbContext db;
        private readonly ControlBaseDate<CartItem> _caritem;
        private readonly ControlBaseDate<Product> _product;

        public CartController(AppDbContext db,ControlBaseDate<CartItem> caritem, ControlBaseDate<Product> product)
        {
            this.db = db;
            _caritem = caritem;
            _product = product;
        }
        private string GetCartUserId()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var id= User.FindFirstValue(ClaimTypes.NameIdentifier);
                if(id != null)
                {
                    return id;
                }
            }
            if(Request.Cookies.TryGetValue("CartId", out string? existCartId))
            {
                return existCartId;
            }
            string newCardId = Guid.NewGuid().ToString();
            Response.Cookies.Append("CartId", newCardId, new CookieOptions { Expires = DateTime.Now.AddDays(30), HttpOnly = true });
            return newCardId;
        }
        [HttpPost]
        public async Task<IActionResult> AddToCard(int productid)
        {
            string userId = GetCartUserId();
            var product = await _product.GetByIdAsync(productid);
            if(product == null)
            {
                return NotFound("Товар не знайдено");
            }
            var cartitem = await db.CartItems.FirstOrDefaultAsync(p => p.UserId == userId && p.ProductId == productid);
            if(cartitem != null)
            {
                cartitem.Quantity++;
                db.CartItems.Update(cartitem);
            }
            else
            {
                cartitem = new CartItem()
                {
                    UserId = userId,
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = 1
                };
                 
             db.CartItems.Add(cartitem);
            }
            await db.SaveChangesAsync();
            return RedirectToAction("Korzuna");
        }
        [HttpGet]
        public async Task<IActionResult> Korzuna()
        {
            string userId = GetCartUserId();
            var cartItems = await db.CartItems
            .Where(c => c.UserId == userId)
            .ToListAsync();
            ViewBag.TotalSum = cartItems.Sum(c => c.TotalPrice);
            return View(cartItems);

        }
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            string userId = GetCartUserId();
            await _caritem.RemoveUserItemAsync(cartItemId, userId);

            return RedirectToAction("Korzuna");
        }
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            string userId = GetCartUserId();
            var cartitem = await db.CartItems.Where(p => p.UserId == userId).ToListAsync();
            if (!cartitem.Any())
            {
                return RedirectToAction("Korzuna");
            }
            ViewBag.CartItems = cartitem;
            ViewBag.TotalSum = cartitem.Sum(c => c.TotalPrice);
            return View(new Order());
        }
        [HttpPost]
        public async Task<IActionResult> Checkout(Order order)
        {
            string userId = GetCartUserId();
            if (ModelState.IsValid)
            {
                var cartitems = await db.CartItems.Where(p => p.UserId == userId).ToListAsync();
                if (!cartitems.Any())
                {
                    return RedirectToAction("Index");
                }
                order.TotalAmount = cartitems.Sum(p => p.TotalPrice);
                foreach (var item in cartitems)
                {
                    order.OrderItems.Add(new OrderItem()
                    {
                        ProductName = item.ProductName,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = item.Price
                    });
                }
                db.Orders.Add(order);
                db.CartItems.RemoveRange(cartitems);
                await db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Оплата успішно пройшла!";
                return RedirectToAction("Korzuna");
            }
            else return BadRequest("Неправильно вказані дані");

        }

        }
}
