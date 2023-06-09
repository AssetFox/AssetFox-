using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// Describe the general settings used in a simulation
    /// </summary>
    public class AnalysisMethodDTO : BaseDTO
    {
        /// <summary>
        /// Not used
        /// </summary>
        public string Attribute { get; set; }

        /// <summary>
        /// User provided description of the simulation
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Optimization strategry used for the selection of treatments
        /// </summary>
        public OptimizationStrategy OptimizationStrategy { get; set; }

        /// <summary>
        /// Spending strategy used for funding treatments
        /// </summary>
        public SpendingStrategy SpendingStrategy { get; set; }

        /// <summary>
        /// Determines if multiple feasible costs should be used in the
        /// simulation
        /// </summary>
        public bool ShouldApplyMultipleFeasibleCosts { get; set; }

        /// <summary>
        /// Determines if assets deteriorate in the middle of a cash
        /// flow treatment
        /// </summary>
        public bool ShouldDeteriorateDuringCashFlow { get; set; }

        /// <summary>
        /// Allows remaining funds to be used in other budgets
        /// </summary>
        public bool ShouldUseExtraFundsAcrossBudgets { get; set; }

        /// <summary>
        /// Describes the attribute used to determine the benefit based
        /// on the improvement to conditions of this specific attribute
        /// </summary>
        public BenefitDTO Benefit { get; set; }

        /// <summary>
        /// Defines the assets to be included in this simulation
        /// </summary>
        public CriterionLibraryDTO CriterionLibrary { get; set; }
    }
}
