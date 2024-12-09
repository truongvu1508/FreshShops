using FreshShop.Models;
using FreshShop.Services.Momo;
using Microsoft.AspNetCore.Mvc;

namespace FreshShop.Controllers
{
    public class PaymentController : Controller
    {
        private IMomoService _momoService;
        
        public PaymentController(IMomoService momoService)
        {
            _momoService = momoService;

        }
        [HttpPost]
        [Route("CreatePaymentUrl")]
        public async Task<IActionResult> CreatePaymentUrl(OrderInfoModel model)
        {
            var response = await _momoService.CreatePaymentAsync(model);
            return Redirect(response.PayUrl);
        }

        [HttpGet]
        public IActionResult PaymentCallBack(){
            var response = _momoService.PaymentExecuteAsync(HttpContext.Request.Query);
            return View(response);
        }

    }
}