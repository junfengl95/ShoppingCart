using AcmeCorpShopperUIWebApp.Models;
using System.Text.Json;

namespace AcmeCorpShopperUIWebApp.ApiClient
{
	public class AcmeCorpClient : IAcmeCorpClient
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IHttpContextAccessor _contextAccessor;

		public AcmeCorpClient(IHttpClientFactory httpClientFactory, IHttpContextAccessor contextAccessor)
		{
			_httpClientFactory = httpClientFactory;
			_contextAccessor = contextAccessor;
		}

		public async Task<List<Product>?> GetAllProductsAsync()
		{
			// Must match client name to that created in the program HTTP
			var client = _httpClientFactory.CreateClient("AcmeCorpProductApiClient");
			var productStreamTask = client.GetStreamAsync("/api/Products");

			return await JsonSerializer.DeserializeAsync<List<Product>>(await productStreamTask);
		}

		public async Task<Product?> GetProductByIdAsync(int id)
		{
			var client = _httpClientFactory.CreateClient("AcmeCorpProductApiClient");
			HttpResponseMessage response = await client.GetAsync($"api/Products/{id}");

			if (response.IsSuccessStatusCode)
			{
				var responseBody = await response.Content.ReadAsStreamAsync();
				return await JsonSerializer.DeserializeAsync<Product>(responseBody);
			}
			return null;
		}

		// Hardcoded endpoint version 1

		//public async Task<Cart?> GetCartByIdAsync(int cartId)
		//{
		//	var client = _httpClientFactory.CreateClient("AcmeCorpCartApiClient");

		//	string endpoint = "101";
		//	HttpResponseMessage response = await client.GetAsync($"api/Cart/{endpoint}");

		//	if (response.IsSuccessStatusCode)
		//	{
		//		var responseBody = await response.Content.ReadAsStreamAsync();
		//		return await JsonSerializer.DeserializeAsync<Cart>(responseBody);
		//	}

		//	return null;
		//}



		public async Task<Cart?> RetrieveCartAsync(int cartId)
		{
			try
			{
				var client = _httpClientFactory.CreateClient("AcmeCorpCartApiClient");
				HttpResponseMessage response = await client.GetAsync($"api/Cart/{cartId}");

				if (response.IsSuccessStatusCode)
				{
					var responseBody = await response.Content.ReadAsStreamAsync();
					return await JsonSerializer.DeserializeAsync<Cart>(responseBody);
				}
				else
				{
					// Log the unsuccessful response
					Console.WriteLine($"Failed to retrieve cart with ID {cartId}. Status code: {response.StatusCode}");

					// Return null or throw an exception, depending on your requirements
					return null;
				}
			}
			catch (Exception ex)
			{
				// Log the error
				Console.WriteLine($"Error occurred while retrieving cart with ID {cartId}: {ex.Message}");

				// Rethrow the exception or return null, depending on your requirements
				throw;
			}
		}

		public async Task<Cart> CreateNewCart()
		{
			try
			{
				var client = _httpClientFactory.CreateClient("AcmeCorpCartApiClient");
				var jsonCart = JsonSerializer.Serialize(new Cart(), typeof(Cart));
				StringContent content = new StringContent(jsonCart, System.Text.Encoding.UTF8, "application/json");
				HttpResponseMessage response = await client.PostAsync("api/Cart", content);

				if (response.IsSuccessStatusCode)
				{
					var responseBody = await response.Content.ReadAsStreamAsync();
					// Deserialize the response body into Cart object
					var createdCart = await JsonSerializer.DeserializeAsync<Cart>(responseBody);

					// Set the CartId to the cookie
					var cartId = createdCart.CartId.ToString();
					var cookieOptions = new CookieOptions
					{
						Expires = DateTimeOffset.UtcNow.AddMinutes(30), // Set expiration time as needed
						HttpOnly = true // Ensures the cookie is only accessible via HTTP(S) requests
					};
					_contextAccessor.HttpContext.Response.Cookies.Append("CartId", cartId, cookieOptions);

					return createdCart;
				}
				else
				{
					// Log the unsuccessful response
					Console.WriteLine($"Failed to create a new cart. Status code: {response.StatusCode}");

					// Return null or throw an exception, depending on your requirements
					return null;
				}
			}
			catch (Exception ex)
			{
				// Log the error
				Console.WriteLine($"Error occurred while creating a new cart: {ex.Message}");

				// Rethrow the exception or return null, depending on your requirements
				throw;
			}
		}

		public async Task<CartItem?> AddProductToCart(int cartId, int productId)
		{
			await Console.Out.WriteLineAsync($"cartId: {cartId}, productId: {productId}");

			var client = _httpClientFactory.CreateClient("AcmeCorpCartApiClient");
			var jsonCartItem = JsonSerializer.Serialize(new CartItem(), typeof(CartItem));
			StringContent content = new StringContent(jsonCartItem, System.Text.Encoding.UTF8, "application/json");
			HttpResponseMessage response = await client.PostAsync($"api/Cart/{cartId}/add-product/{productId}", content);
			var responseBody = await response.Content.ReadAsStreamAsync();

			//Derserialize the response body into CartItem object
			var createdCartItem = await JsonSerializer.DeserializeAsync<CartItem>(responseBody);

			return (createdCartItem);
		}



