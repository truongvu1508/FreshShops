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
        public async Task<IActionResult> Add(int Id)
        {
            ProductModel product = await _dataContext.Products.FindAsync(Id);
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel cartItems = cart.Where(c=>c.ProductId==Id).FirstOrDefault();
            if (cartItems == null)
            {
                cart.Add(new CartItemModel(product));
            }
            else
            {
                cartItems.Quality += 1;
            }
            //Luu tru du lieu Cart vao session
            HttpContext.Session.SetJson("Cart", cart);
            TempData["success"] = "Thêm sản phẩm thành công!";
            //Tra ve trang truoc
            return Redirect(Request.Headers["Referer"].ToString());
        }
        public async Task<IActionResult> Decrease(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            CartItemModel cartItem = cart.Where(c=>c.ProductId == Id).FirstOrDefault();
            if (cartItem.Quality > 1)
            {
                --cartItem.Quality;
            }
            else
            {
                cart.RemoveAll(p=>p.ProductId == Id);
            }
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Increase(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            CartItemModel cartItem = cart.Where(c => c.ProductId == Id).FirstOrDefault();
            if (cartItem.Quality > 1)
            {
                ++cartItem.Quality;
            }
            else
            {
                cart.RemoveAll(p => p.ProductId == Id);
            }
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }
            return RedirectToAction("Index");
        }
        public IActionResult UpdateQuantity([FromBody] UpdateQuantityRequest request)
        {
            var cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            var cartItem = cart.FirstOrDefault(c => c.ProductId == request.ProductId);

            if (cartItem != null)
            {
                cartItem.Quality = request.Quantity;
                HttpContext.Session.SetJson("Cart", cart);

                return Json(new
                {
                    success = true,
                    grandTotal = cart.Sum(x => x.Quality * x.Price)
                });
            }

            return Json(new { success = false, message = "Product not found in cart" });
        }
        public async Task<IActionResult> Remove(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            cart.RemoveAll(p=>p.ProductId == Id);
            if (cart.Count == 0) 
            {
                //cart trong thi xoa session
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                //set lai session
                HttpContext.Session.SetJson("Cart", cart);
            }
            TempData["success"] = "Xóa sản phẩm thành công!";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Clear()
        {
            HttpContext.Session.Remove("Cart");
            return RedirectToAction("Index");
        }
    }
}
