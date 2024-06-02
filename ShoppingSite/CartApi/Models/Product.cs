using System.ComponentModel.DataAnnotations.Schema;
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

        [JsonPropertyName("productDescription")]
        public string ProductDescription { get; set; }
        [JsonPropertyName("productCategory")]
        public string ProductCategory { get; set; }
        [JsonPropertyName("productImage")]
        public string ProductImage { get; set; }

        public override string ToString()
        {
            return $"productId: {ProductId}, productName: {ProductName}, productCategory: {ProductCategory}, productPrice: {ProductPrice}, Quantity: {ProductQuantity}, Rating: {ProductRating} ";
        }

    }
}
