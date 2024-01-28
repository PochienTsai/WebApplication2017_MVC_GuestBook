using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2017_MVC_GuestBook.Models
{
    /// <summary>
    /// 專為第三天的 "List2 範例" 設計的。初學者暫時不用學，可以跳過。
    /// </summary>
    public class ListAllViewModel
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string UserSex { get; set; }

        public DateTime? UserBirthDay { get; set; }

        public string UserMobilePhone { get; set; }
    }
}