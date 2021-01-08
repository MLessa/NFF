using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NudesForFree.Models
{
	public class Comment
	{
		public int CommentID { get; set; }
		public int PostID { get; set; }
		public int UserID { get; set; }
		public string Username { get; set; }
		public string Text { get; set; }

		public DateTime CreationDate { get; set; }
	}
}
