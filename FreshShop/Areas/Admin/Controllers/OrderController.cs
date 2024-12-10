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
            var detailsOrder = await _dataContext.OrderDetails
                .Include(o => o.Product)
                .Where(od => od.OrderCode == ordercode)
                .ToListAsync();

            var order = await _dataContext.Orders.FirstOrDefaultAsync(o => o.OrderCode == ordercode);

            if (order == null)
            {
                return NotFound();
            }

            ViewBag.ShippingCost = order.ShippingCost;
            ViewBag.Status = order.Status;
            ViewBag.CouponValue = order.CouponValue;

            return View(detailsOrder);
        }

        [HttpPost]
        [Route("Admin/Order/UpdateOrder")]
        public async Task<IActionResult> UpdateOrder(string ordercode, int status)
        {
            var order = await _dataContext.Orders.FirstOrDefaultAsync(o => o.OrderCode == ordercode);
            if (order == null)
            {
                return Json(new { success = false, message = "Order not found." });
            }

            order.Status = status;

            if (status == 2) // Status: Đã xử lý
            {
                var detailsOrder = await _dataContext.OrderDetails
                    .Include(od => od.Product)
                    .Where(od => od.OrderCode == order.OrderCode)
                    .Select(od => new
                    {
                        od.Quantity,
                        od.Product.Price,
                        od.Product.CapitalPrice
                    })
                    .ToListAsync();

                var statisticalModel = await _dataContext.Statistical
                    .FirstOrDefaultAsync(s => s.DateCreated.Date == order.CreatedDate.Date);

                if (statisticalModel != null)
                {
                    foreach (var orderDetail in detailsOrder)
                    {
                        statisticalModel.Quantity += 1;
                        statisticalModel.Sold += orderDetail.Quantity;
                        statisticalModel.Revenue += (int)(orderDetail.Quantity * orderDetail.Price);
                        statisticalModel.Profit += (int)(orderDetail.Quantity * (orderDetail.Price - orderDetail.CapitalPrice));
                    }
                    _dataContext.Update(statisticalModel);
                }
                else
                {
                    var newStatistical = new Statistical
                    {
                        DateCreated = order.CreatedDate,
                        Quantity = detailsOrder.Count,
                        Sold = detailsOrder.Sum(od => od.Quantity),
                        Revenue = detailsOrder.Sum(od => (int)(od.Quantity * od.Price)),
                        Profit = detailsOrder.Sum(od => (int)(od.Quantity * (od.Price - od.CapitalPrice)))
                    };
                    _dataContext.Add(newStatistical);
                }
            }

            _dataContext.Update(order);

            try
            {
                await _dataContext.SaveChangesAsync();
                return Json(new { success = true, message = "Order status updated successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while updating the order status.", error = ex.Message });
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
