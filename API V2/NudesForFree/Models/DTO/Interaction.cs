using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NudesForFree.Models.DTO
{
	public enum InteractionType
	{
		LIKE = 0,
		HOT = 1,
		COMMENT = 2,
		REPORT = 3
	}
	public class InteractionDTO
	{
		public int InteractionID { get; set; }
		public int PostID { get; set; }
		public string Filename { get; set; }
		public string Avatar { get; set; }
		public string Username { get; set; }
		public int Likes { get; set; }
		public int Comments { get; set; }
		public string Description { get; set; }
		public Category Category { get; set; }
		public DateTime PostDate { get; set; }
	}
}
