using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DigiKala.Core.ViewModels
{
    public class StoreCategoryViewModel
    {
        public int CategoryId { get; set; }


        [Display(Name = "فایل مدارک")]
        public IFormFile Img { get; set; }

        public string ImgName { get; set; }
    }
}
