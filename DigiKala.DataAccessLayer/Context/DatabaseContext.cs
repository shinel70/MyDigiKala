using System;
using System.Collections.Generic;
using System.Text;

using DigiKala.DataAccessLayer.Entities;

using Microsoft.EntityFrameworkCore;

namespace DigiKala.DataAccessLayer.Context
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {

        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<StoreCategory> StoreCategories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductGallery> ProductGalleries { get; set; }
        public DbSet<Field> Fields { get; set; }
        public DbSet<FieldCategory> FieldCategories { get; set; }
        public DbSet<ProductField> ProductFields { get; set; }
        public DbSet<ProductSeen> ProductSeens { get; set; }
        public DbSet<Banner> Banners { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<BannerDetails> BannerDetails { get; set; }
    }
}
