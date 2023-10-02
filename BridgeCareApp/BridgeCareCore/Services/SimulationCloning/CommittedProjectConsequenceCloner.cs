using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Services.SimulationCloning
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

        internal static List<CommittedProjectConsequenceDTO> CloneList(IEnumerable<CommittedProjectConsequenceDTO> committedProjectConsequences)
        {
            var clone = new List<CommittedProjectConsequenceDTO>();
            foreach (var committedProjectConsequence in committedProjectConsequences)
            {
                var childClone = Clone(committedProjectConsequence);
                clone.Add(childClone);
            }
            return clone;

        }
    }

}
