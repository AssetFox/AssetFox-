using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Services.Treatment
{
    public class TreatmentCostLoadResult
    {
        public List<TreatmentCostDTO> Costs { get; set; }
        public List<string> ValidationMessages { get; set; }
    }
}
