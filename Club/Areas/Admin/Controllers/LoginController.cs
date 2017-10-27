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
                ShowMassage("用户名或密码不能为空");
                return RedirectToAction("Index");
            }
            using (var club = new ClubEntitie())
            {
                psw = psw.MD5Encoding(account);
                var user = club.User.FirstOrDefault(a => a.Account == account);
                if (user == null)
                {
                    ShowMassage("用户名不存在");
                    return RedirectToAction("Index");
                }
                if (user.Password != psw)
                {
                    ShowMassage("密码错误");                    
                    return RedirectToAction("Index");
                }
                if(!user.IsAdmin)
                {
                    ShowMassage("你不是管理员，无权登录");
                    return RedirectToAction("Index");
                }
                Session["adminuser"] = user;
            }
            return Redirect("/Admin/Post");
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <returns></returns>
        public ActionResult SignOut()
        {
            Session["adminuser"] = null;
            return RedirectToAction("Index");
        }
    }
}