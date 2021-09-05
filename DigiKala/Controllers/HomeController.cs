using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using System.Globalization;

using DigiKala.Core.ViewModels;
using DigiKala.Core.Interfaces;
using DigiKala.Core.Services;

using DigiKala.DataAccessLayer.Entities;

using DigiKala.Core.Classes;
using DigiKala.DataAccessLayer.Context;
using Microsoft.EntityFrameworkCore;

namespace DigiKala.Controllers
{
    public class HomeController : Controller
    {
        private ITemp _temp;
        private DatabaseContext _context;

        private PersianCalendar pc = new PersianCalendar();

        public HomeController(ITemp temp, DatabaseContext context)
        {
            _temp = temp;
            _context = context;
        }
        [Route("/Home/{ControllerName}/{CategoryName}")]
        public IActionResult SearchByCategory(string categoryName)
        {
            Category selectCategory = _context.Categories.Include(c => c.Parent).ThenInclude(parent => parent.SubCategories).FirstOrDefault(c => c.Name == categoryName);
            List<Category> categories = _context.Categories.ToList();
            List<int> knownCategoryIds = new List<int>() { selectCategory.Id };
            List<Category> knownCategories = new List<Category>();
            if (selectCategory.Parent != null)
                knownCategories.AddRange(selectCategory.Parent.SubCategories);
            else
                knownCategories.AddRange(categories.Where(c => c.ParentId == null).ToList());
            foreach (var category in categories)
            {
                if (knownCategoryIds.Contains(category.ParentId ?? 0))
                {
                    knownCategoryIds.Add(category.Id);
                    knownCategories.Add(category);
                }
            }

            List<Product> products  = _context.Products.Include(p => p.Brand).Include(p => p.Category).Include(p => p.Store)
                .Where(p => knownCategoryIds.Contains(p.CategoryId)).ToList();

            
            List<Brand> brands = products.Select(p=>p.Brand).Distinct().ToList();
            //Category category = _temp.GetCategory(categoryId);

            //List<Category> categories = _temp.GetCategoryById(Convert.ToInt32(category.ParentId));

            //Category parentCategory = _temp.GetCategory(Convert.ToInt32(category.ParentId));

            //List<Store> stores = _temp.GetStores(categoryId);
            List<Store> stores = products.Select(p => p.Store).Distinct().ToList();

            var viewmodel = new SearchCategoryViewModel();

            viewmodel.FillBrands = brands;
            viewmodel.FillCategories = knownCategories;
            viewmodel.FillProducts = products;
            viewmodel.FillParentCategory = selectCategory.Parent;
            viewmodel.FillSelectCategory = selectCategory;
            viewmodel.FillStores = stores;

            return View(viewmodel);
        }
        [Role]
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Index()
        {
            List<BannerDetails> bannerDetails = _temp.GetBannerDetailsNoExpire();

            if (bannerDetails.Count() > 0)
            {
                foreach (var item in bannerDetails)
                {
                    if (item.ExpireDateTime.CompareTo(DateTime.Now) < 0)       {
                        _temp.UpdateBannerExpire(item.Id);
                    }
                }
            }

            return View();
        }

        public IActionResult Banners()
        {
            List<Banner> banners = _temp.GetBanners();

            return View(banners);
        }

        [Route("/Product/{id}/{title}")]
        public IActionResult Product(int id, string title)
        {
            var viewModel = new ProductDetailsViewModel();

            Product product = _temp.GetProductDetail(id);

            viewModel.FillProduct = product;

            return View(viewModel);
        }
    }
}