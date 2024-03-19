using System;
using System.Collections.Generic;

namespace AcmeCorpShopperProductsRestApi.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public decimal ProductPrice { get; set; }

	public override string ToString()
	{
		return $"productId: {ProductId}, productName: {ProductName}, productPrice: {ProductPrice} ";
	}
}
