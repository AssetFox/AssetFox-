using System;

namespace BridgeCareCore.Models
{
    public abstract class BaseLibraryUpsertPagingRequest<T>
    {
        public T Library { get; set; }
    }
}
