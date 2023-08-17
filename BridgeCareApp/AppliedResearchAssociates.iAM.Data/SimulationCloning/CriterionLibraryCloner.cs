using System;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Static;

namespace AppliedResearchAssociates.iAM.Data.SimulationCloning
{
    internal class CriterionLibraryCloner
    {
        internal static CriterionLibraryDTO Clone(CriterionLibraryDTO critionLibrary)
        {
            var isvalid = critionLibrary.IsValid();
            var newId = isvalid ? Guid.NewGuid() : critionLibrary.Id;
            var clone = new CriterionLibraryDTO
            {
                Id = newId,
                IsSingleUse = critionLibrary.IsSingleUse,
                MergedCriteriaExpression = critionLibrary.MergedCriteriaExpression,
                Name = critionLibrary.Name,
            };
            return clone;
        }

    }
}
