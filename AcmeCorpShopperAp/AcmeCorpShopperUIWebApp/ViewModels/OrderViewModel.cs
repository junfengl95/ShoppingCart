namespace AcmeCorpShopperUIWebApp.ViewModels
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public int CartId { get; set; }
        public decimal TotalPrice { get; set; }
        public IEnumerable<CartItemViewModel> CartItems { get; set; }
    }

    public class CartItemViewModel
    {
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
    }
}
