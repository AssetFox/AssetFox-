using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class TreatmentDTO : BaseDTO
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int ShadowForAnyTreatment { get; set; }

        public int ShadowForSameTreatment { get; set; }

        public CriterionLibraryDTO CriterionLibrary { get; set; }

        public List<TreatmentCostDTO> Costs { get; set; }

        public List<TreatmentConsequenceDTO> Consequences { get; set; }

        public List<Guid> BudgetIds { get; set; }
    }
}
