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

namespace DigiKala.Controllers
{
    public class HomeController : Controller
    {
        private ITemp _temp;

        private PersianCalendar pc = new PersianCalendar();

        public HomeController(ITemp temp)
        {
            _temp = temp;
        }

        public IActionResult SearchByCategory(int id)
        {
            List<Product> products = _temp.GetProducts(id);
            List<Brand> brands = _temp.GetBrands(id);
            Category category = _temp.GetCategory(id);

            List<Category> categories = _temp.GetCategoryById(Convert.ToInt32(category.ParentId));

            Category parentCategory = _temp.GetCategory(Convert.ToInt32(category.ParentId));

            List<Store> stores = _temp.GetStores(id);

            var viewmodel = new SearchCategoryViewModel();

            viewmodel.FillBrands = brands;
            viewmodel.FillCategories = categories;
            viewmodel.FillProducts = products;
            viewmodel.FillParentCategory = parentCategory;
            viewmodel.FillSelectCategory = category;
            viewmodel.FillStores = stores;

            return View(viewmodel);
        }

        //[RoleAttribute(9)]
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Index()
        {
            List<BannerDetails> bannerDetails = _temp.GetBannerDetailsNoExpire();

            if (bannerDetails.Count() > 0)
            {
                string strToday = pc.GetYear(DateTime.Now).ToString("0000") + "/" +
                                  pc.GetMonth(DateTime.Now).ToString("00") + "/" +
                                  pc.GetDayOfMonth(DateTime.Now).ToString("00");

                foreach (var item in bannerDetails)
                {
                    if (item.EndDate.CompareTo(strToday) < 0)
                    {
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