using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DTOs.Static
{
    public static class SimulationLogDtos
    {
        public static SimulationLogDTO ExceptionGeneric(Guid simulationId)
        {
            var r = new SimulationLogDTO
            {
                Id = Guid.NewGuid(),
                Message = $"An exception was thrown. This message does not include the details as they might contain sensitive information.",
                SimulationId = simulationId,
                Status = (int)SimulationLogStatus.Error,
                Subject = (int)SimulationLogSubject.ExceptionThrown,
            };
            return r;
        }
    }
}
