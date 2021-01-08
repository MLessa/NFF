using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NudesForFreeV2.Models
{
	public enum Country
	{
		INTERNATIONAL = 0,
		BRAZIL = 1
	}

	public class User
	{
		public int UserID { get; set; }
		public string Username { get; set; }
		public string EmailAddress { get; set; }
		public string Password { get; set; }		
		public string Avatar { get; set; }
		public Country Country { get; set; }
		public Category PreferredCategory { get; set; }
		public DateTime CreationDate { get; set; }

		public string Token { get; set; }
	}
}
