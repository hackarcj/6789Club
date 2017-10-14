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
            //自定义 
            //      key==1 为搜索
            //      key==2 为帖子最新发布和回复选择
            //      key==3 为帖子类型选择
            var key = Request["key"].ToInt();
            var value = Request["value"];
            var listpost=new List<ListPostModel>();
            using (var db = new ClubEntities())
            {
                var type = db.Type.ToList();
                ViewBag.type = type;
                var post = db.Post.OrderByDescending(a => a.id).Include(a => a.User).Include(a => a.Type).Where(a => a.IsFeatured == true).ToList();
                //按帖子类型查找
                switch(key)
                {
                    case 1:
                        post = post.Where(a => a.Title.Contains(value) || a.User.Name.Contains(value)).ToList();
                        break;
                    case 2:
                        break;
                    case 3:
                        var typeid = value.ToInt();
                        post = post.Where(a => a.Typeid == typeid).ToList();
                        break;
                }                
                foreach (var item in post)
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