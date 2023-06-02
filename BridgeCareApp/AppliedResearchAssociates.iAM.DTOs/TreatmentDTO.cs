using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using static AppliedResearchAssociates.iAM.DTOs.Enums.TreatmentDTOEnum;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// .
    /// </summary>
    public class TreatmentDTO : BaseDTO
    {
        /// <summary>
        /// .
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public int ShadowForAnyTreatment { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public int ShadowForSameTreatment { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public TreatmentType Category { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public AssetType AssetType { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public bool IsModified { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public Guid LibraryId { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public CriterionLibraryDTO CriterionLibrary { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public List<TreatmentCostDTO> Costs { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public List<TreatmentConsequenceDTO> Consequences { get; set; }

        public List<TreatmentPerformanceFactorDTO> PerformanceFactors { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public List<Guid> BudgetIds { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public List<TreatmentBudgetDTO> Budgets { get; set; }
    }
}
