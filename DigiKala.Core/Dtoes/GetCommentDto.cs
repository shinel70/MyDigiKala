using DigiKala.DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Xml.Linq;

namespace DigiKala.Core.Dtoes
{
	public class GetCommentDto
	{
		public int Id { get; set; }
		[Required]
		public int UserId { get; set; }
		[Display(Name ="نام کاربر")]
        public string UserFullName { get; set; }
        public int? ReplyCommentId { get; set; }
		[Display(Name = "نظر")]
		[Required]
		[StringLength(90, ErrorMessage = "{0} نباید بیشتر {1} کلمه باشد")]
		public string Text { get; set; }
		[Display(Name ="تاریخ ثبت نظر")]
		public DateTime DateTime { get; set; }
		public int Depth { get; set; } = 0;
		public virtual IList<GetCommentDto> ChildComments { get; set; }
		public virtual IList<GetCommentLikeDto> CommentLikes { get; set; }
	}
}
