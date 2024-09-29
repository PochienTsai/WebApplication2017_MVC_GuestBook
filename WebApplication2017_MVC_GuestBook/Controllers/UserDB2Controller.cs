using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

// 自己動手寫上命名空間 -- 「專案名稱.Models2」。
using WebApplication2017_MVC_GuestBook.Models2;  

// 以下三個命名空間，ADO.NET 會用到
using System.Web.Configuration;
using System.Data;
using System.Data.SqlClient;
using WebApplication2017_MVC_GuestBook.Models2Northwind;
using System.Data.Entity;

/// <summary>
/// 第六天課程（關連式資料表）         本範例修改之處：
/// (1) UserTable2資料表，多了一個 boolean值的欄位（SQL Server為 bit。0與1、true或false）。
///      新增（Create）的檢視畫面，會自動轉為 CheckBox。
///      
/// (2) /Models2/UserTable2.cs 裡面多了「驗證」，使用 System.ComponentModel.DataAnnotations命名空間
/// 
/// (3) 改用 ADO.NET程式來存取 DB。
/// </summary>

namespace WebApplication2017_MVC_GuestBook.Controllers
{
    public class UserDB2Controller : Controller
    {
        //*************************************   連結 MVC_UserDB2 資料庫  ******* (start)
        private MVC_UserDB2Context _db = new MVC_UserDB2Context();
        // 如果沒寫上方的命名空間 --「專案名稱.Models2」，就得寫成下面這樣，加註「Models2.」字樣。
        // private Models.MVC_UserDB2Context _db = new Models.MVC_UserDB2Context();

        // 資料庫一旦開啟連線，用完就得要關閉連線與釋放資源。https://msdn.microsoft.com/zh-tw/library/system.web.mvc.controller_methods(v=vs.118).aspx
        protected override void Dispose(bool disposing)
        {   // 有開啟DB連結，就得動手關掉、Dispose這個資源。https://msdn.microsoft.com/zh-tw/library/system.idisposable.dispose(v=vs.110).aspx
            // 或是 官方網站的教材（程式碼）https://github.com/aspnet/Docs/blob/master/aspnet/mvc/overview/getting-started/introduction/sample/MvcMovie/MvcMovie/Controllers/MoviesController.cs
            if (disposing)   {
                _db.Dispose();  //***這裡需要自己修改，例如 _db字樣
            }
            base.Dispose(disposing);
            // 資料庫一旦開啟連線，用完就得要關閉連線與釋放資源。
            // The base "Controller" class already implements the "IDisposable" interface, so this code simply adds an "override" to the 
            // "Dispose(bool)" method to explicitly dispose the context instance. 
            // ( "Dispose(bool)"方法標示為 virtual，所以可以用override覆寫。https://msdn.microsoft.com/zh-tw/library/dd492699(v=vs.118).aspx )

            // "Controller" class  https://msdn.microsoft.com/zh-tw/library/system.web.mvc.controller(v=vs.118).aspx
        }

        //// 如果找不到動作（Action）或是輸入錯誤的動作名稱，一律跳回首頁
        //// HandleUnknownAction方法標示為 virtual，所以可以用override覆寫。https://msdn.microsoft.com/zh-tw/library/dd492730(v=vs.118).aspx
        //protected override void HandleUnknownAction(string actionName)
        //{
        //    Response.Redirect("http://公司首頁(網址)/");
        //    base.HandleUnknownAction(actionName);
        //}
        //*************************************************************************** (end)



        //// 補 充 說 明
        //*** 使用 IQueryable的好處是什麼？？************************************
        // The method uses "LINQ to Entities" to specify the column to sort by.The code creates an IQueryable variable 
        // before the switch statement, modifies it in the switch statement, and calls the ".ToList()" method after the 
        // switch statement.When you create and modify IQueryable variables, no query is sent to the database. 
        //
        // The query is not executed until you convert the IQueryable object into a collection by calling a method such as ".ToList()".
        // （直到程式的最後，你把查詢結果 IQueryable，呼叫.ToList()時，這段LINQ才會真正被執行！）
        // Therefore, this code results in a single query that is not executed until the return View statement.

        // (1) http://blog.darkthread.net/post-2012-10-23-iqueryable-experiment.aspx
        //......發現 IQueryable<T> 是在 Server 端作過濾, 再將結果傳回 Client 端, 故若為資料庫存取, 應採用 IQueryable<T>
        // (2) http://jasper-it.blogspot.tw/2015/01/c-ienumerable-ienumerator.html
        //......在資料庫相關的環境下, 用 IQueryable<T> 的效能會比 IEnumerable<T> 來得好.

        //*****************************************************************************

        // 文章說明： When To Use IEnumerable, ICollection, IList And List（文章後面有一張圖）
        // http://www.claudiobernasconi.ch/2013/07/22/when-to-use-ienumerable-icollection-ilist-and-list/
        //  IEnumerable -- 只用於「唯讀」的資料展示。
        //  ICollection -- 您想修改集合或關心其大小(size)。
        //  IList -- 您想要修改集合，並關心集合中元素的排序和/或位置。


        // GET: UserDB2


        public ActionResult Index()
        {   //  無須使用 ViewModel，直接完成。

            //  請參閱圖片 UserDB2_Index.jpg，必須有這個 Context設定 才會自動出現下面的連結。
            //  請看 *** 檢視畫面！*** 自動產生 UserTable2裡面沒有的「科系名稱 (DepartmentName)」
            return View(_db.UserTable2s.ToList());  // 搭配「List」範本

            ////======================================
            ////== .NET Core 6.0版的改變 =====================
            ////======================================
            ////（錯誤）  // return View(_db.UserTable2s);  // 以前 .NET 4.x版MVC、EF的作法，在.NET Core出問題。透過SQL Profiler觀察，上述寫法沒有做JOIN。
            //// .NET Core 6.0版必須改寫如下：
            //return View(_db.UserTable2s.Include(x => x.DepartmentTable2s));

            //// 改寫以後，SQL Profiler的成果如下：
            ////  SELECT[u].[UserId], [u].[DepartmentId], [u].[UserApproved], [u].[UserBirthDay], [u].[UserMobilePhone], [u].[UserName], [u].[UserSex], [d].[DepartmentId], [d].[DepartmentName], [d].[DepartmentYear]
            //// FROM[UserTable2] AS[u]
            //// LEFT JOIN[DepartmentTable2] AS[d] ON[u].[DepartmentId] = [d].[DepartmentId]
        }

        public ActionResult Index_NoContext()
        {   //  無須使用 ViewModel，直接完成。
            //****************************
            // ** 不使用 ** -- 圖片 UserDB2_Index.jpg這個 Context設定。
            //****************************
            return View(_db.UserTable2s.ToList());  // 搭配「List」範本
        }

        //==============================================================================
        // *** (1) . 最簡單的寫法 ***   （不搭配ViewModel，直接使用原有的 Model來做。）
        // 因為 /Models2/DepartmentTables2.cs   最底下有寫 public virtual ICollection<UserTable2> UserTable2s { get; set; } 

        // *** [簡易版] 內容展示 -- 主表明細（Master Details / 主細表） ***
        //  一對多（一個科系有幾位學生？）
        public ActionResult IndexMasterDetails(int _ID = 1)  // 給 _ID變數，一個預設值
        {   ////方法一：
            return View(_db.DepartmentTable2s.Find(_ID));  // 以前 .NET 4.x版MVC、EF的作法，在.NET Core出問題。透過SQL Profiler觀察，沒有做JOIN。
            
            //// .NET Core 6.0版必須改寫如下：*******************************************************************
            ////return View(_db.DepartmentTable2s.Include(x=>x.UserTable2s).Where(d => d.DepartmentId == _ID).FirstOrDefault()); 
            //// 傳回單一筆數據。

            ////方法二：
            //return View(_db.DepartmentTable2s.Where(d => d.DepartmentId == _ID).Single());  // 務必確認 傳回值只有一個結果（不得為 空）
            
            ////  // 以前 .NET 4.x版MVC、EF的作法，在.NET Core出問題。透過SQL Profiler觀察，沒有做JOIN。
            //// .NET Core 6.0版必須改寫如下：*******************************************************************
            //return View(_db.DepartmentTable2s.Where(d => d.DepartmentId == _ID).Include(x => x.UserTable2s).Single());  // 傳回單一筆數據。
            
            // 檢視畫面，請選「Details」範本。  然後，檢視畫面的「下半段」需要自己寫 foreach迴圈
            // 模型類別 -- 請選「DepartmentTable2」

            //方法一 & 二：SQL Profile翻譯後的成果 (兩者相同) ----
            #region
            // (1) exec sp_executesql N'SELECT TOP (2) [Extent1].[DepartmentId] AS[DepartmentId], 
            //    [Extent1].[DepartmentName] AS[DepartmentName], 
            //    [Extent1].[DepartmentYear] AS[DepartmentYear]
            //FROM[dbo].[DepartmentTable2] AS[Extent1]
            //WHERE[Extent1].[DepartmentId] = @p0',N'@p0 int',@p0=1

            // (2) exec sp_executesql N'SELECT [Extent1].[UserId] AS[UserId], 
            //    [Extent1].[UserName] AS[UserName], 
            //    [Extent1].[UserSex] AS[UserSex], 
            //    [Extent1].[UserBirthDay] AS[UserBirthDay], 
            //    [Extent1].[UserMobilePhone] AS[UserMobilePhone], 
            //    [Extent1].[UserApproved] AS[UserApproved], 
            //    [Extent1].[DepartmentId] AS[DepartmentId]
            //FROM[dbo].[UserTable2] AS[Extent1]
            //WHERE[Extent1].[DepartmentId] = @EntityKeyValue1',N'@EntityKeyValue1 int',@EntityKeyValue1=1
            #endregion
        }


