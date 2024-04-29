using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CartApi.Models;

public class Cart
{
    [JsonPropertyName("cartId")]
    public int CartId { get; set; }

    [JsonPropertyName("cartPrice")]
    public decimal? CartPrice { get; set; }

    [JsonPropertyName("cartItems")]
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}
