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
    public class ShippingController : Controller
    {
        private readonly DataContext _dataContext;
        public ShippingController(DataContext context)
        {
            _dataContext = context;
        }
        public async Task<IActionResult> Index()
        {
            var shippingList = await _dataContext.Shipping.ToListAsync();
            ViewBag.Shipping = shippingList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> StoreShipping(ShippingModel shippingModel, string phuong, string quan, string tinh, decimal price)
        {
            shippingModel.City = tinh;
            shippingModel.District = quan;
            shippingModel.Ward = phuong;
            shippingModel.Price = price;

            try
            {
                // Kiểm tra dữ liệu có bị trùng lặp không
                var existingShipping = await _dataContext.Shipping
                    .AnyAsync(x => x.City == tinh && x.District == quan && x.Ward == phuong);

                if (existingShipping)
                {
                    // Trả về kết quả nếu trùng lặp
                    return Ok(new { duplicate = true, message = "Dữ liệu trùng lặp." });

                }

                // Thêm dữ liệu mới
                _dataContext.Shipping.Add(shippingModel);
                await _dataContext.SaveChangesAsync();

                // Trả về kết quả thành công
                return Ok(new { success = true, message = "Thêm shipping thành công." });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Đã xảy ra lỗi: " + ex.Message });
            }
        }

        public async Task<IActionResult> Delete(int Id)
        {
            ShippingModel shipping = await _dataContext.Shipping.FindAsync(Id);
            _dataContext.Shipping.Remove(shipping);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Shipping đã được xóa thành công";
            return RedirectToAction("Index","Shipping");
        }

    }
}

