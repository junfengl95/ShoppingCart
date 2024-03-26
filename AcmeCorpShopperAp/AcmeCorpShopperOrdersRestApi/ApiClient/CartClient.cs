
using AcmeCorpShopperOrdersRestApi.Models;
using System.Text.Json;

namespace AcmeCorpShopperOrdersRestApi.ApiClient
{
    public class CartClient : ICartClient
    {

        private readonly IHttpClientFactory _httpClientFactory;

        public CartClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<bool> CheckCartExistence(int cartId)
        {
            var client = _httpClientFactory.CreateClient("AcmeCorpCartApiClient");
            HttpResponseMessage response = await client.GetAsync($"/api/Cart/{cartId}");

            if (response.IsSuccessStatusCode) // return true if product exist
            {
                var responseBody = await response.Content.ReadAsStreamAsync();
                var order = await JsonSerializer.DeserializeAsync<Order>(responseBody);
                return true;
            }
            return false;
        }
    }
}
