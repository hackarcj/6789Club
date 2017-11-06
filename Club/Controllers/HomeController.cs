using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Club;

namespace Club.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var time = DateTime.Now;
            //把时分秒初始化为0
            time = new DateTime(time.Year, time.Month, time.Day, 0, 0, 0, 0);
            //获取一周的时间段
            //获取当前是周几                
            var week = Convert.ToInt32(time.DayOfWeek);
            //判断 week==0 则为周日
            if (week == 0)
            {
                week = 7;
            }
            //当前这周周一日期
            var Monday = time.AddDays(1 - week);

            //获取一个月的时间段                  
            //获取这个月的1号
            var firstday = time.AddDays(1 - time.Day);            
            //获取这个月的月末
            var ednday = firstday.AddMonths(1).AddDays(-1);
            return View("ok");
        }

        public ActionResult About()
        {
            //using (var club=new ClubEntitie())
            //{
            //    string psw = "000000";
            //    var user = club.User.ToList();
            //    foreach(var item in user)
            //    {
            //        item.Password = psw.MD5Encoding(item.Account);
            //    }
            //    club.SaveChanges();
            //}

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}