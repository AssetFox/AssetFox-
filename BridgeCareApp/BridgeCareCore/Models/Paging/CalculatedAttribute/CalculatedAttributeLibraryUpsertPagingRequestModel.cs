using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Models
{
    public class CalculatedAttributeLibraryUpsertPagingRequestModel : BaseLibraryUpsertPagingRequest<CalculatedAttributeLibraryDTO>
    {
        public CalculatedAttributePagingSyncModel SyncModel { get; set; }
    }
}
