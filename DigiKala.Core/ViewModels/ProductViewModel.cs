using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DigiKala.Core.ViewModels
{
    public class ProductViewModel
    {
        public int BrandId { get; set; }

        public int CategoryId { get; set; }

        [Display(Name = "نام محصول")]
        [MaxLength(100, ErrorMessage = "مقدار {0} نباید بیش تر از {1} کاراکتر باشد")]
        public string Name { get; set; }

        [Display(Name = "تصویر شاخص")]
        public IFormFile Img { get; set; }

        public string ImgName { get; set; }

        [Display(Name = "قیمت")]
        public int Price { get; set; }

        [Display(Name = "قیمت قبل")]
        public int DeletePrice { get; set; }

        [Display(Name = "موجودی")]
        public int Exist { get; set; }

        [Display(Name = "عدم نمایش")]
        public bool NotShow { get; set; }

        [Display(Name = "سایر توضیحات")]
        [DataType(DataType.MultilineText)]
        public string Desc { get; set; }
    }
}
