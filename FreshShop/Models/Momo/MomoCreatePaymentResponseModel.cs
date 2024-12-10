namespace FreshShop.Models.Momo
{
    public class MomoCreatePaymentResponseModel
    {
        public String  RequestId { get; set; }
        public int ErrorCode { get; set; }
        public String OrderId { get; set; }
        public String Message { get; set; }
        public String LocalMessage { get; set; }
        public String RequestType { get; set; }
        public String PayUrl { get; set; }
        public String Signature { get; set; }
        public String QrCodeUrl { get; set; }
        public String Deeplink { get; set; }
        public String DeeplinkWebInApp { get; set; }

    }
}
