namespace WebApplication2017_MVC_GuestBook.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;   // 每個欄位上方的 [ ]符號，就得搭配這個命名空間
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web.Mvc;   //搭配 [Bind(Include=.......)]  的命名空間

    [Table("UserTable")]

    // 寫在 Model的類別檔裡面，就不用重複地寫在新增、刪除、修改每個動作之中。
    // [Bind(Include = "UserId, UserName, UserSex, UserBirthDay, UserMobilePhone")]
    // 可以避免 overposting attacks （過多發佈）攻擊  http://www.cnblogs.com/Erik_Xu/p/5497501.html
    public partial class UserTable
    {
        [Key]    // 主索引鍵（P.K.）
        public int UserId { get; set; }

        [StringLength(50)]
        public string UserName { get; set; }

        [StringLength(1)]
        public string UserSex { get; set; }

        public DateTime? UserBirthDay { get; set; }

        [StringLength(15)]
        public string UserMobilePhone { get; set; }
    }
}
