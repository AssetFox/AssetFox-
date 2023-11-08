using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class ScenarioTreatmentSupersedeRuleImportResultDTO : WarningServiceResultDTO
    {
        public List<TreatmentSupersedeRuleDTO> TreatmentSupersedeRules { get; set; }
    }
}
