using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class TreatmentSupersedeRuleImportResultDTO : WarningServiceResultDTO
    {
        public Dictionary<Guid, List<TreatmentSupersedeRuleDTO>> supersedeRulesPerTreatmentIdDict { get; set; } = new Dictionary<Guid, List<TreatmentSupersedeRuleDTO>>();
    }
}
