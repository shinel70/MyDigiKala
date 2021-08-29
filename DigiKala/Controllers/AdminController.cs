using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using DigiKala.Core.Interfaces;
using DigiKala.Core.Services;
using DigiKala.Core.ViewModels;
using DigiKala.Core.Classes;

using Microsoft.EntityFrameworkCore;

using DigiKala.DataAccessLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;
using X.PagedList;
using System.Reflection;
using System.IO;

namespace DigiKala.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private IAdmin _admin;
        private PersianCalendar pc = new PersianCalendar();

        public AdminController(IAdmin admin)
        {
            _admin = admin;
        }

        public IActionResult ShowActiveStores()
        {
            List<Store> stores = _admin.GetActiveStores();

            return View(stores);
        }

        public IActionResult Setting()
        {
            Setting setting = _admin.GetSetting();

            ViewBag.MyMessage = false;

            if (setting != null)
            {
                AdminSettingViewModel viewModel = new AdminSettingViewModel()
                {
                    MailAddress = setting.MailAddress,
                    MailPassword = setting.MailPassword,
                    SiteDesc = setting.SiteDesc,
                    SiteKeys = setting.SiteKeys,
                    SiteName = setting.SiteName,
                    SmsApi = setting.SmsApi,
                    SmsSender = setting.SmsSender
                };

                return View(viewModel);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public IActionResult Setting(AdminSettingViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (_admin.ExistsSetting())
                {
                    _admin.UpdateSetting(viewModel.SiteName, viewModel.SiteDesc, viewModel.SiteDesc, viewModel.SmsApi, viewModel.SmsSender, viewModel.MailAddress, viewModel.MailPassword);
                }
                else
                {
                    Setting setting = new Setting()
                    {
                        MailAddress = viewModel.MailAddress,
                        MailPassword = viewModel.MailPassword,
                        SiteDesc = viewModel.SiteDesc,
                        SiteKeys = viewModel.SiteKeys,
                        SiteName = viewModel.SiteName,
                        SmsApi = viewModel.SmsApi,
                        SmsSender = viewModel.SmsSender
                    };

                    _admin.InsertSetting(setting);
                }

                ViewBag.MyMessage = true;
            }

            return View(viewModel);
        }

        public IActionResult Permissions(int? page)
        {
            //List<Permission> permissions = _admin.GetPermissions();

            int pageSize = 5;
            var pageNumber = page ?? 1;
            var permissions = _admin.GetPermissions().ToPagedList(pageNumber, pageSize);

            ViewBag.MyModels = permissions;

            return View();
        }

        public IActionResult AddPermission()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddPermission(PermissionViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Permission permission = new Permission()
                {
                    Name = viewModel.Name
                };

                _admin.InsertPermission(permission);

                return RedirectToAction(nameof(Permissions));
            }

            return View(viewModel);
        }

        public IActionResult EditPermission(int id)
        {
            Permission permission = _admin.GetPermission(id);

            PermissionViewModel viewModel = new PermissionViewModel()
            {
                Name = permission.Name
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult EditPermission(int id, PermissionViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                _admin.UpdatePermission(id, viewModel.Name);

                return RedirectToAction(nameof(Permissions));
            }

            return View(viewModel);
        }

        public IActionResult DeletePermission(int? id)
        {
            return View();
        }

        [HttpPost]
        public IActionResult DeletePermission(int id)
        {
            if (ModelState.IsValid)
            {
                _admin.DeletePermission(id);

                return RedirectToAction(nameof(Permissions));
            }

            return View();
        }

        public IActionResult Categories()
        {
            List<Category> categories = _admin.GetCategories();

            return View(categories);
        }

        public IActionResult AddCategory()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddCategory(CategoryViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Category category = new Category()
                {
                    ParentId = null,
                    Icon = viewModel.Icon,
                    Name = viewModel.Name
                };

                _admin.InsertCategory(category);

                return RedirectToAction(nameof(Categories));
            }

            return View(viewModel);
        }

        public IActionResult EditCategory(int id)
        {
            Category category = _admin.GetCategory(id);

            return View(category);
        }

        [HttpPost]
        public IActionResult EditCategory(int id, Category model)
        {
            if (ModelState.IsValid)
            {
                _admin.UpdateCategory(id, model.Name, model.Icon);

                int? parentID = _admin.GetCategoryParentId(id);

                if (parentID != null)
                {
                    Category category1 = _admin.GetCategory((int)parentID);

                    if (category1.ParentId == null)
                    {
                        return Redirect("/Admin/SubCategories/" + parentID);
                    }
                    else
                    {
                        parentID = _admin.GetCategoryParentId((int)parentID);

                        Category category2 = _admin.GetCategory((int)parentID);

                        if (category2.ParentId == null)
                        {
                            return Redirect("/Admin/SubCategories/" + parentID);
                        }
                        else
                        {
                            parentID = _admin.GetCategoryParentId((int)parentID);

                            return Redirect("/Admin/SubCategories/" + parentID);
                        }
                    }
                }
                else
                {
                    return Redirect("/Admin/SubCategories/" + id);
                }
            }

            return View(model);
        }

        public IActionResult DeleteCategory(int? id)
        {
            return View();
        }

        [HttpPost]
        public IActionResult DeleteCategory(int id)
        {
            DeleteCascadeCategory(id);

            return RedirectToAction(nameof(Categories));
        }

        private void DeleteCascadeCategory(int id)
        {
            List<Category> categories = _admin.GetCategoriesByParentId(id);

            if (categories.Count > 0)
            {
                foreach (var item in categories)
                {
                    DeleteCascadeCategory(item.Id);
                }
            }

            _admin.DeleteCategory(id);
        }

        public IActionResult SubCategories(int id)
        {
            List<Category> categories = _admin.GetSubCategories();

            ViewBag.MyId = id;

            return View(categories);
        }

        public IActionResult CreateSubCategory(int id)
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateSubCategory(int id, SubCategoriesViewModel viewModel)
        {
            if (ModelState.IsValid)
            {

                Category category = new Category()
                {
                    Icon = null,
                    Name = viewModel.Name,
                    ParentId = id
                };

                _admin.InsertCategory(category);

                int? parentID = _admin.GetCategoryParentId(id);

                if (parentID != null)
                {
                    Category category1 = _admin.GetCategory((int)parentID);

                    if (category1.ParentId == null)
                    {
                        return Redirect("/Admin/SubCategories/" + parentID);
                    }
                    else
                    {
                        parentID = _admin.GetCategoryParentId((int)parentID);

                        return Redirect("/Admin/SubCategories/" + parentID);
                    }
                }
                else
                {
                    return Redirect("/Admin/SubCategories/" + id);
                }
            }

            return View(viewModel);
        }

        public IActionResult ShowStoreCategories()
        {
            List<StoreCategory> storeCategories = _admin.GetStoreCategories();

            return View(storeCategories);
        }

        public IActionResult EditStoreCategory(int id)
        {
            StoreCategory storeCategory = _admin.GetStoreCategory(id);

            UpdateStoreCategoryViewModel viewModel = new UpdateStoreCategoryViewModel()
            {
                Desc = storeCategory.Desc,
                Img = storeCategory.Img,
                IsActive = storeCategory.IsActive
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult EditStoreCategory(int id, UpdateStoreCategoryViewModel viewModel)
        {
            StoreCategory storeCategory = _admin.GetStoreCategory(id);

            if (ModelState.IsValid)
            {
                _admin.UpdateStoreCategory(id, viewModel.IsActive, viewModel.Desc);

                return RedirectToAction(nameof(ShowStoreCategories));
            }

            UpdateStoreCategoryViewModel storeC = new UpdateStoreCategoryViewModel()
            {
                Desc = storeCategory.Desc,
                Img = storeCategory.Img,
                IsActive = storeCategory.IsActive
            };

            return View(storeC);
        }

        public IActionResult DeleteStoreCategory(int? id)
        {
            return View();
        }

        [HttpPost]
        public IActionResult DeleteStoreCategory(int id)
        {
            _admin.DeleteStoreCategory(id);

            return RedirectToAction(nameof(ShowStoreCategories));
        }

        public IActionResult ShowBrands()
        {
            List<Brand> brands = _admin.GetBrands();

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
                            NotShow = viewModel.NotShow,
                            StoreId = null
                        };

                        _admin.AddBrand(brand);

                        return RedirectToAction(nameof(ShowBrands));
                    }
                }
                else
                {
                    Brand brand = new Brand()
                    {
                        Img = null,
                        Name = viewModel.Name,
                        NotShow = viewModel.NotShow,
                        StoreId = null
                    };

                    _admin.AddBrand(brand);

                    return RedirectToAction(nameof(ShowBrands));
                }
            }

            return View(viewModel);
        }

        public IActionResult EditBrand(int id)
        {
            Brand brand = _admin.GetBrand(id);

            AdminBrandViewModel viewModel = new AdminBrandViewModel()
            {
                Name = brand.Name,
                ImgName = brand.Img,
                NotShow = brand.NotShow
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult EditBrand(AdminBrandViewModel viewModel, int id)
        {
            if (ModelState.IsValid)
            {
                Brand brand = _admin.GetBrand(id);

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

                        _admin.UpdateBrand(id, viewModel.Name, viewModel.ImgName, viewModel.NotShow);

                        return RedirectToAction(nameof(ShowBrands));
                    }
                }
                else
                {
                    _admin.UpdateBrand(id, viewModel.Name, brand.Img, viewModel.NotShow);

                    return RedirectToAction(nameof(ShowBrands));
                }
            }

            return View(viewModel);
        }

        public IActionResult DetailsBrand(int id)
        {
            Brand brand = _admin.GetBrand(id);

            return View(brand);
        }

        public IActionResult DeleteBrand(int? id)
        {
            return View();
        }

        [HttpPost]
        public IActionResult DeleteBrand(int id)
        {
            _admin.DeleteBrand(id);

            return RedirectToAction(nameof(ShowBrands));
        }

        public IActionResult CreateField()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateField(Field model)
        {
            Field field = new Field()
            {
                Name = model.Name
            };

            _admin.AddField(field);

            return RedirectToAction("ShowFields");
        }

        public IActionResult ShowFields()
        {
            List<Field> fields = _admin.GetFields();

            return View(fields);
        }

        public IActionResult EditFields(int id)
        {
            Field field = _admin.GetField(id);

            return View(field);
        }

        [HttpPost]
        public IActionResult EditFields(int id, Field model)
        {
            if (ModelState.IsValid)
            {
                _admin.UpdateField(id, model.Name);

                return RedirectToAction(nameof(ShowFields));
            }

            return View(model);
        }

        public IActionResult DeleteField(int? id)
        {
            return View();
        }

        [HttpPost]
        public IActionResult DeleteField(int id)
        {
            _admin.DeleteField(id);

            return RedirectToAction(nameof(ShowFields));
        }

        public IActionResult FieldCategories(int id)
        {
            List<Field> fields = _admin.GetFields();
            ViewBag.CatId = id;
            return View(fields);
        }

        public IActionResult UpdateFieldCategory(int id, string result)
        {
            _admin.DeleteAll(id);

            int count = result.Length - 1;

            string strID = result.Remove(count, 1);
            char[] dash = new char[] { '-' };
            string[] strResult = strID.Split(dash);

            if (result != "0")
            {
                foreach (string item in strResult)
                {
                    FieldCategory fieldCategory = new FieldCategory()
                    {
                        CategoryId = id,
                        FieldId = Convert.ToInt32(item)
                    };

                    _admin.AddFieldCategory(fieldCategory);
                }
            }

            return RedirectToAction("Categories");
        }

        public IActionResult ShowAllProducts()
        {
            List<Product> products = _admin.GetProducts();

            return View(products);
        }

        public IActionResult ProductSeen(int id)
        {
            List<ProductSeen> productSeens = _admin.GetProductSeens(id);

            return View(productSeens);
        }

        public IActionResult DeleteProduct(int? id)
        {
            return View();
        }

        [HttpPost]
        public IActionResult DeleteProduct(int id)
        {
            _admin.RemoveProduct(id);

            return RedirectToAction(nameof(ShowAllProducts));
        }

        // id == ProductId
        public IActionResult ProductDetails(int id)
        {
            Product product = _admin.GetProduct(id);
            List<ProductGallery> productGalleries = _admin.GetProductGalleries(id);
            List<ProductField> productFields = _admin.GetProductFields(id);

            var viewModel = new ProductAdminViewModel();

            viewModel.FillProduct = product;
            viewModel.FillGallery = productGalleries;
            viewModel.FillField = productFields;

            return View(viewModel);
        }

        public IActionResult ShowSliders()
        {
            List<Slider> sliders = _admin.GetSliders();

            return View(sliders);
        }

        public IActionResult AddSlider()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddSlider(AdminSliderViewModel viewModel)
        {
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
                        filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/sliders/", viewModel.ImgName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            viewModel.Img.CopyTo(stream);
                        }

                        Slider slider = new Slider()
                        {
                            Img = viewModel.ImgName,
                            Title = viewModel.Title,
                            Desc = viewModel.Desc,
                            NotShow = viewModel.NotShow,
                            OrderShow = viewModel.OrderShow
                        };

                        _admin.AddSlider(slider);

                        return RedirectToAction(nameof(ShowSliders));
                    }
                }
                else
                {
                    ModelState.AddModelError("Img", "فایل با پسوند jpg بارگزاری شود");
                }
            }

            return View(viewModel);
        }

        public IActionResult EditSlider(int id)
        {
            Slider slider = _admin.GetSlider(id);

            AdminSliderViewModel viewModel = new AdminSliderViewModel()
            {
                Desc = slider.Desc,
                ImgName = slider.Img,
                NotShow = slider.NotShow,
                OrderShow = slider.OrderShow,
                Title = slider.Title
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult EditSlider(AdminSliderViewModel viewModel, int id)
        {
            if (ModelState.IsValid)
            {
                Slider slider = _admin.GetSlider(id);

                string sliderImg = slider.Img;

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
                        filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/sliders/", viewModel.ImgName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            viewModel.Img.CopyTo(stream);
                        }

                        sliderImg = viewModel.ImgName;
                    }
                }
                
                _admin.UpdateSlider(id, viewModel.Title, sliderImg, viewModel.Desc, viewModel.NotShow, viewModel.OrderShow);

                return RedirectToAction(nameof(ShowSliders));
            }

            return View(viewModel);
        }

        public IActionResult DetailsSlider(int id)
        {
            Slider slider = _admin.GetSlider(id);

            return View(slider);
        }

        public IActionResult DeleteSlider(int? id)
        {
            return View();
        }

        [HttpPost]
        public IActionResult DeleteSlider(int id)
        {
            _admin.DeleteSlider(id);

            return RedirectToAction(nameof(ShowSliders));
        }

        public IActionResult ShowBanners()
        {
            List<Banner> banners = _admin.GetBanners();

            return View(banners);
        }

        public IActionResult DetailsBanner(int id)
        {
            Banner banner = _admin.GetBanner(id);

            return View(banner);
        }

        public IActionResult EditBanner(int id)
        {
            Banner banner = _admin.GetBanner(id);

            AdminBannerViewModel viewModel = new AdminBannerViewModel()
            {
                Day = banner.Day,
                Desc = banner.Desc,
                ImgName = banner.DefaultImg,
                Name = banner.Name,
                Price = banner.Price,
                Size = banner.Size
                
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult EditBanner(AdminBannerViewModel viewModel, int id)
        {
            if (ModelState.IsValid)
            {
                Banner banner = _admin.GetBanner(id);

                string bannerImg = banner.DefaultImg;

                if (viewModel.Img != null)
                {
                    if (Path.GetExtension(viewModel.Img.FileName) != ".png")
                    {
                        ModelState.AddModelError("DefaultImg", "فایل با پسوند png بارگزاری شود");
                    }
                    else
                    {
                        string filePath = "";
                        viewModel.ImgName = CodeGenerators.FileCode() + Path.GetExtension(viewModel.Img.FileName);
                        filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/ads/", viewModel.ImgName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            viewModel.Img.CopyTo(stream);
                        }

                        bannerImg = viewModel.ImgName;
                    }
                }

                _admin.UpdateBanner(id, viewModel.Name, bannerImg, viewModel.Desc, viewModel.Size, viewModel.Day, viewModel.Price);

                return RedirectToAction(nameof(ShowBanners));
            }

            return View(viewModel);
        }

        public IActionResult ShowAllCoupons()
        {
            List<Coupon> coupons = _admin.GetCoupons();

            return View(coupons);
        }

        public IActionResult ShowCouponsByStore(int id)
        {
            List<Coupon> coupons = _admin.GetCouponsByStoreId(id);

            return View(coupons);
        }

        public IActionResult DetailsCoupon(int id)
        {
            Coupon coupon = _admin.GetCoupon(id);

            return View(coupon);
        }

        public IActionResult DeleteCoupon(int? id)
        {
            return View();
        }

        [HttpPost]
        public IActionResult DeleteCoupon(int id)
        {
            _admin.RemoveCoupon(id);

            return RedirectToAction(nameof(ShowAllCoupons));
        }

        public IActionResult ShowBannerDetails(int id)
        {
            List<BannerDetails> bannerDetails = _admin.GetBannerDetails(id);

            ViewBag.MyId = id;

            return View(bannerDetails);
        }

        public IActionResult AddBannerDetails(int id)
        {            
            return View();
        }

        [HttpPost]
        public IActionResult AddBannerDetails(AdminBannerDetailsViewModel viewModel, int id)
        {
            if (ModelState.IsValid)
            {
                string bannerImg = "";

                if (viewModel.Img != null)
                {
                    if (Path.GetExtension(viewModel.Img.FileName) != ".jpg")
                    {
                        ModelState.AddModelError("DefaultImg", "فایل با پسوند jpg بارگزاری شود");
                    }
                    else
                    {
                        string filePath = "";
                        viewModel.ImgName = CodeGenerators.FileCode() + Path.GetExtension(viewModel.Img.FileName);
                        filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/ads/", viewModel.ImgName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            viewModel.Img.CopyTo(stream);
                        }

                        bannerImg = viewModel.ImgName;
                    }
                }

                // 1399/09/14
                string strToday = pc.GetYear(DateTime.Now).ToString("0000") + "/" +
                                pc.GetMonth(DateTime.Now).ToString("00") + "/" +
                                pc.GetDayOfMonth(DateTime.Now).ToString("00");

                Banner banner = _admin.GetBanner(id);

                DateTime dt = pc.ToDateTime(Convert.ToInt32(strToday.Substring(0, 4)), Convert.ToInt32(strToday.Substring(5, 2)),
                    Convert.ToInt32(strToday.Substring(8, 2)), 0, 0, 0, 0);

                DateTime dtExpire = dt.AddDays(Convert.ToDouble(banner.Day));

                BannerDetails bannerDetails = new BannerDetails()
                {
                    BannerId = id,
                    Img = bannerImg,
                    Title = viewModel.Name,
                    IsExpire = false,
                    StartDate = strToday,
                    EndDate = pc.GetYear(dtExpire).ToString("0000") + "/" +
                                pc.GetMonth(dtExpire).ToString("00") + "/" +
                                pc.GetDayOfMonth(dtExpire).ToString("00"),
                    Url = viewModel.Url
                };

                _admin.AddBannerDetails(bannerDetails);

                return Redirect("/Admin/ShowBannerDetails/" + id);
            }

            return View(viewModel);
        }
    }
}