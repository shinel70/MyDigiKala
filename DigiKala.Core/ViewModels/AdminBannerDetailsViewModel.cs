using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DigiKala.Core.ViewModels
{
    public class AdminBannerDetailsViewModel
    {
        [Display(Name = "عنوان")]
        [MaxLength(100, ErrorMessage = "مقدار {0} نباید بیش تر از {1} کاراکتر باشد")]
        public string Name { get; set; }

        [Display(Name = "تصویر بنر")]
        public IFormFile Img { get; set; }

        public string ImgName { get; set; }

        [Display(Name = "آدرس لینک")]
        public string Url { get; set; }
    }
}
