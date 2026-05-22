using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Shared
{
    public class PaginatedResponse<T>
    {
        public IEnumerable<T> Data { get; set; } = new List<T>();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalNumber => (int)Math.Ceiling((double)Total / PageSize);
    }
}
