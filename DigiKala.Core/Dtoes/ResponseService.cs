using System;
using System.Collections.Generic;
using System.Text;

namespace DigiKala.Core.Dtoes
{
	public class ResponseService<T>
	{
		public T Data { get; set; }
		public bool Success { get; set; } = true;
		public string Message { get; set; } = null;
	}
}