        //==============================================================================
        // 以 UserTable2s（學生）為主角。    .Include() 類似 SQL指令的 Inner Join
        public ActionResult IndexMasterDetails2(int _ID = 1)  // 給 _ID變數，一個預設值
        {   // 參考資料：使用 .include()   https://msdn.microsoft.com/en-us/library/jj574232(v=vs.113).aspx
            // https://ithelp.ithome.com.tw/articles/10161589

            // .Include() 請輸入字串，UserTable2s類別裡面，導覽屬性的「名稱」
            // 成功。   *** 想瞭解 .include()的用法，請執行下面這一段！！***
            ////第一種作法：
            var result = _db.UserTable2s.Include("DepartmentTable2s").OrderBy(u => u.DepartmentId);
            // SQL Profile翻譯後的成果 ----
            #region
            // SELECT [Extent1].[UserId] AS[UserId], 
            //    [Extent1].[UserName] AS[UserName], 
            //    [Extent1].[UserSex] AS[UserSex], 
            //    [Extent1].[UserBirthDay] AS[UserBirthDay], 
            //    [Extent1].[UserMobilePhone] AS[UserMobilePhone], 
            //    [Extent1].[UserApproved] AS[UserApproved], 
            //    [Extent1].[DepartmentId] AS[DepartmentId], 
            //    [Extent2].[DepartmentId] AS[DepartmentId1], 
            //    [Extent2].[DepartmentName] AS[DepartmentName], 
            //    [Extent2].[DepartmentYear] AS[DepartmentYear]
            //FROM[dbo].[UserTable2] AS[Extent1]
            //INNER JOIN[dbo].[DepartmentTable2] AS[Extent2] ON[Extent1].[DepartmentId] = [Extent2].[DepartmentId]
            //        ORDER BY[Extent1].[DepartmentId] ASC
            #endregion

            ////第二種作法：
            //var result = _db.UserTable2s.Include("DepartmentTable2s").OrderBy(u => u.DepartmentId).Where(d=>d.DepartmentId == _ID);

            return View(result);
            // 請使用「List」範本，使用的「模型類別」為 /Models2目錄下的 UserTable2s
        }


        // 以 UserTable2s（學生）為主角。  作法同上，透過 UserTable2s類別的「導覽屬性」來串連資料
        public string IndexMasterDetails2_Navigation(int _ID = 1)  // 給 _ID變數，一個預設值
        {   // 透過 UserTable2s類別的「導覽屬性」來串連資料
            // 原廠（中文）說明  https://docs.microsoft.com/zh-tw/dotnet/framework/data/adonet/ef/language-reference/query-expression-syntax-examples-navigating-relationships

            ////第三種作法。  輸出為「匿名類型」
            var result = from ut in _db.UserTable2s
                                 where ut.DepartmentId == _ID
                                 select new {
                                     uID = ut.UserId,     // 輸出結果，重新命名
                                     uNAME = ut.UserName,
                                     uSEX = ut.UserSex,
                                     uBIRTHDAY = ut.UserBirthDay,
                                     uDEPARTMENTID = ut.DepartmentId,
                                     DEPARTMENTs = ut.DepartmentTable2s };
                                     //                           //******************** 重點！！ UserTable2s的「導覽屬性」對應 -- 科系（Department2s）資料表
                                     // SQL Profile翻譯後的成果 ，放在下一個動作裡。

            // 把結果（ "一對多"的列表）呈現出來。
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (var endResult in result)
            {   // 重點！！ UserTable2s的「導覽屬性」對應 -- 科系（Department2s）資料表
                sb.Append("<h3>科系ID -- " + endResult.DEPARTMENTs.DepartmentId + " -- " + endResult.DEPARTMENTs.DepartmentName + "</h3>");   // 科系ID（DepartmentId）
                //                                                            // *****************重點！！                                    // ****************重點！！

                sb.Append("<br />========================<br />");
                sb.Append("<br />學生ID -- " + endResult.uID + " -- " + endResult.uNAME + " -- （科系ID）" + endResult.uDEPARTMENTID);
                //                                                           //***** 改用 select new { } 裡面的新名字
            }
            return sb.ToString();
            // 請使用「List」範本。從上一個檢視畫面來改，使用的「模型類別」為 /Models2目錄下的 UserTable2s
        }


        // ***** 很有趣的練習題。 *********************
        // 以 UserTable2s（學生）為主角。  需搭配 ViewModel。  透過 UserTable2s類別的「導覽屬性」來串連資料
        // 輸出的「匿名類型」，記得使用 ViewModel承接 ( /Models2目錄下的 UserTable2sViewModel ) 並展示在檢視畫面上
        public ActionResult IndexMasterDetails2_NavigationVM(int _ID = 1)  // 給 _ID變數，一個預設值
        {   // 原廠（中文）說明  https://docs.microsoft.com/zh-tw/dotnet/framework/data/adonet/ef/language-reference/query-expression-syntax-examples-navigating-relationships

            ////第三種作法（成功）。  輸出為「匿名類型」，需搭配 ViewModel。IndexMasterDetails5()動作 有類似作法
            var result = from ut in _db.UserTable2s
                         where ut.DepartmentId == _ID
                         //               ************************   //*** 重點！！跟上一個動作的差異在此！！
                         select new UserTable2ViewModel   //*** 重點！！如果您不使用 ViewModel，直接使用 UserTable2類別，
                         {          //    ************************
                                    // 執行檢視畫面時，就會出錯 --「LINQ to Entities 查詢中無法建構實體或複雜類型 'WebApplication2017_MVC_GuestBook.Models2.UserTable2'。」
                                    // 有趣的是 -- ViewModel只是直接把 UserTabl2.cs 複製過來而已（只有去除 [驗證]的部分，其餘都沒有修改）
                             UserId = ut.UserId,
                             UserName = ut.UserName,
                             UserSex = ut.UserSex,
                             UserBirthDay = ut.UserBirthDay,
                             UserMobilePhone = ut.UserMobilePhone,
                             UserApproved = ut.UserApproved,
                             DepartmentTable2s = ut.DepartmentTable2s
                         };   //                           //******************** 重點！！ 
                                                           // UserTable2s的「導覽屬性」對應 -- 科系（Department2s）資料表            

            //// 錯誤版本，讓您比對
            #region
            //var result = from ut in _db.UserTable2s
            //             where ut.DepartmentId == _ID
            //             select new UserTable2   //*** 重點！！如果您不使用 ViewModel，直接使用 UserTable2類別，
            //             {                // 執行檢視畫面時，就會出錯 --「LINQ to Entities 查詢中無法建構實體或複雜類型 'WebApplication2017_MVC_GuestBook.Models2.UserTable2'。」
            //                 UserId = ut.UserId,
            //                 UserName = ut.UserName,
            //                 UserSex = ut.UserSex,
            //                 UserBirthDay = ut.UserBirthDay,
            //                 UserMobilePhone = ut.UserMobilePhone,
            //                 UserApproved = ut.UserApproved,
            //                 DepartmentTable2s = ut.DepartmentTable2s
            //             };   //                           //******************** 重點！！ UserTable2s的「導覽屬性」對應 -- 科系（Department2s）資料表
            #endregion

            // 把結果（ "一對多"的列表）呈現出來。
            return View(result.ToList());
            // 請使用「List」範本。使用的「模型類別」為 /Models2目錄下的 UserTable2sViewModel

            // SQL Profile翻譯後的成果 ----
            #region
            //exec sp_executesql N'SELECT [Extent1].[UserId] AS[UserId], 
            //    [Extent1].[UserName] AS[UserName],  [Extent1].[UserSex] AS[UserSex], 
            //    [Extent1].[UserBirthDay] AS[UserBirthDay],  [Extent1].[UserMobilePhone] AS[UserMobilePhone], 
            //    [Extent1].[UserApproved] AS[UserApproved], [Extent2].[DepartmentId] AS[DepartmentId], 
            //    [Extent2].[DepartmentName] AS[DepartmentName], [Extent2].[DepartmentYear] AS[DepartmentYear]
            //FROM[dbo].[UserTable2] AS[Extent1]
            //INNER JOIN[dbo].[DepartmentTable2] AS[Extent2] ON[Extent1].[DepartmentId] = [Extent2].[DepartmentId]
            //        WHERE[Extent1].[DepartmentId] = @p__linq__0',N'@p__linq__0 int',@p__linq__0=1
            #endregion
        }


