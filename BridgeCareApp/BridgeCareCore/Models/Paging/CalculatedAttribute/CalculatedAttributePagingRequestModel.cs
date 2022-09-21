using System;
using AppliedResearchAssociates.iAM.DTOs;
namespace BridgeCareCore.Models
{
    public class CalculatedAttributePagingRequestModel : BasePagingRequest
    {
        public Guid AttributeId { get; set; }
        public CalculatedAttributePagingSyncModel SyncModel { get; set; }
    }
}
