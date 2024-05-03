using System.Text.Json.Serialization;

namespace ShopperUI.Models
{
	public class Order
	{
		[JsonPropertyName("orderId")]
		public int OrderId { get; set; }

		[JsonPropertyName("dateOfCreation")]
		public DateTime DateOfCreation { get; set; }

		[JsonPropertyName("totalPrice")]
		public decimal? TotalPrice { get; set; }

		[JsonPropertyName("cartId")]
		public int CartId { get; set; }

		[JsonPropertyName("customerId")]
		public string CustomerId { get; set; }
	}
}
