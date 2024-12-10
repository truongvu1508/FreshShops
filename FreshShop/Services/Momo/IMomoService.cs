using FreshShop.Models.Momo;
using FreshShop.Models;
namespace FreshShop.Services.Momo
{
    public interface IMomoService
    {
        Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(OrderInfoModel model);
        MomoExcuteResponseModel PaymentExecuteAsync(IQueryCollection collection);
    }
}
