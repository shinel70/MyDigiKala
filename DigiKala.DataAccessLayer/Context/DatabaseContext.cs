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
        public DbSet<TemporaryCode> TemporaryCodes { get; set; }
		public DbSet<Comment> Comments { get; set; }
		public DbSet<CommentLike> CommentLikes { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderProduct> OrderProducts { get; set; }
		public DbSet<UserAddress> UserAddresses { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
            //modelBuilder.Entity<Comment>().HasMany(c => c.ChildComments).WithOne(c => c.ReplyComment).HasForeignKey(c =>c.ReplyCommentId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<User>().HasMany(u => u.Comments).WithOne(c => c.User).HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.NoAction);
            //modelBuilder.Entity<Product>().HasMany(p => p.Comments).WithOne(c => c.Product).HasForeignKey(c => c.ProductId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<CommentLike>().HasKey(cl =>new { cl.UserId, cl.CommentId});
            modelBuilder.Entity<CommentLike>().HasOne(cl => cl.User).WithMany(u => u.CommentLikes).HasForeignKey(cl => cl.UserId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<OrderProduct>().HasKey(op => new { op.ProductId, op.OrderId });
            modelBuilder.Entity<OrderProduct>().HasOne(op => op.Product).WithMany(p => p.OrderProducts).HasForeignKey(op => op.ProductId).OnDelete(DeleteBehavior.NoAction);
		}
	}
}
