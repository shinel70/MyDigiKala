using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace DigiKala.Core.ViewModels
{
    public class PermissionViewModel
    {
        [Display(Name = "نام دسترسی")]
        [Required(ErrorMessage = "نباید بدون مقدار باشد")]
        [MaxLength(50, ErrorMessage = "مقدار {0} نباید بیش تر از {1} کاراکتر باشد")]
        public string Name { get; set; }
    }
}
