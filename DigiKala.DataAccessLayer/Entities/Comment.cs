using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DigiKala.DataAccessLayer.Entities
{
	public class Comment
	{
		public int Id { get; set; }
		[Required]
		public int UserId { get; set; }
		[Required]
		public int ProductId { get; set; }
		public int? ReplyCommentId { get; set; }
		[Display(Name = "نظر")]
		[Required]
		[StringLength(90, ErrorMessage = "{0} نباید بیشتر {1} کلمه باشد")]
		public string Text { get; set; }
		public DateTime DateTime { get; set; }
		public int Depth { get; set; } = 0;
		[ForeignKey("UserId")]
		public virtual User User { get; set; }
		[ForeignKey("ReplyCommentId")]
		public virtual Comment ReplyComment { get; set; }
		[ForeignKey("ProductId")]
		public virtual Product Product { get; set; }
		public virtual IList<Comment> ChildComments { get; set; }
		public IList<CommentLike> CommentLikes { get; set; }
	}
}
