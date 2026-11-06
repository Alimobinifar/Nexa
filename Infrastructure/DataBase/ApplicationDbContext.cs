using Domain.Entities.Inventories;
using Domain.Entities.Product;
using Domain.Entities.Products;
using Domain.Entities.SalesChannels;
using Domain.Entities.Salse;
using Domain.Entities.SellingChannels;
using Domain.Entities.Suppliers;
using Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataBase
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                    : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<WarehouseInventory> WarehouseInventory { get; set; }

        public DbSet<ProductCategory> ProductCategories { get; set; }

        public DbSet<ProductEntry> ProductEntries { get; set; }

        public DbSet<ProductChannelAllocation> ProductChannelAllocations { get; set; }

        public DbSet<SalesChannel> SalesChannels { get; set; }

        public DbSet<SaleRegistration> SaleRegistrations { get; set; }

        public DbSet<Supplier> Suppliers { get; set; }

    }
}
