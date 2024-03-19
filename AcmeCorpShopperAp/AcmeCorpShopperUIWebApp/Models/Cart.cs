using System.Text.Json.Serialization;

namespace AcmeCorpShopperUIWebApp.Models
{
	public class Cart
	{
		[JsonPropertyName("cartId")]
		public int CartId { get; set; }

		[JsonPropertyName("cartPrice")]
		public decimal? CartPrice { get; set; } = 0; //Setting default value to zero so no need to preset when creating

		[JsonPropertyName("cartItems")]
		public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
	}
}
