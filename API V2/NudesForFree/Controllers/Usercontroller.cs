using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NudesForFree.Models;
using NudesForFree.Models.Integration;
using NudesForFree.Models.ViewModel;
using NudesForFree.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Microsoft.Extensions.Hosting;
using DlibDotNet;

namespace NudesForFree.Controllers
{
	public class UserController : Controller
	{
		private UserService UserService { get; set; }
		private PostService PostService { get; set; }
		private InteractionService InteractionService { get; set; }

		public UserController(UserService userService, PostService postService, InteractionService interactionService)
		{
			this.UserService = userService;
			this.PostService = postService;
			this.InteractionService = interactionService;
		}

		[HttpPost]
		public BaseResponse<UserProfileViewModel> GetProfileData(string userToken)
		{
			if (string.IsNullOrEmpty(userToken))
				return new BaseResponse<UserProfileViewModel> { Success = false, Message = "User token not sent" };
			var user = UserService.GetUser(userToken, true);

			if (user != null)
			{
				List<int> bestPostIDs = new List<int>();
				List<Interaction> interactions = new List<Interaction>();
				var posts = PostService.GetPostIDsByUserID(user.UserID);
				if (posts.Count > 0)
				{
					interactions = InteractionService.GetByPostIDList(posts.Select(p => p.PostID).ToList(), null);

					// TODO - Melhorar isso. Aí vai considerar apenas os posts que tem mais interações, entretanto, talvez melhor seria ordenar pelos
					// que tem mais comentários e depois pelos que tem mais likes. Likes sempre vão ser mais numerosos, então vai acabar aí sempre retornando os com mais likes
					bestPostIDs = interactions
						.GroupBy(i => i.PostID)
						.OrderByDescending(g => g.Count())
						.Take(3)
						.Select(g => g.Key)
						.ToList();
				}

				if(bestPostIDs.Count < 3)
				{
					var morePosts = posts.Where(p => !bestPostIDs.Contains(p.PostID)).Take(3 - bestPostIDs.Count).ToList();
					bestPostIDs.AddRange(morePosts.Select(p => p.PostID));
				}

				var result = new UserProfileViewModel
				{
					Avatar = user.Avatar,
					Username = user.Username,
					PostCount = posts.Count,
					CommentCount = interactions.Where(i => i.IsComment).Count(),
					LikeCount = interactions.Where(i => i.IsLike).Count(),
					BestPosts = posts.Where(p => bestPostIDs.Contains(p.PostID)).ToList()
				};
				return new BaseResponse<UserProfileViewModel> { Success = true, Data = result };
			}
			return new BaseResponse<UserProfileViewModel> { Success = false, Message = "User token dont find user" };
		}

		[HttpPost]
		public BaseResponse<string> UploadUserAvatar()
		{
			var avatarFile = Request.Form.Files[0];
			var userToken = Request.Form["userToken"][0];

			if (string.IsNullOrEmpty(userToken))
				return new BaseResponse<string> {  Success = false, Message = "User token not sent" };
			if (avatarFile == null)
				return new BaseResponse<string> {  Success = false, Message = "Avatar not sent" };

			var user = UserService.GetUser(userToken);

			if (user != null)
			{
				var fileExtension = "png";
				if (avatarFile.ContentType.IndexOf("jpeg") != -1)
					fileExtension = "jpeg";
				else if (avatarFile.ContentType.IndexOf("jpg") != -1)
					fileExtension = "jpg";

				var filename = Helper.Util.GenerateFileName(user.UserID, fileExtension, "avatar_f_");

				if (UserService.UpdateAvatar(user.UserID, filename))
				{
					var uploads = Path.Combine("wwwroot/avatars", filename);
					avatarFile.CopyTo(new FileStream(uploads, FileMode.Create));
					using (var fd = Dlib.GetFrontalFaceDetector())
					{
						var img = Dlib.LoadImage<RgbPixel>(uploads);

						// find all faces in the image
						var faces = fd.Operator(img);
						foreach (var face in faces)
						{
							// draw a rectangle for each face
							Dlib.DrawRectangle(img, face, color: new RgbPixel(0, 255, 255), thickness: 4);
						}
						uploads = uploads.Replace("avatar_f_","avatar_");
						Dlib.SaveJpeg(img, uploads);
					}
					return new BaseResponse<string> { Data = filename, Success = true };
				}
				else
					return new BaseResponse<string> { Success = false, Message = "Error on update avatar" };
			}

			return new BaseResponse<string> { Success = false, Message = "Token not find a user" };
		}


