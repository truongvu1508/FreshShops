using FreshShop.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreshShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        public ProductController(DataContext context)
        {
            _dataContext = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Details(int Id )
        {
            if (Id == null) return RedirectToAction("Index");
            var productsById = _dataContext.Products.Where(p => p.Id == Id).FirstOrDefault();
            // sản phẩm liên quan - liên hệ thằng đình hoàng nếu có vấn đề
            var relatedProducts = await _dataContext.Products
                .Where(p => p.CategoryId == productsById.CategoryId && p.Id != productsById.Id)
                .Take(4)
                .ToListAsync();
            ViewBag.RelatedProducts = relatedProducts;
            return View(productsById);
        }
    }
}
