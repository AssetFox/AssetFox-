using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace BridgeCareCore.Models
{
    public class CreateAttributeRequest
    {
        public AllAttributeDTO Attribute { get; set; }
        public bool SetForAllAttributes { get; set; }
    }
}