        //==============================================================================(start)
        // 以 DeaprtmentTable2s（科系）為主角。    .Include()類似 SQL指令的 Left Join
        // 最簡單、最正常的作法。呈現的結果也最單純！
        public ActionResult IndexMasterDetails3()
        {   // 參考資料：使用 .include()   https://msdn.microsoft.com/en-us/library/jj574232(v=vs.113).aspx

            //第一種作法。 .Include() 請輸入字串，DepartmentTable2s類別裡面，導覽屬性的「名稱」
            var result = _db.DepartmentTable2s.Include("UserTable2s").OrderBy(u => u.DepartmentId);
            // 推薦的作法，請看後續的 IndexJOIN動作
            return View(result.ToList());

            // SQL Profile翻譯後的成果 ----
            #region
            // SELECT [Project1].[DepartmentId] AS[DepartmentId], 
            //    [Project1].[DepartmentName] AS[DepartmentName], [Project1].[DepartmentYear] AS[DepartmentYear], 
            //    [Project1].[C1] AS[C1],  [Project1].[UserId] AS[UserId], 
            //    [Project1].[UserName] AS[UserName], [Project1].[UserSex] AS[UserSex], 
            //    [Project1].[UserBirthDay] AS[UserBirthDay], [Project1].[UserMobilePhone] AS[UserMobilePhone], 
            //    [Project1].[UserApproved] AS[UserApproved],  [Project1].[DepartmentId1] AS[DepartmentId1]
            // FROM (SELECT
            //           [Extent1].[DepartmentId] AS[DepartmentId],
            //           [Extent1].[DepartmentName] AS[DepartmentName],
            //           [Extent1].[DepartmentYear] AS[DepartmentYear],
            //           [Extent2].[UserId] AS[UserId], [Extent2].[UserName] AS[UserName],
            //           [Extent2].[UserSex] AS[UserSex], [Extent2].[UserBirthDay] AS[UserBirthDay],
            //           [Extent2].[UserMobilePhone] AS[UserMobilePhone], [Extent2].[UserApproved] AS[UserApproved],
            //           [Extent2].[DepartmentId] AS[DepartmentId1],
            //           CASE WHEN ([Extent2].[UserId] IS NULL) THEN CAST(NULL AS int) ELSE 1 END AS[C1]
            //           FROM [dbo].[DepartmentTable2] AS[Extent1]
            //           LEFT OUTER JOIN [dbo].[UserTable2] AS[Extent2] ON [Extent1].[DepartmentId] = [Extent2].[DepartmentId]
            //    )  AS [Project1]
            //    ORDER BY[Project1].[DepartmentId] ASC, [Project1].[C1] ASC
            #endregion
        }


        // 以 DeaprtmentTable2s（科系）為主角。    .Join() 翻譯成 SQL指令的INNER JOIN
        // 檢視畫面中，沒有任何東西。
        // https://docs.microsoft.com/zh-tw/dotnet/framework/data/adonet/ef/language-reference/method-based-query-syntax-examples-join-operators
        public string IndexMasterDetails3_Join()
        {    // 微軟範例，多對多。每一個 "一對多"的列表  

            // 傳回一個「匿名類型」
            //                      //********************第一個資料表 DepartmentTable2s，與第二個資料表 UserTable2s彼此 Join
            var result = _db.DepartmentTable2s.Join(_db.UserTable2s,
            //                     //********************              //************* 
                                ut => ut.DepartmentId,
                                dt => dt.DepartmentId,
                                (dt, ut) => new
                                {   // ***** 要列出來的屬性（查詢結果） ***** 
                                    DepartmentID = dt.DepartmentId,
                                    DepartmentNAME = dt.DepartmentName,

                                    UserID = ut.UserId,
                                    UserNAME = ut.UserName,
                                    UserDepartmentID = ut.DepartmentId
                                }).OrderBy(dt => dt.DepartmentID);
            //                              //******************注意！！這個 DepartmentID屬性是「自己命名」的，並非類別檔裡面的DepartmentId 

            // 上面已經JOIN完成，所以只要一個迴圈，把結果（多對多。每一個 "一對多"的列表）呈現出來。
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            foreach (var endResult in result)   {
                sb.Append("<br><h3>科系ID -- " + endResult.DepartmentID + "</h3>");   // 科系ID（DepartmentId）
                sb.Append("<br>========================<br>");
                sb.Append("<br>學生ID -- " + endResult.UserID + " -- " + endResult.UserNAME + " -- （科系ID）" + endResult.UserDepartmentID);
            }
            return sb.ToString();

            // SQL Profile翻譯後的成果 ---- (INNER JOIN)
            #region
            // SELECT [Extent1].[DepartmentId] AS[DepartmentId], 
            //    [Extent1].[DepartmentName] AS[DepartmentName], 
            //    [Extent2].[UserId] AS[UserId], 
            //    [Extent2].[UserName] AS[UserName], 
            //    [Extent2].[DepartmentId] AS[DepartmentId1]
            // FROM[dbo].[DepartmentTable2] AS[Extent1]
            // INNER JOIN[dbo].[UserTable2] AS[Extent2] ON[Extent1].[DepartmentId] = [Extent2].[DepartmentId]
            //        ORDER BY[Extent1].[DepartmentId] ASC
            #endregion
        }


        // 以 DeaprtmentTable2s（科系）為主角。    .GroupJoin()
        // 檢視畫面中，沒有任何東西。
        // https://docs.microsoft.com/zh-tw/dotnet/framework/data/adonet/ef/language-reference/method-based-query-syntax-examples-join-operators
        public string IndexMasterDetails3_GroupJoin()
        {    // 微軟範例，多對多。每一個 "一對多"的列表  

            // 重點！！傳回一個「匿名類型」。傳回值有三個 ---- 
            // (1).  dID（int / 科系ID）
            // (2).  userCount（int / 這個科系有幾名學生？）
            // (3).  TheUsers（IEnumerable<UserTable2> / 這個科系的所有學生（集合））
            var result = _db.DepartmentTable2s.GroupJoin(_db.UserTable2s,
                                        dt => dt.DepartmentId,
                                        ut => ut.DepartmentId,
                                        (dt, dtGroup) => 
                                        new {
                                                dID = dt.DepartmentId,
                                                userCount = dtGroup.Count(),   // 這個科系有幾名學生？
                                                TheUsers = dtGroup  // 這個科系的所有學生（集合，IEnumerable<UserTable2>）
                                        });

            // 雙重迴圈，把結果（多對多。每一個 "一對多"的列表）呈現出來。
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (var group in result)
            {
                sb.Append("<br><h3>科系ID -- " + group.dID + "</h3>");   // 科系ID（DepartmentId）
                sb.Append("<br>uCount（這個科系，共有 " +  group.userCount + " 名學生）<br>");

                foreach (var userInfo in group.TheUsers)    {  // 這個科系的所有學生（集合，IEnumerable<UserTable2>）
                    sb.Append("<br>學生ID --" + userInfo.UserId + " -- " + userInfo.UserName + " -- （科系ID）" + userInfo.DepartmentId);
                }
                sb.Append("<hr>");
            }
            return sb.ToString();

            // SQL Profile翻譯後的成果 ---- 變成多個子查詢
            #region
            // SELECT [Project2].[DepartmentId] AS[DepartmentId], 
            //    [Project2].[C2] AS[C1], [Project2].[C1] AS[C2], 
            //    [Project2].[UserId] AS[UserId], [Project2].[UserName] AS[UserName], 
            //    [Project2].[UserSex] AS[UserSex], [Project2].[UserBirthDay] AS[UserBirthDay], 
            //    [Project2].[UserMobilePhone] AS[UserMobilePhone], 
            //    [Project2].[UserApproved] AS[UserApproved], [Project2].[DepartmentId1] AS[DepartmentId1]
            // FROM(SELECT [Project1].[DepartmentId] AS[DepartmentId],
            //                       [Extent3].[UserId] AS[UserId], [Extent3].[UserName] AS[UserName],
            //                       [Extent3].[UserSex] AS[UserSex], [Extent3].[UserBirthDay] AS[UserBirthDay],
            //                       [Extent3].[UserMobilePhone] AS[UserMobilePhone], [Extent3].[UserApproved] AS[UserApproved],
            //                       [Extent3].[DepartmentId] AS[DepartmentId1],
            // CASE WHEN ([Extent3].[UserId] IS NULL) THEN CAST(NULL AS int) ELSE 1 END AS[C1], [Project1].[C1] AS[C2]
            //
            // FROM(SELECT [Extent1].[DepartmentId] AS[DepartmentId],
            //     (SELECT COUNT(1) AS[A1]
            //      FROM[dbo].[UserTable2] AS[Extent2]
            //      WHERE[Extent1].[DepartmentId] = [Extent2].[DepartmentId]) AS[C1]
            //      FROM[dbo].[DepartmentTable2] AS[Extent1] 
            //      ) AS[Project1]
            //
            // LEFT OUTER JOIN[dbo].[UserTable2] AS[Extent3] 
            //              ON[Project1].[DepartmentId] = [Extent3].[DepartmentId]  )  AS[Project2]
            // ORDER BY[Project2].[DepartmentId] ASC, [Project2].[C1] ASC
            #endregion
    }
        //==============================================================================(end)


