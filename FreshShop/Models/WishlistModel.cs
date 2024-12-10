using System.ComponentModel.DataAnnotations.Schema;

namespace FreshShop.Models
{
    public class WishlistModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; }

        [ForeignKey("ProductId")]
        public ProductModel Product { get; set; }
    }
}
