using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiKala.Core.ViewModels
{
    public class UserRegisterViewModel
    {
        [Display(Name = "شماره موبایل")]
        [Required(ErrorMessage = "نباید بدون مقدار باشد")]
        [MaxLength(11, ErrorMessage = "مقدار {0} نباید بیش تر از {1} کاراکتر باشد")]
        public string Mobile { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "نباید بدون مقدار باشد")]
        [MaxLength(100, ErrorMessage = "مقدار {0} نباید بیش تر از {1} کاراکتر باشد")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "تایید کلمه عبور")]
        [Required(ErrorMessage = "نباید بدون مقدار باشد")]
        [MaxLength(100, ErrorMessage = "مقدار {0} نباید بیش تر از {1} کاراکتر باشد")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "کلمه های عبور همخوانی ندارند")]
        public string ConfirmPassword { get; set; }
        [Display(Name = "کد فعال سازی موبایل")]
        [Required(ErrorMessage = "نباید بدون مقدار باشد")]
        [MaxLength(6, ErrorMessage = "مقدار {0} نباید بیش تر از {1} کاراکتر باشد")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "فقط عدد میتوانید وارد کنید")]
        public string ActiveCode { get; set; }
    }
}
