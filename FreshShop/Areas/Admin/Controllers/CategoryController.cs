﻿using FreshShop.Data;
using FreshShop.Models;
using FreshShop.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FreshShop.Areas.Admin.Controllers
{
    [Authorize(Roles = RoleName.Administrator)]
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly DataContext _dataContext;
        public CategoryController(DataContext context)
        {
            _dataContext = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _dataContext.Categories.OrderByDescending(c => c.Id).ToListAsync());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryModel category)
        {

            if (ModelState.IsValid)
            {
                // Thay thế khoảng trắng trong tên sản phẩm bằng dấu gạch nối
                category.Slug = category.Name.Replace(" ", "-").ToLower();

                // Kiểm tra xem Slug có bị trùng trong cơ sở dữ liệu không
                var slug = await _dataContext.Categories.FirstOrDefaultAsync(p => p.Slug == category.Slug);
                if (slug != null)
                {
                    // Thêm lỗi vào ModelState nếu Slug đã tồn tại trong DB
                    ModelState.AddModelError("", "Danh mục đã có trong DB");
                    return View(category);
                }



                // Thêm sản phẩm vào cơ sở dữ liệu
                _dataContext.Add(category);
                await _dataContext.SaveChangesAsync();

                // Thông báo thành công
                TempData["success"] = "Tạo danh mục thành công.";
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
            return View(category);
        }
        public async Task<IActionResult> Edit(int Id)
        {
			CategoryModel category = await _dataContext.Categories.FindAsync(Id);
			return View(category);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(CategoryModel category)
		{

			if (ModelState.IsValid)
			{
				// Thay thế khoảng trắng trong tên sản phẩm bằng dấu gạch nối
				category.Slug = category.Name.Replace(" ", "-").ToLower();

				// Kiểm tra xem Slug có bị trùng trong cơ sở dữ liệu không
				var slug = await _dataContext.Categories.FirstOrDefaultAsync(p => p.Slug == category.Slug && p.Id != category.Id);
				if (slug != null)
				{
					// Thêm lỗi vào ModelState nếu Slug đã tồn tại trong DB
					ModelState.AddModelError("", "Danh mục đã có trong DB");
					return View(category);
				}



				// Update danh mục vào cơ sở dữ liệu
				_dataContext.Update(category);
				await _dataContext.SaveChangesAsync();

				// Thông báo thành công
				TempData["success"] = "Cập nhật danh mục thành công.";
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
			return View(category);
		}
		public async Task<IActionResult> Delete(int Id)
        {
            CategoryModel category = await _dataContext.Categories.FindAsync(Id);
            _dataContext.Categories.Remove(category);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Danh mục đã xóa";
            return RedirectToAction("Index");

        }
    }
}
