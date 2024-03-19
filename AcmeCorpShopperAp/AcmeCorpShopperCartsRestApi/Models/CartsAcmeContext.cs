﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AcmeCorpShopperCartsRestApi.Models;

public partial class CartsAcmeContext : DbContext
{
    //public CartsAcmeContext()
    //{
    //}

    public CartsAcmeContext(DbContextOptions<CartsAcmeContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Data Source = JUNFENG; Initial Catalog=CartsAcme; Integrated Security=sspi; TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK__Carts__D6AB4759B8192FCC");

            entity.Property(e => e.CartId).HasColumnName("Cart_Id");
            entity.Property(e => e.CartPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Cart_Price");
		});

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.CartItemId).HasName("PK__Cart_Ite__3C0F265CEB07BE20");

            entity.ToTable("Cart_Items");

            entity.Property(e => e.CartItemId).HasColumnName("Cart_Item_Id");
            entity.Property(e => e.FkCartId).HasColumnName("Fk_Cart_Id");
            entity.Property(e => e.ProductId).HasColumnName("Product_Id");

            entity.HasOne(d => d.FkCart).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.FkCartId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Cart_Item__Fk_Ca__398D8EEE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

	partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
