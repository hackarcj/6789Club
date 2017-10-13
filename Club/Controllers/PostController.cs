using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;
using Club;
using Club.Models;

namespace Club.Controllers
{
    public class PostController : BaseController
    {        
        // GET: Post
        public ActionResult Index(int?id)
        {
            var pageIndex = id ?? 1;
            int pageSize = 10;
            //var kw = Request["kw"];
            var listpost=new List<ListPostModel>();
            using (var db = new ClubEntities())
            {                
                var post = db.Post.OrderByDescending(a => a.id).Include(a => a.User).Include(a => a.Type).Where(a => a.IsFeatured == true).ToList();                
                //if (!string.IsNullOrEmpty(kw))
                //{
                //    list = list.Where(a => a.Title.Contains(kw) || a.Contents.Contains(kw));
                //}
                foreach(var item in post)
                {
                    var postModel = new ListPostModel();
                    var reply = db.Reply.Where(a => a.id == item.id);
                    postModel.id = item.id;
                    postModel.title = item.Title;
                    postModel.username = item.User.Name;
                    postModel.image = item.User.Image;
                    postModel.time = item.Time;
                    postModel.visit = item.Visit;
                    postModel.relpy = reply.Count();
                    if(item.Essence==1)
                    {
                        postModel.essence = "精";
                    }
                    listpost.Add(postModel);
                }
                listpost = listpost.ToPagedList(pageIndex: pageIndex, pageSize: pageSize);
                if (Request.IsAjaxRequest())
                {
                    return PartialView("_Post", listpost);
                }
            }

            return View(listpost);
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