        // 透過 Inner JOIN的查詢語法，傳回一段字串
        // 檢視畫面中，沒有任何東西。
        // 參考資料： https://blog.miniasp.com/post/2010/10/14/LINQ-to-SQL-Query-Tips-INNER-JOIN-and-LEFT-JOIN.aspx
        public string IndexMasterDetails4()
        {   
            // 第一種作法：
            var result = from d in _db.DepartmentTable2s
                               join u in _db.UserTable2s on d.DepartmentId equals u.DepartmentId
                              orderby d.DepartmentId
                              select new { d.DepartmentId, d.DepartmentName, u.UserId, u.UserName };

            // SQL Profile翻譯後的成果 ---- Inner Join
            #region
            // SELECT [Extent1].[DepartmentId] AS[DepartmentId], 
            //    [Extent1].[DepartmentName] AS[DepartmentName], 
            //    [Extent2].[UserId] AS[UserId], 
            //    [Extent2].[UserName] AS[UserName]
            // FROM[dbo].[DepartmentTable2] AS[Extent1]
            // INNER JOIN[dbo].[UserTable2] AS[Extent2] ON[Extent1].[DepartmentId] = [Extent2].[DepartmentId]
            #endregion

            //// 第二種作法：   *** 不建議！！***
            //var result = from d in _db.DepartmentTable2s.ToList()   //*** 使用 .ToList()
            //                    join u in _db.UserTable2s.ToList()    //*** 使用 .ToList()
            //                   on d.DepartmentId equals u.DepartmentId
            //                   select new { d.DepartmentId, d.DepartmentName, u.UserId, u.UserName };

            // SQL Profile翻譯後的成果 ---- 分成兩段
            #region
            // 第一段     SELECT [Extent1].[DepartmentId] AS[DepartmentId], 
            //    [Extent1].[DepartmentName] AS[DepartmentName], 
            //    [Extent1].[DepartmentYear] AS[DepartmentYear]
            //    FROM[dbo].[DepartmentTable2] AS[Extent1]
            // 第二段     SELECT [Extent1].[UserId] AS[UserId], 
            //    [Extent1].[UserName] AS[UserName], 
            //    [Extent1].[UserSex] AS[UserSex], 
            //    [Extent1].[UserBirthDay] AS[UserBirthDay], 
            //    [Extent1].[UserMobilePhone] AS[UserMobilePhone], 
            //    [Extent1].[UserApproved] AS[UserApproved], 
            //    [Extent1].[DepartmentId] AS[DepartmentId]
            //    FROM[dbo].[UserTable2] AS[Extent1]
            #endregion

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (var item in result)
            {
                sb.Append("<hr><br>科系ID：" + item.DepartmentId + " ( " + item.DepartmentName + " ) == ");
                sb.Append("學員ID：" + item.UserId + "==" + item.UserName);
            }
            return sb.ToString();
        }


        //=====================================
        // (2). ViewModel版（檔名 /Models2/UserDepartmentViewModel.cs）。 
        //       兩個 Table做 JOIN 並且展示在畫面上。
        // 缺點：重複的資料（科系）會不斷出現。需要額外過濾。
        //=====================================

        // 使用 ViewModel（一對一），檔名 UserDepartmentViewModel.cs
        // 同上一個範例。透過 Inner JOIN 查詢語法
        public ActionResult IndexMasterDetails5()
        {   // 參考資料： https://blog.miniasp.com/post/2010/10/14/LINQ-to-SQL-Query-Tips-INNER-JOIN-and-LEFT-JOIN.aspx
            // 第一種作法：
            var result = from d in _db.DepartmentTable2s
                                 join u in _db.UserTable2s on d.DepartmentId equals u.DepartmentId  // 兩個資料表的連結(JOIN)
                                orderby d.DepartmentId
                                select new UserDepartmentViewModel { DepartmentVM = d, UserVM = u };
            // ViewModel版（一對一。檔名 Models2/UserDepartmentViewModel.cs）。

            //SQL Profile翻譯後的成果 ----
            #region
            // SELECT [Extent1].[DepartmentId] AS[DepartmentId], 
            //    [Extent1].[DepartmentName] AS[DepartmentName], 
            //    [Extent1].[DepartmentYear] AS[DepartmentYear], 
            //    [Extent2].[UserId] AS[UserId],     [Extent2].[UserName] AS[UserName], 
            //    [Extent2].[UserSex] AS[UserSex],     [Extent2].[UserBirthDay] AS[UserBirthDay], 
            //    [Extent2].[UserMobilePhone] AS[UserMobilePhone],     [Extent2].[UserApproved] AS[UserApproved], 
            //    [Extent2].[DepartmentId] AS[DepartmentId1]
            // FROM[dbo].[DepartmentTable2] AS[Extent1]
            // INNER JOIN[dbo].[UserTable2] AS[Extent2] ON[Extent1].[DepartmentId] = [Extent2].[DepartmentId]

            // SELECT [Extent1].[UserId] AS[UserId], 
            //    [Extent1].[UserName] AS[UserName], 
            //    [Extent1].[UserSex] AS[UserSex], 
            //    [Extent1].[UserBirthDay] AS[UserBirthDay], 
            //    [Extent1].[UserMobilePhone] AS[UserMobilePhone], 
            //    [Extent1].[UserApproved] AS[UserApproved], 
            //    [Extent1].[DepartmentId] AS[DepartmentId]
            // FROM[dbo].[UserTable2] AS[Extent1]
            // WHERE[Extent1].[DepartmentId] = @EntityKeyValue1',N'@EntityKeyValue1 int',@EntityKeyValue1=1  。依照科系編號，依次執行 =2 / 3 / 4 /...13 數次
            #endregion

            ////===========================================================
            //// 第二種作法：   ***不建議！！***
            //var result = from d in _db.DepartmentTable2s.ToList()  //*** 使用 .ToList()
            //                   join u in _db.UserTable2s.ToList()  //*** 使用 .ToList()
            //                   on d.DepartmentId equals u.DepartmentId
            //                   select new UserDepartmentViewModel { DepartmentVM = d, UserVM = u };
            //// ViewModel版（一對一。檔名 Models2/UserDepartmentViewModel.cs）。

            // SQL Profile翻譯後的成果 ---- 分成兩段。跟上一個動作（IndexMasterDetails4）相同，再加下面這段。
            #region
            // SELECT [Extent1].[UserId] AS[UserId], 
            //    [Extent1].[UserName] AS[UserName], 
            //    [Extent1].[UserSex] AS[UserSex], 
            //    [Extent1].[UserBirthDay] AS[UserBirthDay], 
            //    [Extent1].[UserMobilePhone] AS[UserMobilePhone], 
            //    [Extent1].[UserApproved] AS[UserApproved], 
            //    [Extent1].[DepartmentId] AS[DepartmentId]
            // FROM[dbo].[UserTable2] AS[Extent1]
            // WHERE[Extent1].[DepartmentId] = @EntityKeyValue1',N'@EntityKeyValue1 int',@EntityKeyValue1=1  。依照科系編號，依次執行 =2 / 3 / 4 /...13 數次
            #endregion

            return View(result.ToList());
            //產生檢視畫面時，請選「List」範本。資料模型請選（檔名 Models2/UserDepartmentViewModel.cs）
        }


        // 上一個範例有個嚴重的錯誤，在檢視畫面上，「科系」會重複出現
        // 受限於 ViewModel的設計，只能在檢視畫面上，多加兩段程式碼，把重複的資料剔除掉
        // （動作並未修改，跟上一個動作相同。修改的地方在檢視畫面裡）
        public ActionResult IndexMasterDetails5_Fix()
        {   // 參考資料： https://blog.miniasp.com/post/2010/10/14/LINQ-to-SQL-Query-Tips-INNER-JOIN-and-LEFT-JOIN.aspx
            // 第一種作法：
            var result = from d in _db.DepartmentTable2s
                                 join u in _db.UserTable2s on d.DepartmentId equals u.DepartmentId  // 兩個資料表的連結(JOIN)
                                 orderby d.DepartmentId
                                 select new UserDepartmentViewModel { DepartmentVM = d, UserVM = u };
            // ViewModel版（一對一。檔名 Models2/UserDepartmentViewModel.cs）。

              return View(result.ToList());
            //產生檢視畫面時，請選「List」範本。資料模型請選（檔名 Models2/UserDepartmentViewModel.cs）
        }


        // 同上一個 IndexMasterDetails5 範例。加入 group by (將查詢結果分組)
        // 上一個範例（IndexMasterDetails5）有個嚴重的錯誤，在檢視畫面上，「科系」會重複出現
        // (1) ***必須改用ViewModel*** 位於 Models2 目錄下的 UDViewModel.cs檔 （一對多）
        // (2) 改用 .GroupJopin()方法來做
        public ActionResult IndexMasterDetails5_GroupBy()
        {    
            // .GroupJoin()方法   https://dotnettutorials.net/lesson/linq-group-join/
           var GroupJoinMS = _db.DepartmentTable2s //Outer Data Source （科系）
                                            //Performing Group Join with Inner Data Source
                                           .GroupJoin( _db.UserTable2s, //Inner Data Source（學生）
                                                            dept => dept.DepartmentId, //Outer Key Selector  i.e. the Common Property
                                                            stu => stu.DepartmentId,       //Inner Key Selector  i.e. the Common Property
                                                            (dept, stu) => new { dept, stu } //Projecting the Result to an Anonymous Type
                                            );

            //// (1) 先輸出純文字，測試成果
            //string strResult = "<hr />";
            ////Outer Foreach is for Each department
            //foreach (var item in GroupJoinMS)
            //{
            //    strResult+= "<br />（科系）Department :" + item.dept.DepartmentName + "<br /><br />";
            //    //Inner Foreach loop for each employee of a Particular department
            //    foreach (var student in item.stu)
            //    {
            //        strResult += "  （學生）UserId : " + student.UserId + " , UserName : " + student.UserName + "<br />";
            //    }
            //}
            //return Content(strResult);

            // (2) 搭配 ViewModel，輸出檢視畫面
            // 位於 Models2 目錄下的 UDViewModel.cs檔 （一對多）
            List<UDViewModel> myList = new List<UDViewModel>();

            //UDViewModel userUDViewModel = new UDViewModel();
            //DepartmentTable2 myDepart = new DepartmentTable2();  //科系
            

            foreach (var item in GroupJoinMS)
            {
                UDViewModel userUDViewModel = new UDViewModel();
                userUDViewModel.DVM = item.dept;   // (1) 科系。加入UDViewModel類別 (ViewModel)

                // (2) 這個科系底下的所有「學生」。Inner Foreach loop for each employee of a Particular department
                List<UserTable2> myStuList = new List<UserTable2>();    //這個科系底下的所有「學生」
                foreach (UserTable2 student in item.stu)
                {
                    myStuList.Add(student);  //單一學生，加入 List裡面
                }
                userUDViewModel.UVM = myStuList; //把這個科系的「所有學生List<UserTable2>」加入UDViewModel類別 (ViewModel)裡面
                // 學生。加入UDViewModel類別 (ViewModel)


                myList.Add(userUDViewModel);  // 把上面完成的最後結果，加入List<>
                // 位於 Models2 目錄下的 UDViewModel.cs檔 （一對多）
            }
            return View(myList);
            // 資料來源 https://dotnettutorials.net/lesson/linq-group-join/
        }



