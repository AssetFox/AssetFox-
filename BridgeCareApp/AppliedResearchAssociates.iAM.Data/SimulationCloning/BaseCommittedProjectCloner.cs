using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.Data.SimulationCloning
{
    internal class BaseCommittedProjectCloner
    {
        internal static BaseCommittedProjectDTO Clone(BaseCommittedProjectDTO baseCommittedProject)
        {
            var visitor = new BaseCommittedProjectDtoClonerVisitor();
            var clone = baseCommittedProject.Accept(visitor);
            return clone;

        }

        internal static List<BaseCommittedProjectDTO> CloneList(IEnumerable<BaseCommittedProjectDTO> baseCommittedProjects)
        {
            var clone = new List<BaseCommittedProjectDTO>();
            foreach (var baseCommittedProject in baseCommittedProjects)
            {
                var childClone = Clone(baseCommittedProject);
                clone.Add(childClone);
            }
            return clone;

        }
    }
}
