using System;
using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Services.SimulationCloning
{
    internal class CriterionLibraryCloner
    {
        internal static CriterionLibraryDTO Clone(CriterionLibraryDTO critionLibrary)
        {
            var clone = new CriterionLibraryDTO
            {
                Id = Guid.NewGuid(),
                IsSingleUse = critionLibrary.IsSingleUse,
                MergedCriteriaExpression = critionLibrary.MergedCriteriaExpression,
            };
            return clone;
        }

    }
}
