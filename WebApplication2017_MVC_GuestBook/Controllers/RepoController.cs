using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//********************************************************
using WebApplication2017_MVC_GuestBook.Models;  // 自己動手寫上命名空間 -- 「專案名稱.Models」。
//********************************************************


// *** Repository（倉庫）******************************************
// (1).  搭配 Models 目錄。
// (2). 「介面（Interface）」檔名為 IUserTableRespository.cs
// (3). 「類別」檔名為 UserTableRepository.cs
//*******************************************************************

namespace WebApplication2017_MVC_GuestBook.Controllers
{
    public class RepoController : Controller
    {
        //*************************************   連結 MVC_UserDB 資料庫  ********************************* (start)
        //public MVC_UserDBContext _db = new MVC_UserDBContext();
        // 上面這一列，寫在「類別檔 UserTableRepository.cs」裡面了。 

        // 這個「IUserTableRepository介面」引用自「UserTableRepository類別」。
        Models.Repo.IUserTableRepository MyDB = new Models.Repo.UserTableRepository();   // 後面寫的是「類別名稱」

        //// 資料庫一旦開啟連線，用完就得要關閉連線與釋放資源。 https://msdn.microsoft.com/zh-tw/library/system.web.mvc.controller_methods(v=vs.118).aspx
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        MyDB.Db_Dispose();  //***這裡需要自己修改。 .Db_Dispose()寫在自己的 UserTableRepository.cs類別檔裡面
        //    }
        //    base.Dispose(disposing);
        //}

        //*************************************   連結 MVC_UserDB 資料庫  ********************************* (end)


        // GET: UserDB


        public ActionResult Index()
        {
            return View();   // 空白的畫面與動作。 無作用。
        }


        //===================================
        //== 新增 ==
        //===================================
        //*** 重點！！本範例 加入檢視畫面時，"沒有" 勾選「參考指令碼程式庫」，所以沒有產生表單驗證
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost, ActionName("Create")]   // 把下面的動作名稱，改成 CreateConfirm 試試看？
        [ValidateAntiForgeryToken]   // 避免XSS、CSRF攻擊
        public ActionResult Create(UserTable _userTable)
        {
            if ((_userTable != null) && (ModelState.IsValid))
            {
                if(MyDB.AddUser(_userTable))
                {
                    //return Content(" 新增一筆記錄，成功！");    // 新增成功後，出現訊息（字串）。
                    return RedirectToAction("List");
                }
            }
            return Content(" 新增一筆記錄，*** 失敗！***");    // 新增失敗後，出現訊息（字串）。
        }


        //===================================
        //== 列表（Master） ==  暫無分頁功能。
        //===================================
        public ActionResult List()
        {
            IQueryable<UserTable> ListAll = MyDB.ListAllUsers();

            //第二種寫法：
            if (ListAll == null)
            {   // 找不到任何記錄
                return HttpNotFound();
            }
            else   {
                return View(ListAll.ToList());   //直接把 UserTables的全部內容 列出來
            }
        }


        //===================================
        //== 列出一筆記錄的明細（Details） ==
        //===================================
        //[HttpPost]    // 改成這樣會報錯。請輸入網址，看見了什麼？？？？ /UserDB/Details?_ID=4
        ////                 // 錯誤訊息 -- '/' 應用程式中發生伺服器錯誤。        找不到資源。 
        [HttpGet]
        public ActionResult Details(int _ID)    // 網址 http://xxxxxx/UserDB/Details?ID=1 
        {
            // 第四種寫法：
            UserTable ut = MyDB.GetUserById(_ID);                      

            if (ut == null)
            {   // 找不到這一筆記錄
                return HttpNotFound();
            }
            else   {
                return View(ut);
            }
        }


        //===================================
        //== 刪除 ==
        //===================================

        //== 刪除前的 Double-Check，先讓您確認這筆記錄的內容？
        public ActionResult Delete(int _ID)    // 網址 http://xxxxxx/UserDB/Delete?_ID=1 
        {
            // 使用上方 Details動作的程式，先列出這一筆的內容，給使用者確認
            UserTable ut = MyDB.GetUserById(_ID);

            if (ut == null)
            {   // 找不到這一筆記錄
                return HttpNotFound();
            }
            else   {
                return View(ut);
            }
        }

        //== 真正刪除這一筆，並回寫資料庫 ===============
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]   // 避免XSS、CSRF攻擊
        // 避免刪除一筆記錄的安全漏洞 http://stephenwalther.com/archive/2009/01/21/asp-net-mvc-tip-46-ndash-donrsquot-use-delete-links-because
        public ActionResult DeleteConfirm(int _ID)
        {
            if (ModelState.IsValid)
            {
                if (MyDB.DeleteUser(_ID))
                {
                    //return Content(" 刪除一筆記錄，成功！");    // 刪除成功後，出現訊息（字串）。
                    return RedirectToAction("List");
                }
            }
            return Content(" 刪除一筆記錄，*** 失敗！***");    // 刪除失敗後，出現訊息（字串）。
        }
           


        //===================================
        //== 搜尋關鍵字。類似上面的 列表（Master） ==
        //== https://docs.microsoft.com/en-us/aspnet/mvc/overview/getting-started/introduction/adding-search
        //===================================
        [HttpPost]
        // 錯誤！  直接在網址輸入 http://xxxxxx/UserDB/Search2?_ID=MVC
        // 錯誤！  直接在網址輸入 http://xxxxxx/UserDB/Search2/MVC
        public ActionResult Search(string _ID)
        {
            // 首先，試試看，能否抓得到檢視頁面傳來的數值？
            // return Content("<h3> 檢視頁面傳來的 -- " + _SearchWord + "</h3>");
            ViewData["SW"] = _ID;

            //第二種寫法：
            if (!String.IsNullOrEmpty(_ID))
            {
                var ListAll = MyDB.GetUserByName(_ID);
                return View(ListAll.ToList());
                // 討論串  https://stackoverflow.com/questions/29824798/need-help-understanding-linq-in-mvc/29825045#29825045
            }
            else
            {   // 找不到任何記錄
                return HttpNotFound();
                //return Content(" 找不到任何一筆記錄，*** 搜尋失敗！***");
            }
        }


    }


}