﻿using System;
using System.Collections.Generic;

namespace AcmeCorpShopperCartsRestApi.Models;

public partial class Cart
{
    public int CartId { get; set; }

    public decimal? CartPrice { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}
