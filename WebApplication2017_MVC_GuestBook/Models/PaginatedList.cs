using System;
using System.Collections.Generic;
using System.Linq;


namespace WebApplication2017_MVC_GuestBook
{
    ///===================================
    ///== 分頁#3（微軟官方教材） ==  LINQ的 .Skip() 與 .Take()
    /// https://docs.microsoft.com/zh-tw/aspnet/core/data/ef-mvc/read-related-data?view=aspnetcore-5.0
    ///===================================
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }    // 目前位於第幾頁？
        public int TotalPages { get; private set; }   // 總頁數

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);  // pageSize每一頁要展現幾筆資料？

            this.AddRange(items);
        }

        public bool HasPreviousPage   //是否展示「上一頁」的超連結？
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage   //是否展示「下一頁」的超連結？
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }

        public static PaginatedList<T> PagerCreate(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}