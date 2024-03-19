using System.Text.Json.Serialization;

namespace AcmeCorpShopperCartsRestApi.Models
{
	public class Product
	{
		// Need to adjust the name to match in Service class
		[JsonPropertyName("productId")]
		public int ProductId { get; set; }

		[JsonPropertyName("productName")]
		public string ProductName { get; set; } = null!;

		[JsonPropertyName("productPrice")]
		public decimal ProductPrice { get; set; }

		public override string ToString()
		{
			return $"Product [ productId={ProductId}, name={ProductName}, price = {ProductPrice.ToString()} ]";
		}
	}
}
