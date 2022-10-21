using System;

namespace BridgeCareCore.Models
{
    public class LibraryUpsertPagingRequestModel<T, Y> : BaseLibraryUpsertPagingRequest<T>
    {
        public LibraryUpsertPagingRequestModel()
        {
            PagingSync = new PagingSyncModel<Y>();
        }
        public PagingSyncModel<Y> PagingSync { get; set; }
    }
}
