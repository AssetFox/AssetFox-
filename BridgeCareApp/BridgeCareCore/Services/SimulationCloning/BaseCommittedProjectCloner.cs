using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace BridgeCareCore.Services.SimulationCloning
{
    internal class BaseCommittedProjectCloner
    {
        internal static BaseCommittedProjectDTO Clone(BaseCommittedProjectDTO baseCommittedProject, Dictionary<Guid, Guid> budgetIdMap)
        {
            var visitor = new BaseCommittedProjectDtoClonerVisitor();
            var clone = baseCommittedProject.Accept(visitor, budgetIdMap);
            return clone;

        }

        internal static List<BaseCommittedProjectDTO> CloneList(IEnumerable<BaseCommittedProjectDTO> baseCommittedProjects, Dictionary<Guid, Guid> budgetIdMap)
        {
            var clone = new List<BaseCommittedProjectDTO>();
            foreach (var baseCommittedProject in baseCommittedProjects)
            {
                if (budgetIdMap.ContainsKey(baseCommittedProject.ScenarioBudgetId.Value))
                {
                    var childClone = Clone(baseCommittedProject, budgetIdMap);
                    clone.Add(childClone);
                }
            }
            return clone;

        }
    }
}
