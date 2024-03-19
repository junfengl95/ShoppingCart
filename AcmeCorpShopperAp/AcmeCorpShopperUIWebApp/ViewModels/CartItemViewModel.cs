namespace AcmeCorpShopperUIWebApp.ModelView
{
    public class CartItemViewModel
    {
        public int CartId { get; set; }
        public string CartName { get; set; }
        public decimal? CartPrice { get; set; }
        public List<CartItemDetails> CartItems { get; set; }
    }

    public class CartItemDetails
    {
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal? ProductPrice { get; set; }
    }
}
