﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Club.Models;
namespace Club.Controllers
{
    public class UserController : BaseController
    {
        // GET: Login
        public ActionResult Index()
        {
            //using (var db = new ClubEntities())
            //{
            //    var user = db.User.ToList();
            //    foreach (var item in user)
            //    {
            //        item.RegistrationTime = DateTime.Now.AddDays(-2);
            //    }
            //    db.SaveChanges();
            //}
            return View();
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(UserLogin UserLogin)
        {
            if (string.IsNullOrEmpty(UserLogin.Account) || string.IsNullOrEmpty(UserLogin.Password))
            {
                TempData["login"] = "用户名或密码不能为空";
                return RedirectToAction("Login");
            }
            using (var club = new ClubEntities())
            {
                UserLogin.Password = UserLogin.Password.MD5Encoding(UserLogin.Account);
                var user = club.User.FirstOrDefault(a => a.Account == UserLogin.Account);
                if (user == null)
                {
                    TempData["login"] = "用户名不存在";
                    return RedirectToAction("Login");
                }
                if (user.Password != UserLogin.Password)
                {
                    TempData["login"] = "密码错误";
                    TempData["account"] = UserLogin.Account;
                    return RedirectToAction("Login");
                }
                Session["loginuser"] = user;
                ViewBag.username = UserLogin.Account;
            }
            return Redirect("/Post");
        }
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(UserRegister UserRegister)
        {
            using (var db=new ClubEntities())
            {
                var user = db.User.ToList();
                foreach(var item in user)
                {
                    if(item.Account==UserRegister.Account)
                    {
                        ShowMassage("账号已存在");
                        return View();
                    }
                }
                var useradd = new User();
                useradd.Account = UserRegister.Account;
                useradd.Name = UserRegister.Name;
                useradd.Password = UserRegister.Password.MD5Encoding(UserRegister.Account);
                useradd.Levelid = 1;
                useradd.RegistrationTime = DateTime.Now;
                db.User.Add(useradd);
                db.SaveChanges();
                ShowMassage("注册成功，请登录");
            }
            return View();
        }
    }
}