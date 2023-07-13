using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// Describes a specific treatment that may be used to improve asset
    /// condition within a specific scenario.
    /// </summary>
    public class TreatmentDTO : BaseDTO
    {
        /// <summary>
        /// Name of the treatment
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Long description of the treatment.  May be null
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Time (in cycles) before ANY treatment may be applied to an asset
        /// after this treatment has been applied
        /// </summary>
        public int ShadowForAnyTreatment { get; set; }

        /// <summary>
        /// Time (in cycles) before THIS treatment may be applied to an asset
        /// after this treatment has been applied
        /// </summary>
        public int ShadowForSameTreatment { get; set; }

        /// <summary>
        /// Category of treatment
        /// </summary>
        public TreatmentCategory Category { get; set; }

        /// <summary>
        /// General types of assets this treatment is applied to.  Note this is
        /// used in reporting - the user must still define the logic for this
        /// treatment using a criterion library
        /// </summary>
        public AssetCategory AssetType { get; set; }

        /// <summary>
        /// Has this setting been modified from the associated library?
        /// </summary>
        public bool IsModified { get; set; }

        /// <summary>
        /// ID of an associated library
        /// </summary>
        public Guid LibraryId { get; set; }

        /// <summary>
        /// Defines the assets that can use this treatment
        /// </summary>
        public CriterionLibraryDTO CriterionLibrary { get; set; }

        /// <summary>
        /// List of costs required to apply this treatment
        /// </summary>
        public List<TreatmentCostDTO> Costs { get; set; }

        /// <summary>
        /// List of changes to condition that applying this treatment will cause
        /// </summary>
        public List<TreatmentConsequenceDTO> Consequences { get; set; }

        /// <summary>
        /// List of performance factors that will be applied to performance models
        /// due to the application of this treatment
        /// </summary>
        public List<TreatmentPerformanceFactorDTO> PerformanceFactors { get; set; }

        /// <summary>
        /// List of the budgets by ID which can fund this treatment.  Note that for a
        /// treatment to be applied, the budget must be present in this list AND
        /// the asset must meet the critiera specified at the budget level
        /// </summary>
        public List<Guid> BudgetIds { get; set; }

        /// <summary>
        /// List of the budgets by name which can fund this treatment.  Note that for a
        /// treatment to be applied, the budget must be present in this list AND
        /// the asset must meet the critiera specified at the budget level
        /// </summary>
        public List<TreatmentBudgetDTO> Budgets { get; set; }
    }
}
