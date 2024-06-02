using System.Text.Json.Serialization;

namespace ShopperUI.Models
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

        [JsonPropertyName("productQuantity")]
        public int ProductQuantity { get; set; }

        [JsonPropertyName("productRating")]
        public decimal? ProductRating { get; set; }

        [JsonPropertyName("productDescription")]
        public string ProductDescription { get; set; }

        [JsonPropertyName("productImage")]
        public string ProductImage { get; set; }

        public override string ToString()
        {
            return $"productId: {ProductId}, productName: {ProductName}, productPrice: {ProductPrice}, Quantity: {ProductQuantity}, Rating: {ProductRating} ";
        }
    }
}
