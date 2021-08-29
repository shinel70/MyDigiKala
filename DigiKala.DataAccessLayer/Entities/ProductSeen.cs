using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigiKala.DataAccessLayer.Entities
{
    public class ProductSeen
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }

        [Display(Name = "تاریخ")]
        [MaxLength(10, ErrorMessage = "مقدار {0} نباید بیش تر از {1} کاراکتر باشد")]
        public string Date { get; set; }

        [Display(Name = "زمان")]
        [MaxLength(10, ErrorMessage = "مقدار {0} نباید بیش تر از {1} کاراکتر باشد")]
        public string Time { get; set; }

        [Display(Name = "IP")]
        [MaxLength(30, ErrorMessage = "مقدار {0} نباید بیش تر از {1} کاراکتر باشد")]
        public string IP { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}
