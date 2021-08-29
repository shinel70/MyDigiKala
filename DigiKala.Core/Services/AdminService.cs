using System;
using System.Collections.Generic;
using System.Text;

using DigiKala.Core.Interfaces;
using DigiKala.DataAccessLayer.Context;
using DigiKala.DataAccessLayer.Entities;

using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace DigiKala.Core.Services
{
    public class AdminService : IAdmin
    {
        private DatabaseContext _context;

        public AdminService(DatabaseContext context)
        {
            _context = context;
        }

        public void AddBannerDetails(BannerDetails bannerDetails)
        {
            _context.BannerDetails.Add(bannerDetails);
            _context.SaveChanges();
        }

        public void AddBrand(Brand brand)
        {
            _context.Brands.Add(brand);
            _context.SaveChanges();
        }

        public void AddField(Field field)
        {
            _context.Fields.Add(field);
            _context.SaveChanges();
        }

        public void AddFieldCategory(FieldCategory fieldCategory)
        {
            _context.FieldCategories.Add(fieldCategory);
            _context.SaveChanges();
        }

        public void AddSlider(Slider slider)
        {
            _context.Sliders.Add(slider);
            _context.SaveChanges();
        }

        public void DeleteAll(int id)
        {
            List<FieldCategory> fieldCategories = _context.FieldCategories.Where(f => f.CategoryId == id).ToList();

            _context.FieldCategories.RemoveRange(fieldCategories);
            _context.SaveChanges();
        }

        public void DeleteBrand(int id)
        {
            Brand brand = _context.Brands.Find(id);

            _context.Brands.Remove(brand);
            _context.SaveChanges();
        }

        public void DeleteCategory(int id)
        {
            Category category = _context.Categories.Find(id);

            _context.Categories.Remove(category);
            _context.SaveChanges();
        }

        public void DeleteField(int id)
        {
            Field field = _context.Fields.Find(id);

            _context.Fields.Remove(field);
            _context.SaveChanges();
        }

        public void DeletePermission(int id)
        {
            Permission permission = _context.Permissions.Find(id);

            _context.Permissions.Remove(permission);
            _context.SaveChanges();
        }

        public void DeleteSlider(int id)
        {
            Slider slider = _context.Sliders.Find(id);

            _context.Sliders.Remove(slider);
            _context.SaveChanges();
        }

        public void DeleteStoreCategory(int id)
        {
            StoreCategory storeCategory = _context.StoreCategories.Find(id);

            _context.StoreCategories.Remove(storeCategory);
            _context.SaveChanges();
        }

        public bool ExistsFieldCategory(int id, int catid)
        {
            return _context.FieldCategories.Any(f => f.FieldId == id && f.CategoryId == catid);
        }

        public bool ExistsSetting()
        {
            Setting setting = _context.Settings.FirstOrDefault();

            if (setting == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public List<Store> GetActiveStores()
        {
            return _context.Stores.Include(s => s.User).Where(s => s.MobileActivate == true && s.MailActivate == true).OrderByDescending(s => s.UserId).ToList();
        }

        public Banner GetBanner(int id)
        {
            return _context.Banners.Find(id);
        }

        public List<BannerDetails> GetBannerDetails(int id)
        {
            return _context.BannerDetails.Where(b => b.BannerId == id).OrderByDescending(b => b.Id).ToList();
        }

        public BannerDetails GetBannerDetailsById(int id)
        {
            return _context.BannerDetails.Find(id);
        }

        public List<Banner> GetBanners()
        {
            return _context.Banners.ToList();
        }

        public Brand GetBrand(int id)
        {
            return _context.Brands.Find(id);
        }

        public List<Brand> GetBrands()
        {
            return _context.Brands.Include(b => b.Store).OrderByDescending(b => b.Id).ToList();
        }

        public List<Brand> GetBrandsByStoreId(int id)
        {
            return _context.Brands.Where(b => b.StoreId == id).OrderByDescending(b => b.Id).ToList();
        }

        public int GetBrandsNotShowCount()
        {
            return _context.Brands.Where(b => b.NotShow == true).ToList().Count();
        }

        public List<Category> GetCategories()
        {
            return _context.Categories.Where(c => c.ParentId == null).ToList();
        }

        public List<Category> GetCategoriesByParentId(int id)
        {
            return _context.Categories.Include(c => c.Parent).Where(c => c.ParentId == id).ToList();
        }

        public Category GetCategory(int id)
        {
            return _context.Categories.Include(c => c.Parent).FirstOrDefault(c => c.Id == id);
        }

        public int? GetCategoryParentId(int id)
        {
            return _context.Categories.FirstOrDefault(c => c.Id == id).ParentId;
        }

        public Coupon GetCoupon(int id)
        {
            return _context.Coupons.Include(c => c.Store).FirstOrDefault(c => c.Id == id);
        }

        public List<Coupon> GetCoupons()
        {
            return _context.Coupons.Include(c => c.Store).OrderByDescending(c => c.Id).ToList();
        }

        public List<Coupon> GetCouponsByStoreId(int id)
        {
            return _context.Coupons.Where(c => c.StoreId == id).OrderByDescending(c => c.Id).ToList();
        }

        public Field GetField(int id)
        {
            return _context.Fields.Find(id);
        }

        public List<FieldCategory> GetFieldCategories(int id)
        {
            return _context.FieldCategories.Where(f => f.CategoryId == id).ToList();
        }

        public List<Field> GetFields()
        {
            return _context.Fields.OrderBy(f => f.Name).ToList();
        }

        public Permission GetPermission(int id)
        {
            return _context.Permissions.Find(id);
        }

        public List<Permission> GetPermissions()
        {
            return _context.Permissions.OrderBy(p => p.Name).ToList();
        }

        public Product GetProduct(int id)
        {
            return _context.Products.Include(p => p.Brand).Include(p => p.Category).Include(p => p.Store).FirstOrDefault(p => p.Id == id);
        }

        public List<ProductField> GetProductFields(int id)
        {
            return _context.ProductFields.Include(p => p.Field).Where(p => p.ProductId == id).ToList();
        }

        public List<ProductGallery> GetProductGalleries(int id)
        {
            return _context.ProductGalleries.Where(p => p.ProductId == id).ToList();
        }

        public List<Product> GetProducts()
        {
            return _context.Products.Include(p => p.Store).Include(p => p.Brand).OrderByDescending(p => p.Id).ToList();
        }

        public List<Product> GetProductsByStore(int id)
        {
            return _context.Products.Where(p => p.StoreId == id).OrderByDescending(p => p.Id).ToList();
        }

        public List<ProductSeen> GetProductSeens(int id)
        {
            return _context.ProductSeens.Where(s => s.ProductId == id).OrderByDescending(s => s.Id).ToList();
        }

        public Setting GetSetting()
        {
            return _context.Settings.FirstOrDefault();
        }

        public Slider GetSlider(int id)
        {
            return _context.Sliders.Find(id);
        }

        public List<Slider> GetSliders()
        {
            return _context.Sliders.OrderByDescending(s => s.Id).ToList();
        }

        public List<StoreCategory> GetStoreCategories()
        {
            return _context.StoreCategories.Include(s => s.Category).Include(s => s.Store).Where(s => s.IsActive == false).OrderByDescending(s => s.Id).ToList();
        }

        public StoreCategory GetStoreCategory(int id)
        {
            return _context.StoreCategories.Find(id);
        }

        public List<Category> GetSubCategories()
        {
            return _context.Categories.ToList();
        }

        public void InsertCategory(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
        }

        public void InsertPermission(Permission permission)
        {
            _context.Permissions.Add(permission);
            _context.SaveChanges();
        }

        public void InsertSetting(Setting setting)
        {
            _context.Settings.Add(setting);
            _context.SaveChanges();
        }

        public void RemoveCoupon(int id)
        {
            Coupon coupon = _context.Coupons.Find(id);

            _context.Coupons.Remove(coupon);
            _context.SaveChanges();
        }

        public void RemoveProduct(int id)
        {
            Product product = _context.Products.Find(id);

            _context.Products.Remove(product);
            _context.SaveChanges();
        }

        public void UpdateBanner(int id, string name, string defaultimg, string desc, string size, int day, int price)
        {
            Banner banner = _context.Banners.Find(id);

            banner.Name = name;
            banner.DefaultImg = defaultimg;
            banner.Desc = desc;
            banner.Size = size;
            banner.Day = day;
            banner.Price = price;

            _context.SaveChanges();
        }

        public void UpdateBrand(int id, string name, string img, bool notshow)
        {
            Brand brand = _context.Brands.Find(id);

            brand.Name = name;
            brand.Img = img;
            brand.NotShow = notshow;

            _context.SaveChanges();
        }

        public void UpdateCategory(int id, string name, string icon)
        {
            Category category = _context.Categories.Find(id);

            category.Name = name;
            category.Icon = icon;

            _context.SaveChanges();
        }

        public void UpdateField(int id, string name)
        {
            Field field = _context.Fields.Find(id);

            field.Name = name;

            _context.SaveChanges();
        }

        public void UpdatePermission(int id, string name)
        {
            Permission permission = _context.Permissions.Find(id);

            permission.Name = name;

            _context.SaveChanges();
        }

        public void UpdateSetting(string name, string desc, string keys, string api, string sender, string mail, string password)
        {
            Setting setting = _context.Settings.FirstOrDefault();

            setting.SiteName = name;
            setting.SiteDesc = desc;
            setting.SiteKeys = keys;
            setting.SmsApi = api;
            setting.SmsSender = sender;
            setting.MailAddress = mail;
            setting.MailPassword = password;

            _context.SaveChanges();
        }

        public void UpdateSlider(int id, string title, string img, string desc, bool notshow, int order)
        {
            Slider slider = _context.Sliders.Find(id);

            slider.Title = title;
            slider.Desc = desc;
            slider.Img = img;
            slider.OrderShow = order;
            slider.NotShow = notshow;

            _context.SaveChanges();
        }

        public void UpdateStoreCategory(int id, bool isactive, string desc)
        {
            StoreCategory storeCategory = _context.StoreCategories.Find(id);

            storeCategory.IsActive = isactive;
            storeCategory.Desc = desc;

            _context.SaveChanges();
        }

        public void UpdateSubCategory(int id, int parentid, string name)
        {
            Category category = _context.Categories.Find(id);

            category.Name = name;
            category.ParentId = parentid;

            _context.SaveChanges();
        }
    }
}
