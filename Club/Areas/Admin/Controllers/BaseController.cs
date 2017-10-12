using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Club.Areas.Admin.Controllers
{
    [LoginCheck(IsNeed=true)]
    public class BaseController : Controller
    {
        //弹出框
        public void ShowMassage(string message)
        {
            TempData["Msg"] = message;
        }
    }
}