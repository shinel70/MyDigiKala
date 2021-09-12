using System;
using System.Collections.Generic;
using System.Text;

namespace DigiKala.DataAccessLayer.Entities
{
	public class OrderProduct
	{
		public int OrderId { get; set; }
		public int ProductId { get; set; }
		public float? Rating { get; set; }
		public virtual Order Order { get; set; }
		public virtual Product Product { get; set; }
	}
}
