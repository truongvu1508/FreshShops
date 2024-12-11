using FreshShop.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FreshShop.Repository
{
    public class DataContext : IdentityDbContext<AppUserModel>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<ProductModel> Products { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<OrderModel> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<CouponModel> Coupons { get; set; }
        public DbSet<ShippingModel> Shipping { get; set; }
        public DbSet<Statistical> Statistical { get; set; }
        public DbSet<ProductQuantityModel> ProductQuantities { get; set; }
        public DbSet<SliderModel> Sliders { get; set; }
        public DbSet<CompareModel> Compares { get; set; }
        public DbSet<WishlistModel> Wishlists { get; set; }
        public DbSet<MomoInfoModel> MomoInfoModels { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
