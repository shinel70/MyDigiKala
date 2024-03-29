﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using DigiKala.Core.Interfaces;
using DigiKala.Core.Services;
using DigiKala.Core.Classes;

using DigiKala.Core.ViewModels;

using DigiKala.DataAccessLayer.Entities;
using Microsoft.AspNetCore.Authorization;

using System.Globalization;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using DigiKala.DataAccessLayer.Context;
using Microsoft.EntityFrameworkCore;

namespace DigiKala.Controllers
{
    [Authorize]
    public class PanelController : Controller
    {
        private readonly IUser _user;
        private readonly IStore _store;
        private readonly DatabaseContext _context;
		private readonly PersianCalendar _pc = new PersianCalendar();

        public PanelController(IUser user, IStore store, DatabaseContext context)
        {
            _user = user;
            _store = store;
            _context = context;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Edit()
        {
            Store store = _store.GetStore(_user.GetUserStore(User.Identity.Name).UserId);

            StorePropertyViewModel viewModel = new StorePropertyViewModel()
            {
                Address = store.Address,
                Desc = store.Desc,
                LogoName = store.Logo,
                Name = store.Name,
                Tel = store.Tel
            };

            ViewBag.MyStatus = false;

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Edit(StorePropertyViewModel viewModel)
        {
            Store store = _store.GetStore(_user.GetUserStore(User.Identity.Name).UserId);

            if (ModelState.IsValid)
            {
                if (viewModel.LogoImg == null)
                {
                    bool isUpdate = _store.UpdateStore(store.UserId, viewModel.Name, viewModel.Tel, viewModel.Address, viewModel.Desc, store.Logo);

                    if (isUpdate)
                    {
                        ViewBag.MyStatus = true;
                        return View(viewModel);
                    }
                }
                else
                {
                    if (viewModel.LogoImg.ContentType != "image/jpg" || viewModel.LogoImg.ContentType != "image/jpeg")
                    {

                        ModelState.AddModelError("LogoImg", "فایل با پسوند jpg بارگزاری شود");
                        ViewBag.MyStatus = false;
                    }
                    //if (Path.GetExtension(viewModel.LogoImg.FileName) != ".jpg")
                    //{
                    //    ModelState.AddModelError("LogoImg", "فایل با پسوند jpg بارگزاری شود");
                    //    ViewBag.MyStatus = false;
                    //}
                    else
                    {
                        string filePath = "";
                        viewModel.LogoName = CodeGenerators.FileCode() + Path.GetExtension(viewModel.LogoImg.FileName);
                        filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/users/stores/", viewModel.LogoName);

                        string imagePath = store.Logo;
                        if (System.IO.File.Exists(imagePath))
                            System.IO.File.Delete(imagePath);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            viewModel.LogoImg.CopyTo(stream);
                        }

                        bool isUpdate = _store.UpdateStore(store.UserId, viewModel.Name, viewModel.Tel, viewModel.Address, viewModel.Desc, viewModel.LogoName);

                        if (isUpdate)
                        {
                            ViewBag.MyStatus = true;
                            return View(viewModel);
                        }
                    }
                }
            }

            ViewBag.MyStatus = false;
            return View(viewModel);
        }

        public IActionResult ShowStoreCategory()
        {
            Store store = _store.GetStore(_user.GetUserStore(User.Identity.Name).UserId);

            List<StoreCategory> storeCategories = _store.GetStoreCategoriesByStoreId(store.UserId);

            return View(storeCategories);
        }

        public IActionResult CreateStoreCategory()
        {
            ViewBag.CategoryId = new SelectList(_store.GetCategoriesByNullParent(), "Id", "Name");

            return View();
        }

        [HttpPost]
        public IActionResult CreateStoreCategory(StoreCategoryViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Store store = _store.GetStore(_user.GetUserStore(User.Identity.Name).UserId);

                if (viewModel.Img != null)
                {
                    if (Path.GetExtension(viewModel.Img.FileName) != ".jpg")
                    {
                        ModelState.AddModelError("Img", "فایل با پسوند jpg بارگزاری شود");
                    }
                    else
                    {
                        string filePath = "";
                        viewModel.ImgName = CodeGenerators.FileCode() + Path.GetExtension(viewModel.Img.FileName);
                        filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/users/stores/", viewModel.ImgName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            viewModel.Img.CopyTo(stream);
                        }

                        StoreCategory storeCategory = new StoreCategory()
                        {
                            CategoryId = viewModel.CategoryId,
                            DateTime = DateTime.Now,
                            Desc = null,
                            Img = viewModel.ImgName,
                            IsActive = false,
                            UserId = store.UserId,
                        };

                        _store.AddStoreCategory(storeCategory);

                        return RedirectToAction(nameof(ShowStoreCategory));
                    }
                }
                else
                {
                    StoreCategory storeCategory = new StoreCategory()
                    {
                        CategoryId = viewModel.CategoryId,
                        DateTime = DateTime.Now,
                        Desc = null,
                        Img = null,
                        IsActive = false,
                        UserId = store.UserId,
                    };

                    _store.AddStoreCategory(storeCategory);

                    return RedirectToAction(nameof(ShowStoreCategory));
                }
            }

