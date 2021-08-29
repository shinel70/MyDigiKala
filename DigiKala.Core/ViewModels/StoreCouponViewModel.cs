using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace DigiKala.Core.ViewModels
{
    public class StoreCouponViewModel
    {
        [Display(Name = "نام کوپن")]
        [Required(ErrorMessage = "نباید بدون مقدار باشد")]
        [MaxLength(100, ErrorMessage = "مقدار {0} نباید بیش تر از {1} کاراکتر باشد")]
        public string Name { get; set; }

        [Display(Name = "کد تخفیف")]
        [Required(ErrorMessage = "نباید بدون مقدار باشد")]
        [MaxLength(40, ErrorMessage = "مقدار {0} نباید بیش تر از {1} کاراکتر باشد")]
        public string Code { get; set; }

        [Display(Name = "سایر توضیحات")]
        [DataType(DataType.MultilineText)]
        public string Desc { get; set; }

        [Display(Name = "درصد تخفیف")]
        public int Percent { get; set; }

        [Display(Name = "مبلغ تخفیف")]
        public int Price { get; set; }

        [Display(Name = "تاریخ شروع")]
        [MaxLength(10, ErrorMessage = "مقدار {0} نباید بیش تر از {1} کاراکتر باشد")]
        public string StartDate { get; set; }

        // 1399/01/01
        [Display(Name = "تاریخ پایان")]
        [MaxLength(10, ErrorMessage = "مقدار {0} نباید بیش تر از {1} کاراکتر باشد")]
        public string EndDate { get; set; }

        [Display(Name = "منقضی شده")]
        public bool IsExpire { get; set; }
    }
}
