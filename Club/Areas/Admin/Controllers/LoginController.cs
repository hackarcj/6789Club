using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Club;

namespace Club.Areas.Admin.Controllers
{
    [LoginCheck(IsNeed = false)]
    public class LoginController : BaseController
    {
        // GET: Admin/Login
        public ActionResult Index()
        {                        
            return View();
        }
        [HttpPost]
        public ActionResult Login()
        {
            var account = Request["account"];
            var psw = Request["password"];
            if(string.IsNullOrEmpty(account) || string.IsNullOrEmpty(psw))
            {
                TempData["login"] = "用户名或密码不能为空";
                return RedirectToAction("Index");
            }
            using (var club = new ClubEntities())
            {
                psw = psw.MD5Encoding(account);
                var user = club.User.FirstOrDefault(a => a.Account == account);
                if (user == null)
                {
                    TempData["login"] = "用户名不存在";
                    return RedirectToAction("Index");
                }
                if (user.Password != psw)
                {
                    TempData["login"] = "密码错误";
                    TempData["account"] = account;
                    return RedirectToAction("Index");
                }
                Session["adminuser"] = user;
                ViewBag.username = account;
            }
            return Redirect("/Admin/Home");
        }
    }
}