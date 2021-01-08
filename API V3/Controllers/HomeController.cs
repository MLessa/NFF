using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using NudesForFreeV2.Models;

namespace NudesForFreeV2.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
            
        }
        public ActionResult Index()
        {
            return RedirectPermanent("index.html");
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public JsonResult Error()
        {
            return Json(new Models.Integration.BaseResponse<string>() { Data = "Error", Message = "Fail", Success = false });
        }
    }
}
