using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreshShop.Models
{
    public class OrderDetails
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string OrderCode { get; set; }

        public int ProductId { get; set; }  // Khóa ngoại tham chiếu tới bảng Product

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        // Mối quan hệ giữa OrderDetails và ProductModel
        [ForeignKey("ProductId")]
        public ProductModel Product { get; set; }

        //public int OrderId { get; set; } // Khóa ngoại trỏ tới Order
        //public OrderModel Order { get; set; } // Mối quan hệ với Order

        //// Tính thành tiền cho đơn hàng
        //[NotMapped]
        //public decimal TotalPrice => Quantity * Price;
    }
}