        //==============================================================================
        // 透過 INNER JOIN 查詢語法   （*** 推薦作法！！***） 
        //         搭配 ViewModel，位於 /Models2目錄下的 UserDepartmentViewModel
        //         以學生（User）為主軸
        //==============================================================================
        // 參考資料：兩個 Table做 JOIN。  http://www.codingfusion.com/Post/How-to-Join-tables-and-return-result-into-view-usi 
        //                   https://blog.miniasp.com/post/2010/10/14/LINQ-to-SQL-Query-Tips-INNER-JOIN-and-LEFT-JOIN.aspx
        public ActionResult IndexJOIN()
        {   
            var resultViewModel = from u in _db.UserTable2s
                                                 join d in _db.DepartmentTable2s
                                                 on u.DepartmentId equals d.DepartmentId into dt2  // ***兩個資料表的 "關連"

                                                 from d in dt2.DefaultIfEmpty()    // ***寫成 Inner JOIN 。
                                                 //orderby d.DepartmentId
                                                 select new UserDepartmentViewModel { UserVM = u, DepartmentVM = d };

            return View(resultViewModel);
            // 產生檢視畫面時，請 不要 選「資料內容類別」https://stackoverflow.com/questions/20816115/do-viewmodels-need-keys

            // SQL Profile翻譯後的成果 ---- 很正常的SQL指令。
            #region
            // SELECT [Extent1].[UserId] AS[UserId], 
            //    [Extent1].[UserName] AS[UserName],  [Extent1].[UserSex] AS[UserSex], 
            //    [Extent1].[UserBirthDay] AS[UserBirthDay], [Extent1].[UserMobilePhone] AS[UserMobilePhone], 
            //    [Extent1].[UserApproved] AS[UserApproved],  [Extent1].[DepartmentId] AS[DepartmentId], 
            //    [Extent2].[DepartmentId] AS[DepartmentId1], [Extent2].[DepartmentName] AS[DepartmentName], 
            //    [Extent2].[DepartmentYear]  AS[DepartmentYear]
            // FROM[dbo].[UserTable2]  AS [Extent1]
            // INNER JOIN[dbo].[DepartmentTable2] AS [Extent2] ON [Extent1].[DepartmentId] = [Extent2].[DepartmentId]
        #endregion
    }



        //*** 同上，改用 ADO.NET來做（SQL指令的 JOIN）  搭配 ViewModel也可做出相同成果！*****
        //    （檔名 Models2/UserDepartmentViewModel.cs）
        public ActionResult Index_AdoNet()
        {
            //----上面已經事先寫好NameSpace --  using System.Web.Configuration; ----     
            // Web.Config設定檔裡面的資料庫連結字串（ConnectionString），此連線名為 MVC_UserDB2
            SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MVC_UserDB2"].ConnectionString);
            //== 第一，連結資料庫。
            Conn.Open();   //---- 開啟連結。這時候才連結DB

            //== 第二，執行SQL指令，使用 Left Join。
            SqlDataReader dr = null;
            string sqlstr = "Select ut.UserId, ut.UserName, ut.DepartmentId, dt.DepartmentId, dt.DepartmentName ";
            sqlstr += "From [UserTable2] ut Left Join [DepartmentTable2] dt ";
            sqlstr += "On ut.DepartmentId = dt.DepartmentId";

            SqlCommand cmd = new SqlCommand(sqlstr, Conn);
            dr = cmd.ExecuteReader();   //---- 執行SQL指令（Select，搜尋、查詢）取出資料

            //==第三，自由發揮，把執行後的結果呈現到畫面上。（ViewModel檔名： Models2/UserDepartmentViewModel.cs）
            List<UserDepartmentViewModel> resultViewModel = new List<UserDepartmentViewModel>();

            while (dr.Read())
            {   //// (1). 傳統寫法：
                //UserTable2 ut = new UserTable2();                
                //    ut.UserId = Convert.ToInt32(dr["UserId"]);
                //    ut.UserName = dr["UserName"].ToString();
                //    ut.DepartmentId = Convert.ToInt32(dr["DepartmentId"]);

                //// (2). 也可以這樣寫（新寫法）：
                UserTable2 ut = new UserTable2
                {
                    UserId = Convert.ToInt32(dr["UserId"]),
                    UserName = dr["UserName"].ToString(),
                    DepartmentId = Convert.ToInt32(dr["DepartmentId"])
                };

                DepartmentTable2 dt = new DepartmentTable2
                {
                    DepartmentId = Convert.ToInt32(dr["DepartmentId"]),
                    DepartmentName = dr["DepartmentName"].ToString()
                };

                // *********************************************************************************
                // *** 把上面兩個資料表（對應的類別檔）的數值，放入 ViewModel裡面。
                resultViewModel.Add(new UserDepartmentViewModel { UserVM = ut, DepartmentVM = dt });
                // *********************************************************************************
            }

            // == 第四，釋放資源、關閉資料庫的連結。       
            if (dr != null)   {
                cmd.Cancel();
                dr.Close();
            }
            if (Conn.State == ConnectionState.Open)   {
                Conn.Close();
            }

            return View(resultViewModel.ToList());
            // 產生檢視畫面時，請 不要 選「資料內容類別」
        }



        // *** 同上，ViewModel版。（檔名 /Models2/UserDepartmentViewModel.cs）
        // ***  推薦作法！！***         內容展示 -- 主表明細（Master Details / 主細表） ***
        // ***  一對多（一個科系有幾位學生？）。一次只能出現一個科系，算是Details的功能。  ***

        // 請輸入  /UserDB2/IndexJOINMasterDetails?_ID=4  觀看結果
        public ActionResult IndexJOINMasterDetails(int _ID=4)  // 給 _ID變數，一個預設值
        {   // 參考資料：兩個 Table做 JOIN。  http://www.codingfusion.com/Post/How-to-Join-tables-and-return-result-into-view-usi      
            // https://kevintsengtw.blogspot.com/2012/12/aspnet-mvc-viewmodel.html
            var resultViewModel = from u in _db.UserTable2s
                                  join d in _db.DepartmentTable2s
                                  on u.DepartmentId equals d.DepartmentId into dt2  //***重點！！
                                  from d in dt2.DefaultIfEmpty()  //***重點！！
                                  where d.DepartmentId == _ID   // 跟上一個範例不同之處。
                                  select new UserDepartmentViewModel { UserVM = u, DepartmentVM = d };
            // ViewModel版（檔名 Models2/UserDepartmentViewModel.cs）。

            // SQL Profile翻譯後的成果 ---- 
            #region
            // exec sp_executesql N'SELECT [Extent1].[UserId] AS[UserId], 
            //    [Extent1].[UserName] AS[UserName],     [Extent1].[UserSex] AS[UserSex], 
            //    [Extent1].[UserBirthDay] AS[UserBirthDay],     [Extent1].[UserMobilePhone] AS[UserMobilePhone], 
            //    [Extent1].[UserApproved] AS[UserApproved],     [Extent1].[DepartmentId] AS[DepartmentId], 
            //    [Extent2].[DepartmentId] AS[DepartmentId1],    [Extent2].[DepartmentName] AS[DepartmentName], 
            //    [Extent2].[DepartmentYear] AS[DepartmentYear]
            //FROM[dbo].[UserTable2] AS[Extent1]
            //INNER JOIN[dbo].[DepartmentTable2] AS[Extent2] ON[Extent1].[DepartmentId] = [Extent2].[DepartmentId]
            //        WHERE[Extent2].[DepartmentId] = @p__linq__0',N'@p__linq__0 int',@p__linq__0=4
            #endregion

            return View(resultViewModel.Distinct());
            // 產生檢視畫面時，請 不要 選「資料內容類別」https://stackoverflow.com/questions/20816115/do-viewmodels-need-keys
        }



        //******************************************************************************
        // (3). 透過 ViewModel （多對多，列表） 展示畫面 --
        //                ViewModels 位於 /Models2 目錄下的 UDViewModel.cs檔 （一對多）
        //                列出「多筆」科系，每一個科系底下又有「多個」學生
        //******************************************************************************
        public ActionResult IndexVM1()
        {   //  使用 ViewModel，位於 Models2 目錄下的 UDViewModel.cs檔 （一對多）
            List<UDViewModel> resultVM = new List<UDViewModel>();

            // 將「多筆」科系資料，逐一放入 ViewModel的第一個類別內
            foreach (var dt in _db.DepartmentTable2s)    // dt 就是 「每一個科系」
            {
                // 就讀 "這一科系"的「多筆」學生資料，逐一放入 ViewModel的第二個類別內
                List<UserTable2> u = new List<UserTable2>();
                foreach (var ut in _db.UserTable2s.Where(item =>
                                                                                    item.DepartmentId == dt.DepartmentId))
                {
                    u.Add(ut);
                }

                // --- 完成了（一對多）！正式寫入 ViewModels裡面 -----------------------------
                resultVM.Add(new UDViewModel
                {
                    DVM = dt,   // 寫入ViewModel的第一個類別（一個 科系 Department）
                    UVM = u     // 寫入ViewModel的第二個類別（多個 學生 User）
                });
            }
            return View(resultVM.ToList());
            // 新增「檢視」時，第四格「資料內容類別」請留白。

            // SQL Profile翻譯後的成果 ---- 分成兩段。
            #region
            //SELECT [Extent1].[DepartmentId] AS[DepartmentId], 
            //    [Extent1].[DepartmentName] AS[DepartmentName], 
            //    [Extent1].[DepartmentYear] AS[DepartmentYear]
            //FROM[dbo].[DepartmentTable2] AS[Extent1]

            //exec sp_executesql N'SELECT [Extent1].[UserId] AS[UserId], 
            //    [Extent1].[UserName] AS[UserName], 
            //    [Extent1].[UserSex] AS[UserSex], 
            //    [Extent1].[UserBirthDay] AS[UserBirthDay], 
            //    [Extent1].[UserMobilePhone] AS[UserMobilePhone], 
            //    [Extent1].[UserApproved] AS[UserApproved], 
            //    [Extent1].[DepartmentId] AS[DepartmentId]
            //FROM[dbo].[UserTable2] AS[Extent1]
            //WHERE[Extent1].[DepartmentId] = @p__linq__0',N'@p__linq__0 int',@p__linq__0=1  。依照科系編號，依次執行 =2 / 3 / 4 /...13 數次
            #endregion
        }


