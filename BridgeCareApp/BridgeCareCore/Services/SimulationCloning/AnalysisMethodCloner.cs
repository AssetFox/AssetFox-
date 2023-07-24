using System;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Services.SimulationCloning;

namespace BridgeCareCore.Services
{
    internal class AnalysisMethodCloner
    {
        internal static AnalysisMethodDTO Clone(AnalysisMethodDTO analysisMethod)
        {
            var cloneBenefit = BenefitCloner.Clone(analysisMethod.Benefit);
            var cloneCritionLibrary = CriterionLibraryCloner.Clone(analysisMethod.CriterionLibrary);
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
