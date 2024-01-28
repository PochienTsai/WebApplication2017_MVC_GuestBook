namespace WebApplication2017_MVC_GuestBook.Models2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;     // 每個欄位上方的 [ ]符號，就得搭配這個命名空間
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DepartmentTable2
    {
        //**************************************************************************
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DepartmentTable2()
        {
            UserTable2s = new HashSet<UserTable2>();
        }
        //**************************************************************************
        
        [Key]    // 主索引鍵（P.K.）
        public int DepartmentId { get; set; }

        [Required]
        [StringLength(50)]
        public string DepartmentName { get; set; }

        [Required]
        [StringLength(50)]
        public string DepartmentYear { get; set; }


        //*************************************************************************
        //*** 導覽屬性（navigation property.）***
        // https://docs.microsoft.com/zh-tw/dotnet/framework/data/adonet/ef/language-reference/query-expression-syntax-examples-navigating-relationships
        // 一對多的關連式資料表（一筆 Department記錄 對應 多筆User）

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserTable2> UserTable2s { get; set; }   // 注意！後面是複數（s）

        // (1) virtual的用意？？
        // 答： Navigation properties are typically defined as "virtual"  
        //         so that they can take advantage of certain Entity Framework functionality such as "lazy loading". 

        // (2) ICollection<T>的用意？？ 
        // 答： If a navigation property can hold "multiple entities" (as in many-to-many（多對多） or one-to-many（一對多） relationships), 
        //          its type must be a "list" in which entries can be  *** added, deleted, and updated ***, such as ICollection.（可以做新增、刪除、修改）

        // (3) 延續上個問題，如果改成 IEnumerable<T> 又有何差別？
        // 答： If a navigation property can hold multiple entities（一對多）, its type  ** must ** implement the ICollection<T> Interface.
        //         For example IList<T> qualifies 
        //         *** but not *** IEnumerable<T>  *** because IEnumerable<T> doesn't implement "Add". ***

        // (4) 文章說明： When To Use IEnumerable, ICollection, IList And List（文章後面有一張圖）
        //      http://www.claudiobernasconi.ch/2013/07/22/when-to-use-ienumerable-icollection-ilist-and-list/
        //  IEnumerable -- 只用於「唯讀」的資料展示。
        //  ICollection -- 您想修改集合或關心其大小(size)。
        //  IList -- 您想要修改集合，並關心集合中元素的排序和/或位置。

        // (5) https://stackoverflow.com/questions/2876616/returning-ienumerablet-vs-iqueryablet
        // This is quite an important difference, and working on IQueryable<T> can in many cases 
        // save you from returning "too many rows" from the database.
        // Another prime example is doing paging（分頁）: 
        // If you use .Take() and .Skip() on IQueryable, you will "only get（只拿你要的）" the number of rows requested; 
        // doing that on an IEnumerable<T> will cause "all of（全部 / 假分頁）" your rows to be loaded in memory.

        // (5-1) To return IQueryable<T> if you want to enable the developer using your method 
        //          to refine（改造） the query you return " before executing".
        // If you want to transport just "a set of Objects" to enumerate over, just take "IEnumerable".

        // (5-2) Imagine an IQueryable as that what it is, a "query" for data (which you can refine（改造） if you want to)
        // An IEnumerable is a set of objects (which has "already" been received or was created) over which you can enumerate.

        // (6) https://www.linkedin.com/pulse/ienumerable-iqueryable-linq-umesh-ghaywat/
        // 文章裡面有兩張圖片說明。
        // IEnumerable<T> -- 唯讀、單一方向（forward direction），無法增加與刪除。
        // IQueryable<T> -- 查詢，尤其是遠端資料庫
        //                              Remember that an IQueryable is not a result set, it is a query.
        //
        // 結論：The IEnumerable <T> works with collection in local memory（本機的記憶體裡面） 
        //            whereas IQueryable<T> works with queryable data provider（如 資料庫）.
        // Both IQueryable<T> and IEnumerable <T> support "lazy loading" of data from remote database servers.
    }
}
