using ShopperUI.Models;

namespace ShopperUI.ViewModels
{
	public class CartViewModel
	{
		public int CartId { get; set; }

		public decimal? CartPrice { get; set; }

		public List<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();


	}
}