            return View(viewModel);
        }

        public IActionResult EditStoreCategory(int id)
        {
            StoreCategory storeCategory = _store.GetStoreCategory(id);

            ViewBag.CategoryId = new SelectList(_store.GetCategoriesByNullParent(), "Id", "Name", storeCategory.CategoryId);

            StoreCategoryViewModel viewModel = new StoreCategoryViewModel()
            {
                CategoryId = storeCategory.CategoryId,
                ImgName = storeCategory.Img
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult EditStoreCategory(StoreCategoryViewModel viewModel, int id)
        {
            if (ModelState.IsValid)
            {
                StoreCategory storeCategory = _store.GetStoreCategory(id);

                if (viewModel.Img != null)
                {
                    if (Path.GetExtension(viewModel.Img.FileName) != ".jpg")
                    {
                        ModelState.AddModelError("Img", "فایل با پسوند jpg بارگزاری شود");
                    }
                    else
                    {
                        string filePath = "";
                        viewModel.ImgName = CodeGenerators.FileCode() + Path.GetExtension(viewModel.Img.FileName);
                        filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/users/stores/", viewModel.ImgName);

                        string imagePath = storeCategory.Img;
                        if (System.IO.File.Exists(imagePath))
                            System.IO.File.Delete(imagePath);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            viewModel.Img.CopyTo(stream);
                        }

                        _store.UpdateStoreCategory(id, viewModel.CategoryId, viewModel.ImgName);

                        return RedirectToAction(nameof(ShowStoreCategory));
                    }
                }
                else
                {
                    _store.UpdateStoreCategory(id, viewModel.CategoryId, storeCategory.Img);

                    return RedirectToAction(nameof(ShowStoreCategory));
                }
            }

            return View(viewModel);
        }

        public IActionResult DetailsStoreCategory(int id)
        {
            StoreCategory storeCategory = _store.GetStoreCategory(id);

            return View(storeCategory);
        }

        public IActionResult DeleteStoreCategory(int? id)
        {
            return View();
        }

        [HttpPost]
        public IActionResult DeleteStoreCategory(int id)
        {
            if (ModelState.IsValid)
            {
                _store.DeleteStoreCategory(id);

                return RedirectToAction(nameof(ShowStoreCategory));
            }

            return View();
        }

        public IActionResult ShowBrands()
        {
            string username = User.Identity.Name;

            Store store = _user.GetUserStore(username);

            List<Brand> brands = _store.GetBrands(store.UserId);

            return View(brands);
        }

        public IActionResult AddBrand()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddBrand(AdminBrandViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                string username = User.Identity.Name;

                Store store = _user.GetUserStore(username);

                if (!_store.ExistsBrand(viewModel.Name))
                {
                    if (viewModel.Img != null)
                    {
                        if (Path.GetExtension(viewModel.Img.FileName) != ".jpg")
                        {
                            ModelState.AddModelError("Img", "فایل با پسوند jpg بارگزاری شود");
                        }
                        else
                        {
                            string filePath = "";
                            viewModel.ImgName = CodeGenerators.FileCode() + Path.GetExtension(viewModel.Img.FileName);
                            filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/brands/", viewModel.ImgName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                viewModel.Img.CopyTo(stream);
                            }

                            Brand brand = new Brand()
                            {
                                Img = viewModel.ImgName,
                                Name = viewModel.Name,
                                NotShow = true,
                                StoreId = store.UserId
                            };

                            _store.AddBrand(brand);

                            return RedirectToAction(nameof(ShowBrands));
                        }
                    }
                    else
                    {
                        Brand brand = new Brand()
                        {
                            Img = null,
                            Name = viewModel.Name,
                            NotShow = true,
                            StoreId = store.UserId
                        };

                        _store.AddBrand(brand);

                        return RedirectToAction(nameof(ShowBrands));
                    }
                }
            }

