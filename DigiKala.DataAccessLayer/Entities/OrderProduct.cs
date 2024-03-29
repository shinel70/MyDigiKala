﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DigiKala.DataAccessLayer.Entities
{
	public class OrderProduct
	{
		public int OrderId { get; set; }
		public int ProductId { get; set; }
		[Range(0,5)]
		public float? Rating { get; set; }
		[Display(Name = "تعداد")]
		public int Qty { get; set; }
		[Display(Name ="وضعیت")]
		public OrderStatusEnum Status { get; set; } = OrderStatusEnum.درحالارسال;
		public virtual Order Order { get; set; }
		public virtual Product Product { get; set; }
		[Display(Name = "قیمت")]
		public ulong Price { get; set; }
		[Display(Name = "قیمت قبل")]
		public ulong DeletePrice { get; set; }
	}
}
