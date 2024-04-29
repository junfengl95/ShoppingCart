
using CartApi.Models;
using System.Text.Json;

namespace CartApi.ProductsApiClient
{
	public class ProductClient : IProductClient
	{
		private readonly IHttpClientFactory _httpClientFactory;

		public ProductClient(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}

		public async Task<bool> CheckProductExistence(int productId)
		{
			var client = _httpClientFactory.CreateClient("ProductsApiClient");
			HttpResponseMessage response = await client.GetAsync($"/api/Product/{productId}");

			if (response.IsSuccessStatusCode)
			{
				var responseBody = await response.Content.ReadAsStreamAsync();
				var product = await JsonSerializer.DeserializeAsync<Product>(responseBody);
				return true;
			}
			return false;
		}

		public async Task<decimal> GetTotalProductPrice(List<int> productIds)
		{
			var client = _httpClientFactory.CreateClient("ProductsApiClient");
			// initialize totalPrice = 0
			decimal totalPrice = 0;

			foreach(var productId in productIds )
			{
				HttpResponseMessage response = await client.GetAsync($"/api/Product/{productId}");

				if (response.IsSuccessStatusCode)
				{
					var responseBody = await response.Content.ReadAsStreamAsync();
					var product = await JsonSerializer.DeserializeAsync<Product>(responseBody);

					if (object.Equals(product, null))
					{
						continue;
					}

					var productPrice = product.ProductPrice;

					await Console.Out.WriteLineAsync($"product name: {product.ProductName}, product price: {product.ProductPrice}");

					totalPrice += productPrice;
				}
			}
			return totalPrice;
		}
	}
}