		[HttpPost]
		public BaseResponse<List<Post>> GetUserPosts(string token, int count = 10, int lastID = 0)
		{
			try
			{
				var user = UserService.GetUser(token);
				if (user != null)
				{
					return new BaseResponse<List<Post>>() { Data = PostService.FindByFilter(user.UserID, null, lastID, count).ToList(), Message = "OK", Success = true };
				}
				else
				{
					return new BaseResponse<List<Post>>() { Data = new List<Post>(), Message = "User not found!", Success = false };
				}
			}
			catch (Exception ex)
			{
				return new BaseResponse<List<Post>>() { Data = new List<Post>(), Message = "Operation fail, try again!<!--" + ex.Message + "-->", Success = false };
			}
		}

		[HttpPost]
		public BaseResponse<User> GetByID(string token)
		{
			try
			{
				return new BaseResponse<User>() { Data = UserService.GetUser(token), Message = "OK", Success = true };
			}
			catch (Exception ex)
			{
				return new BaseResponse<User>() { Data = new Models.User(), Message = "Operation fail, try again!<!--" + ex.Message + "-->", Success = false };
			}
		}

		[HttpPost]
		public BaseResponse<string> Create(UserViewModel user)
		{
			try
			{
				if (string.IsNullOrEmpty(user.Password) || string.IsNullOrEmpty(user.Username))
					return new BaseResponse<string>() { Data = "", Message = "User creation fail!", Success = false };
				var tmpUser = UserService.FindByFilter(user.EmailAddress, user.Username).FirstOrDefault();
				if (tmpUser == null)
				{
					UserService.Insert(new Models.User() { Country = user.Country, Avatar = user.Avatar ?? "avatar_blank.png", CreationDate = DateTime.Now, EmailAddress = user.EmailAddress, Password = Helper.Util.MD5Encode(user.Password), PreferredCategory = user.PreferredCategory, Username = user.Username, Token = Guid.NewGuid().ToString().ToUpper() });
					tmpUser = UserService.FindByFilter(user.EmailAddress, user.Username).FirstOrDefault();
					if (tmpUser != null)
					{
						return new BaseResponse<string>() { Data = tmpUser.Token, Message = "OK", Success = true };
					}
					else
					{
						return new BaseResponse<string>() { Data = "", Message = "User creation fail!", Success = false };
					}
				}
				else
				{
					return new BaseResponse<string>() { Data = "", Message = "User already exists!", Success = false };
				}
			}
			catch (Exception ex)
			{
				return new BaseResponse<string>() { Data = "", Message = "Operation fail, try again!<!--" + ex.Message + "-->", Success = false };
			}
		}

		[HttpPost]
		public BaseResponse<bool> Delete(string token)
		{
			try
			{
				var user = UserService.GetUser(token);
				if (user != null)
				{
					UserService.Delete(user);
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

		public BaseResponse<User> Update(string token, string password, int country, int category, string username)
		{
			try
			{
				var user = UserService.GetUser(token);
				if (user != null)
				{
					if (!string.IsNullOrEmpty(password))
						user.Password = Helper.Util.MD5Encode(password);
					if (!string.IsNullOrEmpty(username))
						user.Username = username;
					user.PreferredCategory = (Category)category;
					user.Country = (Country)country;
					UserService.Update(user);
					user = UserService.GetUser(token);
					if (user != null)
					{
						return new BaseResponse<User>() { Data = user, Message = "OK", Success = true };
					}
					else
					{
						return new BaseResponse<User>() { Data = null, Message = "User update fail!", Success = false };
					}
				}
				else
				{
					return new BaseResponse<User>() { Data = null, Message = "User update fail!", Success = false };
				}
			}
			catch (Exception ex)
			{
				return new BaseResponse<User>() { Data = null, Message = "Operation fail, try again!<!--" + ex.Message + "-->", Success = false };
			}
		}

		[HttpPost]
		public async Task<BaseResponse<string>> LoginUser(User user)
		{
			try
			{
				var tmpUser = UserService.FindByFilter(user.EmailAddress?? user.Username, user.Username).FirstOrDefault();
				if (tmpUser != null && tmpUser.Password == NudesForFree.Helper.Util.MD5Encode(user.Password))
				{
					return new BaseResponse<string>() { Data = tmpUser.Token, Message = "OK", Success = true };
				}
				else
				{
					return new BaseResponse<string>() { Data = "", Message = "User not found!", Success = false };
				}
			}
			catch (Exception ex)
			{
				return new BaseResponse<string>() { Data = "", Message = "Operation fail, try again!<!--" + ex.Message + "-->", Success = false };
			}
		}
	}
}
