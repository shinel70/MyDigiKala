using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DigiKala.DataAccessLayer.Entities
{
    public class TemporaryCode
    {
        public int Id { get; set; }
        public string Identity { get; set; }
        [Display(Name = "کد فعال سازی موبایل یا ایمیل")]
        [Required(ErrorMessage = "نباید بدون مقدار باشد")]
        [MaxLength(6, ErrorMessage = "مقدار {0} نباید بیش تر از {1} کاراکتر باشد")]
        public string ActiveCode { get; set; }
        public DateTime ExpireDateTime { get; set; }

    }
}
