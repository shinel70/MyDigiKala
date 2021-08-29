using System;
using System.Collections.Generic;
using System.Text;

using DigiKala.DataAccessLayer.Entities;

namespace DigiKala.Core.Interfaces
{
    public interface IStore
    {
        Store GetStore(int id);

        bool UpdateStore(int id, string name, string tel, string address, string des, string logo);

        List<Category> GetCategoriesByNullParent();

        void UpdateBankCard(int id, string card);

        #region StoreCategory

        void AddStoreCategory(StoreCategory storeCategory);
        void UpdateStoreCategory(int id, int categoryid, string img);
        void DeleteStoreCategory(int id);
        List<StoreCategory> GetStoreCategoriesByStoreId(int id);
        StoreCategory GetStoreCategory(int id);

        #endregion

        #region Brand

        void AddBrand(Brand brand);

        List<Brand> GetBrands(int id);

        List<Brand> AllBrands();

        bool ExistsBrand(string name);

        #endregion

        #region Product

        void AddProduct(Product product);

        void DeleteProduct(int id);

        void UpdateProduct(int id, int brandid, int catid, string name, string img, int price, int delprice, int exists, bool notshow, string desc);

        Product GetProduct(int id);

        List<Product> GetProducts(int id);

        List<Category> GetCategories(int id);

        Category GetCategory(int id);

        List<FieldCategory> GetFieldCategories(int id);

        int GetProductSeen(int id);

        #endregion

        #region Gallery

        void AddGallery(ProductGallery productGallery);

        void DeleteGallery(int id);

        ProductGallery GetProductGallery(int id);

        List<ProductGallery> GetProductGalleries(int id);

        #endregion

        #region ProductField

        void AddProductField(ProductField productField);

        void DeleteAllProductFields(int id);

        List<ProductField> GetProductFields(int id);

        ProductField GetProductField(int id, int pid);

        #endregion

        #region Coupon

        bool ExistsCouponCode(string code);

        void AddCoupon(Coupon coupon);

        void UpdateCoupon(int id, string name, string code, bool expire, string desc, string start, string end, int percent, int price);

        void RemoveCoupon(int id);

        List<Coupon> GetCoupons(int id);

        Coupon GetCoupon(int id);

        #endregion
    }
}
