using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class SimulationLogDtos
    {
        public static SimulationLogDTO Dto(Guid simulationId)
        {
            var id = Guid.NewGuid();
            var dto = new SimulationLogDTO
            {
                SimulationId = simulationId,
                Message = "Simulation log",
                Id = id,
                Subject = 123,
            };
            return dto;
        }
    }
}
