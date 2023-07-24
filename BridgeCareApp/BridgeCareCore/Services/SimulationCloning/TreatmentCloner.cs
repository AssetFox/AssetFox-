using AppliedResearchAssociates.iAM.DTOs;
using System;

namespace BridgeCareCore.Services.SimulationCloning
{
    internal class TreatmentCloner
    {
        internal static TreatmentDTO Clone(TreatmentDTO treatment)
        {            
            var cloneCritionLibrary = CriterionLibraryCloner.Clone(treatment.CriterionLibrary);           
            var clone = new TreatmentDTO
            {
              Name = treatment.Name,
              LibraryId = treatment.LibraryId,
              Description = treatment.Description,
              ShadowForAnyTreatment = treatment.ShadowForAnyTreatment,
              ShadowForSameTreatment = treatment.ShadowForSameTreatment,
              CriterionLibrary = cloneCritionLibrary,
              AssetType = treatment.AssetType,
              BudgetIds = treatment.BudgetIds,
              Budgets = treatment.Budgets,
              Category = treatment.Category,
              Consequences = treatment.Consequences,
              Costs = treatment.Costs,
              Id = Guid.NewGuid(),
              IsModified = treatment.IsModified,
              PerformanceFactors = treatment.PerformanceFactors,              
            };
            return clone;
        }

    }
}
