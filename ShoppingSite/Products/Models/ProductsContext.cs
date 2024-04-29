using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ProductApi.Models;

public partial class ProductsContext : DbContext
{
	//public ProductsContext()
	//{
	//}

	public ProductsContext(DbContextOptions<ProductsContext> options)
		: base(options)
	{
	}

	public virtual DbSet<Product> Products { get; set; }

	//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
	//        => optionsBuilder.UseSqlServer("Data Source= JUNFENG;Initial Catalog=Products; Integrated Security=sspi; TrustServerCertificate=True");

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Product>(entity =>
		{
			entity.HasKey(e => e.ProductId).HasName("PK__Products__9834FBBA37139518");

			entity.Property(e => e.ProductId).HasColumnName("Product_Id");
			entity.Property(e => e.ProductName)
				.HasMaxLength(100)
				.HasColumnName("Product_Name");
			entity.Property(e => e.ProductPrice)
				.HasColumnType("decimal(10, 2)")
				.HasColumnName("Product_Price");
			entity.Property(e => e.ProductQuantity).HasColumnName("Product_Quantity");
			entity.Property(e => e.ProductRating)
				.HasColumnType("decimal(3, 1)")
				.HasColumnName("Product_Rating");
		});

		OnModelCreatingPartial(modelBuilder);
	}

	partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
