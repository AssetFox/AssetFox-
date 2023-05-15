using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// .
    /// </summary>
    public class AnalysisMethodDTO : BaseDTO
    {
        /// <summary>
        /// .
        /// </summary>
        public string Attribute { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public OptimizationStrategy OptimizationStrategy { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public SpendingStrategy SpendingStrategy { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public bool ShouldApplyMultipleFeasibleCosts { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public bool ShouldDeteriorateDuringCashFlow { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public bool ShouldUseExtraFundsAcrossBudgets { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public BenefitDTO Benefit { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public CriterionLibraryDTO CriterionLibrary { get; set; }
    }
}
