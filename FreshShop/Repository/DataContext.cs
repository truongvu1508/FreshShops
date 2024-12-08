using FreshShop.Models;
using Microsoft.EntityFrameworkCore;

namespace FreshShop.Repository
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }

        public DbSet<ProductQuantityModel> ProductQuantities { get; set; }
        public DbSet<SliderModel> Sliders { get; set; }
    }
}
