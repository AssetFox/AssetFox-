using System;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Data.SimulationCloning;

namespace AppliedResearchAssociates.iAM.Data.SimulationCloning
{
    internal class AnalysisMethodCloner
    {
        internal static AnalysisMethodDTO Clone(AnalysisMethodDTO analysisMethod, Guid ownerId)
        {
            var cloneBenefit = BenefitCloner.Clone(analysisMethod.Benefit);
            var cloneCritionLibrary = CriterionLibraryCloner.Clone(analysisMethod.CriterionLibrary, ownerId);
            var clone = new AnalysisMethodDTO
            {
                Id = Guid.NewGuid(),
                Attribute = analysisMethod.Attribute,
                Description = analysisMethod.Description,
                Benefit = cloneBenefit,
                CriterionLibrary = cloneCritionLibrary,
                OptimizationStrategy = analysisMethod.OptimizationStrategy,
                ShouldApplyMultipleFeasibleCosts = analysisMethod.ShouldApplyMultipleFeasibleCosts,
                ShouldDeteriorateDuringCashFlow = analysisMethod.ShouldDeteriorateDuringCashFlow,
                ShouldUseExtraFundsAcrossBudgets = analysisMethod.ShouldUseExtraFundsAcrossBudgets,
                SpendingStrategy = analysisMethod.SpendingStrategy,
            };
            return clone;
        }
            
    }
}
