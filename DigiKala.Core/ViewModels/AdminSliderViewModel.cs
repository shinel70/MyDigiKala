using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DigiKala.Core.ViewModels
{
    public class AdminSliderViewModel
    {
        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "نباید بدون مقدار باشد")]
        [MaxLength(100, ErrorMessage = "مقدار {0} نباید بیش تر از {1} کاراکتر باشد")]
        public string Title { get; set; }

        [Display(Name = "عدم نمایش")]
        public bool NotShow { get; set; }

        [Display(Name = "ترتیب نمایش")]
        public int OrderShow { get; set; }

        [Display(Name = "سایر توضیحات")]
        [DataType(DataType.MultilineText)]
        public string Desc { get; set; }

        [Display(Name = "لوگو")]
        public IFormFile Img { get; set; }

        public string ImgName { get; set; }
    }
}
