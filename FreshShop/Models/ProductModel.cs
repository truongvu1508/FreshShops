using FreshShop.Repository.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreshShop.Models
{
    public class ProductModel
    {
        [Key]
        public int Id { get; set; }
        [Required, MinLength(4, ErrorMessage = "Yêu cầu nhập tên sản phẩm")]
        public string Name { get; set; }
        public string Slug { get; set; }
        //Slug chuyen doi tu Name
        [Required, MinLength(4, ErrorMessage = "Yêu cầu nhập mô tả sản phẩm")]
        public string Description { get; set; }

        [Required( ErrorMessage = "Yêu cầu nhập giá vốn sản phẩm")]
        public decimal CapitalPrice{ get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public CategoryModel Category { get; set; }
        public string Image { get; set; }

        public int Quantity { get; set; }

        public int Sold { get; set; }

        [NotMapped]
        [FileExtension]
        public IFormFile? ImageUpload { get; set; }
    }
}
