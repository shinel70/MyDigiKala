using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigiKala.DataAccessLayer.Entities
{
    public class BannerDetails
    {
        [Key]
        public int Id { get; set; }

        public int BannerId { get; set; }

        [Display(Name = "عنوان")]
        [MaxLength(100, ErrorMessage = "مقدار {0} نباید بیش تر از {1} کاراکتر باشد")]
        public string Title { get; set; }

        [Display(Name = "تصویر بنر")]
        [MaxLength(100, ErrorMessage = "مقدار {0} نباید بیش تر از {1} کاراکتر باشد")]
        public string Img { get; set; }

        [Display(Name = "تاریخ شروع")]
        [MaxLength(10, ErrorMessage = "مقدار {0} نباید بیش تر از {1} کاراکتر باشد")]
        public string StartDate { get; set; }

        [Display(Name = "تاریخ پایان")]
        [MaxLength(10, ErrorMessage = "مقدار {0} نباید بیش تر از {1} کاراکتر باشد")]
        public string EndDate { get; set; }

        [Display(Name = "آدرس سایت")]
        public string Url { get; set; }

        [Display(Name = "منقضی")]
        public bool IsExpire { get; set; }

        [ForeignKey("BannerId")]
        public virtual Banner Banner { get; set; }
    }
}
