using Microsoft.AspNetCore.Mvc;
using NudesForFree.Models.Integration;
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
		public CommentController(UserService userService, CommentService commentService)
		{
			this.CommentService = commentService;
			this.UserService = userService;			
		}

		[HttpPost]
		public BaseResponse<bool> CreateComment(string token, int postID, string text)
		{
			try
			{
				var user = UserService.GetUser(token);
				if (user != null)
				{
					CommentService.Insert(new Models.Comment() { PostID = postID, CreationDate = DateTime.Now, Text = text, UserID = user.UserID, Username = user.Username });
					return new BaseResponse<bool>() { Data = true, Message = "OK", Success = true };
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
		public async Task<BaseResponse<List<Models.Comment>>> GetComments(int postID, int lastID=0, int count = 5)
		{
			try
			{
				return new BaseResponse<List<Models.Comment>>() { Data = CommentService.FindByFilter(postID, null, lastID, count).ToList(), Message = "OK", Success = true };
			}
			catch (Exception ex)
			{
				return new BaseResponse<List<Models.Comment>>() { Data = new List<Models.Comment>(), Message = "Operation fail, try again!<!--" + ex.Message + "-->", Success = false };
			}
		}
	}
}
