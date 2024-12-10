using Microsoft.AspNetCore.Mvc;
using FreshShop.Models;
using FreshShop.Models.ViewModels;
using FreshShop.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FreshShop.Areas.Identity.Models.AccountViewModels;
using Microsoft.EntityFrameworkCore;

namespace FreshShop.Controllers
{
    public class HistoryController : Controller
    {
        private UserManager<AppUserModel> _userManage;
        private SignInManager<AppUserModel> _signInManager;
        private readonly IEmailSender _emailSender;
        private DataContext _dataContext;
        public HistoryController(
           IEmailSender emailSender,
           UserManager<AppUserModel> userManage,
           SignInManager<AppUserModel> signInManager,
           DataContext context)
        {
            _dataContext = context;
            _userManage = userManage;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }
        public async Task<IActionResult> History()
        { 
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var Orders = await _dataContext.Orders
                .Where(od => od.UserName == userEmail).OrderByDescending(od => od.Id).ToListAsync();
            ViewBag.UserEmail = userEmail;
            return View(Orders);
        }
		public async Task<IActionResult> CancelOrder(string ordercode)
		{
			try
			{
				// Tìm đơn hàng cần hủy dựa trên mã đơn hàng
				var order = await _dataContext.Orders.Where(o => o.OrderCode == ordercode).FirstOrDefaultAsync();

				// Cập nhật trạng thái của đơn hàng thành "đã hủy" (giả sử trạng thái 3 là đã hủy)
				order.Status = 3;

				// Lưu thay đổi vào cơ sở dữ liệu
				_dataContext.Update(order);
				await _dataContext.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				// Nếu có lỗi xảy ra, trả về thông báo lỗi
				return BadRequest("An error occurred while canceling the order.");
			}

			// Nếu không có lỗi, chuyển hướng đến trang lịch sử đơn hàng
			return RedirectToAction("History", "History");
		}

		public async Task<IActionResult> ViewHistory(string orderCode)
		{
			var userEmail = User.FindFirstValue(ClaimTypes.Email);

			// Fetch the order by the orderCode
			var orderDetails = await _dataContext.OrderDetails
				.Where(od => od.OrderCode == orderCode)
				.Include(od => od.Product) // Include product details in the order
				.ToListAsync();

			if (orderDetails == null || !orderDetails.Any())
			{
				return NotFound("Order details not found.");
			}

			// Fetch the order information (e.g., shipping cost, coupon)
			var order = await _dataContext.Orders
				.Where(o => o.OrderCode == orderCode && o.UserName == userEmail)
				.FirstOrDefaultAsync();

			if (order == null)
			{
				return NotFound("Order not found.");
			}

			// Pass order details and other information to the View
			ViewBag.ShippingCost = order.ShippingCost;
			ViewBag.CouponValue = order.CouponValue;
			ViewBag.Status = order.Status;

			return View(orderDetails);
		}

	}
}

