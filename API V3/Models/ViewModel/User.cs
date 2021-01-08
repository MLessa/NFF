using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NudesForFreeV2.Models.ViewModel
{
	public class UserViewModel : NudesForFreeV2.Models.User
	{
		//	public IFormFile Avatar { get; set; }
	}

	public class UserProfileViewModel
	{
		public string Username { get; set; }
		public string Avatar { get; set; }
		public int PostCount { get; set; }
		public int CommentCount { get; set; }
		public int LikeCount { get; set; }
		public List<Post> BestPosts { get; set; }
	}
}
