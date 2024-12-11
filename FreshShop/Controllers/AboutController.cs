using Microsoft.AspNetCore.Mvc;

namespace FreshShop.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
