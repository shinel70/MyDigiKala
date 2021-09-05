using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigiKala.DataAccessLayer.Entities
{
    public class StoreCategory
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public int CategoryId { get; set; }

        [Display(Name = "تاریخ و زمان")]
        public DateTime DateTime { get; set; }


        [Display(Name = "تأیید")]
        public bool IsActive { get; set; }

        [Display(Name = "سایر توضیحات")]
        public string Desc { get; set; }

        [Display(Name = "تصویر مدارک")]
        [MaxLength(100, ErrorMessage = "مقدار {0} نباید بیش تر از {1} کاراکتر باشد")]
        public string Img { get; set; }

        [ForeignKey("UserId")]
        public virtual Store Store { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
    }
}
