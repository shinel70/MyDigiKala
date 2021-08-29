using System;
using System.Collections.Generic;
using System.Text;

using DigiKala.DataAccessLayer.Entities;

namespace DigiKala.Core.Interfaces
{
    public interface ITemp
    {
        #region Category

        List<Category> GetCategories();

        List<Category> GetCategoryById(int id);

        Category GetCategory(int id);

        #endregion

        #region Slider

        List<Slider> GetSliders();

        #endregion

        #region Banner

        List<Banner> GetBanners();

        bool CheckBannerImg(int id);

        BannerDetails GetBannerDetails(int id);

        List<BannerDetails> GetBannerDetailsNoExpire();

        void UpdateBannerExpire(int id);

        #endregion

        List<Product> GetProducts(int id);

        List<Store> GetStores(int id);

        List<Brand> GetBrands(int id);

        #region ProductDetails

        Product GetProductDetail(int id);

        #endregion

    }
}
