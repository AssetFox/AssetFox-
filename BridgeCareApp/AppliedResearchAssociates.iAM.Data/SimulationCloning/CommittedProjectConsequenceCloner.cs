using System;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.Data.SimulationCloning
{
    internal class CommittedProjectConsequenceCloner
    {
        internal static CommittedProjectConsequenceDTO Clone(CommittedProjectConsequenceDTO committedProjectConsequence)
        {                      
            var clone = new CommittedProjectConsequenceDTO            
            {
                Id = Guid.NewGuid(),
                CommittedProjectId = committedProjectConsequence.CommittedProjectId,
                Attribute = committedProjectConsequence.Attribute,
                ChangeValue = committedProjectConsequence.ChangeValue,
                PerformanceFactor = committedProjectConsequence.PerformanceFactor,
            };
            return clone;
        }

    }

}
