using System;
using System.Collections.Generic;
using System.Text;

namespace DigiKala.DataAccessLayer.Entities
{
	public class UserAddress
	{
		public int Id { get; set; }
		public string Address { get; set; }
		public int UserId { get; set; }
		public virtual User User { get; set; }
	}
}
