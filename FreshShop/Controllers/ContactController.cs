using Microsoft.AspNetCore.Mvc;

namespace FreshShop.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
