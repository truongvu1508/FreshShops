namespace FreshShop.Models.ViewModels
{
    public class CartItemViewModel
    {
        public List<CartItemModel> CartItems { get; set; }
        //Tong tien
        public decimal GrandTotal { get; set; }
        public decimal ShippingCost { get; set; }
        public int CouponValue { get; set; }
    }
}
