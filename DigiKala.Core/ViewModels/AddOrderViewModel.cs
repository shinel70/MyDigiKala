using System;
using System.Collections.Generic;
using System.Text;

namespace DigiKala.Core.ViewModels
{
	public  class AddOrderViewModel
	{
		public string ProductIdQties { get; set; }
		public int UserAddressId { get; set; }
		public string Address { get; set; }
		public bool IsAddressBecomeSave { get; set; }
	}
}
