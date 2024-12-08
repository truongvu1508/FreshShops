using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FreshShop.Models;
using Microsoft.AspNetCore.Authorization;
using FreshShop.Data;
namespace FreshShop.Controllers
{
    [Authorize(Roles = RoleName.Administrator)]
    public class AdminHomeController : Controller
    {
        private readonly ILogger<AdminHomeController> _logger;

        public AdminHomeController(ILogger<AdminHomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

