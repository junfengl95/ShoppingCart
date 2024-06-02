﻿// <auto-generated />
using System;
using CartApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CartApi.Migrations
{
    [DbContext(typeof(CartsContext))]
    partial class CartsContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CartApi.Models.Cart", b =>
                {
                    b.Property<int>("CartId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("Cart_Id")
                        .HasAnnotation("Relational:JsonPropertyName", "cartId");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CartId"));

                    b.Property<decimal?>("CartPrice")
                        .HasColumnType("decimal(10, 2)")
                        .HasColumnName("Cart_Price")
                        .HasAnnotation("Relational:JsonPropertyName", "cartPrice");

                    b.HasKey("CartId")
                        .HasName("PK__Carts__D6AB47591143749D");

                    b.ToTable("Carts");
                });

            modelBuilder.Entity("CartApi.Models.CartItem", b =>
                {
                    b.Property<int>("CartItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("Cart_Item_Id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CartItemId"));

                    b.Property<int>("FkCartId")
                        .HasColumnType("int")
                        .HasColumnName("Fk_Cart_Id");

                    b.Property<int>("ProductId")
                        .HasColumnType("int")
                        .HasColumnName("Product_Id");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("CartItemId")
                        .HasName("PK__Cart_Ite__3C0F265C18841E04");

                    b.HasIndex("FkCartId");

                    b.ToTable("Cart_Items", (string)null);

                    b.HasAnnotation("Relational:JsonPropertyName", "cartItems");
                });

            modelBuilder.Entity("CartApi.Models.CartItem", b =>
                {
                    b.HasOne("CartApi.Models.Cart", "FkCart")
                        .WithMany("CartItems")
                        .HasForeignKey("FkCartId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__Cart_Item__Fk_Ca__3C69FB99");

                    b.Navigation("FkCart");
                });

            modelBuilder.Entity("CartApi.Models.Cart", b =>
                {
                    b.Navigation("CartItems");
                });
#pragma warning restore 612, 618
        }
    }
}