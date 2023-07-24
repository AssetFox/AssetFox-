using AppliedResearchAssociates.iAM.DTOs;
using System;

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

    }
}
