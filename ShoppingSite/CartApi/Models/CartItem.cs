﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CartApi.Models;

public partial class CartItem
{
    public int CartItemId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public int FkCartId { get; set; }
    [JsonIgnore]
    public virtual Cart FkCart { get; set; } = null!;
}
