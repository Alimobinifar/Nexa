using Domain.Entities.Inventories;
using Domain.Entities.Others;
using Domain.Entities.Product;
using Domain.Entities.Products;
using Domain.Entities.SalesChannels;
using Domain.Entities.Salse;
using Domain.Entities.SellingChannels;
using Domain.Entities.Suppliers;
using Domain.Entities.Tenants;
using Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataBase
{
    public class MasterDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public MasterDbContext(DbContextOptions<MasterDbContext> options)
            : base(options)
        {
        }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Tenant>(entity =>
            {
                entity.ToTable("Tenants");
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(x => x.ConnectionString)
                      .IsRequired()
                      .HasMaxLength(1000);

                entity.HasIndex(x => x.Name).IsUnique();

                entity.HasQueryFilter(x => !x.IsDeleted);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(x => x.FirstName)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(x => x.LastName)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(x => x.UserCredit)
                      .HasPrecision(18, 2);

                entity.Property(x => x.Bio)
                      .HasMaxLength(1000);

                entity.Property(x => x.AvatarUrl)
                      .HasMaxLength(500);
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.TokenHash).IsRequired().HasMaxLength(200);
                entity.HasIndex(x => x.TokenHash).IsUnique();
                entity.Ignore(x => x.IsActive);

                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(x => x.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<User>()
                .HasOne(x => x.Tenant)
                .WithMany()
                .HasForeignKey(x => x.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
