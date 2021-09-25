using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DigiKala.Core.Dtoes
{
	public class AddCommentDto
	{
		[Required]
		public int UserId { get; set; }
		[Required]
		public int ProductId { get; set; }
		public int? ReplyCommentId { get; set; }
		[Required(ErrorMessage ="این فیلد نباید خالی باشد")]
		[Display(Name ="نظر")]
		[StringLength(90,ErrorMessage ="{0} نباید بیشتر از {1} کلمه باشد")]
		public string Text { get; set; }
		[Required]
		public int Depth { get; set; } = 0;
		[Display(Name ="تاریخ ثبت نظر")]
		public DateTime DateTime { get; set; } = DateTime.Now;
	}
}
