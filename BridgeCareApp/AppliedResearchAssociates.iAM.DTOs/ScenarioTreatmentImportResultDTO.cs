﻿using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class ScenarioTreatmentSupersedeRuleImportResultDTO: WarningServiceResultDTO
    {        
        public List<TreatmentDTO> Treatments { get; set; }
    }
}
