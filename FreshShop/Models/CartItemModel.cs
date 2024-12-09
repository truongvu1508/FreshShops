using System.Drawing;

namespace FreshShop.Models
{
    public class CartItemModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quality { get; set; }
        public decimal Price { get; set; }
        public decimal Total {            
            get { return Quality*Price; }
        }
        public string Image {  get; set; }
        public CartItemModel() 
        {
            
        }
        public CartItemModel(ProductModel product)
        {
            ProductId = product.Id;
            ProductName = product.Name;
            Price = product.Price;
            Quality = 1;
            Image = product.Image;
        }
    }
}
