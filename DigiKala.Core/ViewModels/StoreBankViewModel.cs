using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace DigiKala.Core.ViewModels
{
    public class StoreBankViewModel
    {
        [Display(Name = "شماره شبا")]
        [Required(ErrorMessage = "لطفا شماره شبا معتبر وارد نمایید")]
        [MaxLength(24, ErrorMessage = "مقدار {0} نباید بیش تر از {1} کاراکتر باشد")]
        [MinLength(24, ErrorMessage = "مقدار {0} نباید کم تر از {1} کاراکتر باشد")]
        [Phone(ErrorMessage = "لطفا شماره شبا معتبر وارد نمایید")]
        public string BankCard { get; set; }
    }
}
