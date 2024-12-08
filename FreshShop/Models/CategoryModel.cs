using System.ComponentModel.DataAnnotations;

namespace FreshShop.Models
{
    public class CategoryModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập tên danh mục")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập mô tả danh mục")]
        public string Description { get; set; }
        public string Slug { get; set; }
        //Slug chuyen doi tu Name
        public int Status { get; set; }
    }
}
