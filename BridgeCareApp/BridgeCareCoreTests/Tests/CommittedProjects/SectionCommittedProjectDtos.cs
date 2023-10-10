using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace BridgeCareCoreTests.Tests
{
    public static class SectionCommittedProjectDtos
    {
        public static SectionCommittedProjectDTO Dto(Guid? id = null, Guid? scenarioBudgetId = null, Guid simulationId = new Guid(), ProjectSourceDTO projectSource = ProjectSourceDTO.None)
        {
            var resolveId = id ?? Guid.NewGuid();
            var resolveScenarioBudgetId = scenarioBudgetId ?? Guid.NewGuid();
            var dto = new SectionCommittedProjectDTO
            {
                Id = resolveId,
                ScenarioBudgetId = resolveScenarioBudgetId,
                SimulationId = simulationId,
                ProjectSource = projectSource
            };
            return dto;
        }
    }
}
