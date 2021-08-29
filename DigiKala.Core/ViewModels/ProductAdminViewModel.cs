using System;
using System.Collections.Generic;
using System.Text;

using DigiKala.DataAccessLayer.Entities;

namespace DigiKala.Core.ViewModels
{
    public class ProductAdminViewModel
    {
        public Product FillProduct { get; set; }

        public List<ProductGallery> FillGallery { get; set; }

        public List<ProductField> FillField { get; set; }
    }
}
