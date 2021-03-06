﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;
using Club;

namespace Club.Areas.Admin.Controllers
{
    public class UserController : BaseController
    {
        static int pageSize=5;
        static int pageIndex = 1;
        static int levelid = 0;
        /// <summary>
        /// 用户列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public ActionResult Index(int? id,int? size,int? isselect)
        {
            pageIndex = id ?? pageIndex;
            pageSize = size ?? pageSize;
            //该参数用于判断用户角色
            levelid = isselect ?? levelid;
            var kw = Request["kw"];           
            PagedList<User> pageList;            
            using (var club = new ClubEntitie())
            {
                var list = club.User.Where(a => a.IsAbort == false).OrderByDescending(a => a.id).Include(a => a.Level);
                if(!string.IsNullOrEmpty(kw))
                {
                    list = list.Where(a => a.Account.Contains(kw) || a.Name.Contains(kw));
                }
                if(levelid>0)
                {
                    list = list.Where(a => a.Levelid==levelid);
                }
                pageList = list.ToPagedList(pageIndex: pageIndex, pageSize:pageSize);
                if (Request.IsAjaxRequest())
                {
                    return PartialView("_User", pageList);
                }
            }
            
            return View(pageList);            
        }
        /// <summary>
        /// 用户信息修改与增加
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Information(int? id)
        {                        
            int userid = id ?? 0;            
            var user = new User();            
            using (var club = new ClubEntitie())
            {
                if(userid!=0)
                {
                    user = club.User.Include(a => a.Level).FirstOrDefault(a => a.id == userid);
                }
                var selectlistitem = new List<SelectListItem>();
                var level = club.Level.ToList();                
                foreach (var item in level)
                {
                    var selectitem = new SelectListItem();
                    selectitem.Text = item.Name;                    
                    selectitem.Value = item.id.ToString();                    
                    if (userid!=0 && (user.Levelid==item.id))
                    {
                        selectitem.Selected = true;
                    }
                    selectlistitem.Add(selectitem);
                }
                ViewBag.SelectItem = selectlistitem;
            }
            
            return View(user);
        }
        [HttpPost]
        public ActionResult Information()
        {
            var message = "";
            //var id = int.Parse(Request["id"]); 
            int id = Request["id"].ToInt();
            var name = Request["name"];            
            var levleid = int.Parse(Request["levelid"]);
            var gold = int.Parse(Request["gold"]);
            var image = Request["image"];
            var password = Request["password"];
            using (var club=new ClubEntitie())
            {
                var user = new User();
                if (id == 0)
                {
                    user.Account = Request["account"];                   
                    club.User.Add(user);
                    message = "新增成功";
                }
                else
                {
                    id = int.Parse(Request["id"]);
                    user = club.User.FirstOrDefault(a => a.id == id);                    
                    message = "更新成功";
                }
                //判断密码是否更新
                if (!string.IsNullOrEmpty(password))
                {
                    user.Password = password.MD5Encoding(user.Account);
                }
                user.Name = name;
                user.Levelid = levleid;
                user.Gold = gold;
                user.Image = image;
                club.SaveChanges();
            }
            ShowMassage(message);
            
            return RedirectToAction("Index");
        }
        /// <summary>
        /// 用户删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            int userid = Request["id"].ToInt();
            var massage = "删除失败";
            if (userid>0)
            {
                using (var club = new ClubEntitie())
                {
                    var user = club.User.FirstOrDefault(a => a.id == userid);
                    if (user != null)
                    {
                        //删除
                        //club.User.Remove(user);

                        //逻辑删除
                        user.IsAbort = true;
                        club.SaveChanges();
                        massage = "删除成功";
                    }                    
                }
            }
            else
            {
                massage = "数据格式错误";
            }
            ShowMassage(massage);            
            
