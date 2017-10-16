using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;
using Club;

namespace Club.Areas.Admin.Controllers
{
    public class PostController : BaseController
    {         
        // GET: Admin/Post
        //帖子列表        
        public ActionResult Index(int? id)
        {            
            var pageIndex = id ?? 1;
            int pageSize = 4;
            var kw = Request["kw"];
            PagedList<Post> pageList;
            using (var club = new ClubEntitie())
            {
                var list = club.Post.OrderByDescending(a => a.id).Include(a => a.User).Include(a => a.Type).Where(a=>a.IsAbout==false);                
                if (!string.IsNullOrEmpty(kw))
                {
                    list = list.Where(a => a.Title.Contains(kw) || a.Contents.Contains(kw));
                }
                pageList = list.ToPagedList(pageIndex: pageIndex, pageSize: pageSize);
                if (Request.IsAjaxRequest())
                {
                    return PartialView("_Post", pageList);
                }
            }
            
            return View(pageList);
        }
        [HttpGet]
        public ActionResult Information(int? id)
        {
            var postid = id ?? 0;
            var post = new Post();
            using (var club = new ClubEntitie())
            {
                post = club.Post.Include(a => a.Type).Include(a => a.User).FirstOrDefault(a => a.id == postid);
                var selectlisttype = new List<SelectListItem>();
                var selectlistis = new List<SelectListItem>();
                //帖子类型下拉
                var type = club.Type.ToList();
                foreach (var item in type)
                {
                    var selectitem = new SelectListItem();
                    selectitem.Text = item.Name;
                    selectitem.Value = item.id.ToString();
                    if (post.Typeid == item.id)
                    {
                        selectitem.Selected = true;
                    }
                    selectlisttype.Add(selectitem);
                }
                ViewBag.type = selectlisttype;
                //是否审核下拉
                string[,] isfeatured = { { "0", "未审核" }, { "1", "已审核" } };
                for (int i = 0; i < 2; i++)
                {
                    var selectitem = new SelectListItem();
                    selectitem.Text = isfeatured[i, 1];
                    selectitem.Value = isfeatured[i, 0];
                    if (post.IsFeatured == i.ToBit())
                    {
                        selectitem.Selected = true;
                    }
                    selectlistis.Add(selectitem);
                }
                ViewBag.isfeatured = selectlistis;
            }
            return View(post);
        }
        [HttpPost]
        public ActionResult Information()
        {
            var id = Request["id"].ToInt();
            int isfeaturedid = Request["isfeaturedid"].ToInt();
            int typeid = Request["typeid"].ToInt();
            using (var club = new ClubEntitie())
            {
                var post = club.Post.FirstOrDefault(a => a.id == id);
                post.Typeid = typeid;
                post.IsFeatured = isfeaturedid.ToBit();
                club.SaveChanges();
            }
            ShowMassage("更新成功");
            return RedirectToAction("Index");
        }
        public ActionResult Edit(int id)
        {
            int postid = id;
            var massage = "删除失败";
            if(postid!=0)
            {
                using (var club = new ClubEntitie())
                {
                    var post = club.Post.FirstOrDefault(a => a.id == postid);
                    post.IsAbout = true;
                    club.SaveChanges();
                    massage = "删除成功";
                }
            }
            ShowMassage(massage);
            return RedirectToAction("Index");
        }
        //帖子类型
        public ActionResult Type(int? id)
        {
            int pageIndex = id ?? 1;
            int pageSize = 4;
            PagedList<Type> pageList;            
            using (var club = new ClubEntitie())
            {                
                pageList = club.Type.OrderByDescending(a => a.id).ToPagedList(pageIndex: pageIndex, pageSize: pageSize);
                if (Request.IsAjaxRequest())
                {
                    return PartialView("_Type", pageList);
                }
            }
            
            return View(pageList);
        }
        [HttpGet]
        public ActionResult TypeInformation(int? id)
        {
            var typeid = id ?? 0;
            var type = new Type();
            using (var club=new ClubEntitie())
            {
                if(typeid!=0)
                {
                    type = club.Type.FirstOrDefault(a => a.id == typeid);
                }                
            }
            return View(type);
        }
        [HttpPost]
        public ActionResult TypeInformation()
        {
            int typeid = Request["id"].ToInt();
            var name = Request["name"];
            var type = new Type();
            var massage = "";
            using (var club = new ClubEntitie())
            {
                if(typeid==0)
                {                    
                    club.Type.Add(type);
                    massage = "添加成功";
                }
                else
                {
                    type = club.Type.FirstOrDefault(a => a.id == typeid);
                    massage = "更新成功";
                }
                type.Name = name;
                club.SaveChanges();
            }
            ShowMassage(massage);
            return RedirectToAction("Type");
        }
        public ActionResult TypeEdit(int id)
        {
            var typeid = id;
            using (var club = new ClubEntitie())
            {
                var type = club.Type.FirstOrDefault(a => a.id == typeid);
                if (type != null)
                {
                    club.Type.Remove(type);
                    club.SaveChanges();
                }
                else
                {
                    ShowMassage("该类型删除失败！");
                }
            }
            return RedirectToAction("Type");
        }
    }
}