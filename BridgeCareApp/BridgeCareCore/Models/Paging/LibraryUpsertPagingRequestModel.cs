using System;

namespace BridgeCareCore.Models
{
    public class LibraryUpsertPagingRequestModel<T, Y>
    {
        public T Library { get; set; }
        public PagingSyncModel<Y> PagingSync { get; set; }
    }
}
