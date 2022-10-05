using System;
using AppliedResearchAssociates.iAM.DTOs;
namespace BridgeCareCore.Models
{
    public class CalculatedAttributePagingRequestModel : BasePagingRequest
    {
        public CalculatedAttributePagingRequestModel()
        {
            SyncModel = new CalculatedAttributePagingSyncModel();
        }
        public Guid AttributeId { get; set; }
        public CalculatedAttributePagingSyncModel SyncModel { get; set; }
    }
}
