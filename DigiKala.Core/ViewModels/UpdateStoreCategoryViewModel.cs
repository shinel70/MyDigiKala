using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace DigiKala.Core.ViewModels
{
    public class UpdateStoreCategoryViewModel
    {
        [Display(Name = "تأیید")]
        public bool IsActive { get; set; }

        [Display(Name = "سایر توضیحات")]
        [DataType(DataType.MultilineText)]
        public string Desc { get; set; }

        public string Img { get; set; }
    }
}
