using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DigiKala.Core.ViewModels
{
    public class AdminBannerViewModel
    {
        [Display(Name = "نام جایگاه")]
        [MaxLength(100, ErrorMessage = "مقدار {0} نباید بیش تر از {1} کاراکتر باشد")]
        public string Name { get; set; }

        [Display(Name = "سایز جایگاه")]
        [MaxLength(50, ErrorMessage = "مقدار {0} نباید بیش تر از {1} کاراکتر باشد")]
        public string Size { get; set; }

        [Display(Name = "توضیحات")]
        [DataType(DataType.MultilineText)]
        public string Desc { get; set; }

        [Display(Name = "مبلغ اجاره")]
        public int Price { get; set; }

        [Display(Name = "تعداد روز")]
        public int Day { get; set; }

        [Display(Name = "تصویر پیش فرض")]
        public IFormFile Img { get; set; }

        public string ImgName { get; set; }
    }
}
