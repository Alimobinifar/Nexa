using Domain.Entities.Inventories;
using Domain.Entities.Product;
using Domain.Entities.Products;
using Domain.Entities.SalesChannels;
using Domain.Entities.Salse;
using Domain.Entities.SellingChannels;
using Domain.Entities.Suppliers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.DataBase
{
    public class TenantDbContext : DbContext
    {
        public TenantDbContext(DbContextOptions<TenantDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<WarehouseInventory> WarehouseInventory { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductEntry> ProductEntries { get; set; }
        public DbSet<ProductChannelAllocation> ProductChannelAllocations { get; set; }
        public DbSet<SalesChannel> SalesChannels { get; set; }
        public DbSet<SaleRegistration> SaleRegistrations { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<InventoryTransaction> InventoryTransaction { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // SaleRegistration Configuration
            modelBuilder.Entity<SaleRegistration>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasOne(x => x.SalesChannel)
                      .WithMany(x => x.SaleRegistrations)
                      .HasForeignKey(x => x.ChannelId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Product)
                      .WithMany()
                      .HasForeignKey(x => x.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(x => x.SalePrice).HasPrecision(18, 2);
                entity.Property(x => x.BuyPrice).HasPrecision(18, 2);
                entity.Property(x => x.SendPrice).HasPrecision(18, 2);
                entity.Property(x => x.PackingPrice).HasPrecision(18, 2);
                entity.Property(x => x.CommissionRate).HasPrecision(5, 2);
                entity.Property(x => x.TotalSaleAmount).HasPrecision(18, 2);
                entity.Property(x => x.PurchaseNetCost).HasPrecision(18, 2);
                entity.Property(x => x.CommissionCost).HasPrecision(18, 2);
                entity.Property(x => x.NetProfit).HasPrecision(18, 2);
                entity.Property(x => x.NetProfitPercent).HasPrecision(18, 2);
            });

            // SalesChannel Configuration
            modelBuilder.Entity<SalesChannel>(entity =>
            {
                entity.Property(x => x.Name)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(x => x.DefaultCommissionRate).HasPrecision(5, 2);
                entity.Property(x => x.FixedFee).HasPrecision(18, 2);
            });

            // ProductChannelAllocation Configuration
            modelBuilder.Entity<ProductChannelAllocation>(entity =>
            {
                entity.HasKey(x => x.Id);

                // رابطه با محصول
                entity.HasOne(x => x.Product)
                      .WithMany(x => x.ProductChannelAllocations)
                      .HasForeignKey(x => x.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);

                // رابطه با کانال فروش
                entity.HasOne(x => x.SalesChannel)
                      .WithMany(x => x.ProductChannelAllocations)
                      .HasForeignKey(x => x.ChannelId)
                      .OnDelete(DeleteBehavior.Restrict);

                // جلوگیری از تخصیص تکراری یک محصول به یک کانال
                entity.HasIndex(x => new { x.ProductId, x.ChannelId })
                      .IsUnique();

                // این پراپرتی در دیتابیس ذخیره نمی‌شود
                entity.Ignore(x => x.AvailableQuantity);
            });
        }
    }
}
