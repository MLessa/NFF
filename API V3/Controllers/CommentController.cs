using NudesForFreeV2.Models.Integration;
using NudesForFreeV2.Models.ViewModel;
using NudesForFreeV2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace NudesForFreeV2.Controllers
{	
	public class CommentController : Controller
	{
		public CommentController()
		{
	
		}

		[HttpPost]
		public JsonResult CreateComment(string token, int postID, string text)
		{
			try
			{
				var user = UserService.GetUser(token);
				if (user != null)
				{
					var comment = new Models.Comment() { PostID = postID, CreationDate = DateTime.Now, Text = text, UserID = user.UserID, Username = user.Username };
					var commentID = CommentService.Insert(comment);
					InteractionService.Insert(new Models.Interaction() { CreationDate = DateTime.Now, IsComment = true, IsLike = false, IsReport = false, ReportType = Models.ReportType.None, UserID = user.UserID, PostID = postID, CommentID = commentID });
					return Json(new BaseResponse<Models.Comment>() { Data = comment, Message = "OK", Success = true });
				}
				else
				{
					return Json(new BaseResponse<Models.Comment>() { Data = new Models.Comment(), Message = "User not found!", Success = false });
				}
			}
			catch (Exception ex)
			{
				return Json(new BaseResponse<Models.Comment>() { Data = new Models.Comment(), Message = "Operation fail, try again!<!--" + ex.Message + "-->", Success = false });
			}
		}

		[HttpPost]
		public JsonResult DeleteComment(int commentID, string token)
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
						return Json(new BaseResponse<bool>() { Data = true, Message = "OK", Success = true });
					}
					else
						return Json(new BaseResponse<bool>() { Data = false, Message = "User not found!", Success = false });
				}
				else
				{
					return Json(new BaseResponse<bool>() { Data = false, Message = "User not found!", Success = false });
				}
			}
			catch (Exception ex)
			{
				return Json(new BaseResponse<bool>() { Data = false, Message = "Operation fail, try again!<!--" + ex.Message + "-->", Success = false });
			}
		}

		[HttpPost]
		public JsonResult GetComments(int postID, int lastID=0, int count = 5)
		{
			try
			{
				return Json(new BaseResponse<List<PublicationComment>>() { Data = CommentService.FindByFilter(postID, null, lastID, count).ToList(), Message = "OK", Success = true });
			}
			catch (Exception ex)
			{
				return Json(new BaseResponse<List<PublicationComment>>() { Data = new List<PublicationComment>(), Message = "Operation fail, try again!<!--" + ex.Message + "-->", Success = false });
			}
		}
	}
}
