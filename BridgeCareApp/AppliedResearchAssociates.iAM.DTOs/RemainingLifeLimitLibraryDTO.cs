using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class RemainingLifeLimitLibraryDTO : BaseLibraryDTO
    {
        public List<RemainingLifeLimitDTO> RemainingLifeLimits { get; set; } = new List<RemainingLifeLimitDTO>();
    }
}
