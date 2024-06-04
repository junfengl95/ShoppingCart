namespace ShopperUI.ViewModels
{
	public class CartItemViewModel
	{
		public int CartItemId { get; set; }

		public int ProductId { get; set; }

		public string ProductName { get; set; } = string.Empty;

		public string ProductImage {  get; set; } = string.Empty ;

		public int Quantity { get; set; }

		public decimal ProductPrice { get; set; }

		public int FKCartId { get; set; }
	}
}
