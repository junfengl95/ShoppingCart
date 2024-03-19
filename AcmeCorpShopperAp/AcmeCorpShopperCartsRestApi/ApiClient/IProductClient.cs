

using Microsoft.AspNetCore.Mvc;

namespace AcmeCorpShopperCartsRestApi.ApiClient
{
	public interface IProductClient
	{
		Task<bool> CheckProductExistence(int productId);
		Task<decimal> GetTotalProductPrice(List<int> productIds);
	}
}
