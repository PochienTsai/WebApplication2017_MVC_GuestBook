namespace WebApplication2017_MVC_GuestBook.Models2Northwind
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [NotMapped] // 這個 Order_ViewModel類別檔在資料庫上沒有，所以寫了[NotMapped] 
    public class Order_ViewModel
    {       
        public int OrderID { get; set; }
        public string CustomerID { get; set; }

        //*****************************************
        [NotMapped]    // 這個欄位在資料庫上沒有，所以寫了[NotMapped] 
        public string CompanyName { get; set; }
        //*****************************************

        public DateTime? OrderDate { get; set; }
        

        //==== 導覽屬性（Navigation Property）==========   與其他資料表之間的關連性。
        //****************************************************************************
        [NotMapped]    // 這個 Order_Detail_ViewModel類別檔在資料庫上沒有，所以寫了[NotMapped] 
        public virtual ICollection<Order_Detail_ViewModel> OD_VMs { get; set; }
        //****************************************************************************
    }
}
