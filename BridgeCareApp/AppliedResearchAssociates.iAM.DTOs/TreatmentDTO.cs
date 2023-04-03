using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using static AppliedResearchAssociates.iAM.DTOs.Enums.TreatmentDTOEnum;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class TreatmentDTO : BaseDTO
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int ShadowForAnyTreatment { get; set; }

        public int ShadowForSameTreatment { get; set; }

        public double PerformanceFactor { get; set; }

        public TreatmentCategory Category { get; set; }

        public AssetType AssetType { get; set; }

        public CriterionLibraryDTO CriterionLibrary { get; set; }

        public List<TreatmentCostDTO> Costs { get; set; }

        public List<TreatmentConsequenceDTO> Consequences { get; set; }

        public List<Guid> BudgetIds { get; set; }

        public List<TreatmentBudgetDTO> Budgets { get; set; }
    }
}
