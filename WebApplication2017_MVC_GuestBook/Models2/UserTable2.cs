namespace WebApplication2017_MVC_GuestBook.Models2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;     // 每個欄位上方的 [ ]符號，就得搭配這個命名空間
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UserTable2
    {
        [Key]    // 主索引鍵（P.K.）
        public int UserId { get; set; }


        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "名稱（UserName）")]
        [Required (ErrorMessage = "*** 自訂訊息 *** 必填欄位 ***")]     // 必填欄位
        public string UserName { get; set; }


        [StringLength(1)]
        [Display(Name = "性別（UserSex）")]
        public string UserSex { get; set; }


        [Display(Name = "生日（UserBirthDay）")]
        [DataType(DataType.Date)]    // 只有日期 - 「年月日」。如果是 DateTime就是「日期與時間」
        // 加了DataType，請使用 Chrome瀏覽器來觀賞。會出現簡單的「日曆」欄位。https://stackoverflow.com/questions/12633471/mvc4-datatype-date-editorfor-wont-display-date-value-in-chrome-fine-in-interne

        [Range(typeof(DateTime), "1/1/1950", "1/1/2020", ErrorMessage = "日期區間，只能在1950年以後~2020年之前")]   // 設定日期區間（月/日/年）
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]   // 設定日期為 yyyy/MM/dd 格式
        public DateTime? UserBirthDay { get; set; }


        [StringLength(15, ErrorMessage = "*** 自訂訊息 *** 不得超過15個數字 ***")]
        [Display(Name = "手機號碼（UserMobilePhone）")]
        [RegularExpression(@"^(09)([0-9]{8})$")]   // 前兩個數字必須是09，後面跟著八個數字（必須是0~9的數字）。
        // 正規運算、正規表達（regular expression）。  ^符號 代表開始，$符號 代表結束。
        [Required]     // 必填欄位
        public string UserMobilePhone { get; set; }

        
        //***** 新增的資料表欄位（類別的屬性） ******************************
        [Display(Name = "帳號已啟用？（UserApproved）")]
        public bool UserApproved { get; set; }


        public int? DepartmentId { get; set; }


        //*************************************************************************
        //*** 導覽屬性（navigation property.）***
        // https://docs.microsoft.com/zh-tw/dotnet/framework/data/adonet/ef/language-reference/query-expression-syntax-examples-navigating-relationships

        public virtual DepartmentTable2 DepartmentTable2s { get; set; }   // 注意！後面是複數（s）
        // (1) virtual的用意？？
        // 答： Navigation properties are typically defined as "virtual" 
        //         so that they can take advantage of certain Entity Framework functionality such as "lazy loading". 
    }
}
