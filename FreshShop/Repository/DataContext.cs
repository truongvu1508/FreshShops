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
        public DbSet<ProductQuantityModel> ProductQuantities { get; set; }
        public DbSet<SliderModel> Sliders { get; set; }
        public DbSet<CompareModel> Compares { get; set; }
        public DbSet<WishlistModel> Wishlists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình mối quan hệ giữa OrderDetails và Product (Many-to-One)
            modelBuilder.Entity<OrderDetails>()
                .HasOne(od => od.Product)
                .WithMany() // Một Product có thể có nhiều OrderDetails
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.Restrict); // Không xóa tự động khi Product bị xóa

            // Cấu hình mối quan hệ giữa Order và OrderDetails (One-to-Many)
            modelBuilder.Entity<OrderModel>()
                .HasMany(o => o.OrderDetails)
                .WithOne(od => od.Order) // Liên kết Order với OrderDetails
                .HasForeignKey(od => od.OrderId);
        }
    }
}
