using Microsoft.AspNetCore.Mvc;
using NudesForFree.Models.Integration;
using NudesForFree.Models.ViewModel;
using NudesForFree.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NudesForFree.Controllers
{	
	public class CommentController : Controller
	{
		public CommentService CommentService { get; set; }
		public UserService UserService { get; set; }
		public InteractionService InteractionService { get; set; }
		public PostService PostService { get; set; }
		public CommentController(UserService userService, CommentService commentService, InteractionService interactionService, PostService postService)
		{
			this.CommentService = commentService;
			this.UserService = userService;
			this.InteractionService = interactionService;
			this.PostService = postService;
		}

		[HttpPost]
		public BaseResponse<Models.Comment> CreateComment(string token, int postID, string text)
		{
			try
			{
				var user = UserService.GetUser(token);
				if (user != null)
				{
					var comment = new Models.Comment() { PostID = postID, CreationDate = DateTime.Now, Text = text, UserID = user.UserID, Username = user.Username };
					var commentID = CommentService.Insert(comment);
					InteractionService.Insert(new Models.Interaction() { CreationDate = DateTime.Now, IsComment = true, IsLike = false, IsReport = false, ReportType = Models.ReportType.None, UserID = user.UserID, PostID = postID, CommentID = commentID });
					return new BaseResponse<Models.Comment>() { Data = comment, Message = "OK", Success = true };
				}
				else
				{
					return new BaseResponse<Models.Comment>() { Data = new Models.Comment(), Message = "User not found!", Success = false };
				}
			}
			catch (Exception ex)
			{
				return new BaseResponse<Models.Comment>() { Data = new Models.Comment(), Message = "Operation fail, try again!<!--" + ex.Message + "-->", Success = false };
			}
		}

		[HttpPost]
		public BaseResponse<bool> DeleteComment(int commentID, string token)
		{
			try
			{
				var user = UserService.GetUser(token);
				if (user != null)
				{
					var comment = CommentService.GetComment(commentID);
					if (comment != null && comment.UserID == user.UserID)
					{
						CommentService.Delete(new Models.Comment() { CommentID = commentID });
						return new BaseResponse<bool>() { Data = true, Message = "OK", Success = true };
					}
					else
						return new BaseResponse<bool>() { Data = false, Message = "User not found!", Success = false };
				}
				else
				{
					return new BaseResponse<bool>() { Data = false, Message = "User not found!", Success = false };
				}
			}
			catch (Exception ex)
			{
				return new BaseResponse<bool>() { Data = false, Message = "Operation fail, try again!<!--" + ex.Message + "-->", Success = false };
			}
		}

		[HttpPost]
		public async Task<BaseResponse<List<PublicationComment>>> GetComments(int postID, int lastID=0, int count = 5)
		{
			try
			{
				return new BaseResponse<List<PublicationComment>>() { Data = CommentService.FindByFilter(postID, null, lastID, count).ToList(), Message = "OK", Success = true };
			}
			catch (Exception ex)
			{
				return new BaseResponse<List<PublicationComment>>() { Data = new List<PublicationComment>(), Message = "Operation fail, try again!<!--" + ex.Message + "-->", Success = false };
			}
		}
	}
}
