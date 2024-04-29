using System.Text.Json.Serialization;

namespace CartApi.Models
{
	public class Product
	{
		[JsonPropertyName("productId")]
		public int ProductId { get; set; }

		[JsonPropertyName("productName")]
		public string ProductName { get; set; } = null!;

		[JsonPropertyName("productPrice")]
		public decimal ProductPrice { get; set; }

		[JsonPropertyName("productQuantity")]
		public int ProductQuantity { get; set; }

		[JsonPropertyName("productRating")]
		public decimal? ProductRating { get; set; }

		public override string ToString()
		{
			return $"productId: {ProductId}, productName: {ProductName}, productPrice: {ProductPrice}, Quantity: {ProductQuantity}, Rating: {ProductRating} ";
		}


	}
}
