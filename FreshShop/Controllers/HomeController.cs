using FreshShop.Models;
using FreshShop.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FreshShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUserModel> _userManager;

        public HomeController(ILogger<HomeController> logger, DataContext context, UserManager<AppUserModel> userManager)
        {
            _logger = logger;
            _dataContext = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var products = _dataContext.Products.Include("Category").ToList();
            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> AddWishlist(int Id, WishlistModel wishlist)
        {
            var user = await _userManager.GetUserAsync(User);
            wishlist.ProductId = Id;
            wishlist.UserId = user.Id;

            // Kiểm tra xem sản phẩm đã có trong danh sách yêu thích của người dùng chưa
            var existingWishlist = await _dataContext.Wishlists
                .FirstOrDefaultAsync(w => w.ProductId == Id && w.UserId == user.Id);

            if (existingWishlist != null)
            {
                // Nếu đã có sản phẩm trong danh sách yêu thích, trả về thông báo
                return Ok(new { success = false, message = "Sản phẩm đã có trong danh sách yêu thích." });
            }

            // Nếu chưa có, thêm vào danh sách yêu thích
            _dataContext.Add(wishlist);
            try
            {
                await _dataContext.SaveChangesAsync();
                return Ok(new { success = true, message = "Thêm vào danh sách yêu thích thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        public async Task<IActionResult> AddCompare(int Id, CompareModel compare)
        {
            var user = await _userManager.GetUserAsync(User);
            compare.ProductId = Id;
            compare.UserId = user.Id;

            // Kiểm tra xem sản phẩm đã có trong danh sách so sánh của người dùng chưa
            var existingCompare = await _dataContext.Compares
                .FirstOrDefaultAsync(c => c.ProductId == Id && c.UserId == user.Id);

            if (existingCompare != null)
            {
                // Nếu đã có sản phẩm trong danh sách so sánh, trả về thông báo
                return Ok(new { success = false, message = "Sản phẩm đã có trong danh sách so sánh." });
            }

            // Nếu chưa có, thêm vào danh sách so sánh
            _dataContext.Add(compare);
            try
            {
                await _dataContext.SaveChangesAsync();
                return Ok(new { success = true, message = "Thêm vào danh sách so sánh thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        public async Task<IActionResult> Compare()
        {
            var compare_product = await (from c in _dataContext.Compares
                                         join p in _dataContext.Products on c.ProductId equals p.Id
                                         join u in _dataContext.Users on c.UserId equals u.Id
                                         select new { User = u, Product = p, Compares = c }).ToListAsync();
            return View(compare_product);
        }

        public async Task<IActionResult> Wishlist()
        {
            var wishlist_product = await (from w in _dataContext.Wishlists
                                         join p in _dataContext.Products on w.ProductId equals p.Id
                                         join u in _dataContext.Users on w.UserId equals u.Id
                                         select new { User = u, Product = p, Wishlists=w }).ToListAsync();
            return View(wishlist_product);
        }

        public async Task<IActionResult> DeleteCompare(int Id)
        {
            CompareModel compare = await _dataContext.Compares.FirstAsync(c => c.Id == Id);
            _dataContext.Compares.Remove(compare);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction("Compare","Home");
        }

        public async Task<IActionResult> DeleteWishlist(int Id)
        {
            WishlistModel wishlist = await _dataContext.Wishlists.FirstAsync(w => w.Id == Id);
            _dataContext.Wishlists.Remove(wishlist);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction("Wishlist", "Home");
        }
    }
}
