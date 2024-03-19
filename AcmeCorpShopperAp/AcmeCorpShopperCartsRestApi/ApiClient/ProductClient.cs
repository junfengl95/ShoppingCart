
using AcmeCorpShopperCartsRestApi.Models;
using System.Text.Json;

namespace AcmeCorpShopperCartsRestApi.ApiClient
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
			// check if the product exist 
			var client = _httpClientFactory.CreateClient("AcmeCorpProductApiClient");
			HttpResponseMessage response = await client.GetAsync($"/api/Products/{productId}");

			if(response.IsSuccessStatusCode) // return true if product exist
			{
				var responseBody = await response.Content.ReadAsStreamAsync();
				var product = await JsonSerializer.DeserializeAsync<Product>(responseBody);
                return true;
			}
			return false;
		}

		// For getting one product price

		//public async Task<decimal?> GetProductPrice(int productId)
		//{
		//	// check if product exist 
		//	var client = _httpClientFactory.CreateClient("AcmeCorpProductApiClient");
		//	HttpResponseMessage response = await client.GetAsync($"/api/Products/{productId}");

		//	if (response.IsSuccessStatusCode)
		//	{
		//		var responseBody = await response.Content.ReadAsStreamAsync();
		//		var product = await JsonSerializer.DeserializeAsync<Product>(responseBody);
		//		if (object.Equals(product, null))
		//		{
		//			return 0;
		//		}
		//		return product.ProductPrice;
		//	}
		//	return 0;
		//}

		public async Task<decimal> GetTotalProductPrice(List<int> productIds)
		{
			var client = _httpClientFactory.CreateClient("AcmeCorpProductApiClient");
			// Initialize totalPrice = 0;
			decimal totalPrice = 0;

			// Check the list of productIds available
			foreach (var productId in productIds)
			{
				HttpResponseMessage response = await client.GetAsync($"/api/Products/{productId}");
				
				// Initialize the product price
				//decimal productPrice = 0;

				if (response.IsSuccessStatusCode) // if url success
				{
					var responseBody = await response.Content.ReadAsStreamAsync();
					var product = await JsonSerializer.DeserializeAsync<Product>(responseBody);

					if (object.Equals(product, null)) // If price not set or product is removed
					{
						continue; // Skip to next product
					}

					var productPrice = product.ProductPrice;
                    //await Console.Out.WriteLineAsync($"product name: {product.ProductName}, product price: {product.ProductPrice}");
                    totalPrice += productPrice;
				}
			}
			return totalPrice;
		}
		
	}
}
