using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigiKala.DataAccessLayer.Entities
{
    public class Coupon
    {
        [Key]
        public int Id { get; set; }

        public int StoreId { get; set; }

        [Display(Name = "نام کوپن")]
        [MaxLength(100, ErrorMessage = "مقدار {0} نباید بیش تر از {1} کاراکتر باشد")]
        public string Name { get; set; }

        [Display(Name = "کد تخفیف")]
        [MaxLength(40, ErrorMessage = "مقدار {0} نباید بیش تر از {1} کاراکتر باشد")]
        public string Code { get; set; }

        [Display(Name = "سایر توضیحات")]
        public string Desc { get; set; }

        [Display(Name = "درصد تخفیف")]
        public int Percent { get; set; }

        [Display(Name = "مبلغ تخفیف")]
        public int Price { get; set; }

        [Display(Name = "تاریخ و زمان شروع")]
        [Required(ErrorMessage = "نباید بدون مقدار باشد")]
        public DateTime StartDateTime { get; set; }

        // 1399/01/01
        [Display(Name = "تاریخ و زمان پایان")]
        [Required(ErrorMessage = "نباید بدون مقدار باشد")]
        public DateTime ExpireDateTime { get; set; }

        [Display(Name = "منقضی شده")]
        public bool IsExpire { get; set; }

        [ForeignKey("StoreId")]
        public virtual Store Store { get; set; }
    }
}
