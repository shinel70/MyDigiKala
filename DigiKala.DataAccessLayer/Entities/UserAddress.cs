using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DigiKala.DataAccessLayer.Entities
{
	public class UserAddress
	{
		public int Id { get; set; }
		[Display(Name = "آدرس کاربر")]
		[StringLength(75, MinimumLength = 7, ErrorMessage = "{0} باید بین {1} تا {2} کلمه باشد")]
		public string Address { get; set; }
		public int UserId { get; set; }
		public virtual User User { get; set; }
	}
}
