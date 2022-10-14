using System;
using System.Collections.Generic;
using System.Text;

namespace Balo.Data.ViewModels
{

    public class PagingModel
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalSize { get; set; }
        public object Data { get; set; }
    }
    public class PagingRequest
    {
        public int PageSize { get; set; } = 20;
        public int PageIndex { get; set; } = 0;
    }
    public class PagingModel<T>
    {
        public List<T> Data { get; set; }
        public long TotalRows { get; set; }
        public long TotalPages { get; set; }
    }
}
