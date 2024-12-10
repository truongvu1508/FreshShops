using FreshShop.Models;
using FreshShop.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace FreshShop.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly DataContext _dataContext;
        public CheckoutController(DataContext context)
        {
            _dataContext = context;
        }
        public async Task<IActionResult> Checkout()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var orderCode = Guid.NewGuid().ToString();
                var orderItem = new OrderModel();
                orderItem.OrderCode = orderCode;
                //lay shipping tu cookie
                var shippingPriceCookie = Request.Cookies["ShippingPrice"];
                decimal shippingPrice = 0;
                
                if (shippingPriceCookie != null)
                {
                    var shippingPriceJson = shippingPriceCookie;
                    shippingPrice = JsonConvert.DeserializeObject<decimal>(shippingPriceJson);
                }

                //lay coupon tu cookie
                var couponValueCookie = Request.Cookies["CouponValue"];
                int couponValue = 0;
                if (couponValueCookie != null)
                {
                    var couponValueJson = couponValueCookie;
                    couponValue = JsonConvert.DeserializeObject<int>(couponValueJson);
                }
                orderItem.ShippingCost = shippingPrice;
                orderItem.CouponValue = couponValue;
                orderItem.UserName = userEmail;
                orderItem.Status = 1;
                orderItem.CreatedDate = DateTime.Now;
                _dataContext.Add(orderItem);
                _dataContext.SaveChanges();
                List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
                foreach (var cart in cartItems)
                {
                    var orderDetails = new OrderDetails();
                    orderDetails.UserName = userEmail;
                    orderDetails.OrderCode = orderCode;
                    orderDetails.ProductId = cart.ProductId;
                    orderDetails.Price = cart.Price;
                    orderDetails.Quantity = cart.Quality;
                    var product = await _dataContext.Products.Where(p => p.Id == cart.ProductId).FirstAsync();
                    product.Quantity -= cart.Quality;
                    product.Sold += cart.Quality;
					_dataContext.Update(product);
					_dataContext.Add(orderDetails);
                    _dataContext.SaveChanges();
                }
                HttpContext.Session.Remove("Cart");
                TempData["success"] = "Checkout thành công, vui lòng chờ duyệt đơn hàng";
                return RedirectToAction("History", "History");
            }
            return View();
        }
    }
}