using FreshShop.Models;
using FreshShop.Models.ViewModels;
using FreshShop.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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

            //Nhan shipping tu cookie
            var shippingPriceCookie = Request.Cookies["ShippingPrice"];
            decimal shippingPrice = 0;
            if (shippingPriceCookie != null)
            {
                var shippingPriceJson = shippingPriceCookie;
                shippingPrice = JsonConvert.DeserializeObject<decimal>(shippingPriceJson);
            }

            //Nhan coupon tu cookie
            var couponValueCookie = Request.Cookies["CouponValue"];
            int couponValue = 0;
            if (couponValueCookie != null)
            {
                couponValue = JsonConvert.DeserializeObject<int>(couponValueCookie);
            }
            CartItemViewModel cartVM = new CartItemViewModel()
            {
                CartItems = cartItems,
                GrandTotal = cartItems.Sum(x => x.Quality * x.Price), 
                ShippingCost = shippingPrice,
                CouponValue = couponValue,
            };
            return View(cartVM);
        }
        public IActionResult Checkout()
        {
            return View("~/Views/Checkout/Index.cshtml");
        }
        public async Task<IActionResult> Add(int Id)
        {
            ProductModel product = await _dataContext.Products.FindAsync(Id);
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel cartItems = cart.Where(c => c.ProductId == Id).FirstOrDefault();
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
            CartItemModel cartItem = cart.Where(c => c.ProductId == Id).FirstOrDefault();
            if (cartItem.Quality > 1)
            {
                --cartItem.Quality;
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
            cart.RemoveAll(p => p.ProductId == Id);
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
        [HttpPost]
        [Route("Cart/GetShipping")]
        public async Task<IActionResult> GetShipping(ShippingModel shippingModel, string quan, string tinh, string phuong)
        {
            //Tim phi ship tuong ung
            var existingShipping = await _dataContext.Shipping.FirstOrDefaultAsync(x=>x.City == tinh && x.District == quan && x.Ward == phuong);
            decimal shippingPrice = 0;
            if (existingShipping != null)
            {
                shippingPrice = existingShipping.Price;
            }
            else
            {
                shippingPrice = 50000;
            }
            //Chuyen sang kieu Json
            var shippingPriceJson = JsonConvert.SerializeObject(shippingPrice);
            try
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                    Secure = true //Dung Https cung duoc
                };
                //Day gia vao cookieOption
                Response.Cookies.Append("ShippingPrice", shippingPriceJson, cookieOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi thêm cookie shipping: {ex.Message}");
            }
            return Json(new { shippingPrice });
        }
        [HttpGet]
        [Route("Cart/RemoveShippingCookie")]
        public IActionResult RemoveShippingCookie()
        {
            Response.Cookies.Delete("ShippingPrice");
            return RedirectToAction("Index");
        }
        [HttpPost]
        [Route("Cart/GetCoupon")]
        public async Task<IActionResult> GetCoupon(string coupon_value)
        {
            // Tìm mã giảm giá tương ứng
            var validCoupon = await _dataContext.Coupons.FirstOrDefaultAsync(x => x.Name == coupon_value);

            if (validCoupon != null)
            {
                TimeSpan remainingTime = validCoupon.DateExpired - DateTime.Now;
                if (remainingTime.Days >= 0)
                {
                    try
                    {
                        // Lưu trực tiếp giá trị giảm giá vào cookie
                        var cookieOptions = new CookieOptions
                        {
                            HttpOnly = true,
                            Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                            Secure = true,
                            SameSite = SameSiteMode.Strict
                        };

                        // Chuyển `CouponValue` thành JSON
                        var couponValueJson = JsonConvert.SerializeObject(validCoupon.CouponValue);

                        // Lưu giá trị vào cookie
                        Response.Cookies.Append("CouponValue", couponValueJson, cookieOptions);

                        return Ok(new { success = true, message = "Dùng mã giảm giá thành công" });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Lỗi khi lưu mã giảm giá vào cookie: {ex.Message}");
                        return Ok(new { success = false, message = "Dùng mã giảm giá thất bại" });
                    }
                }
                else
                {
                    return Ok(new { success = false, message = "Mã giảm giá đã hết hạn" });
                }
            }

            return Ok(new { success = false, message = "Mã giảm giá không tồn tại" });
        }
        [HttpGet]
        [Route("Cart/RemoveCouponCookie")]
        public IActionResult RemoveCouponCookie()
        {
            Response.Cookies.Delete("CouponValue");
            return RedirectToAction("Index");
        }

    }
}