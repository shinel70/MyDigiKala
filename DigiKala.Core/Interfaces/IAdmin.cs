using System;
using System.Collections.Generic;
using System.Text;

using DigiKala.DataAccessLayer.Entities;

namespace DigiKala.Core.Interfaces
{
    public interface IAdmin
    {
        #region For Setting

        void InsertSetting(Setting setting);

        void UpdateSetting(string name, string desc, string keys, string api, string sender, string mail, string password);

        bool ExistsSetting();

        Setting GetSetting();

        #endregion

        #region For Permission

        void InsertPermission(Permission permission);

        void UpdatePermission(int id, string name);

        void DeletePermission(int id);

        List<Permission> GetPermissions();

        Permission GetPermission(int id);

        #endregion

        #region Category

        void InsertCategory(Category category);

        void UpdateCategory(int id, string name, string icon);

        void UpdateSubCategory(int id, int parentid, string name);

        void DeleteCategory(int id);

        Category GetCategory(int id);

        List<Category> GetCategories();

        List<Category> GetSubCategories();

        int? GetCategoryParentId(int id);

        List<Category> GetCategoriesByParentId(int id);

        #endregion

        #region Store

        List<Store> GetActiveStores();

        #endregion

        #region StoreCategory

        void UpdateStoreCategory(int id, bool isactive, string desc);
        List<StoreCategory> GetStoreCategories();
        StoreCategory GetStoreCategory(int id);
        void DeleteStoreCategory(int id);

        #endregion

        #region Brand

        void AddBrand(Brand brand);

        void UpdateBrand(int id, string name, string img, bool notshow);

        void DeleteBrand(int id);

        Brand GetBrand(int id);

        List<Brand> GetBrands();

        List<Brand> GetBrandsByStoreId(int id);

        int GetBrandsNotShowCount();

        #endregion

        #region Field

        void AddField(Field field);

        void UpdateField(int id, string name);

        void DeleteField(int id);

        Field GetField(int id);

        List<Field> GetFields();

        #endregion

        #region FieldCategory

        void DeleteAll(int id);

        void AddFieldCategory(FieldCategory fieldCategory);

        List<FieldCategory> GetFieldCategories(int id);

        bool ExistsFieldCategory(int id, int catid);

        #endregion

        #region Product

        List<Product> GetProducts();

        List<Product> GetProductsByStore(int id);

        Product GetProduct(int id);

        List<ProductField> GetProductFields(int id);

        List<ProductGallery> GetProductGalleries(int id);

        void RemoveProduct(int id);

        List<ProductSeen> GetProductSeens(int id);

        #endregion

        #region Slider

        List<Slider> GetSliders();

        Slider GetSlider(int id);

        void UpdateSlider(int id, string title, string img, string desc, bool notshow, int order);

        void DeleteSlider(int id);

        void AddSlider(Slider slider);

        #endregion

        #region Banner

        List<Banner> GetBanners();

        Banner GetBanner(int id);

        void UpdateBanner(int id, string name, string defaultimg, string desc, string size, int day, int price);

        void AddBannerDetails(BannerDetails bannerDetails);

        List<BannerDetails> GetBannerDetails(int id);

        BannerDetails GetBannerDetailsById(int id);

        #endregion

        #region Coupon

        List<Coupon> GetCoupons();

        List<Coupon> GetCouponsByStoreId(int id);

        Coupon GetCoupon(int id);

        void RemoveCoupon(int id);

        #endregion
    }
}
