using System.ComponentModel.DataAnnotations;

namespace FreshShop.Models
{
    public class ProductQuantityModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "yêu cầu k ddc bỏ trống số lượng")]
        public int Quantity { get; set; }
       
        public int ProductId { get; set; }
        public DateTime DateCreated {  get; set; }

    }
}
