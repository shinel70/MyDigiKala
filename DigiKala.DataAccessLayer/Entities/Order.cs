using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DigiKala.DataAccessLayer.Entities
{
    public class Order
	{
		public int Id { get; set; }
		[StringLength(100)]
		public string Address { get; set; }
		public DateTime StartDateTime { get; set; }
		public DateTime? CloseDateTime { get; set; }
		public int UserId { get; set; }
		public User User { get; set; }
		public IList<OrderProduct> OrderProducts { get; set; }
	}
}
