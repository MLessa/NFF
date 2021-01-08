using DlibDotNet;
using DlibDotNet.ImageDatasetMetadata;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NudesForFree.Models;
using NudesForFree.Models.Integration;
using NudesForFree.Models.ViewModel;
using NudesForFree.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
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
					InteractionService.Insert(new Interaction() { CreationDate = DateTime.Now, IsComment = false, IsLike = false, IsReport = true, ReportType = reportType, UserID = user.UserID, PostID = postID });
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
		public async Task<BaseResponse<List<Post>>> GetPosts(Category category, int count = 10, int lastID = 0, string token=null)
		{
			try
			{
				var listPosts = PostService.FindByFilter(null, (int)category, lastID, count).ToList();
				if (!string.IsNullOrEmpty(token))
				{
					var user = UserService.GetUser(token);
					if (user != null)
					{
						string postIDS = string.Join(',', listPosts.Select(x => x.PostID).ToArray());
						var listInteration = InteractionService.FindByFilter(null, user.UserID, count, 0, postIDS);
						listPosts.ForEach(x => x.HasLike = listInteration.FirstOrDefault(z => z.PostID == x.PostID) != null);						
					}
				}
				return new BaseResponse<List<Post>>() { Data = listPosts, Message = "OK", Success = true };
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

					var filename = Helper.Util.GenerateFileName(user.UserID, fileExtension, "post_f_");

					var uploads = Path.Combine("wwwroot/Posts", filename);
					photo.CopyTo(new FileStream(uploads, FileMode.Create));
					using (var fd = Dlib.GetFrontalFaceDetector())
					{
						var img = Dlib.LoadImage<RgbPixel>(uploads);

						// find all faces in the image
						var faces = fd.Operator(img);
						uploads = uploads.Replace("post_f_", "post_");
						filename = filename.Replace("post_f_", "post_");
						//bool needSaveByDlib = true;
						foreach (var face in faces)
						{
							
							// draw a rectangle for each face
							Dlib.DrawRectangle(img, face, color: new RgbPixel(0, 0, 0), thickness: 10);
							Dlib.FillRect(img, face, new RgbPixel(0, 0, 0));
							//var image = Bitmap.FromFile(uploads);
							/*using (var stream = new MemoryStream())
							{
								photo.CopyTo(stream);
								using (var img2 = System.Drawing.Image.FromStream(stream))
								{
									using (var graphic = Graphics.FromImage(img2))
									{
										//var font = new Font(FontFamily.GenericSansSerif, 20, FontStyle.Bold, GraphicsUnit.Pixel);
										//var color = Color.FromArgb(128, 255, 255, 255);
										//var brush = new SolidBrush(color);
										//var point = new System.Drawing.Point(img2.Width - 120, img2.Height - 30);

										//graphic.DrawString("edi.wang", font, brush, point);
										using (var imgEmotion = System.Drawing.Image.FromFile(Path.Combine("wwwroot/images", "emotion.png")))
											graphic.DrawImage(imgEmotion, new RectangleF(face.Left, face.Top, face.Width, face.Height));

										img2.Save(uploads, System.Drawing.Imaging.ImageFormat.Png);
										needSaveByDlib = false;
									}
								}
							}*/
						}
						//if (needSaveByDlib)
						Dlib.SaveJpeg(img, uploads);
					}
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
					if (InteractionService.GetByPostIDList(new List<int>() { postID }, user.UserID).FirstOrDefault(x => x.IsLike) == null)
					{
						InteractionService.Insert(new Interaction() { CreationDate = DateTime.Now, IsComment = false, IsLike = true, IsReport = false, ReportType = ReportType.None, UserID = user.UserID, PostID = postID, CommentID = null });
						return new BaseResponse<bool>() { Data = true, Message = "OK", Success = true };
					}
					else
						return new BaseResponse<bool>() { Data = false, Message = "INVALID", Success = true };
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
