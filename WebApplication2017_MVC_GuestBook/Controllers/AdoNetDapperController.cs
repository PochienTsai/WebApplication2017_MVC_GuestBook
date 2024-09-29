using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//**************************************************
using Dapper;   // 請透過 NuGet安裝「Dapper」
//**************************************************
using WebApplication2017_MVC_GuestBook.Models;    // 自己動手寫上命名空間 -- 「專案名稱.Models」。

// 以下三個命名空間，ADO.NET 會用到
using System.Web.Configuration;   // 透過Web.config管理DB連結字串
using System.Data;
using System.Data.SqlClient;
//**************************************************


namespace WebApplication2017_MVC_GuestBook.Controllers
{

    public class AdoNetDapperController : Controller
    {
        // 搭配 /Models目錄 UserTable.cs類別檔

        //*** Dapper 資料來源 與 說明 ********************************
        // https://www.huanlintalk.com/2014/03/a-micro-orm-dapper.html
        // https://dotblogs.com.tw/topcat/2016/10/06/165217
        // 原廠範例  https://github.com/StackExchange/Dapper （放在網頁最下方，很清楚！包含JOIN的寫法、預存程序......等等）
        // JOIN的寫法  http://dapper-tutorial.net/querymultiple 或 http://dapper-tutorial.net/result-multi-mapping 

        // http://blog.darkthread.net/post-2014-05-15-dapper.aspx
        // 搜尋條件 -- http://blog.darkthread.net/post-2015-08-17-where-1-1-and-performance.aspx

        // MVC5 + Dapper做CRUD， http://www.compilemode.com/2015/09/crud-operations-in-asp-net-mvc-5-using-dapper.html



        // GET: AdoNetDapper
        public ActionResult Index()
        {
            return View();
        }

        //===== 主表明細（Master-Details） =========================================
        // 傳回 字串(string)
        public string List1_String()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            // Web.Config設定檔裡面的資料庫連結字串（ConnectionString），此連線名為 MVC_UserDB
            using (SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MVC_UserDB"].ConnectionString))
            {
                //== 第一，連結資料庫。
                Conn.Open();

                //== 第二，執行SQL指令。
                string sqlstr = "Select * from UserTable";
                var userTable = Conn.Query<UserTable>(sqlstr, Conn);
                //                               ********************** 重點！！

                //==第三，自由發揮。                
                foreach (var ut in userTable)
                {
                    sb.Append(ut.UserId + " --- " + ut.UserName + "<br />");
                }

                ////== 第四，釋放資源、關閉資料庫的連結。
                //// 已經使用 using來管理，所以無須手動關閉。
                //if (Conn.State == ConnectionState.Open){
                //    Conn.Close();
                //}
            }   // using Conn 結束
            return sb.ToString();
        }


