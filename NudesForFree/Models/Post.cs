using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NudesForFree.Models
{
	public enum Category
	{
		FEMALE = 0,
		MALE = 1,
		COUPLE = 2,
		HOMOSEXUAL = 3
	}

	public class Post
	{
		public int PostID { get; set; }
		public int UserID { get; set; }
		public string Username { get; set; }
		public string Description { get; set; }
		public string Filename { get; set; }
		public int Likes { get; set; }
		public int Comments { get; set; }
		public DateTime CreationDate { get; set; }
		public Category Category { get; set; }
	}
}
