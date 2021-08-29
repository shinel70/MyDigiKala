using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using DigiKala.Core.Interfaces;

using DigiKala.DataAccessLayer.Entities;
using DigiKala.DataAccessLayer.Context;

using Microsoft.EntityFrameworkCore;

namespace DigiKala.Core.Services
{
    public class StoreService : IStore
    {
        private DatabaseContext _context;

        public StoreService(DatabaseContext context)
        {
            _context = context;
        }

        public void AddBrand(Brand brand)
        {
            _context.Brands.Add(brand);
            _context.SaveChanges();
        }

        public void AddCoupon(Coupon coupon)
        {
            _context.Coupons.Add(coupon);
            _context.SaveChanges();
        }

        public void AddGallery(ProductGallery productGallery)
        {
            _context.ProductGalleries.Add(productGallery);
            _context.SaveChanges();
        }

        public void AddProduct(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        public void AddProductField(ProductField productField)
        {
            _context.ProductFields.Add(productField);
            _context.SaveChanges();
        }

        public void AddStoreCategory(StoreCategory storeCategory)
        {
            _context.StoreCategories.Add(storeCategory);
            _context.SaveChanges();
        }

        public List<Brand> AllBrands()
        {
            return _context.Brands.OrderBy(b => b.Name).ToList();
        }

        public void DeleteAllProductFields(int id)
        {
            List<ProductField> productFields = _context.ProductFields.Where(p => p.ProductId == id).ToList();

            _context.ProductFields.RemoveRange(productFields);
            _context.SaveChanges();
        }

        public void DeleteGallery(int id)
        {
            ProductGallery productGallery = _context.ProductGalleries.Find(id);

            _context.ProductGalleries.Remove(productGallery);
            _context.SaveChanges();

        }

        public void DeleteProduct(int id)
        {
            Product product = _context.Products.Find(id);

            _context.Products.Remove(product);
            _context.SaveChanges();
        }

        public void DeleteStoreCategory(int id)
        {
            StoreCategory storeCategory = _context.StoreCategories.Find(id);

            _context.StoreCategories.Remove(storeCategory);
            _context.SaveChanges();
        }

        public bool ExistsBrand(string name)
        {
            return _context.Brands.Any(b => b.Name == name);
        }

        public bool ExistsCouponCode(string code)
        {
            return _context.Coupons.Any(c => c.Code == code);
        }

        public List<Brand> GetBrands(int id)
        {
            return _context.Brands.Where(b => b.StoreId == id).OrderByDescending(b => b.Id).ToList();
        }

        public List<Category> GetCategories(int id)
        {
            return _context.Categories.Where(c => c.Parent.ParentId == id).ToList();
        }

        public List<Category> GetCategoriesByNullParent()
        {
            return _context.Categories.Where(c => c.ParentId == null).OrderBy(c => c.Name).ToList();
        }

        public Category GetCategory(int id)
        {
            return _context.Categories.Include(c => c.Parent).FirstOrDefault(c => c.Id == id);
        }

        public Coupon GetCoupon(int id)
        {
            return _context.Coupons.Find(id);
        }

        public List<Coupon> GetCoupons(int id)
        {
            return _context.Coupons.Where(c => c.StoreId == id).OrderByDescending(c => c.Id).ToList();
        }

        public List<FieldCategory> GetFieldCategories(int id)
        {
            return _context.FieldCategories.Include(f => f.Field).Where(f => f.CategoryId == id).ToList();
        }

        public Product GetProduct(int id)
        {
            return _context.Products.Include(p => p.Brand).Include(p => p.Category).FirstOrDefault(p => p.Id == id);
        }

        public ProductField GetProductField(int id, int pid)
        {
            return _context.ProductFields.Include(p => p.Field).FirstOrDefault(p => p.ProductId == pid && p.FieldId == id);
        }

        public List<ProductField> GetProductFields(int id)
        {
            return _context.ProductFields.Where(p => p.ProductId == id).ToList();
        }

        public List<ProductGallery> GetProductGalleries(int id)
        {
            return _context.ProductGalleries.Where(p => p.ProductId == id).ToList();
        }

        public ProductGallery GetProductGallery(int id)
        {
            return _context.ProductGalleries.Find(id);
        }

        public List<Product> GetProducts(int id)
        {
            return _context.Products.Where(p => p.StoreId == id).OrderByDescending(p => p.Id).ToList();
        }

        public int GetProductSeen(int id)
        {
            return _context.ProductSeens.Where(s => s.ProductId == id).ToList().Count();
        }

        public Store GetStore(int id)
        {
            return _context.Stores.Include(s => s.User).FirstOrDefault(s => s.UserId == id);
        }

        public List<StoreCategory> GetStoreCategoriesByStoreId(int id)
        {
            return _context.StoreCategories.Include(s => s.Category).Where(s => s.UserId == id).OrderByDescending(s => s.Id).ToList();
        }

        public StoreCategory GetStoreCategory(int id)
        {
            return _context.StoreCategories.Find(id);
        }

        public void RemoveCoupon(int id)
        {
            Coupon coupon = _context.Coupons.Find(id);

            _context.Coupons.Remove(coupon);
            _context.SaveChanges();
        }

        public void UpdateBankCard(int id, string card)
        {
            Store store = _context.Stores.Find(id);

            store.BankCard = card;
            _context.SaveChanges();
        }

        public void UpdateCoupon(int id, string name, string code, bool expire, string desc, string start, string end, int percent, int price)
        {
            Coupon coupon = _context.Coupons.Find(id);

            if (!_context.Coupons.Any(c => c.Code == code && c.Id != id))
            {
                coupon.Code = code;
            }

            coupon.Name = name;            
            coupon.IsExpire = expire;
            coupon.Desc = desc;
            coupon.StartDate = start;
            coupon.EndDate = end;
            coupon.Price = price;
            coupon.Percent = percent;

            _context.SaveChanges();
        }

        public void UpdateProduct(int id, int brandid, int catid, string name, string img, int price, int delprice, int exists, bool notshow, string desc)
        {
            Product product = _context.Products.Find(id);

            product.BrandId = brandid;
            product.CategoryId = catid;
            product.DeletePrice = delprice;
            product.Price = price;
            product.Name = name;
            product.Img = img;
            product.NotShow = notshow;
            product.Exist = exists;
            product.Desc = desc;

            _context.SaveChanges();
        }

        public bool UpdateStore(int id, string name, string tel, string address, string des, string logo)
        {
            Store store = _context.Stores.Find(id);

            if (store != null)
            {
                store.Name = name;
                store.Tel = tel;
                store.Address = address;
                store.Desc = des;
                store.Logo = logo;

                _context.SaveChanges();

                return true;
            }
            else
            {
                return false;
            }
        }

        public void UpdateStoreCategory(int id, int categoryid, string img)
        {
            StoreCategory storeCategory = _context.StoreCategories.Find(id);

            storeCategory.CategoryId = categoryid;
            storeCategory.Img = img;
            storeCategory.IsActive = false;

            _context.SaveChanges();
        }
    }
}
