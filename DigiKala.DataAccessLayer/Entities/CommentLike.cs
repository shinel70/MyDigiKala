using System;
using System.Collections.Generic;
using System.Text;

namespace DigiKala.DataAccessLayer.Entities
{
	public class CommentLike
	{
		public int CommentId { get; set; }
		public int UserId { get; set; }
		public virtual Comment Comment { get; set; }
		public virtual User User { get; set; }
	}
}
