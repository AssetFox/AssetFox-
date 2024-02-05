using System;
using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Services
{
    internal class AnalysisMethodCloner
    {
        internal static AnalysisMethodDTO Clone(AnalysisMethodDTO analysisMethod, Guid ownerId)
        {
            var cloneBenefit = BenefitCloner.Clone(analysisMethod.Benefit);
            var cloneCriterionLibrary = CriterionLibraryCloner.CloneNullPropagating(analysisMethod.CriterionLibrary, ownerId);
            var clone = new AnalysisMethodDTO
            {
                Id = Guid.NewGuid(),
                Attribute = analysisMethod.Attribute,
                Description = analysisMethod.Description,
                Benefit = cloneBenefit,
                CriterionLibrary = cloneCriterionLibrary,
                OptimizationStrategy = analysisMethod.OptimizationStrategy,
                ShouldApplyMultipleFeasibleCosts = analysisMethod.ShouldApplyMultipleFeasibleCosts,
                ShouldDeteriorateDuringCashFlow = analysisMethod.ShouldDeteriorateDuringCashFlow,
                ShouldUseExtraFundsAcrossBudgets = analysisMethod.ShouldUseExtraFundsAcrossBudgets,
                shouldAllowMultipleTreatments = analysisMethod.shouldAllowMultipleTreatments,
                SpendingStrategy = analysisMethod.SpendingStrategy,

            };
            return clone;
        }
            
    }
}
