using AppliedResearchAssociates.iAM.DTOs;
using System.Collections.Generic;

namespace BridgeCareCore.Services.Treatment
{
    public class TreatmentSupersedeRulesLoadResult
    {
        public List<TreatmentDTO> Treatments { get; set; }
        public List<string> ValidationMessages { get; set; }

    }
}
