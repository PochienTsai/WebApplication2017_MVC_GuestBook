namespace WebApplication2017_MVC_GuestBook.Models2
{
    using System;
    using System.Data.Entity;    // *** 重點！！ DbContext需要用到！
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class MVC_UserDB2Context : DbContext
    {
        public MVC_UserDB2Context()
            : base("name=MVC_UserDB2")   // DB連結字串，務必與「資料庫名稱」相同！！
        {   // 否則將會尋找「專案名稱 . Models2 . EF檔名」這樣的資料庫名稱。因為找不到，就會報錯！
            // 以本範例而言，將會尋找「WebApplication2017_MVC_GuestBook.Models2.MVC_UserDB2Context」這個資料庫名稱。
        }

        //***********************************************************************************(start)
        public virtual DbSet<DepartmentTable2> DepartmentTable2s { get; set; }   // 這裡的名稱是複數（s）
        public virtual DbSet<UserTable2> UserTable2s { get; set; }   // 這裡的名稱是複數（s）
        // (1) UserTable2 表示 資料表裡面的「一筆記錄」。
        // (2) DbSet<UserTable2> 表示 「UserTable資料表」。
        // (3) virtual的用意？？
        // 答： Navigation properties are typically defined as "virtual" 
        //          so that they can take advantage of certain Entity Framework functionality such as "lazy loading". 
        //***********************************************************************************(end)

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DepartmentTable2>()   //******
                .HasMany(e => e.UserTable2s)
                .WithRequired(e => e.DepartmentTable2s)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserTable2>()
                .Property(e => e.UserSex)
                .IsFixedLength();
            // 可參閱這影片 https://www.youtube.com/watch?v=Sj-6MCmWqGc
        }
    }
}
