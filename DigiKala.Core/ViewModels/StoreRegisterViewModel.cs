using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiKala.Core.ViewModels
{
    public class StoreRegisterViewModel
    {
        [Display(Name = "ایمیل")]
        [Required(ErrorMessage = "نباید بدون مقدار باشد")]
        [EmailAddress(ErrorMessage = "لطفا ایمیل معتبر وارد کنید")]
        public string Mail { get; set; }

        [Display(Name = "شماره موبایل")]
        [Required(ErrorMessage = "نباید بدون مقدار باشد")]
        [MaxLength(11, ErrorMessage = "مقدار {0} نباید بیش تر از {1} کاراکتر باشد")]
        [MinLength(11, ErrorMessage = "مقدار {0} نباید کم تر از {1} کاراکتر باشد")]
        [Phone(ErrorMessage = "فقط عدد می توانید وارد کنید")]
        public string Mobile { get; set; }
        [Display(Name = "کد فعال سازی موبایل")]
        [Required(ErrorMessage = "نباید بدون مقدار باشد")]
        [MaxLength(6, ErrorMessage = "مقدار {0} نباید بیش تر از {1} کاراکتر باشد")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "فقط عدد میتوانید وارد کنید")]
        public string ActiveMobileCode { get; set; }
        [Display(Name = "کد فعال سازی ایمیل")]
        [Required(ErrorMessage = "نباید بدون مقدار باشد")]
        [MaxLength(6, ErrorMessage = "مقدار {0} نباید بیش تر از {1} کاراکتر باشد")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "فقط عدد میتوانید وارد کنید")]
        public string ActiveEmailCode { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "نباید بدون مقدار باشد")]
        [MaxLength(100, ErrorMessage = "مقدار {0} نباید بیش تر از {1} کاراکتر باشد")]
        [MinLength(4, ErrorMessage = "مقدار {0} نباید کم تر از {1} کاراکتر باشد")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
