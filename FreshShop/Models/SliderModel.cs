using FreshShop.Repository.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreshShop.Models
{
    public class SliderModel
    {
        public int Id {  get; set; }
        [Required(ErrorMessage = "yêu cầu không được bỏ trống tên")]
        public string Name { get; set; }

        public string Description {  get; set; }
        public int ? Status { get; set; }

        public string Image {  get; set; }

        [NotMapped]
        [FileExtension]
        public IFormFile? ImageUpload { get; set; }
    }
}
