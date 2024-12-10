using FreshShop.Data;
using FreshShop.Models;
using FreshShop.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreshShop.Areas.Admin.Controllers
{
    [Authorize(Roles = RoleName.Administrator)]
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly DataContext _dataContext;
        public OrderController(DataContext context)
        {
            _dataContext = context;
        }
		public async Task<IActionResult> Index()
        {
            return View(await _dataContext.Orders.OrderByDescending(c => c.Id).ToListAsync());
        }
		public async Task<IActionResult> ViewOrder(string ordercode)
		{
            var DetailsOrder = await _dataContext.OrderDetails.Include(o=>o.Product).Where(od=>od.OrderCode == ordercode).ToListAsync();
            var Order = _dataContext.Orders.Where(o => o.OrderCode == ordercode).First();
            ViewBag.ShippingCost = Order.ShippingCost;
            ViewBag.Status = Order.Status;
            var CouponValue = _dataContext.Orders.Where(o => o.OrderCode == ordercode).First();
            ViewBag.CouponValue = CouponValue.CouponValue;
            return View(DetailsOrder);
		}

        [HttpGet]
        [Route("PaymentMomoInfo")]
        public async Task<IActionResult> PaymentMomoInfo(string orderId)
        {
            var momoInfo = await _dataContext.MomoInfoModels
                .FirstOrDefaultAsync(m => m.OrderId == orderId);

            if (momoInfo == null)
            {
                return NotFound();
            }

            return View(momoInfo);
        }

        [HttpPost]
        [Route("UpdateOrder")]
        public async Task<IActionResult> UpdateOrder(string ordercode, int status)
        {
            var order = await _dataContext.Orders.FirstOrDefaultAsync(o => o.OrderCode == ordercode);
            if (order == null)
            {
                return NotFound();
            }
            order.Status = status;
            _dataContext.Update(order);
            if (status == 2) { 
                var DetailsOrder = await _dataContext.OrderDetails.Include(od=>od.Product)
                    .Where(od=>od.OrderCode ==order.OrderCode).Select (od=>new
                    { 
                        od.Quantity,
                        od.Product.Price,
                        od.Product.CapitalPrice
                    }).ToListAsync();
                var statisticalModel = await _dataContext.Statistical.FirstOrDefaultAsync(
                    s => s.DateCreated.Date == order.CreatedDate.Date);
                if (statisticalModel != null)
                {
                    foreach (var oderDetail in DetailsOrder)
                    {
                        statisticalModel.Quantity += 1;
                        statisticalModel.Sold += oderDetail.Quantity;
                        statisticalModel.Revenue += (int)(oderDetail.Quantity * oderDetail.Price);
                        statisticalModel.Profit += (int)(oderDetail.Price - oderDetail.CapitalPrice);
                    }
                    _dataContext.Update(statisticalModel);
                }
                else {
                    int new_quantity = 0;
                    int new_sold = 0;
                    decimal new_profit = 0;
                    foreach (var oderDetail in DetailsOrder)
                    {
                        new_quantity += 1;
                        new_sold += oderDetail.Quantity;
                        new_profit += oderDetail.Price - oderDetail.CapitalPrice;

                        statisticalModel = new Statistical
                        {
                            DateCreated = order.CreatedDate,
                            Quantity = new_quantity,
                            Sold = new_sold,
                            Revenue = (int)(oderDetail.Quantity * oderDetail.Price),
                            Profit = (int)(new_profit)
                        };
                    }
                    _dataContext.Add(statisticalModel);
                }
            }
            try
            {
                await _dataContext.SaveChangesAsync();
                return Ok(new { success = true, message = "Order status updated successsfully" });
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while updating the order status.");

            }
        }
        // GET: Delete Order
        public async Task<IActionResult> Delete(int Id)
        {
            var order = await _dataContext.Orders.FindAsync(Id);

            if (order == null)
            {
                TempData["error"] = "Đơn hàng không tồn tại.";
                return RedirectToAction("Index");
            }

            _dataContext.Orders.Remove(order);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Đơn hàng đã được xóa.";
            return RedirectToAction("Index");
        }
    }
}
