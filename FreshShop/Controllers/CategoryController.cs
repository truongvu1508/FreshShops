using System.Globalization;
using FreshShop.Models;
using FreshShop.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreshShop.Controllers
{
    public class CategoryController : Controller
    {
        private readonly DataContext _dataContext;
        public CategoryController(DataContext context)
        {
            _dataContext = context;
        }
		public async Task<IActionResult> Index(string Slug = "", decimal? startprice = null, decimal? endprice = null, string sort_by = "")
		{
			if (string.IsNullOrEmpty(Slug))
			{
				return RedirectToAction("Index", "Home");
			}

			CategoryModel category = _dataContext.Categories.Where(c => c.Slug == Slug).FirstOrDefault();
			if (category == null) 
			{
				return RedirectToAction("Index", "Home");
			}

			var productsByCategory = _dataContext.Products
				.Include("Category")
				.Where(c => c.CategoryId == category.Id);

			if (startprice.HasValue && endprice.HasValue)
			{
				productsByCategory = productsByCategory.Where(p => p.Price >= startprice && p.Price <= endprice);
			}
			// Add sorting logic
			switch (sort_by)
			{
				case "price_desc":
					productsByCategory = productsByCategory.OrderByDescending(p => p.Price);
					break;
				case "price_inc":
					productsByCategory = productsByCategory.OrderBy(p => p.Price);
					break;
				default:
					productsByCategory = productsByCategory.OrderByDescending(p => p.Id);
					break;
			}
			ViewBag.CurrentSlug = Slug;
			return View(await productsByCategory.ToListAsync());
		}
	}
}
