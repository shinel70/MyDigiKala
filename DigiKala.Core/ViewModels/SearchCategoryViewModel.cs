using System;
using System.Collections.Generic;
using System.Text;

using DigiKala.DataAccessLayer.Entities;

namespace DigiKala.Core.ViewModels
{
    public class SearchCategoryViewModel
    {
        public List<Product> FillProducts { get; set; }

        public List<Category> FillCategories { get; set; }

        public Category FillParentCategory { get; set; }

        public Category FillSelectCategory { get; set; }

        public List<Store> FillStores { get; set; }

        public List<Brand> FillBrands { get; set; }
    }
}
