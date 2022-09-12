using System;

namespace BridgeCareCore.Models
{
    public class LibraryUpsertPagingRequestModel<T, Y> : BaseLibraryUpsertPagingRequest<T>
    {
        public PagingSyncModel<Y> PagingSync { get; set; }
    }
}
