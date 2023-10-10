using AppliedResearchAssociates.iAM.DTOs;
using System;
using System.Collections.Generic;

namespace BridgeCareCore.Services
{
    internal class RemainingLifeLimitCloner
    {
        internal static RemainingLifeLimitDTO Clone(RemainingLifeLimitDTO remainingLifeLimit, Guid ownerId)
        {
            var cloneCriterionLibrary = CriterionLibraryCloner.CloneNullPropagating(remainingLifeLimit.CriterionLibrary, ownerId);
            var clone = new RemainingLifeLimitDTO
            {
                LibraryId = remainingLifeLimit.LibraryId,
                CriterionLibrary = cloneCriterionLibrary,
                Attribute = remainingLifeLimit.Attribute,
                IsModified = remainingLifeLimit.IsModified,
                Value = remainingLifeLimit.Value,
            };
            return clone;
        }

        internal static List<RemainingLifeLimitDTO> CloneList(IEnumerable<RemainingLifeLimitDTO> remainingLifeLimits, Guid ownerId)
        {
            var clone = new List<RemainingLifeLimitDTO>();
            foreach (var remainingLifeLimit in remainingLifeLimits)
            {
                var childClone = Clone(remainingLifeLimit, ownerId);
                clone.Add(childClone);
            }
            return clone;

        }
    }
}
