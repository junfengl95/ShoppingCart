using System.Text.Json.Serialization;

namespace AcmeCorpShopperUIWebApp.Models
{
    public class Order
    {
		[JsonPropertyName("orderId")]
        public int OrderId { get; set; }

        [JsonPropertyName("customerName")]
        public string CustomerName { get; set; } = null!;

        [JsonPropertyName("cartId")]
        public int CartId { get; set; }
    }
}