        //*** 同上，改用 ADO.NET來做（分成兩段SQL指令，不用JOIN）  搭配 ViewModel也可做出相同成果！*****
        //    使用 List<UDViewModel> 一對一的「列表」 來呈現結果。（檔名 Models2/UDViewModel.cs）

        // 改用 Dapper來做，變得很簡單！！請參閱 AdoNetDapper控制器底下的 IndexVM1動作
        public ActionResult IndexVM1_AdoNet()
        {
            // DB連結字串使用了 MARS (MultipleActiveResultSets)
            SqlConnection Conn = new SqlConnection("Data Source =.\\SQLExpress; Initial Catalog = MVC_UserDB2; Integrated Security = True; MultipleActiveResultSets = True");
            //== 第一，連結資料庫。
            Conn.Open();   //---- 開啟連結。這時候才連結DB

            //== 第二，執行SQL指令，使用 Left Join。
            SqlDataReader dr1 = null;
            SqlDataReader dr2 = null;
            SqlCommand cmd1 = new SqlCommand("Select * From [DepartmentTable2] Order By DepartmentId", Conn);
            SqlCommand cmd2 = null;

            dr1 = cmd1.ExecuteReader();   //---- 執行SQL指令（Select，搜尋、查詢）取出資料

            //==第三，自由發揮，把執行後的結果呈現到畫面上。（ViewModel檔名： Models2/UDViewModel.cs）
            List<UDViewModel> resultViewModel = new List<UDViewModel>();

            while (dr1.Read())
            {   // === (1) 將這一個科系，寫入類別 =================
                DepartmentTable2 dt = new DepartmentTable2
                {
                    DepartmentId = Convert.ToInt32(dr1["DepartmentId"]),
                    DepartmentName = dr1["DepartmentName"].ToString()
                };


                //---- 啟動MARS之後（MultipleActiveResultSets=True）。
                //---- 第一個 DataReader（變數名稱dr1）尚未關閉，就直接使用第二個DataReader（變數名稱dr2）。
                //=== (2) 列出這個科系裡面的所有「學生」============
                #region
                cmd2 = new SqlCommand("Select * From UserTable2 Where DepartmentId = @ID", Conn);
                cmd2.Parameters.AddWithValue("@ID", dr1["DepartmentId"]);
                dr2 = cmd2.ExecuteReader();

                List<UserTable2> utList = new List<UserTable2>();

                if (dr2.HasRows)   {                    
                    while (dr2.Read())
                    {
                         utList.Add(new UserTable2
                         {
                            UserId = Convert.ToInt32(dr2["UserId"]),
                            UserName = dr2["UserName"].ToString(),
                            DepartmentId = Convert.ToInt32(dr2["DepartmentId"])
                        });
                    }
                }

                if (dr2 != null)
                {
                    cmd2.Cancel();
                    dr2.Close();
                }
                #endregion


                // *********************************************************************************
                // *** 把上面兩個資料表（對應的類別檔）的數值，放入 ViewModel裡面。
                resultViewModel.Add(new UDViewModel { DVM = dt, UVM = utList });
                // *********************************************************************************
            }

            // == 第四，釋放資源、關閉資料庫的連結。       
            if (dr1 != null)   {
                cmd1.Cancel();
                dr1.Close();
            }
            if (Conn.State == ConnectionState.Open)   {
                Conn.Close();
            }

            return View(resultViewModel.ToList());
            // 產生檢視畫面時，請 不要 選「資料內容類別」
        }



        //******************************************************************************
        // (4). 透過 ViewModel （多對多，列表） 展示畫面 --
        //                ViewModels 位於 /Models2 目錄下的 UDmultiViewModel.cs檔 （多對多）
        //                列出「多筆」科系，每一個科系底下又有「多個」學生
        //******************************************************************************
        public ActionResult IndexVM2()
        {   //  使用 ViewModel，位於 Models2 目錄下的 UDmultiViewModel.cs檔 （一對多）

            // --- 完成了（多對多）！正式寫入 ViewModels（多對多）裡面 -------------------
            UDmultiViewModel resultVM = new UDmultiViewModel
            {   //                                              //*********（改寫）
                DVM = _db.DepartmentTable2s.ToList(),   // 寫入ViewModel的第一個類別（多個 科系 Department）
                UVM = _db.UserTable2s.ToList()                // 寫入ViewModel的第二個類別（多個 學生 User）
            };   //                                //*********（改寫） 
            
            // SQL Profile翻譯後的成果 ---- 分成兩段。跟上一個動作（IndexMasterDetails4）相同

            return View(resultVM);
            // 新增「檢視」時，請選 Details範本，第四格「資料內容類別」請留白。
            // 產生後的檢視畫面為空白，必須自己動手改造、自己寫 for 迴圈。  
        }


        public ActionResult IndexVM2_AdoNet()
        {   //  使用 ViewModel，位於 Models2 目錄下的 UDmultiViewModel.cs檔 （一對多）
            // Web.Config設定檔裡面的資料庫連結字串（ConnectionString），此連線名為 MVC_UserDB2
            SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MVC_UserDB2"].ConnectionString);
            //== 第一，連結資料庫。
            Conn.Open();   //---- 開啟連結。這時候才連結DB

            //== 第二，執行SQL指令。(將所有科系，放入List裡面。)
            SqlDataReader dr1 = null;
            SqlCommand cmd1 = new SqlCommand("Select * From [DepartmentTable2]", Conn);
            dr1 = cmd1.ExecuteReader();   //---- 執行SQL指令（Select，搜尋、查詢）取出資料
            #region
            //==第三，自由發揮，把執行後的結果呈現到畫面上。
            List<DepartmentTable2> resultDT = new List<DepartmentTable2>();
            // 為何不用 IQueryable<T>來做呢？？ 請參考 https://stackoverflow.com/questions/434737/how-do-i-add-a-new-record-to-an-iqueryable-variable
            // Remember that an IQueryable is not a result set, it is a query.
            while (dr1.Read())
            {    // 傳統方法，一筆一筆記錄（類別）加入List 裡面
                resultDT.Add(new DepartmentTable2
                {
                    DepartmentId = Convert.ToInt32(dr1["DepartmentId"]),
                    DepartmentName = dr1["DepartmentName"].ToString()
                });
            }
            // == 第四，釋放資源、關閉資料庫的連結。       
            if (dr1 != null) {
                cmd1.Cancel();
                dr1.Close();
            }
            #endregion

            //== 第二，執行SQL指令。(將所有學生，放入List裡面。)
            SqlDataReader dr2 = null;
            SqlCommand cmd2 = new SqlCommand("Select * From [UserTable2]", Conn);
            dr2 = cmd2.ExecuteReader();   //---- 執行SQL指令（Select，搜尋、查詢）取出資料
            #region
            //==第三，自由發揮，把執行後的結果呈現到畫面上。
            List<UserTable2> resultUT = new List<UserTable2>();
            // 為何不用 IQueryable<T>來做呢？？ 請參考 https://stackoverflow.com/questions/434737/how-do-i-add-a-new-record-to-an-iqueryable-variable
            // Remember that an IQueryable is not a result set, it is a query.
            while (dr2.Read())
            {    // 傳統方法，一筆一筆記錄（類別）加入List 裡面
                resultUT.Add(new UserTable2
                {
                    UserId = Convert.ToInt32(dr2["UserId"]),
                    UserName = dr2["UserName"].ToString(),
                    DepartmentId = Convert.ToInt32(dr2["DepartmentId"])
                });
            }
            // == 第四，釋放資源、關閉資料庫的連結。       
            if (dr2 != null)   {
                cmd2.Cancel();
                dr2.Close();
            }
            #endregion

            if (Conn.State == ConnectionState.Open)   {
                Conn.Close();
            }

            //-------------------------------------------------------------------------------------
            // --- 完成了（多對多）！正式寫入 ViewModels（多對多）裡面 ----------------
            UDmultiViewModel resultVM = new UDmultiViewModel   {   
                DVM = resultDT,   // 寫入ViewModel的第一個類別（多個 科系 Department）
                UVM = resultUT   // 寫入ViewModel的第二個類別（多個 學生 User）
            };   
            //-------------------------------------------------------------------------------------

            return View(resultVM);
            // 新增「檢視」時，請選 Details範本，第四格「資料內容類別」請留白。
            // 產生後的檢視畫面為空白，必須自己動手改造、自己寫 for 迴圈。  
        }




