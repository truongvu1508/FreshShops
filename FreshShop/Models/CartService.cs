namespace FreshShop.Models
{
	public interface ICartService
	{
		List<CartItemModel> AddToCart(int productId);
	}

	public class CartService : ICartService
	{
		private readonly ISession _session;

		public CartService(IHttpContextAccessor httpContextAccessor)
		{
			_session = httpContextAccessor.HttpContext.Session;
		}

		public List<CartItemModel> AddToCart(int productId)
		{
			// Lấy giỏ hàng từ session
			var cart = _session.GetObjectFromJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

			// Thêm sản phẩm vào giỏ
			var product = GetProductById(productId); // Tìm sản phẩm theo ID
			var cartItem = cart.FirstOrDefault(x => x.ProductId == productId);

			if (cartItem != null)
			{
				cartItem.Quality++;
			}
			else
			{
				cart.Add(new CartItemModel { ProductId = productId, Quality = 1, Price = product.Price, ProductName = product.Name });
			}

			// Cập nhật giỏ hàng trong session
			_session.SetObjectAsJson("Cart", cart);

			return cart;
		}

		private ProductModel GetProductById(int productId)
		{
			// Hàm tìm sản phẩm trong database (ví dụ)
			return new ProductModel { Id = productId, Price = 10000, Name = "Example Product" }; // Ví dụ
		}
	}
}
