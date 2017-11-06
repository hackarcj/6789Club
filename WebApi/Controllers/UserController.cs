using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class UserController : ApiController
    {
        public List<UserModel> List()
        {
            var list = new List<UserModel>();
            using (var db=new ClubEntities())
            {
                var user = db.User;
                foreach (var item in user)
                {
                    var usermodel = new UserModel();
                    usermodel.Id = item.id;
                    usermodel.name = item.Name;
                    usermodel.account = item.Account;
                    list.Add(usermodel);
                }
            }                
            return list;
        }
    }
}
