using AcmeCorpShopperUIWebApp.Models;

namespace AcmeCorpShopperUIWebApp.ApiClient
{
	public interface IAcmeCorpClient
	{
        
        Task<List<Product>?> GetAllProductsAsync();

        Task<Product?> GetProductByIdAsync(int productId);

		Task<Cart?> RetrieveCartAsync(int cartId);

		Task<Cart> CreateNewCart();

		Task<Order> CreateNewOrder(Order order);

        Task<CartItem> AddProductToCartAsync(int cartId, int productId);
        Task DeleteProductFromCartAsync(int cartId, int productId);
        Task ClearCart(int cartId);

        Task<Cart> GetCartById(int cartId);
		Task<Order?> GetOrderByIdAsync(int orderId);
	}
}
