using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Club;

namespace Club.Areas.Admin.Controllers
{
    public class HomeController : BaseController
    {
        [LoginCheck]
        // GET: Admin/Home
        public ActionResult Index()
        {
            //var loginuser = Session["adminuser"];
            //if(loginuser==null)
            //{
            //    return Redirect("/Admin/Login");
            //}
            return View();
        }        
    }
}