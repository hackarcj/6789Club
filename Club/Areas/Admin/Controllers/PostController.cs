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
        static int pageSize=5;
        static int pageIndex = 1;
        static int isFeatured = 2;
        static int selectType = 0;
        /// <summary>
        /// 帖子列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>                      
        public ActionResult Index(int? id,int? size,int? isselect,int? type)
        {                       
            pageIndex = id ?? pageIndex;
            pageSize = size ?? pageSize;
            //该参数用于判断审核分类
            isFeatured = isselect ?? isFeatured;
            //该参数用于判断帖子类型
            selectType = type ?? selectType;
            var kw = Request["kw"];
            PagedList<Post> pageList;
            var selecttypelist = new List<SelectListItem>();
            using (var club = new ClubEntitie())
            {
                var typelist = club.Type.ToList();
                var selectitem = new SelectListItem();
                selectitem.Text = "全部";
                selectitem.Value = "0";
                selectitem.Selected = true;
                selecttypelist.Add(selectitem);
                foreach (var item in typelist)
                {
                    selectitem = new SelectListItem();
                    selectitem.Text = item.Name;
                    selectitem.Value = item.id.ToString();
                    selecttypelist.Add(selectitem);
                }
                ViewBag.type = selecttypelist;
                var list = club.Post.OrderByDescending(a => a.id).Include(a => a.User).Include(a => a.Type).Where(a=>a.IsAbout==false);                
                if (!string.IsNullOrEmpty(kw))
                {
                    list = list.Where(a => a.Title.Contains(kw) || a.Contents.Contains(kw));
                }
                if(isFeatured==0 || isFeatured==1)
                {
                    bool featuredis = isFeatured.ToBit();
                    list = list.Where(a => a.IsFeatured== featuredis);
                }
                if(selectType>0)
                {
                    list = list.Where(a => a.Typeid == selectType);
                }
                pageList = list.ToPagedList(pageIndex: pageIndex, pageSize: pageSize);
                if (Request.IsAjaxRequest())
                {
                    return PartialView("_Post", pageList);
                }
            }
            return View(pageList);
        }
        /// <summary>
        /// 帖子编辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 确认编辑帖子
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// 帖子审核
        /// </summary>
        /// <returns></returns>
        public ActionResult ToExamine()
        {
            var postid = Request["postid"].ToInt();
            PagedList<Post> pageList;
            using (var club=new ClubEntitie())
            {                
                 if(postid > 0)
                {
                    var post = club.Post.FirstOrDefault(a => a.id == postid);
                    post.IsFeatured = true;
                    club.SaveChanges();
                }
                if (Request.IsAjaxRequest())
                {
                    var list = club.Post.OrderByDescending(a => a.id).Include(a => a.User).Include(a => a.Type).Where(a => a.IsAbout == false);
                    pageList = list.ToPagedList(pageIndex: pageIndex, pageSize: pageSize);
                    return PartialView("_Post", pageList);
                }
            }
            return View();
        }

        /// <summary>
        /// 帖子类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Type(int? id,int? size)
        {
            pageIndex = id ?? pageIndex;
            pageSize = size ?? pageSize;
            PagedList<Type> pageList;            
            using (var club = new ClubEntitie())
            {
                var typelist = club.Type.OrderByDescending(a => a.id);                
                pageList = typelist.ToPagedList(pageIndex: pageIndex, pageSize: pageSize);
                if (Request.IsAjaxRequest())
                {
                    return PartialView("_Type", pageList);
                }
            }            
            return View(pageList);
        }
        /// <summary>
        /// 确认编辑帖子类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult TypeInformation(int? id)
        {
            var typeid = id ?? 0;
            var type = new Type();
            using (var club=new ClubEntitie())
            {
                if(typeid>0)
                {
                    type = club.Type.FirstOrDefault(a => a.id == typeid);
                }                
            }
            return View(type);
        }
        /// <summary>
        /// 帖子类型添加
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult TypeInformation()
        {
            int typeid = Request["id"].ToInt();
            var name = Request["name"];
            var type = new Type();
            var massage = "";
            using (var club = new ClubEntitie())
            {
                if(typeid>0)
                {
                    type = club.Type.FirstOrDefault(a => a.id == typeid);
                    massage = "更新成功";
                }
                else
                {
                    club.Type.Add(type);
                    massage = "添加成功";                    
                }
                type.Name = name;
                club.SaveChanges();
            }
            ShowMassage(massage);
            return RedirectToAction("Type");
        }
        /// <summary>
        /// 帖子类型删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult TypeEdit(int id)
        {
            var typeid = id;
            var massage = "该类型删除失败！";
            using (var club = new ClubEntitie())
            {
                var type = club.Type.FirstOrDefault(a => a.id == typeid);
                if (type != null)
                {
                    type.IsAbout = true;
                    club.SaveChanges();
                    massage="删除成功";
                }
            }
            ShowMassage(massage);
            return RedirectToAction("Type");
        }
        /// <summary>
        /// 批量审批、删除操作
        /// </summary>
        /// <returns></returns>
        public ActionResult BatchOperation()
        {
            var str = Request["str"];
            //判断str是否为空
            if(!string.IsNullOrWhiteSpace(str))
            {
                //将str拆分成字符数组
                string[] split = str.Split(new char[] { '|' });
                PagedList<Post> pageList;
                using (var club = new ClubEntitie())
                {
                    for (int i = 0; i < split.Length; i++)
                    {
                        int postid = split[i].ToInt();
                        var post = club.Post.FirstOrDefault(a => a.id == postid);
                        if (post != null)
                        {
                            //split[0]为1，则为批量删除操作;
                            //split[0]为2，则为批量审核操作;
                            if (split[0] == "1")
                            {
                                post.IsAbout = true;
                            }                            
                            else if(split[0] == "2")
                            {
                                post.IsFeatured = true;
                            }
                            club.SaveChanges();
                        }
                    }
                    if (Request.IsAjaxRequest())
                    {
                        var list = club.Post.OrderByDescending(a => a.id).Include(a => a.User).Include(a => a.Type).Where(a => a.IsAbout == false);
                        pageList = list.ToPagedList(pageIndex: pageIndex, pageSize: pageSize);
                        return PartialView("_Post", pageList);
                    }
                }                
            }
            return View();
        }
    }
}