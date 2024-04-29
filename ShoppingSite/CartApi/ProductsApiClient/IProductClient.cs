namespace CartApi.ProductsApiClient
{
	public interface IProductClient
	{
		Task<bool> CheckProductExistence(int productId);
		Task<decimal> GetTotalProductPrice(List<int> productIds);
	}
}