        //====================================================
        //=== 下拉式選單（DropDownLinst）  +  ADO.NET
        //====================================================
        public ActionResult Create()
        {   // 檢視畫面上，有多種 下拉式選單（DropDownList）的寫法
            return View();
            // *** 重點！！加入檢視畫面時，務必勾選「參考指令碼程式庫」才能產生表單驗證
        }

        [HttpPost]
        [ValidateAntiForgeryToken]   // 避免 CSRF攻擊，請配合檢視畫面 Html.BeginForm()表單裡的「@Html.AntiForgeryToken()」這一列程式
        public ActionResult Create(UserTable2 _userTable)
        {
            //**********************************************
            #region  //** 第一種作法。EF (Entity Framework)的作法。
            //// 程式碼 折疊（區塊關閉）
            //if (ModelState.IsValid)
            //{
            //    _db.UserTable2s.Add(_userTable);
            //    _db.SaveChanges();
            //}

            ////return Content(" 新增一筆記錄，成功！");    // 新增成功後，出現訊息（字串）。
            //return RedirectToAction("List");
            #endregion

            //**********************************************
            #region  //** 第二種作法。使用 ADO.NET做「資料新增」
            // 程式碼 折疊（區塊關閉）

            int RecordsAffected = 0;
            if (ModelState.IsValid)
            {   //== (1). 開啟資料庫的連結。上面已經事先寫好NameSpace --  using System.Web.Configuration; ----     
                SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MVC_UserDB2"].ConnectionString);
                Conn.Open();

                //== (2). 執行SQL指令。或是查詢、撈取資料。  
                //*** 參數（Parameter），可避免SQL Injection攻擊 ************** (start)
                string sqlstr = "INSERT INTO [UserTable2] ([UserName],[UserSex],[UserBirthDay],[UserMobilePhone],[UserApproved], [DepartmentId]) ";
                sqlstr += " VALUES (@UserName,@UserSex, @UserBirthDay, @UserMobilePhone, @UserApproved, @DepartmentId)";
                SqlCommand cmd = new SqlCommand(sqlstr, Conn);

                ////-- 方法一。精簡版。
                cmd.Parameters.AddWithValue("@UserName", _userTable.UserName);  
                cmd.Parameters.AddWithValue("@UserSex", _userTable.UserSex);
                cmd.Parameters.AddWithValue("@UserBirthDay", _userTable.UserBirthDay);
                cmd.Parameters.AddWithValue("@UserMobilePhone", _userTable.UserMobilePhone);
                cmd.Parameters.AddWithValue("@UserApproved", _userTable.UserApproved);
                cmd.Parameters.AddWithValue("@DepartmentId", _userTable.DepartmentId);

                ////-- 方法二，另一種寫法，效率高！只寫一個參數作為示範。
                //cmd.Parameters.Add("@UserName", SqlDbType.NVarChar, 50)
                //cmd.Parameters("@UserName").Value = _userTable.UserName   ////-- 參考資料 http://msdn.microsoft.com/zh-tw/library/system.data.sqlclient.sqlparametercollection.addwithvalue.aspx
                //*** 參數（Parameter），可避免SQL Injection攻擊 **************  (end)

                //== (3). 自由發揮。
                RecordsAffected = cmd.ExecuteNonQuery();

                //== (4). 釋放資源、關閉資料庫的連結。
                cmd.Cancel();
                if (Conn.State == ConnectionState.Open)   {
                    Conn.Close();
                }
            }
            else
            {   // 需搭配檢視畫面的 @Html.ValidationSummary(true)
                // 並且 return View() 回到原本的新增畫面上，顯示錯誤訊息！
                ModelState.AddModelError("", "輸入資料有誤！");             
            }
            return Content(" [ADO.NET] 執行 Insert Into的SQL指令以後，影響了" + RecordsAffected + "列的紀錄。");
            //-- 或者是，您可以這樣寫，代表有新增一些紀錄。
            //if (RecordsAffected > 0)  {
            //    return Content(" [ADO.NET] 資料新增成功。共有" + RecordsAffected + "列紀錄被影響。");
            // }
            #endregion
        }
        
 


        //=============================================
        //== 列表（Master） ==  // 使用 ADO.NET做「資料列表」
        //=============================================
        public ActionResult List()
        {
            //----上面已經事先寫好NameSpace --  using System.Web.Configuration; ----     
            // Web.Config設定檔裡面的資料庫連結字串（ConnectionString），此連線名為 MVC_UserDB2
            SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MVC_UserDB2"].ConnectionString);

            //== 第一，連結資料庫。
            Conn.Open();   //---- 開啟連結。這時候才連結DB

            //== 第二，執行SQL指令。
            SqlDataReader dr = null;
            SqlCommand cmd = new SqlCommand("Select * From [UserTable2]", Conn);
            dr = cmd.ExecuteReader();   //---- 執行SQL指令（Select，搜尋、查詢）取出資料

            //==第三，自由發揮，把執行後的結果呈現到畫面上。
            //==寫迴圈，展示每一筆記錄與其中的欄位==
            List<UserTable2> result = new List<UserTable2>();

            while (dr.Read())
            {    // 傳統方法，一筆一筆加入List 裡面
                result.Add(new UserTable2 {
                    UserId = Convert.ToInt32(dr["UserId"]),
                    UserName = dr["UserName"].ToString(),
                    UserSex = dr["UserSex"].ToString(),
                    UserBirthDay = Convert.ToDateTime(dr["UserBirthDay"]),
                    UserMobilePhone = dr["UserMobilePhone"].ToString(),
                    UserApproved = Convert.ToBoolean(dr["UserApproved"])
                });
            }

            // == 第四，釋放資源、關閉資料庫的連結。       
            if (dr != null)   {
                cmd.Cancel();
                dr.Close();
            }
            if (Conn.State == ConnectionState.Open)   {
                Conn.Close();
            }

            return View(result);   //直接把 UserTables的全部內容 列出來
        }


        //=============================================
        //== 單一文章的內容（Details） ==  // 使用 ADO.NET做「資料列表」
        //      建議使用 Dateails2 動作
        //=============================================

