using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using DigiKala.Core.Interfaces;
using DigiKala.DataAccessLayer.Context;
using DigiKala.DataAccessLayer.Entities;

namespace DigiKala.Core.Services
{
    public class TempService : ITemp
    {
        private DatabaseContext _context;

        public TempService(DatabaseContext context)
        {
            _context = context;
        }

        public bool CheckBannerImg(int id)
        {
            return _context.BannerDetails.Any(b => b.BannerId == id && b.IsExpire == false);
        }

        public BannerDetails GetBannerDetails(int id)
        {
            return _context.BannerDetails.FirstOrDefault(b => b.BannerId == id && b.IsExpire == false);
        }

        public List<BannerDetails> GetBannerDetailsNoExpire()
        {
            return _context.BannerDetails.Where(b => b.IsExpire == false).ToList();
        }

        public List<Banner> GetBanners()
        {
            return _context.Banners.ToList();
        }

        public List<Brand> GetBrands(int id)
        {
            List<Brand> brands = (from b in _context.Brands
                                  join p in _context.Products
                                  on b.Id equals p.BrandId
                                  where p.CategoryId == id
                                  select b).OrderBy(b => b.Name).Distinct().ToList();

            return brands;
        }

        public List<Category> GetCategories()
        {
            return _context.Categories.Where(c => c.ParentId == null).ToList();
        }

        public Category GetCategory(int id)
        {
            return _context.Categories.Include(c => c.Parent).FirstOrDefault(c => c.Id == id);
        }

        public List<Category> GetCategoryById(int id)
        {
            return _context.Categories.Where(c => c.ParentId == id).ToList();
        }

        public Product GetProductDetail(int id)
        {
            return _context.Products.Include(p => p.Store).Include(p => p.Brand).Include(p => p.Category).FirstOrDefault(p => p.Id == id);
        }

        public List<Product> GetProducts(int id)
        {
            return _context.Products.Include(p => p.Brand).Include(p => p.Category).Include(p => p.Store)
                    .Where(p => p.CategoryId == id).OrderByDescending(p => p.Id).ToList();
        }

        public List<Slider> GetSliders()
        {
            return _context.Sliders.Where(c => c.NotShow == false).OrderBy(c => c.OrderShow).ToList();
        }

        public List<Store> GetStores(int id)
        {
            List<Store> stores = (from s in _context.Stores
                                  join p in _context.Products
                                  on s.UserId equals p.StoreId
                                  where p.CategoryId == id
                                  select s).OrderBy(s => s.Name).Distinct().ToList();

            return stores;
        }

        public void UpdateBannerExpire(int id)
        {
            BannerDetails bannerDetails = _context.BannerDetails.Find(id);

            bannerDetails.IsExpire = true;

            _context.SaveChanges();
        }
    }
}
