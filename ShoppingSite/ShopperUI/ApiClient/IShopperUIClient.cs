﻿using ShopperUI.Models;

namespace ShopperUI.ApiClient
{
    public interface IShopperUIClient
    {
        Task<List<Product>?> GetAllProductsAsync();

        Task<Product?> GetProductByIdAsync(int productId);

        Task<Cart?> CreateNewCart();


        Task<CartItem?> AddProductToCartAsync(int cartId, int productId);
        Task DeleteProductFromCartAsync(int cartId, int productId);
        Task ClearCart(int cartId);

        Task<Cart?> GetCartById(int cartId);

        Task<List<Order>?> GetAllOrders(string customerId);

        Task<Order?> CreateNewOrder(Order order);

        //Task<Order?> GetOrderByIdAsync(int orderId);

    }
}