        public ActionResult List()
        {
            // Web.Config設定檔裡面的資料庫連結字串（ConnectionString），此連線名為 MVC_UserDB
            using (SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MVC_UserDB"].ConnectionString))
            {
                //== 第一，連結資料庫。
                Conn.Open();

                //== 第二，執行SQL指令。
                string sqlstr = "Select * from UserTable";
                IEnumerable<UserTable> userTable = Conn.Query<UserTable>(sqlstr, Conn);
                //var userTable = Conn.Query<UserTable>(sqlstr, Conn);
                //                                  ********************** 重點！！
                ////如果不指定 .Query<UserTable>()方法的話，會出現錯誤訊息：
                ////      傳入此字典的模型項目為型別 'System.Collections.Generic.List`1[Dapper.SqlMapper+DapperRow]'，
                ////      但是此字典需要型別 'System.Collections.Generic.IEnumerable`1[WebApplication2017_MVC_GuestBook.Models.UserTable]' 的模型項目。

                ////==第三，自由發揮。（可省略，因為結果已經放入  UserTable類別之中）
                //System.Text.StringBuilder sb = new System.Text.StringBuilder();   // 使用 System.Text命名空間
                //foreach (var ut in userTable)   {
                //    sb.Append(ut.UserName + "<br />");
                //}

                ////== 第四，釋放資源、關閉資料庫的連結。
                //// 已經使用 using來管理，所以無須手動關閉。
                //if (Conn.State == ConnectionState.Open)   {
                //    Conn.Close();
                //}
                return View(userTable);
            }   // using Conn 結束                      
        }


        public ActionResult Details(int _ID = 1)    // 注意！參數在 Dapper裡面的寫法
        {
            // Web.Config設定檔裡面的資料庫連結字串（ConnectionString），此連線名為 MVC_UserDB
            using (SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MVC_UserDB"].ConnectionString))
            {
                //== 第一，連結資料庫。
                Conn.Open();

                // 資料來源  http://dapper-tutorial.net/result-strongly-typed
                ////== 第二，執行SQL指令 (1)。================================================
                //string sqlstr = "Select * from UserTable Where UserId = @UserId";   //使用參數，避免SQL Injection攻擊
                ////                                                                                                          *********************** 
                //var userTable = Conn.Query<UserTable>(sqlstr, new { UserId = _ID }).FirstOrDefault();
                ////                               ********** 重點！！               ****** 參數 *****   ** 沒加上會報錯！****
                //// 如果沒有加上 .FirstOrDefault()，錯誤訊息如下：
                ////       傳入此字典的模型項目為型別 'System.Collections.Generic.List`1[WebApplication2017_MVC_GuestBook.Models.UserTable]'，
                ////       但是此字典需要型別 'WebApplication2017_MVC_GuestBook.Models.UserTable' 的模型項目。 

                ////或是寫成這樣（多個參數）：
                ////var sql = "Select * from UserTable Where UserId = @UserId and UserName = @UserName";
                ////var sqlparameters = new
                ////{
                ////    UserId = "A001",    // 參數的前後順序 不要錯亂！
                ////    UserName = "李大明"
                ////};
                ////var userTable = Conn.Query<UserTable>(sqlstr, sqlparameters).FirstOrDefault();

                ////== 第二，執行SQL指令(2)。===============================================
                //string sqlstr = "Select * from UserTable Where UserId = @UserId";   //使用參數，避免SQL Injection攻擊
                ////                                                                                                          *********************** 
                //var userTable = Conn.QuerySingle<UserTable>(sqlstr, new { UserId = _ID });
                ////                              ***************（修改的地方，與上一個寫法不同）

                //== 第二，執行SQL指令(3)。================================================
                string sqlstr = "Select * from UserTable Where UserId = @UserId";   //使用參數，避免SQL Injection攻擊
                //                                                                                                          *********************** 
                var userTable = Conn.QueryFirstOrDefault<UserTable>(sqlstr, new { UserId = _ID });
                //                              ***************（修改的地方，與前兩個寫法不同）


                ////==第三，自由發揮。（可省略，因為結果已經放入  UserTable類別之中）

                ////== 第四，釋放資源、關閉資料庫的連結。
                //// 已經使用 using來管理，所以無須手動關閉。
                //if (Conn.State == ConnectionState.Open)   {
                //    Conn.Close();
                //}

                return View(userTable);
                // https://my.oschina.net/chengxinjing/blog/522471
                // http://kevintsengtw.blogspot.tw/2015/09/dapper-dynamic.html
            }
        }


        //===== 兩個資料表 (一對多。一個科系對應多名學生。) =============================
        //===== 使用  /Model2目錄底下的 UDViewModel類別。
        //===== DB連線名為 MVC_UserDB2
        public ActionResult IndexVM0()
        {
            // Web.Config設定檔裡面的資料庫連結字串（ConnectionString），此連線名為 MVC_UserDB2
            using (SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MVC_UserDB2"].ConnectionString))
            {
                //== 第一，連結資料庫。
                Conn.Open();
                //== 第二，執行SQL指令。一對多。一個科系對應多名學生。
                string sqlstr = "SELECT * FROM DepartmentTable2 WHERE DepartmentId = @DepartmentId; ";
                sqlstr += "SELECT * FROM UserTable2 WHERE DepartmentId = @DepartmentId;";

                var multi = Conn.QueryMultiple(sqlstr, new { DepartmentId = 1 });
                //                         ******* 重點！******
                var dt = multi.Read<Models2.DepartmentTable2>().First();
                var u = multi.Read<Models2.UserTable2>().ToList();

                //==第三，自由發揮。（可省略，因為結果已經放入 類別之中）
                // --- 完成（一對多）！正式寫入 ViewModels裡面。使用  /Model2目錄底下的 UDViewModel類別。---
                Models2.UDViewModel resultVM = new Models2.UDViewModel
                {
                    DVM = dt,   // 寫入ViewModel的第一個類別（一個 科系 Department）
                    UVM = u     // 寫入ViewModel的第二個類別（多個 學生 User）
                };

                ////== 第四，釋放資源、關閉資料庫的連結。
                //// 已經使用 using來管理，所以無須手動關閉。
                //if (Conn.State == ConnectionState.Open)   {
                //    Conn.Close();
                //}

                return View(resultVM);
                // 新增「檢視」時，第四格「資料內容類別」請留白。
                // 產生後的檢視畫面為空白，必須自己動手改造、自己寫 for 迴圈。
            }
        }


        //===== 兩個資料表 (多對多，列表。一個科系對應多名學生。) =========================
        //===== 使用  /Model2目錄底下的 UDViewModel類別。
        // 從 Dapper 原廠範例修改而來 http://dapper-tutorial.net/querymultiple

        //自己寫ADO.NET塞入類別檔，會很辛苦。請參閱 UserDB2 控制器的 IndexVM1_AdoNet動作
        public ActionResult IndexVM1()    //多對多（一對多 的 列表）
        {
            List<Models2.UDViewModel> resultVM = new List<Models2.UDViewModel>();

            // Web.Config設定檔裡面的資料庫連結字串（ConnectionString），此連線名為 MVC_UserDB2
            using (SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MVC_UserDB2"].ConnectionString))
            {
                //== 第一，連結資料庫。
                Conn.Open();
                //== 第二，執行SQL指令。一對多。一個科系對應多名學生。
                string sqlstr = "SELECT * FROM DepartmentTable2; ";
                sqlstr += "SELECT * FROM UserTable2;";

                //******************************************
                var multi = Conn.QueryMultiple(sqlstr);
                //                         ******* 重點！******
                //******************************************
                List<Models2.DepartmentTable2> dResult = multi.Read<Models2.DepartmentTable2>().ToList();
                List<Models2.UserTable2> uResult = multi.Read<Models2.UserTable2>().ToList();
                // 把這兩段寫在 for迴圈內會出現錯誤。有點類似DataReader讀取前，就已經被關閉。

                //==第三，自由發揮。（可省略，因為結果已經放入 類別之中）
                // --- 完成了（多對多）！正式寫入 ViewModels裡面。使用  /Model2目錄底下的 UDViewModel類別。---
                // 將「多筆」科系資料，逐一放入 ViewModel的第一個類別內
                foreach (var dt in dResult)    // dt 就是 「每一個科系」
                {
                    // 就讀 "這一科系"的「多筆」學生資料，逐一放入 ViewModel的第二個類別內
                    List<Models2.UserTable2> u = new List<Models2.UserTable2>();
                    foreach (var ut in uResult.Where(item => item.DepartmentId == dt.DepartmentId))
                    {   //                                                          *** 重點！ *********************************
                        u.Add(ut);
                    }

                    // --- 完成了（多對多）！正式寫入 ViewModels裡面 -----------------------------
                    resultVM.Add(new Models2.UDViewModel
                    {
                        DVM = dt,   // 寫入ViewModel的第一個類別（一個 科系 Department）
                        UVM = u     // 寫入ViewModel的第二個類別（多個 學生 User）
                    });
                }

                ////== 第四，釋放資源、關閉資料庫的連結。
                //// 已經使用 using來管理，所以無須手動關閉。
                //if (Conn.State == ConnectionState.Open)   {
                //    Conn.Close();
                //}

                return View(resultVM.ToList());
                // 新增「檢視」時，第四格「資料內容類別」請留白。
                // 產生後的檢視畫面為空白，必須自己動手改造、自己寫 for 迴圈。
            }
        }


        //===== 兩個資料表 (多對多，列表。一個科系對應多名學生。) =====================(start)
        //===== 使用  /Model2目錄底下的 *** UDmultiViewModel類別 ***
        // 原廠說明 http://dapper-tutorial.net/result-multi-mapping
        // 範例  http://www.compilemode.com/2016/08/show-multiple-model-class-data-on-single-view-in-Asp-net-mvc.html
        public ActionResult IndexVM2()    //多對多（列表），需要搭配底下自己寫的function -- GetMasterDetails()
        {
            List<Models2.UDmultiViewModel> MasterDetails = GetAllMasterDetails().ToList();
            //                                                                                 *********************  搭配底下自己寫的function -- GetMasterDetails()

            Models2.UDmultiViewModel result = new Models2.UDmultiViewModel();
            result.DVM = MasterDetails[0].DVM;
            result.UVM = MasterDetails[0].UVM;

            return View(result);
            // 新增「檢視」時，請選 Details範本，第四格「資料內容類別」請留白。
            // 產生後的檢視畫面為空白，必須自己動手改造、自己寫 for 迴圈。            
        }


        //***************************************************************************
        // 執行「 "個別的" 兩段SQL指令（不用JOIN）」，把結果塞入兩個類別之中。
        public IEnumerable<Models2.UDmultiViewModel> GetAllMasterDetails()
        {
            // Web.Config設定檔裡面的資料庫連結字串（ConnectionString），此連線名為 MVC_UserDB2
            using (SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MVC_UserDB2"].ConnectionString))
            {
                //== 第一，連結資料庫。
                Conn.Open();
                //== 第二，執行SQL指令。一對多。一個科系對應多名學生。
                string sqlstr = "SELECT * FROM DepartmentTable2; ";
                sqlstr += "SELECT * FROM UserTable2;";

                var multi = Conn.QueryMultiple(sqlstr);
                //                         ******* 重點！******

                //===== 使用  /Model2目錄底下的 *** UDmultiViewModel類別 ************
                Models2.UDmultiViewModel resultVM = new Models2.UDmultiViewModel();

                resultVM.DVM = multi.Read<Models2.DepartmentTable2>().ToList();
                resultVM.UVM = multi.Read<Models2.UserTable2>().ToList();
                //*******************************************************************************

                //==第三，自由發揮。（可省略，因為結果已經放入 類別之中）
                // --- 完成了（多對多）！正式寫入 ViewModels裡面。使用  /Model2目錄底下的 UDmultiViewModel類別。---
                List<Models2.UDmultiViewModel> resultList = new List<Models2.UDmultiViewModel>();
                resultList.Add(resultVM);  // 將「多對多」的結果，透過List<T>傳回去

                ////== 第四，釋放資源、關閉資料庫的連結。
                //// 已經使用 using來管理，所以無須手動關閉。
                //if (Conn.State == ConnectionState.Open)   {
                //    Conn.Close();
                //}
                return resultList;
            }
        }
        //===== 兩個資料表 (多對多，列表。一個科系對應多名學生。) ======================(end)



        //===== 新增 (Create) ===================================================
        public ActionResult Create()
        {
            return View();
        }


        [HttpPost, ActionName("Create")]   // 把下面的動作名稱，改成 CreateConfirm 試試看？
        [ValidateAntiForgeryToken]   // 避免XSS、CSRF攻擊
        public ActionResult Create(UserTable _userTable)
        {
            if ((_userTable != null) && (ModelState.IsValid))   // ModelState.IsValid，通過表單驗證（Server-side validation）需搭配 Model底下類別檔的 [驗證]
            {    // 新增與參數寫法（避免SQL Injection攻擊），資料來源 http://dapper-tutorial.net/execute

                // Web.Config設定檔裡面的資料庫連結字串（ConnectionString），此連線名為 MVC_UserDB
                using (SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MVC_UserDB"].ConnectionString))
                {
                    //== 第一，連結資料庫。
                    Conn.Open();

                    //== 第二，執行SQL指令。
                    string sqlstr = "INSERT INTO [UserTable] ([UserName],[UserSex],[UserBirthDay],[UserMobilePhone]) ";
                    sqlstr += " VALUES (@UserName,@UserSex, @UserBirthDay, @UserMobilePhone)";

                    ////==第三，自由發揮。（新增、刪除、修改，只會傳回一個整數 - 異動的資料列數目）
                    int affectedRows = Conn.Execute(sqlstr, new
                    {
                        UserName = _userTable.UserName,
                        UserSex = _userTable.UserSex,
                        UserBirthDay = _userTable.UserBirthDay,
                        UserMobilePhone = _userTable.UserMobilePhone
                    });

                    ////== 第四，釋放資源、關閉資料庫的連結。
                    //// 已經使用 using來管理，所以無須手動關閉。
                    //if (Conn.State == ConnectionState.Open)   {
                    //    Conn.Close();
                    //}
                }   // using Conn 結束     

                //return Content(" 新增一筆記錄，成功！");    // 新增成功後，出現訊息（字串）。
                return RedirectToAction("List");
            }
            else
            {   // 搭配 ModelState.IsValid，如果驗證沒過，就出現錯誤訊息。
                ModelState.AddModelError("Value1", " 自訂錯誤訊息(1) ");   // 第一個輸入值是 key，第二個是錯誤訊息（字串）
                ModelState.AddModelError("Value2", " 自訂錯誤訊息(2) ");
                return View();   // 將錯誤訊息，返回並呈現在「新增」的檢視畫面上
            }
        }



        //================================================================
        // Example - Query Multi-Mapping (One to Many)。  模組 請使用 /Models2目錄
        // http://dapper-tutorial.net/result-multi-mapping （一對多）    // 完整範例  https://dotnetfiddle.net/DPiy2b
        //================================================================
        public ActionResult Index_MultiMapping()
        {
            // Web.Config設定檔裡面的資料庫連結字串（ConnectionString），此連線名為 MVC_UserDB2
            using (SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MVC_UserDB2"].ConnectionString))
            {   //== 第一，連結資料庫。
                Conn.Open();
                //== 第二，執行SQL指令 (JOIN)。一對多。一個科系對應多名學生。
                string sqlstr = "SELECT d.DepartmentId, d.DepartmentName, u.UserId, u.UserName, u.DepartmentId";
                sqlstr += " FROM DepartmentTable2 d LEFT JOIN UserTable2 u";
                sqlstr += " On d.DepartmentId = u.DepartmentId Order By d.DepartmentId, u.UserId";
                // 以下是原廠範例的SQL寫法：（寫成 Inner Join，結果還是一樣）
                //string sqlstr = "SELECT * FROM DepartmentTable2 AS A ";
                //sqlstr += " INNER JOIN UserTable2 AS B ON A.DepartmentId = B.DepartmentId;";

                //==第三，自由發揮。
                var resultDictionary = new Dictionary<int, Models2.DepartmentTable2>();   

                //下面這段指令的結果，資料型態是 List<Models2.DepartmentTable2>
                var endData = Conn.Query<Models2.DepartmentTable2, Models2.UserTable2, Models2.DepartmentTable2>(
                        //                                //*** 「一對多」的 兩個關連式資料表                    *** 第三個小心！！仍用第一個資料表*******  
                        sqlstr,
                        (dt, ut) =>
                        {
                            if (!resultDictionary.TryGetValue(dt.DepartmentId, out Models2.DepartmentTable2 dtEntry))
                            {
                                dtEntry = dt;  // 科系（"一"對多）
                                dtEntry.UserTable2s = new List<Models2.UserTable2>();  // 該科系的學生（一對"多"）
                                // 科系 (Department) 底下的「導覽屬性」UserTables。一對多。

                                resultDictionary.Add(dtEntry.DepartmentId, dtEntry);
                                // 把結果加入Dictionary裡面
                            }

                            dtEntry.UserTable2s.Add(ut);
                            return dtEntry;
                        },
                        splitOn: "UserId")   //重點！！"一對多" 兩個關連式資料表，表示 "多"的那個（學生）資料表的ID（Key）
                                             // splitOn: 是做什麼用的？？ 
                                             // https://blog.csdn.net/u014180504/article/details/70870970
                                             // https://dotblogs.com.tw/supershowwei/2017/07/11/222837

                    .Distinct()   //加上這一段，可以把「重複的科系」資料取消。  如果不加上這一句，「科系」會重複出現。
                    .ToList();                

                ////== 第四，釋放資源、關閉資料庫的連結。
                //// 已經使用 using來管理，所以無須手動關閉。

                return View(endData);
                // 新增「檢視」時，請選擇：List範本、 /Models2/DepartmentTable2、第四格「資料內容類別」請留白。
                // 產生後的檢視畫面為空白，必須自己動手寫 雙重foreach迴圈。
            }  // end of using
        }





        //================================================================
        // 北風資料庫 #1 （訂單，一對多）Example - Query Multi-Mapping (One to Many)。  模組 請使用 /Models2Northwind目錄
        // http://dapper-tutorial.net/result-multi-mapping （一對多）    // 完整範例  https://dotnetfiddle.net/DPiy2b
        //================================================================
        public ActionResult Index_MultiMapping_Northwind1()
        {
            // Web.Config設定檔裡面的資料庫連結字串（ConnectionString），此連線名為 Northwind
            using (SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["Northwind"].ConnectionString))
            {   //== 第一，連結資料庫。
                Conn.Open();
                //== 第二，執行SQL指令 (JOIN)。一對多。一張訂單 對應 多筆訂購項目。
                string sqlstr = "SELECT Top 10 * FROM [Orders] o LEFT JOIN [Order Details] od";
                sqlstr += " On o.OrderID = od.OrderID Order By o.OrderID";

                //==第三，自由發揮。
                var resultDictionary = new Dictionary<int, Models2Northwind.Order>();

                //下面這段指令的結果，資料型態是 List<Models2Northwind.Order>
                var endData = Conn.Query<Models2Northwind.Order, Models2Northwind.Order_Detail, Models2Northwind.Order>(
                        //                                //*** 「一對多」的 兩個關連式資料表                    *** 第三個小心！！仍用第一個資料表*******  
                        sqlstr,
                        (o, od) =>
                        {
                            if (!resultDictionary.TryGetValue(o.OrderID, out Models2Northwind.Order dtEntry))
                            {
                                dtEntry = o;  // 訂單（"一"對多）
                                dtEntry.Order_Details = new List<Models2Northwind.Order_Detail>();  // 一張訂單 對應 "多筆"訂購項目。（一對"多"）
                                // 訂單 (Order) 底下的「導覽屬性」Order_Details。一對多。

                                resultDictionary.Add(dtEntry.OrderID, dtEntry);
                                // 把結果加入Dictionary裡面
                            }

                            dtEntry.Order_Details.Add(od);
                            return dtEntry;
                        },
                        splitOn: "OrderID")   //重點！！"一對多" 兩個關連式資料表，表示 "多"的那個（訂購項目 Order_Detail）資料表的ID（Key）
                                              // splitOn: 是做什麼用的？？ 
                                              // https://blog.csdn.net/u014180504/article/details/70870970
                                              // https://dotblogs.com.tw/supershowwei/2017/07/11/222837

                    .Distinct()   //加上這一段，可以把「重複的訂單」資料取消。  如果不加上這一句，「訂單」會重複出現。
                    .ToList();
                
                ////== 第四，釋放資源、關閉資料庫的連結。
                //// 已經使用 using來管理，所以無須手動關閉。

                return View(endData);
                // 新增「檢視」時，請選擇：List範本、 /Models2/DepartmentTable2、第四格「資料內容類別」請留白。
                // 產生後的檢視畫面為空白，必須自己動手寫 雙重foreach迴圈。
            }  // end of using
        }


        //================================================================
        // 北風資料庫 #2 （訂單，一對多），使用 ViewModel
        // 串連四個資料表 = Orders + Order Details + Products + Customers。
        //
        // Example - Query Multi-Mapping (One to Many)。  模組 請使用 /Models2Northwind目錄
        // http://dapper-tutorial.net/result-multi-mapping （一對多）    // 完整範例  https://dotnetfiddle.net/DPiy2b
        //================================================================
        public ActionResult Index_MultiMapping_Northwind2()
        {
            // Web.Config設定檔裡面的資料庫連結字串（ConnectionString），此連線名為 Northwind
            using (SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["Northwind"].ConnectionString))
            {   //== 第一，連結資料庫。
                Conn.Open();
                //== 第二，執行SQL指令 (JOIN)。一對多。一張訂單 對應 多筆訂購項目。
                // 串連四個資料表 = Orders + Order Details + Products + Customers。
                string sqlstr = "SELECT Top 10 o.OrderID, o.OrderDate, o.CustomerID, c.CompanyName, od.OrderID, od.ProductID, od.Quantity, p.ProductName ";
                sqlstr += " FROM [Orders] o LEFT JOIN [Order Details] od On o.OrderID = od.OrderID";
                sqlstr += " LEFT JOIN [Products] p On od.ProductID = p.ProductID";
                sqlstr += " LEFT JOIN [Customers] c On o.CustomerID = c.CustomerID Order By o.OrderID";

                //==第三，自由發揮。
                var resultDictionary = new Dictionary<int, Models2Northwind.Order_ViewModel>();

                ///                                                                                    //**********************************************************
                var endData = Conn.Query<Models2Northwind.Order_ViewModel, Models2Northwind.Order_Detail_ViewModel, Models2Northwind.Order_ViewModel>(
                        sqlstr,
                        (o, od) =>
                        {
                            if (!resultDictionary.TryGetValue(o.OrderID, out Models2Northwind.Order_ViewModel dtEntry))
                            {
                                dtEntry = o;  // 訂單（"一"對多）
                                //          ********                                               //******************************************    
                                dtEntry.OD_VMs = new List<Models2Northwind.Order_Detail_ViewModel>();  // 一張訂單 對應 "多筆"訂購項目。（一對"多"）
                                // 訂單 (Order) 底下的「導覽屬性」OD_Products。一對多。
                                // **************************************************     

                                resultDictionary.Add(dtEntry.OrderID, dtEntry);
                                // 把結果加入Dictionary裡面
                            }

                            dtEntry.OD_VMs.Add(od);
                            //          **************************
                            // 訂單 (Order) 底下的「導覽屬性」OD_Products。一對多。
                            return dtEntry;
                        },
                        splitOn: "OrderID")   //重點！！"一對多" 兩個關連式資料表，表示 "多"的那個（訂購項目 Order_Detail）資料表的ID（Key）
                                              // splitOn: 是做什麼用的？？ 
                                              // https://blog.csdn.net/u014180504/article/details/70870970
                                              // https://dotblogs.com.tw/supershowwei/2017/07/11/222837

                    .Distinct()   //加上這一段，可以把「重複的訂單」資料取消。  如果不加上這一句，「訂單」會重複出現。
                    .ToList();

                ////== 第四，釋放資源、關閉資料庫的連結。
                //// 已經使用 using來管理，所以無須手動關閉。

                return View(endData);
                // 新增「檢視」時，請選擇：List範本、 /Models2Northwind/Order_ViewModel.cs、第四格「資料內容類別」請留白。
                // 產生後的檢視畫面為空白，必須自己動手寫 雙重foreach迴圈。
            }  // end of using
        }



        

    }
}