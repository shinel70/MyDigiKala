using DigiKala.DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DigiKala.Core.ViewModels
{
	public class AddOrderViewModel
	{
		public List<Product> Products { get; set; }
		public User User { get; set; }
		[Display(Name ="آدرس تحویل کالا")]
		[StringLength(90,MinimumLength =5,ErrorMessage ="{0} باید بین {2} تا {1} کلمه باشد}"),Required(ErrorMessage ="{0} نباید بدون مقدار باشد")]
		public string Address { get; set; }
	}
}
