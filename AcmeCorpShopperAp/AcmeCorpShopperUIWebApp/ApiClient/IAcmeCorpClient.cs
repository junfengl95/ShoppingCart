using AcmeCorpShopperUIWebApp.Models;

namespace AcmeCorpShopperUIWebApp.ApiClient
{
	public interface IAcmeCorpClient
	{
        
        Task<List<Product>?> GetAllProductsAsync();

        Task<Product?> GetProductByIdAsync(int productId);
        Task<Cart> CreateNewCart();

        Task<Cart?> RetrieveCartAsync(int cartId);

        Task<CartItem> AddProductToCartAsync(int cartId, int productId);
        Task DeleteProductFromCartAsync(int cartId, int productId);
        Task ClearCart(int cartId);
    }
}
