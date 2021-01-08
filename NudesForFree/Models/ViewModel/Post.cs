using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NudesForFree.Models.ViewModel
{
	public class Publication
	{
		public Post Post { get; set; }
		public User CreatorUser { get; set; }
		public List<PublicationComment> Comments { get; set; }
	}


	public class PublicationComment : Comment
	{
		public string Avatar { get; set; }
	}

	public class PostViewModel : Post
	{
		public IFormFile Photo { get; set; }
		public string Token { get; set; }
	}

}
