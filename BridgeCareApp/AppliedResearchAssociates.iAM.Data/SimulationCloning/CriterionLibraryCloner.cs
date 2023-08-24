using System;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Static;

namespace AppliedResearchAssociates.iAM.Data.SimulationCloning
{
    internal class CriterionLibraryCloner
    {
        internal static CriterionLibraryDTO CloneNullPropagating(CriterionLibraryDTO criterionLibrary, Guid ownerId)
        {
            if (criterionLibrary == null)
            {
                return null;
            }
            var isvalid = criterionLibrary.IsValid();
            var newId = isvalid ? Guid.NewGuid() : ownerId;
            var clone = new CriterionLibraryDTO
            {
                Id = newId,
                IsSingleUse = criterionLibrary.IsSingleUse,
                MergedCriteriaExpression = criterionLibrary.MergedCriteriaExpression,
                Name = criterionLibrary.Name,
                Owner = ownerId,
            };
            return clone;
        }

    }
}
