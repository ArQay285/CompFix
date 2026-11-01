
using CompFix.Models;
using CompFix.Services;
using Microsoft.AspNetCore.Mvc;

namespace CompFix.Controllers
{
    //Контролер, який приймає команди від кнопок на сайті ("Додати в кошик", "Видалити", "Оформити замовлення").
    public class CartController : Controller
    {
        private readonly AppDbContext db;

        public CartController(AppDbContext db)
        {
            this.db = db;
        }




    }
}
