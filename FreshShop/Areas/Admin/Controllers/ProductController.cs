﻿using FreshShop.Data;
using FreshShop.Models;
using FreshShop.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace FreshShop.Areas.Admin.Controllers
{
    [Authorize(Roles = RoleName.Administrator)]
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductModel product)
        {
            // Gán lại danh sách Categories vào ViewBag để hiển thị dropdown
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);

            if (ModelState.IsValid)
            {
                // Thay thế khoảng trắng trong tên sản phẩm bằng dấu gạch nối
                product.Slug = product.Name.Replace(" ", "-").ToLower();

                // Kiểm tra xem Slug có bị trùng trong cơ sở dữ liệu không
                var slug = await _dataContext.Products.FirstOrDefaultAsync(p => p.Slug == product.Slug);
                if (slug != null)
                {
                    // Thêm lỗi vào ModelState nếu Slug đã tồn tại trong DB
                    ModelState.AddModelError("", "Sản phẩm đã có trong DB");
                    return View(product);
                }

                // Kiểm tra và lưu ảnh nếu có
                if (product.ImageUpload != null)
                {
                    // Đường dẫn đến thư mục lưu trữ ảnh
                    string uploaddir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                    string imgname = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                    string filepath = Path.Combine(uploaddir, imgname);

                    // Lưu ảnh vào thư mục
                    using (FileStream fs = new FileStream(filepath, FileMode.Create))
                    {
                        await product.ImageUpload.CopyToAsync(fs);
                    }

                    // Lưu tên ảnh vào thuộc tính Image của sản phẩm
                    product.Image = imgname;
                }

                // Thêm sản phẩm vào cơ sở dữ liệu
                _dataContext.Add(product);
                await _dataContext.SaveChangesAsync();

                // Thông báo thành công
                TempData["success"] = "Tạo sản phẩm thành công.";
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
        }

        public async Task<IActionResult> Edit(int Id)
        {
            ProductModel product = await _dataContext.Products.FirstAsync(p => p.Id == Id);
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Id, ProductModel product)
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);

            var exisxted_product = await _dataContext.Products.FindAsync(Id);
            if (exisxted_product == null)
            {
                TempData["error"] = "Sản phẩm không tồn tại.";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                // Tạo Slug
                product.Slug = product.Name.Replace(" ", "-").ToLower();

                // Kiểm tra Slug trùng
                var slug = await _dataContext.Products.FirstOrDefaultAsync(p => p.Slug == product.Slug && p.Id != Id);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Sản phẩm đã tồn tại trong DB với tên này.");
                    return View(product);
                }

                // Kiểm tra và xử lý ảnh
                if (product.ImageUpload != null)
                {
                    string uploaddir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                    string imgname = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                    string filepath = Path.Combine(uploaddir, imgname);

                    string oldfilepath = Path.Combine(uploaddir, exisxted_product.Image);
                    try
                    {
                        if (System.IO.File.Exists(oldfilepath))
                        {
                            System.IO.File.Delete(oldfilepath);
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Không thể xóa ảnh cũ: " + ex.Message);
                        return View(product);
                    }

                    using (FileStream fs = new FileStream(filepath, FileMode.Create))
                    {
                        await product.ImageUpload.CopyToAsync(fs);
                    }

                    exisxted_product.Image = imgname;
                }

                // Cập nhật các thuộc tính
                exisxted_product.Name = product.Name;
                exisxted_product.Price = product.Price;
                exisxted_product.CapitalPrice = product.CapitalPrice;
                exisxted_product.Description = product.Description;
                exisxted_product.CategoryId = product.CategoryId;
                exisxted_product.Slug = product.Slug;

                await _dataContext.SaveChangesAsync();

                TempData["success"] = "Cập nhật sản phẩm thành công.";
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
                TempData["error"] = "Có lỗi trong quá trình nhập liệu.";
                return View(product);
            }
        }


        public async Task<IActionResult> Delete(int Id)
        {
            ProductModel product = await _dataContext.Products.FirstAsync(p => p.Id == Id);
                string uploaddir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                string oldfile = Path.Combine(uploaddir, product.Image);
                if (System.IO.File.Exists(oldfile))
                {
                    System.IO.File.Delete(oldfile);
                }
            _dataContext.Products.Remove(product);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction("Index");

        }

        [Route("AddQuantity")]
        [HttpGet]
        public async Task<IActionResult> AddQuantity(int Id)
        {
            var productQuantity = await _dataContext.ProductQuantities.Where(p => p.ProductId == Id).ToListAsync();
            ViewBag.Id = Id;
            ViewBag.ProductByQuantity = productQuantity;
            return View();
        }


        [Route("StoreProductQuantity")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StoreProductQuantity(ProductQuantityModel model)
        {
            var product = _dataContext.Products.Find(model.ProductId);
            if (product == null)
            {
                return NotFound();
            }
            product.Quantity += model.Quantity;

            model.DateCreated = DateTime.Now;

            _dataContext.Add(model);
            _dataContext.SaveChangesAsync();
            TempData["success"] = "Thêm sl thành công";
            return RedirectToAction("AddQuantity", "Product", new { Id = model.ProductId });

        }
        public async Task<IActionResult> Index(int pg = 1)
        {
            const int pageSize = 10; // Number of items per page

            // Ensure pg is at least 1
            if (pg < 1) pg = 1;

            // Get all products with their categories
            List<ProductModel> products = await _dataContext.Products
                                                             .Include(p => p.Category)
                                                             .OrderByDescending(p => p.Id)
                                                             .ToListAsync();

            // Total number of products
            int recsCount = products.Count;

            // Create pagination info
            var pager = new Paginate(recsCount, pg, pageSize);

            // Skip products based on the page number and page size
            int recSkip = (pg - 1) * pageSize;

            // Get the products for the current page
            var data = products.Skip(recSkip).Take(pager.PageSize).ToList();

            // Pass pagination info to the view
            ViewBag.Pager = pager;

            // Return the paginated view with the product data
            return View(data);
        }

        public async Task<IActionResult> Search(string search)
        {
            // Lấy danh sách sản phẩm từ database
            var products = from p in _dataContext.Products.Include(p => p.Category)
                           select p;

            // Nếu có từ khóa tìm kiếm, áp dụng bộ lọc tìm theo tên sản phẩm
            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.Name.ToLower().Contains(search.ToLower()));
                ViewData["SearchTerm"] = search; // Lưu từ khóa tìm kiếm để hiển thị lại
            }

            // Trả về View Index và danh sách sản phẩm
            return View("Index", await products.ToListAsync());
        }


    }
}