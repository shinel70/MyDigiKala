using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DigiKala.DataAccessLayer.Entities
{
    public class Order
	{
		public int Id { get; set; }
		[Display(Name ="آدرس تحویل کالا")]
		[StringLength(100)]
		public string Address { get; set; }
		[Display(Name ="تاریخ ثبت سفارش")]
		public DateTime StartDateTime { get; set; }
		[Display(Name ="تاریخ تحویل کالا")]
		public DateTime? CloseDateTime { get; set; }
		[Display(Name ="شماره پیگیری")]
		public long TrackingNumber { get; set; }
		[Display(Name ="وضعیت")]
		public OrderStatusEnum Status { get; set; }
		public int UserId { get; set; }
		public virtual User User { get; set; }
		public virtual IList<OrderProduct> OrderProducts { get; set; }
	}
}
