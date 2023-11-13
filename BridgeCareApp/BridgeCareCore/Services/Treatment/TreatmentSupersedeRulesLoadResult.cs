using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Services.Treatment
{
    public class TreatmentSupersedeRulesLoadResult
    {        
        public Dictionary<Guid, List<TreatmentSupersedeRuleDTO>> supersedeRulesPerTreatmentId { get; set; }

        public List<string> ValidationMessages { get; set; }
    }
}