            return View(viewModel);
        }

        public IActionResult ShowProducts()
        {
            string username = User.Identity.Name;

            Store store = _user.GetUserStore(username);

            List<Product> products = _store.GetProducts(store.UserId);

            return View(products);
        }

        public IActionResult DetailsProduct(int id)
        {
            Product product = _store.GetProduct(id);

            return View(product);
        }

        public IActionResult AddProduct()
        {



            //Store store = _user.GetUserStore(User.Identity.Name);
            //var cat = _store.GetStoreCategoriesByStoreId(store.UserId).FirstOrDefault();
            //int id = cat == null ? 0 : cat.Id;
            //StoreCategory storeCategory = _store.GetStoreCategory(id);
            //ViewBag.CategoryId = new SelectList(_store.GetCategories(storeCategory.CategoryId), "Id", "Name");

            var userId = _context.Users.FirstOrDefault(u => u.Mobile == User.Identity.Name).Id;
            List<Category> categories = _context.Categories.ToList();
            List<int> knownIds = _context.StoreCategories.Where(sc => sc.UserId == userId).Select(sc => sc.CategoryId).ToList();
            for (int i = 0; i < categories.Count; i++)
            {
                if (categories[i].ParentId is null)
                {
                    if(!knownIds.Contains(categories[i].Id))
                    {
                        categories.RemoveAt(i);
                        i--;
                    }
                    continue;
                }
                else if(!knownIds.Contains(categories[i].ParentId ?? 0))
                {
                    categories.RemoveAt(i);
                    i--;
                    continue;
                }
                knownIds.Add(categories[i].Id);
            }
            ViewBag.CategoryId = new SelectList(categories, "Id", "Name");
            ViewBag.BrandId = new SelectList(_store.AllBrands(), "Id", "Name");

            return View();
        }

        [HttpPost]
        public IActionResult AddProduct(ProductViewModel viewModel)
        {
            string username = User.Identity.Name;

            Store store = _user.GetUserStore(username);

            var userId = _context.Users.FirstOrDefault(u => u.Mobile == User.Identity.Name).Id;
            List<Category> categories = _context.Categories.ToList();
            List<int> knownIds = _context.StoreCategories.Where(sc => sc.UserId == userId).Select(sc => sc.CategoryId).ToList();
            for (int i = 0; i < categories.Count; i++)
            {
                if (categories[i].ParentId is null)
                {
                    if (!knownIds.Contains(categories[i].Id))
                    {
                        categories.RemoveAt(i);
                        i--;
                    }
                    continue;
                }
                else if (!knownIds.Contains(categories[i].ParentId ?? 0))
                {
                    categories.RemoveAt(i);
                    i--;
                    continue;
                }
                knownIds.Add(categories[i].Id);
            }

            if (ModelState.IsValid)
            {
                if(!knownIds.Contains(viewModel.CategoryId))
                {
                    ModelState.AddModelError("CategoryId", "لطفا دسته بندی صحیح را وارد کنید");
                    ViewBag.CategoryId = new SelectList(categories, "Id", "Name", viewModel.CategoryId);
                    ViewBag.BrandId = new SelectList(_store.AllBrands(), "Id", "Name", viewModel.BrandId);
                    return View(viewModel);
                }
                if (viewModel.Img != null)
                {
                    if (Path.GetExtension(viewModel.Img.FileName) != ".jpg")
                    {
                        ModelState.AddModelError("Img", "فایل با پسوند jpg بارگزاری شود");
                    }
                    else
                    {
                        string filePath = "";
                        viewModel.ImgName = CodeGenerators.FileCode() + Path.GetExtension(viewModel.Img.FileName);
                        filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products/", viewModel.ImgName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            viewModel.Img.CopyTo(stream);
                        }

                        Product product = new Product()
                        {
                            Img = viewModel.ImgName,
                            BrandId = viewModel.BrandId,
                            CategoryId = viewModel.CategoryId,
                            DateTime = DateTime.Now,
                            DeletePrice = viewModel.DeletePrice,
                            Exist = viewModel.Exist,
                            Desc = viewModel.Desc,
                            Name = viewModel.Name,
                            NotShow = viewModel.NotShow,
                            Price = viewModel.Price,
                            StoreId = store.UserId
                        };

                        _store.AddProduct(product);

                        return RedirectToAction(nameof(ShowProducts));
                    }
                }
            }

           
            ViewBag.CategoryId = new SelectList(categories, "Id", "Name",viewModel.CategoryId);
            ViewBag.BrandId = new SelectList(_store.AllBrands(), "Id", "Name", viewModel.BrandId);

            return View(viewModel);
        }

        public IActionResult EditProduct(int id)
        {
            var userId = _context.Users.FirstOrDefault(u => u.Mobile == User.Identity.Name).Id;
            List<Category> categories = _context.Categories.ToList();
            List<int> knownIds = _context.StoreCategories.Where(sc => sc.UserId == userId).Select(sc => sc.CategoryId).ToList();
            for (int i = 0; i < categories.Count; i++)
            {
                if (categories[i].ParentId is null)
                {
                    if (!knownIds.Contains(categories[i].Id))
                    {
                        categories.RemoveAt(i);
                        i--;
                    }
                    continue;
                }
                else if (!knownIds.Contains(categories[i].ParentId ?? 0))
                {
                    categories.RemoveAt(i);
                    i--;
                    continue;
                }
                knownIds.Add(categories[i].Id);
            }
            Product product = _store.GetProduct(id);

            ViewBag.CategoryId = new SelectList(categories, "Id", "Name",product.Category);

            ViewBag.BrandId = new SelectList(_store.AllBrands(), "Id", "Name");

           

            ProductViewModel viewModel = new ProductViewModel()
            {
                BrandId = product.BrandId,
                CategoryId = product.CategoryId,
                DeletePrice = product.DeletePrice,
                Exist = product.Exist,
                Desc = product.Desc,
                ImgName = product.Img,
                Name = product.Name,
                NotShow = product.NotShow,
                Price = product.Price
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult EditProduct(ProductViewModel viewModel, int id)
        {
            string username = User.Identity.Name;

            Store store = _user.GetUserStore(username);
            var userId = _context.Users.FirstOrDefault(u => u.Mobile == User.Identity.Name).Id;
            List<Category> categories = _context.Categories.ToList();
            List<int> knownIds = _context.StoreCategories.Where(sc => sc.UserId == userId).Select(sc => sc.CategoryId).ToList();
            for (int i = 0; i < categories.Count; i++)
            {
                if (categories[i].ParentId is null)
                {
                    if (!knownIds.Contains(categories[i].Id))
                    {
                        categories.RemoveAt(i);
                        i--;
                    }
                    continue;
                }
                else if (!knownIds.Contains(categories[i].ParentId ?? 0))
                {
                    categories.RemoveAt(i);
                    i--;
                    continue;
                }
                knownIds.Add(categories[i].Id);
            }
            ViewBag.CategoryId = new SelectList(categories, "Id", "Name");

            if (ModelState.IsValid)
            {
                if (!knownIds.Contains(viewModel.CategoryId))
                {
                    ModelState.AddModelError("CategoryId", "لطفا دسته بندی صحیح را وارد کنید");
                    ViewBag.CategoryId = new SelectList(categories, "Id", "Name", viewModel.CategoryId);
                    ViewBag.BrandId = new SelectList(_store.AllBrands(), "Id", "Name", viewModel.BrandId);
                    return View(viewModel);
                }
                Product product = _store.GetProduct(id);

                if (viewModel.Img != null)
                {
                    if (Path.GetExtension(viewModel.Img.FileName) != ".jpg")
                    {
                        ModelState.AddModelError("Img", "فایل با پسوند jpg بارگزاری شود");
                    }
                    else
                    {
                        string filePath = "";
                        viewModel.ImgName = CodeGenerators.FileCode() + Path.GetExtension(viewModel.Img.FileName);
                        filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products/", viewModel.ImgName);
                        string imagePath = product.Img;
                        if (System.IO.File.Exists(imagePath))
                            System.IO.File.Delete(imagePath);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            viewModel.Img.CopyTo(stream);
                        }

                        _store.UpdateProduct(id, viewModel.BrandId, viewModel.CategoryId, viewModel.Name,
                            viewModel.ImgName, viewModel.Price, viewModel.DeletePrice, viewModel.Exist, viewModel.NotShow, viewModel.Desc);

                        return RedirectToAction(nameof(ShowProducts));
                    }
                }
                else
                {
                    _store.UpdateProduct(id, viewModel.BrandId, viewModel.CategoryId, viewModel.Name,
                            product.Img, viewModel.Price, viewModel.DeletePrice, viewModel.Exist, viewModel.NotShow, viewModel.Desc);

                    return RedirectToAction(nameof(ShowProducts));
                }
            }

            int cid = _store.GetStoreCategoriesByStoreId(store.UserId).FirstOrDefault().Id;

            StoreCategory storeCategory = _store.GetStoreCategory(cid);

            ViewBag.CategoryId = new SelectList(_store.GetCategories(storeCategory.CategoryId), "Id", "Name", viewModel.CategoryId);
            ViewBag.BrandId = new SelectList(_store.AllBrands(), "Id", "Name", viewModel.BrandId);

            return View(viewModel);
        }

        public IActionResult DeleteProduct(int? id)
        {
            return View();
        }

        [HttpPost]
        public IActionResult DeleteProduct(int id)
        {
            if (ModelState.IsValid)
            {
                _store.DeleteProduct(id);

                return RedirectToAction(nameof(ShowProducts));
            }

            return View();
        }

        public IActionResult CreateGallery(int id)
        {
            ViewBag.MyId = id;

            return View();
        }

        [HttpPost]
        public IActionResult CreateGallery(int id, GalleryViewModel viewModel)
        {
            if (viewModel.Img != null)
            {
                if (Path.GetExtension(viewModel.Img.FileName) != ".jpg")
                {
                    ModelState.AddModelError("Img", "فایل با پسوند jpg بارگزاری شود");
                }
                else
                {
                    string filePath = "";
                    viewModel.ImgName = CodeGenerators.FileCode() + Path.GetExtension(viewModel.Img.FileName);
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/galleries/", viewModel.ImgName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        viewModel.Img.CopyTo(stream);
                    }

                    ProductGallery productGallery = new ProductGallery()
                    {
                        ProductId = id,
                        Img = viewModel.ImgName
                    };

                    _store.AddGallery(productGallery);
                }
            }

            ViewBag.MyId = id;
            return View();
        }

        public IActionResult IndexGallery(int id)
        {
            List<ProductGallery> productGalleries = _store.GetProductGalleries(id);

            return View(productGalleries);
        }

        public IActionResult DeleteGallery(int id)
        {
            ProductGallery productGallery = _store.GetProductGallery(id);

            _store.DeleteGallery(id);

            return Redirect("/Panel/CreateGallery/" + productGallery.ProductId);
        }

        public IActionResult IndexProductFields(int id)
        {
            Product product = _store.GetProduct(id);
            int categoryId = _store.GetCategory(product.CategoryId).Id;

            var categories = _context.Categories.ToList();
            var category = categories.FirstOrDefault(c => c.Id == categoryId);
            while(category.ParentId != null)
			{
                category = category.Parent;
			}
            List<FieldCategory> fieldCategories = _store.GetFieldCategories(category.Id);

            ViewBag.MyId = id;

            return View(fieldCategories);
        }

        public IActionResult UpdateProductFileds(int id, string result)
        {
            char[] dash = new char[] { '-' };
            string[] strResult = result.Split(dash);

            _store.DeleteAllProductFields(id);

            List<ProductFieldViewModel> models = new List<ProductFieldViewModel>();

            int counter = 1;

            foreach (var item in strResult)
            {
                ProductFieldViewModel productField = new ProductFieldViewModel()
                {
                    Id = counter,
                    Value = item
                };

                models.Add(productField);

                counter += 1;
            }

            Product product = _store.GetProduct(id);
            int categoryId = _store.GetCategory(product.CategoryId).Id;

            var categories = _context.Categories.ToList();
            var category = categories.FirstOrDefault(c => c.Id == categoryId);
            while (category.ParentId != null)
            {
                category = category.Parent;
            }
            List<FieldCategory> fieldCategories = _store.GetFieldCategories(category.Id);

            counter = 1;

            foreach (var item in fieldCategories)
            {
                ProductFieldViewModel viewModel = models.FirstOrDefault(x => x.Id == counter);

                ProductField productField = new ProductField()
                {
                    ProductId = id,
                    Value = viewModel.Value,
                    FieldId = item.FieldId
                };

                _store.AddProductField(productField);

                counter += 1;
            }

            return Redirect("/Panel/ShowProducts/");
        }

        public IActionResult ShowCoupons()
        {
            string username = User.Identity.Name;

            Store store = _user.GetUserStore(username);

            var coupons = _store.GetCoupons(store.UserId);
            List<StoreCouponViewModel> storeCouponViewModel = new List<StoreCouponViewModel>();
            foreach (var item in coupons)
            {
                storeCouponViewModel.Add(new StoreCouponViewModel()
                {
                    Id = item.Id,
                    Code = item.Code,
                    Desc = item.Desc,
                    EndDate = _pc.GetYear(item.ExpireDateTime) + "/" + _pc.GetMonth(item.ExpireDateTime) + "/" + _pc.GetDayOfMonth(item.ExpireDateTime),
                    IsExpire = item.IsExpire,
                    Name = item.Name,
                    Percent = item.Percent,
                    Price = item.Price,
                    StartDate = _pc.GetYear(item.StartDateTime) + "/" + _pc.GetMonth(item.StartDateTime) + "/" + _pc.GetDayOfMonth(item.StartDateTime)
                });
            }

            return View(storeCouponViewModel);
        }

        public IActionResult AddCoupon()
        {
            string strToday = _pc.GetYear(DateTime.Now).ToString("0000") + "/" +
                _pc.GetMonth(DateTime.Now).ToString("00") + "/" + _pc.GetDayOfMonth(DateTime.Now).ToString("00");

            ViewBag.MyDate = strToday;

            return View();
        }

        [HttpPost]
        public IActionResult AddCoupon(StoreCouponViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (_store.ExistsCouponCode(viewModel.Code))
                {
                    return View(viewModel);
                }
                else
                {
                    string username = User.Identity.Name;

                    Store store = _user.GetUserStore(username);

                    string[] StartDate = viewModel.StartDate.Split("/");
                    string[] EndDate = viewModel.EndDate.Split("/");
                    for (int i = 0; i < StartDate.Length; i++)
                    {
                        StartDate[i] = StartDate[i].Replace("/", "");
                        EndDate[i] = EndDate[i].Replace("/", "");
                    }
                    Coupon coupon = new Coupon()
                    {
                        Code = viewModel.Code,
                        Desc = viewModel.Desc,
                        ExpireDateTime = _pc.ToDateTime(Convert.ToInt32(EndDate[0]), Convert.ToInt32(EndDate[1]), Convert.ToInt32(EndDate[2]), 0, 0, 0, 0),
                        IsExpire = false,
                        StoreId = store.UserId,
                        Name = viewModel.Name,
                        Percent = viewModel.Percent,
                        Price = viewModel.Price,
                        StartDateTime = _pc.ToDateTime(Convert.ToInt32(StartDate[0]), Convert.ToInt32(StartDate[1]), Convert.ToInt32(StartDate[2]), 0, 0, 0, 0)
                    };

                    _store.AddCoupon(coupon);

                    return RedirectToAction(nameof(ShowCoupons));
                }
            }

            return View(viewModel);
        }

        public IActionResult EditCoupon(int id)
        {
            Coupon coupon = _store.GetCoupon(id);



            StoreCouponViewModel viewModel = new StoreCouponViewModel()
            {
                Code = coupon.Code,
                Desc = coupon.Desc,
                EndDate = _pc.GetYear(coupon.ExpireDateTime).ToString("0000") + "/" + _pc.GetMonth(coupon.ExpireDateTime).ToString("00") + "/" + _pc.GetDayOfMonth(coupon.ExpireDateTime).ToString("00"),
                Name = coupon.Name,
                Percent = coupon.Percent,
                Price = coupon.Price,
                StartDate = _pc.GetYear(coupon.StartDateTime).ToString("0000") + "/" + _pc.GetMonth(coupon.StartDateTime).ToString("00") + "/" + _pc.GetDayOfMonth(coupon.StartDateTime).ToString("00"),
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult EditCoupon(int id, StoreCouponViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Coupon coupon = _store.GetCoupon(id);
                string[] StartDate = viewModel.StartDate.Split("/");
                string[] EndDate = viewModel.EndDate.Split("/");
                for (int i = 0; i < StartDate.Length; i++)
                {
                    StartDate[i] = StartDate[i].Replace("/", "");
                    EndDate[i] = EndDate[i].Replace("/", "");
                }


                DateTime sdate = _pc.ToDateTime(Convert.ToInt32(StartDate[0]), Convert.ToInt32(StartDate[1]), Convert.ToInt32(StartDate[2]), 0, 0, 0, 0);
                DateTime edate = _pc.ToDateTime(Convert.ToInt32(EndDate[0]), Convert.ToInt32(EndDate[1]), Convert.ToInt32(EndDate[2]), 0, 0, 0, 0);
                _store.UpdateCoupon(id, viewModel.Name, viewModel.Code, viewModel.IsExpire, viewModel.Desc,
                    sdate, edate, viewModel.Percent, viewModel.Price);

                return RedirectToAction(nameof(ShowCoupons));
            }

            return View(viewModel);
        }

        public IActionResult DetailsCoupon(int id)
        {
            Coupon coupon = _store.GetCoupon(id);
            StoreCouponViewModel storeCouponViewModel = new StoreCouponViewModel()
            {
                Id = coupon.Id,
                Code = coupon.Code,
                Desc = coupon.Desc,
                EndDate = _pc.GetYear(coupon.ExpireDateTime) + "/" + _pc.GetMonth(coupon.ExpireDateTime) + "/" + _pc.GetDayOfMonth(coupon.ExpireDateTime),
                IsExpire = coupon.IsExpire,
                Name = coupon.Name,
                Percent = coupon.Percent,
                Price = coupon.Price,
                StartDate = _pc.GetYear(coupon.StartDateTime) + "/" + _pc.GetMonth(coupon.StartDateTime) + "/" + _pc.GetDayOfMonth(coupon.StartDateTime)
            };
            return View(storeCouponViewModel);
        }

        public IActionResult DeleteCoupon(int? id)
        {
            return View();
        }

        [HttpPost]
        public IActionResult DeleteCoupon(int id)
        {
            if (ModelState.IsValid)
            {
                _store.RemoveCoupon(id);

                return RedirectToAction(nameof(ShowCoupons));
            }

            return View();
        }

        public IActionResult BankCard()
        {
            string username = User.Identity.Name;

            Store store = _user.GetUserStore(username);

            StoreBankViewModel viewModel = new StoreBankViewModel()
            {
                BankCard = store.BankCard
            };

            ViewBag.IsSuccess = false;

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult BankCard(StoreBankViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                string username = User.Identity.Name;

                Store store = _user.GetUserStore(username);

                _store.UpdateBankCard(store.UserId, viewModel.BankCard);

                ViewBag.IsSuccess = true;

                return View(viewModel);
            }

            ViewBag.IsSuccess = false;

            return View(viewModel);
        }
        public IActionResult ShowOrders()
		{
            var userId = _context.Users.FirstOrDefault(u => u.Mobile == User.Identity.Name).Id;
            var op = _context.OrderProducts.Include(op => op.Order).Include(op => op.Product).ThenInclude(p => p.Brand).Where(op => op.Product.StoreId == userId).OrderBy(op => op.Status).ToList();
            return View(op);
		}
        [HttpGet]
        public IActionResult ConfirmOrder(int? orderId, int? productId)
        {
            return View(new List<int>() { orderId.Value, productId.Value });
        }
        [HttpPost]
        public IActionResult ConfirmOrder(int orderId,int productId)
		{
            var userId = _context.Users.FirstOrDefault(u => u.Mobile == User.Identity.Name).Id;
            var op = _context.OrderProducts.Include(op => op.Product).FirstOrDefault(op => op.OrderId==orderId && op.ProductId == productId && op.Product.StoreId==userId);
            op.Status = OrderStatusEnum.ارسالشده;
            _context.Update(op);
            _context.SaveChanges();
            return RedirectToAction(nameof(ShowOrders));
		}
    }
}