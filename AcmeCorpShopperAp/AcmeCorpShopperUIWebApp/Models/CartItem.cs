using System.Text.Json.Serialization;

namespace AcmeCorpShopperUIWebApp.Models
{
	public class CartItem
	{
		[JsonPropertyName("cartItemId")]
		public int CartItemId { get; set; }

		[JsonPropertyName("productId")]
		public int ProductId { get; set; }

		[JsonPropertyName("fkCartId")]
		public int FkCartId { get; set; }

		[JsonPropertyName("product")]
		public Product? Product { get; set; }

		[JsonIgnore]
		public virtual Cart FkCart { get; set; } = null!;
	}
}
