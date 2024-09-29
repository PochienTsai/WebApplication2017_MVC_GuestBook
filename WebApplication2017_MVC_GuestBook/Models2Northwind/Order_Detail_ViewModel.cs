namespace WebApplication2017_MVC_GuestBook.Models2Northwind
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [NotMapped]    // �o�� Order_Detail_ViewModel���O�ɦb��Ʈw�W�S���A�ҥH�g�F[NotMapped] 
    public class Order_Detail_ViewModel
    {
        public int OrderID { get; set; }
        public int ProductID { get; set; }

        //*******************************************
        [NotMapped]    // �o�����b��Ʈw�W�S���A�ҥH�g�F[NotMapped] 
        public string ProductName { get; set; }
        //*******************************************

        public short Quantity { get; set; }



        //==== �����ݩʡ]Navigation Property�^==========   �P��L��ƪ��������s�ʡC
        [NotMapped]    // �o�� Order_ViewModels���O�ɦb��Ʈw�W�S���A�ҥH�g�F[NotMapped] 
        public virtual Order_ViewModel O_VMs { get; set; }
    }
}
