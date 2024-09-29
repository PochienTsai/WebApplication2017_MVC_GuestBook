namespace WebApplication2017_MVC_GuestBook.Models2Northwind
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Order Details")]
    public partial class Order_Detail
    {
        [Key]   // 重點！！兩個欄位一起當成主索引鍵（PK）
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int OrderID { get; set; }

        [Key]   // 重點！！兩個欄位一起當成主索引鍵（PK）
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProductID { get; set; }

        [Column(TypeName = "money")]
        public decimal UnitPrice { get; set; }

        public short Quantity { get; set; }

        public float Discount { get; set; }


        //==== 導覽屬性（Navigation Property）==========   與其他資料表之間的關連性。
        public virtual Order Order { get; set; }

        public virtual Product Product { get; set; }
    }
}
