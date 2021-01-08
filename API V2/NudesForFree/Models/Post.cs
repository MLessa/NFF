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
		MxF = 2,
		FxF = 3,
		MxM = 4
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
		public bool IsHot { get; set; }
		public bool HasLike { get; set; }
		public string CreatorAvatar { get; set; }

	}
}
