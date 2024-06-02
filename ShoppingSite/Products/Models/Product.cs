using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductApi.Models;

public partial class Product
{
	[Key]
	[Column("Product_Id")]
	public int ProductId { get; set; }

    [Column("Product_Name")]
    public string ProductName { get; set; } = null!;
    [Column("Product_Price")]
    public decimal ProductPrice { get; set; }
    [Column("Product_Quantity")]
    public int ProductQuantity { get; set; }
    [Column("Product_Rating")]
    public decimal? ProductRating { get; set; }
    [Column("Product_Description")]
    public string ProductDescription { get; set; }
    [Column("Product_Category")]
    public string ProductCategory { get; set; }
    [Column("Product_Image")]
    public string ProductImage { get; set; }


	public override string ToString()
	{
		return $"productId: {ProductId}, productName: {ProductName}, productCategory: {ProductCategory}, productPrice: {ProductPrice}, Quantity: {ProductQuantity}, Rating: {ProductRating} ";
	}
}
