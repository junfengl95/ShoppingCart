using ShopperUI.Models;

namespace ShopperUI.ApiClient
{
    public interface IShopperUIClient
    {
        Task<List<Product>?> GetAllProductsAsync();

        Task<Product?> GetProductByIdAsync(int productId);

        Task<Cart?> CreateNewCart();


        Task<CartItem?> AddProductToCartAsync(int cartId, int productId, int quantity);
        Task DeleteProductFromCartAsync(int cartId, int productId, int quantity);
        Task ClearCart(int cartId);

        Task<Cart?> GetCartById(int cartId);

        Task<List<Order>?> GetAllOrders(string customerId);

        Task<Order?> CreateNewOrder(int cartId, string userId, decimal? totalPrice);

        Task<Order?> GetOrderByIdAsync(int orderId);

    }
}
