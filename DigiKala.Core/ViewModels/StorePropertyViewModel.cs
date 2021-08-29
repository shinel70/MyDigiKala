using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DigiKala.Core.ViewModels
{
    public class StorePropertyViewModel
    {
        [Display(Name = "نام فروشگاه")]
        [Required(ErrorMessage = "نباید بدون مقدار باشد")]
        [MaxLength(100, ErrorMessage = "مقدار {0} نباید بیش تر از {1} کاراکتر باشد")]
        public string Name { get; set; }

        [Display(Name = "آدرس فروشگاه")]
        [Required(ErrorMessage = "نباید بدون مقدار باشد")]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }

        [Display(Name = "شماره های تماس")]
        [Required(ErrorMessage = "نباید بدون مقدار باشد")]
        [MaxLength(40, ErrorMessage = "مقدار {0} نباید بیش تر از {1} کاراکتر باشد")]
        public string Tel { get; set; }

        [Display(Name = "فایل لوگو")]
        public IFormFile LogoImg { get; set; }
             
        public string LogoName { get; set; }

        [Display(Name = "سایر توضیحات")]
        [DataType(DataType.MultilineText)]
        public string Desc { get; set; }
    }
}
