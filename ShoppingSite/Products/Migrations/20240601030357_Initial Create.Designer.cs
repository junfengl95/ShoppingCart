﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProductApi.Models;

#nullable disable

namespace ProductApi.Migrations
{
    [DbContext(typeof(ProductsContext))]
    [Migration("20240601030357_Initial Create")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ProductApi.Models.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("Product_Id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductId"));

                    b.Property<string>("ProductCategory")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProductDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProductImage")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("Product_Name");

                    b.Property<decimal>("ProductPrice")
                        .HasColumnType("decimal(10, 2)")
                        .HasColumnName("Product_Price");

                    b.Property<int>("ProductQuantity")
                        .HasColumnType("int")
                        .HasColumnName("Product_Quantity");

                    b.Property<decimal?>("ProductRating")
                        .HasColumnType("decimal(3, 1)")
                        .HasColumnName("Product_Rating");

                    b.HasKey("ProductId")
                        .HasName("PK__Products__9834FBBA37139518");

                    b.ToTable("Products");
                });
#pragma warning restore 612, 618
        }
    }
}
