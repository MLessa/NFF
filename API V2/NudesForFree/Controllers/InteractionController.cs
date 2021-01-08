using Microsoft.AspNetCore.Mvc;
using NudesForFree.Models;
using NudesForFree.Models.DTO;
using NudesForFree.Models.Integration;
using NudesForFree.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NudesForFree.Controllers
{
	public class InteractionController : Controller
	{
		public InteractionService InteractionService { get; set; }
		public UserService UserService { get; set; }
		public InteractionController(InteractionService interactionService, UserService userService)
		{
			this.InteractionService = interactionService;
			this.UserService = userService;
		}

		[HttpPost]
		public BaseResponse<List<InteractionDTO>> GetInteractions(string token, int count = 5, int lastID = 0)
		{
			try
			{
				var user = UserService.GetUser(token);
				if (user != null)
				{
					return new BaseResponse<List<InteractionDTO>>()
					{
						Data = InteractionService.GetInteractionsDTO(user.UserID, count, lastID),
						Message = "OK",
						Success = true
					};
				}
				else
				{
					return new BaseResponse<List<InteractionDTO>>() { Data = new List<InteractionDTO>(), Message = "User not found!", Success = false };
				}
			}
			catch (Exception ex)
			{
				return new BaseResponse<List<InteractionDTO>>() { Data = new List<InteractionDTO>(), Message = "Operation fail, try again!<!--" + ex.Message + "-->", Success = false };
			}
		}

		[HttpPost]
		public BaseResponse<bool> ReportPost(string token, int postID, ReportType reportType)
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

	}
}
