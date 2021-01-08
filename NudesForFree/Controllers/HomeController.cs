using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NudesForFree.Models;

namespace NudesForFree.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
            
        }      

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public Models.Integration.BaseResponse<string> Error()
        {
            return new Models.Integration.BaseResponse<string>() { Data = "Error", Message = "Fail", Success = false };
        }
    }
}
