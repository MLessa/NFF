using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NudesForFreeV2.Models
{
	public enum ReportType
	{
		NotNude = 0,
		InappropriateContent = 1,
		Famous = 2,
		Leak = 3,
		None = 4,
	}

	public class Interaction
	{
		public int InteractionID { get; set; }
		public int UserID { get; set; }
		public int PostID { get; set; }
		public bool IsLike { get; set; }		
		public bool IsComment { get; set; }
		public int? CommentID { get; set; }
		public bool IsReport { get; set; }
		public ReportType ReportType { get; set; }
		public DateTime CreationDate { get; set; }
	}
}
