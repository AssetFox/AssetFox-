using System.Collections.Generic;

namespace BridgeCareCore.Models
{
    public class PagingModel<T>
    {
        public List<T> Items { get; set; }
        public int TotalItems { get; set; }
        public int Page { get; set; }
        public int RowsPerPage { get; set; }
    }
}
