using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Services.Treatment
{
    public class TreatmentConsequenceLoadResult
    {
        public List<TreatmentConsequenceDTO> Consequences { get; set; }
        public List<string> ValidationMessages { get; set; }
    }
}
