using System;

namespace BridgeCareCore.Models
{
    public abstract class BaseLibraryUpsertPagingRequest<T>
    {
        public T Library { get; set; }
        public bool IsNewLibrary { get; set; }
        public Guid? ScenarioId { get; set; }
    }
}