        //**** 把資料填入 List 裡面（List<UserTable2>） ****  
        // 檢視畫面 需做一些修正，不然會報錯！
        // 重點！新增檢視的時候，請選擇「List」樣版！！  因為本範例把資料填入 List 裡面（List<UserTable2>）
        public ActionResult Details(int? _ID)    // 網址 http://xxxxxx/UserDB2/Details?_ID=1 
        {
            if (_ID == null)
            {   // 沒有輸入文章編號（ID），就會報錯 - Bad Request
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            //----上面已經事先寫好NameSpace --  using System.Web.Configuration; ----     
            // Web.Config設定檔裡面的資料庫連結字串（ConnectionString），此連線名為 MVC_UserDB2
            SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MVC_UserDB2"].ConnectionString);

            //== 第一，連結資料庫。
            Conn.Open();   //---- 開啟連結。這時候才連結DB

            //== 第二，執行SQL指令。
            SqlDataReader dr = null;
            SqlCommand cmd = new SqlCommand("Select * From [UserTable2] Where UserId = @ID", Conn);
            cmd.Parameters.AddWithValue("@ID", _ID.ToString());

            dr = cmd.ExecuteReader();   //---- 執行SQL指令（Select，搜尋、查詢）取出資料

            //==第三，自由發揮，把執行後的結果呈現到畫面上。
            List<UserTable2> result = new List<UserTable2>();

            if (dr.Read())  {    // （不是迴圈）有資料的話，將會讀取到一筆記錄            
                result.Add(new UserTable2 {
                    UserId = Convert.ToInt32(dr["UserId"]),
                    UserName = dr["UserName"].ToString(),
                    UserSex = dr["UserSex"].ToString(),
                    UserBirthDay = Convert.ToDateTime(dr["UserBirthDay"]),
                    UserMobilePhone = dr["UserMobilePhone"].ToString(),
                    UserApproved = Convert.ToBoolean(dr["UserApproved"])
                });
            }

            // == 第四，釋放資源、關閉資料庫的連結。       
            if (dr != null)   {
                cmd.Cancel();
                dr.Close();
            }
            if (Conn.State == ConnectionState.Open)   {
                Conn.Close();
            }

            return View(result);   // 把這一筆記錄呈現出來
            // 重點！新增檢視的時候，請選擇「 List」樣版！！  因為本範例把資料填入 List 裡面（List<UserTable2>） 
            // 如果您選擇「Details」樣版，就會出現錯誤，請看 DetailsError.cshtml。想想看是哪裡有錯？
        }


        //**** 把資料填入 UserTable2「類別」 裡面 ****   // 重點！新增檢視的時候，請選擇「 Details」樣版！！
        public ActionResult Details2(int? _ID)    // 網址 http://xxxxxx/UserDB2/Details2?_ID=1 
        {
            if (_ID == null)
            {   // 沒有輸入文章編號（ID），就會報錯 - Bad Request
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            //----上面已經事先寫好NameSpace --  using System.Web.Configuration; ----     
            // Web.Config設定檔裡面的資料庫連結字串（ConnectionString），此連線名為 MVC_UserDB2
            SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MVC_UserDB2"].ConnectionString);

            //== 第一，連結資料庫。
            Conn.Open();   //---- 開啟連結。這時候才連結DB

            //== 第二，執行SQL指令。
            SqlDataReader dr = null;
            SqlCommand cmd = new SqlCommand("Select * From [UserTable2] Where UserId = @ID", Conn);
            cmd.Parameters.AddWithValue("@ID", _ID.ToString());

            dr = cmd.ExecuteReader();   //---- 執行SQL指令（Select，搜尋、查詢）取出資料

            //==第三，自由發揮，把執行後的結果呈現到畫面上。
            //*** 與上一個範例不同之處 ********************************************(start)
            UserTable2 resultClass = new UserTable2();

            if (dr.Read())   // 不是迴圈
            {    // 有資料的話，將讀取一筆記錄。並放入 UserTable2類別裡面            
                resultClass.UserId = Convert.ToInt32(dr["UserId"]);
                resultClass.UserName = dr["UserName"].ToString();
                resultClass.UserSex = dr["UserSex"].ToString();
                resultClass.UserBirthDay = Convert.ToDateTime(dr["UserBirthDay"]);
                resultClass.UserMobilePhone = dr["UserMobilePhone"].ToString();
                resultClass.UserApproved = Convert.ToBoolean(dr["UserApproved"]);
            }
            else   {
                return Content("抱歉！沒有找到任何一筆記錄");
            }
            //*** 與上一個範例不同之處 ********************************************(end)

            // == 第四，釋放資源、關閉資料庫的連結。       
            if (dr != null)   {
                cmd.Cancel();
                dr.Close();
            }
            if (Conn.State == ConnectionState.Open)   {
                Conn.Close();
            }

            return View(resultClass);   // 把這一筆記錄呈現出來
            // 重點！新增檢視的時候，請選擇「 Details」樣版！！

        }




        //=============================================
        //== 修改（編輯）畫面 ==  請沿用上方 Details2動作即可
        //     （資料填入 UserTable2類別 裡面）
        //=============================================
        public ActionResult Edit(int? _ID)    // 網址 http://xxxxxx/UserDB2/Edit?ID=1
        {
            if (_ID == null)
            {   // 沒有輸入文章編號（ID），就會報錯 - Bad Request
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            #region  // 使用上方 Details2動作的程式(ADO.NET)，先列出這一筆的內容，給使用者確認

            // Web.Config設定檔裡面的資料庫連結字串（ConnectionString），此連線名為 MVC_UserDB2
            SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MVC_UserDB2"].ConnectionString);

            //== 第一，連結資料庫。
            Conn.Open();   //---- 開啟連結。這時候才連結DB

            //== 第二，執行SQL指令。
            SqlDataReader dr = null;
            SqlCommand cmd = new SqlCommand("Select * From [UserTable2] Where UserId = @ID", Conn);
            cmd.Parameters.AddWithValue("@ID", _ID.ToString());

            dr = cmd.ExecuteReader();   //---- 執行SQL指令（Select，搜尋、查詢）取出資料

            //==第三，自由發揮，把執行後的結果呈現到畫面上。
            //*** 與上一個範例（Details）不同之處 ********************************************(start)
            UserTable2 resultClass = new UserTable2();

            if (dr.Read())   // 不是迴圈
            {    // 有資料的話，將讀取一筆記錄。並放入 UserTable2類別裡面            
                resultClass.UserId = Convert.ToInt32(dr["UserId"]);
                resultClass.UserName = dr["UserName"].ToString();
                resultClass.UserSex = dr["UserSex"].ToString();
                resultClass.UserBirthDay = Convert.ToDateTime(dr["UserBirthDay"]);
                resultClass.UserMobilePhone = dr["UserMobilePhone"].ToString();
                resultClass.UserApproved = Convert.ToBoolean(dr["UserApproved"]);
                resultClass.DepartmentId = Convert.ToInt32(dr["DepartmentId"]);
            }
            else   {
                return Content("抱歉！沒有找到任何一筆記錄");
            }
            //*** 與上一個範例（Details）不同之處 ********************************************(end)

            // == 第四，釋放資源、關閉資料庫的連結。       
            if (dr != null)   {
                cmd.Cancel();
                dr.Close();
            }
            if (Conn.State == ConnectionState.Open)   {
                Conn.Close();
            }
            #endregion


            // 檢視畫面上的「下拉式選單（DropDownList）」。
            // 直接連結一個資料表，當作 DropDownList的資料來源。
            #region   // 請參閱 DropDownList控制器，底下的 Create1動作。  ADO.NET版 請看 Create1A動作
            var dt = _db.DepartmentTable2s.ToList();   // 這一列改成 ADO.NET程式，就能到資料庫撈取您要的數據

            //*******************************************************************************
            //*** 重點！！這裡需要用 第四個參數。當作「預設值（SelectedValue）」
            //*******************************************************************************
            SelectList listItems = new SelectList(dt, "DepartmentId", "DepartmentName", resultClass.DepartmentId);
            // 先寫 <option>子選項的 value，再寫 text

            ViewBag.DtListItem = listItems;
            //*******************
            #endregion
            

            return View(resultClass);   // 把這一筆記錄呈現出來。   
            // 新增檢視的時候，請選擇「 Edit」樣版！！    

            // *** 重點！***        
            // 產生檢視畫面時，請參閱圖片檔 Edit.jpg的說明。
            // (1). 挑選下面（第四個）的「資料內容類別（UserDB2Context）」，DepartmentId 會變成「下拉式選單（Html.DropDownList）」
            // (2).  "不" 挑選下面的「資料內容類別」，保持空白的話。DepartmentId 就會變成 Html.EditorFor（詳見   Edit_比較版.cshtml）

            // *** 重點！***        
            // 產生檢視畫面之後，兩個重點 -- (1). CheckBox   (2). DropDownList
        }
        

        //== 修改（更新），回寫資料庫 ===============
        [HttpPost]
        [ValidateAntiForgeryToken]   // 避免 CSRF攻擊，請配合檢視畫面 Html.BeginForm()表單裡的「@Html.AntiForgeryToken()」這一列程式

        // 參考資料 http://blog.kkbruce.net/2011/10/aspnet-mvc-model-binding6.html
        // [Bind(Include=.......)] 也可以寫在 Model的 "類別檔"裡面，就不用重複地寫在新增、刪除、修改每個動作之中。需搭配System.Web.Mvc命名空間。
        // 可以避免 OverPosting attacks （過多發佈）攻擊  http://www.cnblogs.com/Erik_Xu/p/5497501.html

        //public ActionResult Edit([Bind(Include = "UserId, UserName, UserSex, UserBirthDay, UserMobilePhone, UserApproved, DepartmentId")]UserTable2 _userTable)
        public ActionResult Edit(UserTable2 _userTable)
        {   
            if (_userTable == null)
            {   // 沒有輸入內容，就會報錯 - Bad Request
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            string sqlstr = null;

            if (ModelState.IsValid)
            {   //** 原本 EF的寫法 **********************************************************(start)
                //_db.Entry(_userTable).State = System.Data.Entity.EntityState.Modified;
                //_db.SaveChanges();
                //** 原本 EF的寫法 **********************************************************(end)

                // 以下的 ADO.NET程式可以取代上面這兩列。
                #region  //** 第二種作法。類似 ADO.NET做「資料新增」的寫法
                // 程式碼 折疊（區塊關閉）

                int RecordsAffected = 0;
                //== (1). 開啟資料庫的連結。上面已經事先寫好NameSpace --  using System.Web.Configuration; ----     
                    SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MVC_UserDB2"].ConnectionString);
                    Conn.Open();

                //== (2). 執行SQL指令。或是查詢、撈取資料。  
                //*** 參數（Parameter），可避免SQL Injection攻擊 ****** (start)
                    sqlstr = "Update [UserTable2] Set [UserName] = @UserName, [UserSex] = @UserSex, [UserBirthDay] = @UserBirthDay, [UserMobilePhone] = @UserMobilePhone, [UserApproved] = @UserApproved, [DepartmentId] = @DepartmentId ";
                    sqlstr += " Where UserId = @UserId";
                    SqlCommand cmd = new SqlCommand(sqlstr, Conn);

                ////-- 方法一。精簡版。
                    cmd.Parameters.AddWithValue("@UserName", _userTable.UserName);
                    cmd.Parameters.AddWithValue("@UserSex", _userTable.UserSex);
                    cmd.Parameters.AddWithValue("@UserBirthDay", _userTable.UserBirthDay);
                    cmd.Parameters.AddWithValue("@UserMobilePhone", _userTable.UserMobilePhone);
                    cmd.Parameters.AddWithValue("@UserApproved", _userTable.UserApproved);   // 1 為 true
                    cmd.Parameters.AddWithValue("@DepartmentId", _userTable.DepartmentId);
                    cmd.Parameters.AddWithValue("@UserId", _userTable.UserId);

                ////-- 方法二，另一種寫法，效率高！只寫一個參數作為示範。
                //cmd.Parameters.Add("@UserName", SqlDbType.NVarChar, 50)
                //cmd.Parameters("@UserName").Value = _userTable.UserName
                //-- 參考資料 http://msdn.microsoft.com/zh-tw/library/system.data.sqlclient.sqlparametercollection.addwithvalue.aspx
                //*** 參數（Parameter），可避免SQL Injection攻擊 ******  (end)

                //== (3). 自由發揮。
                RecordsAffected = cmd.ExecuteNonQuery();

                //== (4). 釋放資源、關閉資料庫的連結。
                    cmd.Cancel();
                    if (Conn.State == ConnectionState.Open)   {
                        Conn.Close();
                    }
                #endregion

                return Content(" [ADO.NET] 執行 Update的SQL指令以後，影響了" + RecordsAffected + "列的紀錄。");
                //return RedirectToAction("List");
            }
            else   {                
                return Content(" 更新失敗！！更新失敗！！ ");
            }            
        }



    }
}