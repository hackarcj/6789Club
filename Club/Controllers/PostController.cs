using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Club.Controllers
{
    public class PostController : BaseController
    {
        // GET: Post
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Search()
        {
            return View();
        }
        [UserCheck(IsNeed = true)]
        public ActionResult New()
        {
            using (var db=new ClubEntities())
            {
                var type = db.Type.ToList();
                ViewBag.type = type;
            }
            return View();
        }
        [UserCheck(IsNeed = true)]
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Save()
        {
            var typeId = Request["typeId"].ToInt();
            var title = Request["title"];
            var content = Request["content"];
            var loginUser = (User)Session["loginuser"];
            using (var db=new ClubEntities())
            {
                var post = new Post();
                post.Title = title;
                post.Contents = content;
                post.Userid = loginUser.id;
                post.Typeid = typeId;
                post.System = "Win10";
                post.Time = DateTime.Now;
                db.Post.Add(post);
                db.SaveChanges();
            }            
            return Redirect("/");
        }
    }
}