using FreshShop.Models;
using Microsoft.EntityFrameworkCore;

namespace FreshShop.Repository
{
    public class SeedData
    {
        public static void SeedingData(DataContext _context)
        {
            //_context.Database.Migrate();
            //if (!_context.Products.Any())
            //{
            //    CategoryModel fruit = new CategoryModel
            //    {
            //        Name = "Fruit",
            //        Slug = "fruit",
            //        Description = "Trai cay",
            //        Status = 1
            //    };

            //    CategoryModel vegetable = new CategoryModel
            //    {
            //        Name = "Vegetable",
            //        Slug = "vegetable",
            //        Description = "Rau cu",
            //        Status = 1
            //    };
            //    _context.Products.AddRange(
            //        new ProductModel { Name = "Banana", Slug = "banana", Description = "Trai chuoi", Image = "1.jpg", Category = fruit , Price=50000},
            //        new ProductModel { Name = "Ca rot", Slug = "carot", Description = "Cu ca rot", Image = "1.jpg", Category = vegetable, Price = 50000 }
            //    );
            //    _context.SaveChanges();
            //}
        }
    }
}
