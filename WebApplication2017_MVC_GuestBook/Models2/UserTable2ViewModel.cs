using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2017_MVC_GuestBook.Models2
{    //**********************************************************
    // ViewModel -- 直接把 UserTabl2.cs 內容複製過來而已（只有去除 [驗證]的部分，其餘都沒有修改）
    //**********************************************************
    public class UserTable2ViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserSex { get; set; }
        public DateTime? UserBirthDay { get; set; }
        
        public string UserMobilePhone { get; set; }


        //***** 新增的資料表欄位（類別的屬性） ******************************        
        public bool UserApproved { get; set; }

        public int? DepartmentId { get; set; }


        //*************************************************************************
        //*** 導覽屬性（navigation property.）***
        // https://docs.microsoft.com/zh-tw/dotnet/framework/data/adonet/ef/language-reference/query-expression-syntax-examples-navigating-relationships

        public virtual DepartmentTable2 DepartmentTable2s { get; set; }   // 注意！後面是複數（s）
    }
}