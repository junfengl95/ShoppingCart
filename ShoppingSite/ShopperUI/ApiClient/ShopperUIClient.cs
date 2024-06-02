using Microsoft.AspNetCore.Identity;
using ShopperUI.Areas.Identity.Data;
using ShopperUI.Models;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace ShopperUI.ApiClient
{
    public class ShopperUIClient : IShopperUIClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
		private readonly UserManager<ShopperUIUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

		public ShopperUIClient(IHttpClientFactory httpClientFactory, UserManager<ShopperUIUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

		public async Task<Cart?> CreateNewCart()
        {
            var client = _httpClientFactory.CreateClient("CartApi");
            var jsonCart = JsonSerializer.Serialize(new Cart(), typeof(Cart));
            StringContent content = new StringContent(jsonCart, System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("api/Cart", content);

            if (response.IsSuccessStatusCode)
            {
                var responseBody =  await response.Content.ReadAsStreamAsync();

                var createdCart = await JsonSerializer.DeserializeAsync<Cart>(responseBody);

                if (createdCart != null)
                {
					Console.WriteLine($"New cart created with cartId: {createdCart.CartId}");

					return createdCart;
				}
                else
                {
                    return null;
                }
			}
            else
            {
                Console.WriteLine($"Failed to create a new cart. Status code: {response.StatusCode}");

                // Return null or throw an exception, depending on your requirements
                return null;
            }
        }

        public async Task<Cart?> GetCartById(int cartId)
        {
            var client = _httpClientFactory.CreateClient("CartApi");
            HttpResponseMessage response = await client.GetAsync($"/api/Cart/{cartId}");

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var responseBody = await response.Content.ReadAsStreamAsync();
                    return await JsonSerializer.DeserializeAsync<Cart>(responseBody);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to deserialize cart with ID {cartId}. Exception: {ex}");
                    return null;
                }
            }
            else
            {
                Console.WriteLine($"Failed to retrieve cart with ID {cartId}. Status code: {response.StatusCode}");
                return null;
            }
        }

        public async Task<List<Product>?> GetAllProductsAsync()
        {
            //Call ProductApiClient
            var client = _httpClientFactory.CreateClient("ProductApi");

            // Retrieve the current logged-in user
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            if (user != null)
            {
                // Retrieve the authentication token for the user
                var accessToken = await _userManager.GetAuthenticationTokenAsync(user, "Bearer", "access_token");

                if (!string.IsNullOrEmpty(accessToken))
                {
                    // Include the authentication token in the request headers
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                }
            }

            var products = client.GetStreamAsync("/api/Product");
            return await JsonSerializer.DeserializeAsync<List<Product>>(await products);
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("ProductApi");
            HttpResponseMessage response = await client.GetAsync($"/api/Product/{id}");

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<Product>(responseBody);
            }

            return null;
        }

        public async Task<CartItem?> AddProductToCartAsync(int cartId, int productId, int quantity)
        {
            var client = _httpClientFactory.CreateClient("CartApi");
            var jsonCartItem = JsonSerializer.Serialize(new CartItem(), typeof(CartItem));
            StringContent content = new StringContent(jsonCartItem, System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync($"/api/Cart/{cartId}/add-product/{productId}/{quantity}", content);
            var responseBody = await response.Content.ReadAsStreamAsync();

            //Deserialize the response body into Cart Item
            var createdCartItem = await JsonSerializer.DeserializeAsync<CartItem>(responseBody);

            return createdCartItem;
        }

        public async Task DeleteProductFromCartAsync(int cartId, int productId, int quantity)
        {
            var client = _httpClientFactory.CreateClient("CartApi");
            var response = await client.DeleteAsync($"/api/Cart/{cartId}/remove-product/{productId}/{quantity}");

            if (!response.IsSuccessStatusCode)
            {
                // Handle the case where the request failed
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to delete product from cart. Status code: {response.StatusCode}. Error: {errorMessage}");
            }
            else
            {
                // Handle the case where the request succeeded
                // You may log or handle the response depending on your requirements
                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Product removed from cart. Response: {responseBody}");
            }
        }

        public async Task ClearCart(int cartId)
        {
            var client = _httpClientFactory.CreateClient("CartApi");
            var response = await client.DeleteAsync($"/api/Cart/clear-items/{cartId}");

            if (!response.IsSuccessStatusCode)
            {
                // Handle the case where the request failed
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to clear product from cart. Status code: {response.StatusCode}. Error: {errorMessage}");
            }
            else
            {
                // Handle the case where the request succeeded
                // You may log or handle the response depending on your requirements
                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Cart cleared Response: {responseBody}");
            }
        }

        public async Task<List<Order>?> GetAllOrders(string customerId)
        {

			//Call ProductApiClient
			var client = _httpClientFactory.CreateClient("OrdersApi");
			var orders = client.GetStreamAsync($"/api/Order/user/{customerId}");

            if (object.ReferenceEquals(orders, null))
            {
                return null;
            }

			return await JsonSerializer.DeserializeAsync<List<Order>>(await orders);
		}


		public async Task<Order?> CreateNewOrder(int cartId, string userId, decimal? totalPrice)
		{
			try
			{
				var client = _httpClientFactory.CreateClient("OrdersApi");

				var order = new Order
				{
					DateOfCreation = DateTime.Now,
                    CartId = cartId,
                    TotalPrice = totalPrice,
                    CustomerId = userId
				};

				Console.WriteLine($"time of creation: {order.DateOfCreation}");

				var jsonOrder = JsonSerializer.Serialize(order, typeof(Order));
				StringContent content = new StringContent(jsonOrder, System.Text.Encoding.UTF8, "application/json");
				HttpResponseMessage response = await client.PostAsync("/api/Order", content);

				if (response.IsSuccessStatusCode)
				{
					var responseBody = await response.Content.ReadAsStreamAsync();
					var createdOrder = await JsonSerializer.DeserializeAsync<Order>(responseBody);
					return createdOrder;
				}
				else
				{
					Console.WriteLine($"Failed to create order: {response.StatusCode}");
					return null;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Exception occurred: {ex}");
				return null;
			}
		}

		public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            var client = _httpClientFactory.CreateClient("OrdersApi");
            HttpResponseMessage response = await client.GetAsync($"/api/Order/{orderId}"); 
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<Order>(responseBody);
            }

            return null;
        }
	}
}
