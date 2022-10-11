using System;

namespace BridgeCareCore.Models
{
    public class LibraryUpsertPagingRequestModel<T, Y> : BaseLibraryUpsertPagingRequest<T>
    {
        public LibraryUpsertPagingRequestModel(): base()
        {
            PagingSync = new PagingSyncModel<Y>();
        }
        public PagingSyncModel<Y> PagingSync { get; set; }
    }
}
