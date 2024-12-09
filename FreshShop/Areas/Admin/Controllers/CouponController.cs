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
            return View(coupon_list);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CouponModel coupon)
        {
            if (ModelState.IsValid)
            {
                _dataContext.Add(coupon);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Tạo phiếu giảm giá thành công.";
                return RedirectToAction("Index");
            }

            TempData["error"] = "Có lỗi trong quá trình nhập liệu.";
            return View(coupon);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var coupon = await _dataContext.Coupons.FindAsync(id);
            if (coupon == null)
            {
                return NotFound();
            }
            return View(coupon);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CouponModel coupon)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _dataContext.Update(coupon);
                    await _dataContext.SaveChangesAsync();
                    TempData["success"] = "Cập nhật phiếu giảm giá thành công.";
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CouponExists(coupon.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }
            TempData["error"] = "Có lỗi trong quá trình cập nhật.";
            return View(coupon);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var coupon = await _dataContext.Coupons.FindAsync(id);
            if (coupon == null)
            {
                return NotFound();
            }

            _dataContext.Coupons.Remove(coupon);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Xóa phiếu giảm giá thành công.";
            return RedirectToAction("Index");
        }

        private bool CouponExists(int id)
        {
            return _dataContext.Coupons.Any(e => e.Id == id);
        }
    }
}