using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NudesForFree.Models;
using NudesForFree.Models.Integration;
using NudesForFree.Models.ViewModel;
using NudesForFree.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NudesForFree.Controllers
{
	public class PostController : Controller
	{
		public PostService PostService { get; set; }
		public UserService UserService { get; set; }
		public CommentService CommentService { get; set; }
		public InteractionService InteractionService { get; set; }
		public PostController(PostService postService, UserService userService, CommentService commentService, InteractionService interactionService)
		{
			this.PostService = postService;
			this.UserService = userService;
			this.CommentService = commentService;
			this.InteractionService = interactionService;
		}

		[HttpPost]
		public BaseResponse<bool> DeletePost(int postID, string token)
		{
			try
			{
				var user = UserService.GetUser(token);
				if (user != null)
				{
					var deleted = PostService.Delete(postID, user.UserID);
					if(deleted == 1)
						return new BaseResponse<bool>() { Data = true, Success = true };

					return new BaseResponse<bool>() { Data = false, Message = "There is no post or the user sent isn't the post owner", Success = false };
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
		public async Task<BaseResponse<bool>> Report(int postID, string token, ReportType reportType)
		{
			try
			{
				var user = UserService.GetUser(token);
				if (user != null)
				{
					InteractionService.Insert(new Interaction() { CreationDate = DateTime.Now, IsComment = false, IsHot = false, IsLike = false, IsReport = true, ReportType = reportType, UserID = user.UserID, PostID = postID });
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
		public async Task<BaseResponse<Publication>> GetPublication(int postID)
		{
			try
			{
				return new BaseResponse<Publication>() { Data = PostService.GetPublication(postID), Message = "OK", Success = true };
			}
			catch (Exception ex)
			{
				return new BaseResponse<Publication>() { Data = new Publication(), Message = "Operation fail, try again!<!--" + ex.Message + "-->", Success = false };
			}
		}

		[HttpPost]
		public async Task<BaseResponse<List<Post>>> GetPosts(Category category, int count = 10, int lastID = 0)
		{
			try
			{
				return new BaseResponse<List<Post>>() { Data = PostService.FindByFilter(null, (int)category, lastID, count).ToList(), Message = "OK", Success = true };
			}
			catch (Exception ex)
			{
				return new BaseResponse<List<Post>>() { Data = new List<Post>(), Message = "Operation fail, try again!<!--" + ex.Message + "-->", Success = false };
			}
		}

		[HttpPost]
		public BaseResponse<bool> CreatePost()
		{
			try
			{
				var photo = Request.Form.Files[0];
				var token = Request.Form["userToken"][0];
				var description = Request.Form["description"][0];
				var category = (Category)int.Parse(Request.Form["category"][0]);

				PostViewModel postViewModel = new PostViewModel() { Description = description, Token = token, Category = category };
				var user = UserService.GetUser(postViewModel.Token);
				if (user != null && photo != null)
				{
					var fileExtension = "png";
					if (photo.ContentType.IndexOf("jpeg") != -1)
						fileExtension = "jpeg";
					else if (photo.ContentType.IndexOf("jpg") != -1)
						fileExtension = "jpg";

					var filename = Helper.Util.GenerateFileName(user.UserID, fileExtension, "post_");

					var uploads = Path.Combine("wwwroot/Posts", filename);
					photo.CopyTo(new FileStream(uploads, FileMode.Create));

					PostService.Insert(new Post()
					{
						Category = postViewModel.Category,
						CreationDate = DateTime.Now,	
						Description = postViewModel.Description,
						Filename = filename,
						Comments = 0,
						Likes = 0,
						Username = user.Username,
						UserID = user.UserID
					});
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
		public BaseResponse<bool> HotPost(int postID, string token)
		{
			try
			{
				var user = UserService.GetUser(token);
				if (user != null)
				{
					InteractionService.Insert(new Interaction() { CreationDate = DateTime.Now, IsComment = false, IsHot = true, IsLike = false, IsReport = false, ReportType = ReportType.None, UserID = user.UserID, PostID = postID, CommentID = null });
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
	}
}
