using Microsoft.AspNetCore.Mvc;

namespace FreshShop.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