		public async Task<CartItem> AddProductToCartAsync(int cartId, int productId)
		{
			var client = _httpClientFactory.CreateClient("AcmeCorpCartApiClient");
			var jsonCartItem = JsonSerializer.Serialize(new CartItem(), typeof(CartItem));
			StringContent content = new StringContent(jsonCartItem, System.Text.Encoding.UTF8, "application/json");
			HttpResponseMessage response = await client.PostAsync($"api/Cart/{cartId}/add-product/{productId}", content);

			if (response.IsSuccessStatusCode)
			{
				var responseBody = await response.Content.ReadAsStreamAsync();
				var addedCartItem = await JsonSerializer.DeserializeAsync<CartItem>(responseBody);
				Console.WriteLine($"product: {productId} was added to cart: {cartId}");
				return addedCartItem;
			}
			else
			{
				return null;
			}
		}

		public async Task DeleteProductFromCartAsync(int cartId, int productId)
		{
			var client = _httpClientFactory.CreateClient("AcmeCorpCartApiClient");
			var response = await client.DeleteAsync($"api/Cart/{cartId}/remove-product/{productId}");

		}

		public async Task ClearCart(int cartId)
		{
			var client = _httpClientFactory.CreateClient("AcmeCorpCartApiClient");
			var response = await client.DeleteAsync($"api/Cart/clear-items/{cartId}");

		}

		public async Task<Order> CreateNewOrder(Order order)
		{
			var client = _httpClientFactory.CreateClient("AcmeCorpOrderApiClient");
			var jsonOrder = JsonSerializer.Serialize(order, typeof(Order));
			StringContent content = new StringContent(jsonOrder, System.Text.Encoding.UTF8, "application/json");
			HttpResponseMessage response = await client.PostAsync("api/Order", content);

			if (response.IsSuccessStatusCode)
			{
				var responseBody = await response.Content.ReadAsStreamAsync();
				//Deserialize the response body into Cart object
				var createdOrder = await JsonSerializer.DeserializeAsync<Order>(responseBody);

				await Console.Out.WriteLineAsync($"order id: {createdOrder.OrderId}, with customerName: {createdOrder.CustomerName} for cartID {createdOrder.CartId}");

				return createdOrder;
			}
			else
			{
				await Console.Out.WriteLineAsync($"Fail to create order");
				return null;
			}
		}

		// Hardcoded Order Endpoint works
		//public async Task<Order> CreateNewOrder(Order order)
		//{
		//	try
		//	{
		//		var client = _httpClientFactory.CreateClient("AcmeCorpOrderApiClient");

		//		// Hardcoded JSON string for testing
		//		string hardcodedJsonOrder = "{ \"customerName\": \"TestCustomer2\", \"cartId\": 116}";

		//		// Create StringContent from the hardcoded JSON
		//		StringContent content = new StringContent(hardcodedJsonOrder, System.Text.Encoding.UTF8, "application/json");

		//		HttpResponseMessage response = await client.PostAsync("api/Order", content);

		//		if (response.IsSuccessStatusCode)
		//		{
		//			var responseBody = await response.Content.ReadAsStringAsync();
		//			var createdOrder = JsonSerializer.Deserialize<Order>(responseBody);
		//			return createdOrder;
		//		}
		//		else
		//		{
		//			// Handle the case where order creation failed
		//			throw new Exception($"Failed to create order. Status code: {response.StatusCode}");
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		// Log or handle the exception
		//		Console.WriteLine($"An error occurred while creating the order: {ex.Message}");
		//		throw;
		//	}
		//}

		public async Task<Order?> GetOrderByIdAsync(int id)
		{
			var client = _httpClientFactory.CreateClient("AcmeCorpOrderApiClient");
			HttpResponseMessage response = await client.GetAsync($"/api/Order/{id}");
			if (response.IsSuccessStatusCode)
			{
				var responseBody = await response.Content.ReadAsStreamAsync();
				return await JsonSerializer.DeserializeAsync<Order>(responseBody);
			}

			return null;
		}

		public async Task<Cart?> GetCartById(int cartId)
		{
			var client = _httpClientFactory.CreateClient("AcmeCorpCartApiClient");
			HttpResponseMessage response = await client.GetAsync($"/api/Cart/{cartId}");
			if (response.IsSuccessStatusCode)
			{
				var responseBody = await response.Content.ReadAsStreamAsync();
				return await JsonSerializer.DeserializeAsync<Cart>(responseBody);
			}

			return null;
		}
	}
}
