using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class SimulationReportDetailDtos
    {
        public static SimulationReportDetailDTO Dto(Guid simulationId)
        {
            var dto = new SimulationReportDetailDTO
            {
                SimulationId = simulationId,
                Status = "Simulation report status",
            };
            return dto;
        }
    }
}
