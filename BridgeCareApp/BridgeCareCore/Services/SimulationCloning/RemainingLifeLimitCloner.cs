using AppliedResearchAssociates.iAM.DTOs;
using System;
using System.Collections.Generic;

namespace BridgeCareCore.Services.SimulationCloning
{
    internal class RemainingLifeLimitCloner
    {
        internal static RemainingLifeLimitDTO Clone(RemainingLifeLimitDTO remainingLifeLimit)
        {
            var cloneCritionLibrary = CriterionLibraryCloner.Clone(remainingLifeLimit.CriterionLibrary);
            var clone = new RemainingLifeLimitDTO
            {
                LibraryId = remainingLifeLimit.LibraryId,
                CriterionLibrary = cloneCritionLibrary,
                Attribute = remainingLifeLimit.Attribute,
                IsModified = remainingLifeLimit.IsModified,
                Value = remainingLifeLimit.Value,
            };
            return clone;
        }

        internal static List<RemainingLifeLimitDTO> CloneList(IEnumerable<RemainingLifeLimitDTO> remainingLifeLimits)
        {
            var clone = new List<RemainingLifeLimitDTO>();
            foreach (var remainingLifeLimit in remainingLifeLimits)
            {
                var childClone = Clone(remainingLifeLimit);
                clone.Add(childClone);
            }
            return clone;

        }
    }
}
