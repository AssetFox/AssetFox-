using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.SimulationCloning
{
    public static class CloneSimulationDtos
    {
        public static CloneSimulationDTO Create(Guid simulationId, Guid networkId, string newSimulationName)
        {
            var dto = new CloneSimulationDTO
            {
                Id = Guid.NewGuid(),
                networkId = networkId,
                scenarioId = simulationId,
                scenarioName = newSimulationName
            };
            return dto;
        }
    }
}
