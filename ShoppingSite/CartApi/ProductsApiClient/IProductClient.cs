namespace CartApi.ProductsApiClient
{
	public interface IProductClient
	{
		Task<bool> CheckProductExistence(int productId);
		Task<decimal> GetTotalProductPrice(Dictionary<int, int> productQuantities);

		Task<bool> UpdateProductQuantity(int productId, int quantity);
	}
}
