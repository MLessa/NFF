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
		public InteractionType InteractionType { get; set; }
		public Post Post { get; set; }
		public User User { get; set; }
	}
}