            return RedirectToAction("Index");
        }
        /// <summary>
        /// 用户级别
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Level(int? id)
        {
            var pageIndex = id ?? 1;
            int pageSize = 4;
            PagedList<Level> pageList;
            var user = new List<Level>();
            using (var club = new ClubEntitie())
            {
                pageList = club.Level.OrderByDescending(a => a.id).ToPagedList(pageIndex: pageIndex, pageSize: pageSize);
                if (Request.IsAjaxRequest())
                {
                    return PartialView("_Level", pageList);
                }
            }            
            return View(pageList);
        }
        /// <summary>
        /// 用户级别修改与增加
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult LevelInformation(int? id)
        {
            int levelid = id ?? 0;
            var level = new Level();
            if(levelid!=0)
            {
                using (var club=new ClubEntitie())
                {
                    level = club.Level.FirstOrDefault(a => a.id == levelid);
                }
            }            
            return View(level);
        }
        /// <summary>
        /// 用户级别新增和更新
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LevelInformation()
        {
            var message = "";
            int levelid = int.Parse(Request["id"]);
            var name = Request["name"];
            var level = new Level();
            using (var club=new ClubEntitie())
            {
                if (levelid == 0)
                {
                    club.Level.Add(level);
                    message = "新增成功";
                }
                else
                {
                    level = club.Level.FirstOrDefault(a => a.id == levelid);
                    message = "更新成功";
                }
                level.Name = name;
                ShowMassage(message);
                club.SaveChanges();
            }
            
            return RedirectToAction("Level");
        }
        /// <summary>
        /// 用户级别删除
        /// </summary>
        /// <returns></returns>
        public ActionResult LevelEdit()
        {
            int levelid = Request["id"].ToInt();
            var massage = "删除失败";
            if(levelid > 0)
            {
                using (var club=new ClubEntitie())
                {
                    var level = club.Level.FirstOrDefault(a=>a.id==levelid);
                    if(level!=null)
                    {
                        club.Level.Remove(level);
                        club.SaveChanges();
                        massage = "删除成功";
                    }
                }
            }
            else
            {
                massage = "数据格式错误";
            }
            ShowMassage(massage);
            
            return RedirectToAction("Level");
        }
        /// <summary>
        /// 帖子收藏管理
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Collection(int? id)
        {
            var pageIndex = id ?? 1;
            int pageSize = 4;
            var kw = Request["kw"];
            PagedList<Collection> pageList;
            using (var club = new ClubEntitie())
            {
                var list = club.Collection.Where(a => a.IsCollection == true).OrderByDescending(a => a.id).Include(a => a.User).Include(a=>a.Post);
                if (!string.IsNullOrEmpty(kw))
                {
                    list = list.Where(a => a.User.Name.Contains(kw) || a.Post.Title.Contains(kw));
                }
                pageList = list.ToPagedList(pageIndex: pageIndex, pageSize: pageSize);
                if (Request.IsAjaxRequest())
                {
                    return PartialView("_Collection", pageList);
                }
            }
            
            return View(pageList);
        }
        /// <summary>
        /// 赞贴管理
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult PraiseRecord(int? id)
        {
            var pageIndex = id ?? 1;
            int pageSize = 4;
            var kw = Request["kw"];
            PagedList<PraiseRecord> pageList;
            using (var club = new ClubEntitie())
            {
                var list = club.PraiseRecord.OrderByDescending(a => a.id).Include(a => a.User).Include(a => a.Post);
                if (!string.IsNullOrEmpty(kw))
                {
                    list = list.Where(a => a.User.Name.Contains(kw) || a.Post.Title.Contains(kw));
                }
                pageList = list.ToPagedList(pageIndex: pageIndex, pageSize: pageSize);
                if (Request.IsAjaxRequest())
                {
                    return PartialView("_PraiseRecord", pageList);
                }
            }
            
            return View(pageList);
        }
        /// <summary>
        /// 管理员个人信息跳转
        /// </summary>
        /// <returns></returns>
        public ActionResult InformationJump()
        {
            var userid = Request["id"].ToInt();
            if(userid>0)
            {
                using (var club=new ClubEntitie())
                {
                    var user = club.User.Include(a=>a.Level).FirstOrDefault(a => a.id == userid);
                    Session["loginuser"] = user;
                }
                return Redirect("/User/Index?id=" + userid);
            }
            return View();
        }
    }
}