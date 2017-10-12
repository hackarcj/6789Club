using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Club
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class LoginCheck : ActionFilterAttribute
    {        
        public bool IsNeed { get; set; } = false;
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {            
            if(!IsNeed)
            {
                base.OnActionExecuting(filterContext);
                return;
            }
            var loginuser = (User)filterContext.HttpContext.Session["adminuser"];
            if (loginuser == null)
            {
                filterContext.HttpContext.Response.Redirect("/Admin/Login");
                return;
            }
            base.OnActionExecuting(filterContext);
            return;
        }
    }
}