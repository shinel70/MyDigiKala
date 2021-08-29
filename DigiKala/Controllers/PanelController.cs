using System;
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

namespace DigiKala.Controllers
{
    [Authorize]
    public class PanelController : Controller
    {
        private IUser _user;
        private IStore _store;

        private PersianCalendar pc = new PersianCalendar();

        public PanelController(IUser user, IStore store)
        {
            _user = user;
            _store = store;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Activate()
        {
            ViewBag.IsOK = false;

            return View();
        }

        [HttpPost]
        public IActionResult Activate(StoreActivateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                string username = User.Identity.Name;

                Store store = _user.GetUserStore(username);

                if (_user.ExistsMobileActivate(username, viewModel.MobileCode))
                {
                    if (_user.ExistsMailActivate(username, viewModel.MailCode))
                    {

                        _user.ActiveMobileNumber(username);
                        _user.ActiveMailAddress(store.Mail);

                        ViewBag.IsOK = true;
                    }
                    else
                    {
                        ModelState.AddModelError("MailCode", "کد فعالسازی شما اشتباه است");

                        ViewBag.IsOK = false;
                    }
                }
                else
                {
                    ModelState.AddModelError("MobileCode", "کد فعالسازی شما اشتباه است");

                    ViewBag.IsOK = false;
                }
            }

            return View(viewModel);
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
                    if (Path.GetExtension(viewModel.LogoImg.FileName) != ".jpg")
                    {
                        ModelState.AddModelError("LogoImg", "فایل با پسوند jpg بارگزاری شود");

                        ViewBag.MyStatus = false;
                    }
                    else
                    {
                        string filePath = "";
                        viewModel.LogoName = CodeGenerators.FileCode() + Path.GetExtension(viewModel.LogoImg.FileName);
                        filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/users/stores/", viewModel.LogoName);

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
                            Date = pc.GetYear(DateTime.Now).ToString("0000") + "/" + pc.GetMonth(DateTime.Now).ToString("00") +
                             "/" + pc.GetDayOfMonth(DateTime.Now).ToString("00"),
                            Desc = null,
                            Img = viewModel.ImgName,
                            IsActive = false,
                            UserId = store.UserId,
                            Time = pc.GetHour(DateTime.Now).ToString("00") + ":" + pc.GetMinute(DateTime.Now).ToString("00")
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
                        Date = pc.GetYear(DateTime.Now).ToString("0000") + "/" + pc.GetMonth(DateTime.Now).ToString("00") +
                             "/" + pc.GetDayOfMonth(DateTime.Now).ToString("00"),
                        Desc = null,
                        Img = null,
                        IsActive = false,
                        UserId = store.UserId,
                        Time = pc.GetHour(DateTime.Now).ToString("00") + ":" + pc.GetMinute(DateTime.Now).ToString("00")
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
            Store store = _user.GetUserStore(User.Identity.Name);

            int id = _store.GetStoreCategoriesByStoreId(store.UserId).FirstOrDefault().Id;

            StoreCategory storeCategory = _store.GetStoreCategory(id);

            ViewBag.CategoryId = new SelectList(_store.GetCategories(storeCategory.CategoryId), "Id", "Name");
            ViewBag.BrandId = new SelectList(_store.AllBrands(), "Id", "Name");

            return View();
        }

        [HttpPost]        
        public IActionResult AddProduct(ProductViewModel viewModel)
        {
            string username = User.Identity.Name;

            Store store = _user.GetUserStore(username);

            if (ModelState.IsValid)
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
                            Date = pc.GetYear(DateTime.Now).ToString("0000") + "/" + pc.GetMonth(DateTime.Now).ToString("00") +
                         "/" + pc.GetDayOfMonth(DateTime.Now).ToString("00"),
                            Time = pc.GetHour(DateTime.Now).ToString("00") + ":" + pc.GetMinute(DateTime.Now).ToString("00"),
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

            int id = _store.GetStoreCategoriesByStoreId(store.UserId).FirstOrDefault().Id;

            StoreCategory storeCategory = _store.GetStoreCategory(id);

            ViewBag.CategoryId = new SelectList(_store.GetCategories(storeCategory.CategoryId), "Id", "Name", viewModel.CategoryId);
            ViewBag.BrandId = new SelectList(_store.AllBrands(), "Id", "Name", viewModel.BrandId);

            return View(viewModel);
        }

        public IActionResult EditProduct(int id)
        {
            Store store = _user.GetUserStore(User.Identity.Name);

            int cid = _store.GetStoreCategoriesByStoreId(store.UserId).FirstOrDefault().Id;

            StoreCategory storeCategory = _store.GetStoreCategory(cid);

            ViewBag.CategoryId = new SelectList(_store.GetCategories(storeCategory.CategoryId), "Id", "Name");
            ViewBag.BrandId = new SelectList(_store.AllBrands(), "Id", "Name");

            Product product = _store.GetProduct(id);

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

            if (ModelState.IsValid)
            {
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
            Category category = _store.GetCategory(product.CategoryId);

            int? categoryId = category.Parent.ParentId;

            List<FieldCategory> fieldCategories = _store.GetFieldCategories((int)categoryId);

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
            Category category = _store.GetCategory(product.CategoryId);

            int? categoryId = category.Parent.ParentId;

            List<FieldCategory> fieldCategories = _store.GetFieldCategories((int)categoryId);

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

            List<Coupon> coupons = _store.GetCoupons(store.UserId);

            return View(coupons);
        }

        public IActionResult AddCoupon()
        {
            string strToday = pc.GetYear(DateTime.Now).ToString("0000") + "/" +
                pc.GetMonth(DateTime.Now).ToString("00") + "/" + pc.GetDayOfMonth(DateTime.Now).ToString("00");

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

                    Coupon coupon = new Coupon()
                    {
                        Code = viewModel.Code,
                        Desc = viewModel.Desc,
                        EndDate = viewModel.EndDate,
                        IsExpire = false,
                        StoreId = store.UserId,
                        Name = viewModel.Name,
                        Percent = viewModel.Percent,
                        Price = viewModel.Price,
                        StartDate = viewModel.StartDate                        
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
                EndDate = coupon.EndDate,
                Name = coupon.Name,
                Percent = coupon.Percent,
                Price = coupon.Price,
                StartDate = coupon.StartDate
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult EditCoupon(int id, StoreCouponViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Coupon coupon = _store.GetCoupon(id);

                _store.UpdateCoupon(id, viewModel.Name, viewModel.Code, viewModel.IsExpire, viewModel.Desc,
                    viewModel.StartDate, viewModel.EndDate, viewModel.Percent, viewModel.Price);

                return RedirectToAction(nameof(ShowCoupons));
            }

            return View(viewModel);
        }

        public IActionResult DetailsCoupon(int id)
        {
            Coupon coupon = _store.GetCoupon(id);

            return View(coupon);
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
    }
}