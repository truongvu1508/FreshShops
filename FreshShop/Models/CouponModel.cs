using System.ComponentModel.DataAnnotations;

namespace FreshShop.Models
{
    public class CouponModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Yêu cầu không được bỏ trống tên Coupon")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Yêu cầu không được bỏ trống mô tả Coupon")]
        public string Description { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateExpired { get; set; }

        [Required(ErrorMessage = "Yêu cầu không được bỏ trống số lượng Coupon")]
        public int Quantity { get; set; }
        [Required(ErrorMessage = "Yêu cầu không được bỏ trống giá trị giảm giá")]
        [Range(1, 100, ErrorMessage = "Giá trị giảm giá phải từ 1% đến 100%")]
        public int CouponValue { get; set; }
        public int Status { get; set; }
    }
}
