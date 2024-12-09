namespace FreshShop.Models
{
    public class CompareModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; }
        public ProductModel Product { get; set; }
    }
}
