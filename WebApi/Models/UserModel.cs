using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    public class UserModel
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 用户账号
        /// </summary>
        public string account { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        public string name { get; set; }
        
    }
}