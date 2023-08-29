using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class SimulationAnalysisDetailDtos
    {
        public static SimulationAnalysisDetailDTO ForSimulation(Guid simulationId)
        {
            var dto = new SimulationAnalysisDetailDTO
            {
                LastRun = DateTime.Now,
                RunTime = "1 minute",
                SimulationId = simulationId,
                Status = "Completed",
            };
            return dto;
        }
    }
}
