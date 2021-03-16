using AppliedResearchAssociates.iAM.DataAccess;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs
{
    public class AnalysisMethodDTO : BaseDTO
    {
        public string Attribute { get; set; }
        public string Description { get; set; }
        public OptimizationStrategy OptimizationStrategy { get; set; }
        public SpendingStrategy SpendingStrategy { get; set; }
        public bool ShouldApplyMultipleFeasibleCosts { get; set; }
        public bool ShouldDeteriorateDuringCashFlow { get; set; }
        public bool ShouldUseExtraFundsAcrossBudgets { get; set; }
        public BenefitDTO Benefit { get; set; }
        public CriterionLibraryDTO CriterionLibrary { get; set; }
    }
}
