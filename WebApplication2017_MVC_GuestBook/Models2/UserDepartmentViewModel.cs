using WebApplication2017_MVC_GuestBook.Models2;

namespace WebApplication2017_MVC_GuestBook.Models2
{
    public class UserDepartmentViewModel
    {    // 一個 UserTable 與 一個 DepartmentTable
        public UserTable2 UserVM { get; set; }

        public DepartmentTable2 DepartmentVM { get; set; }


        // 比較一下，跟另一個 ViewModel有何不同？（另一個為 UDViewModel.cs，一對多）
        //                                                                       （另一個為 UDmultiViewModel.cs，多對多）

    }
}