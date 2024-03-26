namespace AcmeCorpShopperOrdersRestApi.ApiClient
{
    public interface ICartClient
    {
        Task<bool> CheckCartExistence(int productId);
    }
}
