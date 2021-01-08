using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace NudesForFreeV2.Models.ViewModel
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
		public HttpPostedFile Photo { get; set; }
		public string Token { get; set; }
	}

}
