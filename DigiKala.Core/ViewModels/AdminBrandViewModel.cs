using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DigiKala.Core.ViewModels
{
    public class AdminBrandViewModel
    {
        [Display(Name = "برند")]
        [MaxLength(100, ErrorMessage = "مقدار {0} نباید بیش تر از {1} کاراکتر باشد")]
        public string Name { get; set; }

        [Display(Name = "عدم نمایش")]
        public bool NotShow { get; set; }

        [Display(Name = "لوگو")]
        public IFormFile Img { get; set; }

        public string ImgName { get; set; }
    }
}
