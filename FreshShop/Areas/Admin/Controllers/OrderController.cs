using FreshShop.Data;
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
            var ShippingCost = _dataContext.Orders.Where(o => o.OrderCode == ordercode).First();
            ViewBag.ShippingCost = ShippingCost.ShippingCost;
            var CouponValue = _dataContext.Orders.Where(o => o.OrderCode == ordercode).First();
            ViewBag.CouponValue = CouponValue.CouponValue;
            return View(DetailsOrder);
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
            try
            {
                await _dataContext.SaveChangesAsync();
                return Ok(new { success = true, message = "Order status updated succcessfully" });
            }
            catch (Exception ex) 
            {
                return StatusCode(500, "Lỗi khi cập nhật trạng thái");
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
