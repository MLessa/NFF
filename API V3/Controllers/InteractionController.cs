using NudesForFreeV2.Models;
using NudesForFreeV2.Models.DTO;
using NudesForFreeV2.Models.Integration;
using NudesForFreeV2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace NudesForFreeV2.Controllers
{
	public class InteractionController : Controller
	{
		
		public InteractionController()
		{
			
		}

		[HttpPost]
		public JsonResult GetInteractions(string token, int count = 5, int lastID = 0)
		{
			try
			{
				var user = UserService.GetUser(token);
				if (user != null)
				{
					return Json(new BaseResponse<List<InteractionDTO>>()
					{
						Data = InteractionService.GetInteractionsDTO(user.UserID, count, lastID),
						Message = "OK",
						Success = true
					});
				}
				else
				{
					return Json(new BaseResponse<List<InteractionDTO>>() { Data = new List<InteractionDTO>(), Message = "User not found!", Success = false });
				}
			}
			catch (Exception ex)
			{
				return Json(new BaseResponse<List<InteractionDTO>>() { Data = new List<InteractionDTO>(), Message = "Operation fail, try again!<!--" + ex.Message + "-->", Success = false });
			}
		}

		[HttpPost]
		public JsonResult ReportPost(string token, int postID, ReportType reportType)
		{
			try
			{
				var user = UserService.GetUser(token);
				if (user != null)
				{
					InteractionService.Insert(new Interaction() { CreationDate = DateTime.Now, IsComment = false, IsLike = false, IsReport = true, ReportType = reportType, UserID = user.UserID, PostID = postID });
					return Json(new BaseResponse<bool>() { Data = true, Message = "OK", Success = true });
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

	}
}
