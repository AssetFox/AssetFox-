using System;
using System.Collections.Generic;

namespace BridgeCareCore.Models
{
    public abstract class BasePagingRequest
    {
        public int Page { get; set; }
        public int RowsPerPage { get; set; }
        public bool isDescending { get; set; }
        public string sortColumn { get; set; }
        public string search { get; set; }
    }
}
