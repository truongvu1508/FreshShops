using FreshShop.Models;
using FreshShop.Models.ViewModels;
using FreshShop.Repository;
using Microsoft.AspNetCore.Mvc;

namespace FreshShop.Controllers
{
    public class CartController : Controller
    {
        private readonly DataContext _dataContext;
        public CartController(DataContext _context)
        {
            _dataContext = _context;
        }
        public IActionResult Index()
        {
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemViewModel cartVM = new CartItemViewModel()
            {
                CartItems = cartItems,
                GrandTotal = cartItems.Sum(x=>x.Quality*x.Price)
            };
            return View(cartVM);
        }
        public IActionResult Checkout()
        {
            return View("~/View/Checkout/Index.cshtml");
        }
    }
}
