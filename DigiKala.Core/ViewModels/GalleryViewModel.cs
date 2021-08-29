using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DigiKala.Core.ViewModels
{
    public class GalleryViewModel
    {
        [Display(Name = "تصویر گالری")]
        public IFormFile Img { get; set; }

        public string ImgName { get; set; }
    }
}
