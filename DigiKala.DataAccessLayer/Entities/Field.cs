using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace DigiKala.DataAccessLayer.Entities
{
    public class Field
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "نام مشخصه")]
        [MaxLength(100, ErrorMessage = "مقدار {0} نباید بیش تر از {1} کاراکتر باشد")]
        public string Name { get; set; }

        public virtual ICollection<FieldCategory> FieldCategories { get; set; }

        public virtual ICollection<ProductField> ProductFields { get; set; }
    }
}
