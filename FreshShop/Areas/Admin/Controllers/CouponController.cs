using FreshShop.Models;
using FreshShop.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreshShop.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class CouponController : Controller
	{
		private readonly DataContext _dataContext;
		public CouponController(DataContext context)
		{
			_dataContext = context;
		}
		public async Task<IActionResult> Index()
		{
			var coupon_list = await _dataContext.Coupons.ToListAsync();
			ViewBag.CouponList = coupon_list;
			return View();
		}
		[Route("Create")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(CouponModel coupon)
		{

			if (ModelState.IsValid)
			{

				// Thêm sản phẩm vào cơ sở dữ liệu
				_dataContext.Add(coupon);
				await _dataContext.SaveChangesAsync();

				// Thông báo thành công
				TempData["success"] = "Tạo phiếu giảm giá thành công.";
				return RedirectToAction("Index");
			}
			else
			{
				// Thông báo lỗi nếu model không hợp lệ
				TempData["error"] = "Có lỗi trong quá trình nhập liệu.";
				List<string> errors = new List<string>();
				foreach (var value in ModelState.Values)
				{
					foreach (var error in value.Errors)
					{
						errors.Add(error.ErrorMessage);
					}
				}
				string errormess = string.Join("\n", errors);
				return BadRequest(errormess);
			}
			return View();
		}
	}
}
