using Microsoft.AspNetCore.Mvc;
using FreshShop.Models;
using FreshShop.Models.ViewModels;
using FreshShop.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FreshShop.Areas.Identity.Models.AccountViewModels;
using Microsoft.EntityFrameworkCore;

namespace FreshShop.Controllers
{
    public class HistoryController : Controller
    {
        private UserManager<AppUserModel> _userManage;
        private SignInManager<AppUserModel> _signInManager;
        private readonly IEmailSender _emailSender;
        private DataContext _dataContext;
        public HistoryController(
           IEmailSender emailSender,
           UserManager<AppUserModel> userManage,
           SignInManager<AppUserModel> signInManager,
           DataContext context)
        {
            _dataContext = context;
            _userManage = userManage;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }
        public async Task<IActionResult> History()
        { 
            if((bool)!User.Identity?.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");

            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var Orders = await _dataContext.Orders
                .Where(od => od.UserName == userEmail).OrderByDescending(od => od.Id).ToListAsync();
            ViewBag.UserEmail = userEmail;
            return View(Orders);
        }

    }
}

