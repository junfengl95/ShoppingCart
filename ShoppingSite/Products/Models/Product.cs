using System;
using System.Collections.Generic;

namespace ProductApi.Models;

public partial class Product
{
	public int ProductId { get; set; }

	public string ProductName { get; set; } = null!;

	public decimal ProductPrice { get; set; }

	public int ProductQuantity { get; set; }

	public decimal? ProductRating { get; set; }


	public override string ToString()
	{
		return $"productId: {ProductId}, productName: {ProductName}, productPrice: {ProductPrice}, Quantity: {ProductQuantity}, Rating: {ProductRating} ";
	}
}
