using System;
using AppliedResearchAssociates.iAM.DTOs;
namespace BridgeCareCore.Models
{
    public class CalculatedAttributePagingRequestModel : PagingRequestModel<CalculatedAttributeDTO>
    {
        public CalculatedAttributePagingRequestModel()
        {
            SyncModel = new CalculatedAttributePagingSyncModel();
        }
        public Guid AttributeId { get; set; }
        public new CalculatedAttributePagingSyncModel SyncModel { get; set; }
    }
}
