using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class ScenarioTreatmentSupersedeRuleImportResultDTO : WarningServiceResultDTO
    {
        public Dictionary<Guid, List<TreatmentSupersedeRuleDTO>> supersedeRulesPerTreatmentId { get; set; } = new Dictionary<Guid, List<TreatmentSupersedeRuleDTO>>();
    }
}
