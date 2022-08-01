using System;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DTOs.Static
{
    public static class SimulationLogDtos
    {
        public static SimulationLogDTO GenericException(Guid simulationId, Exception exception)
        {
            var returnValue = new SimulationLogDTO
            {
                Id = Guid.NewGuid(),
                Message = $"An exception was thrown. {exception.Message}. This log entry does not include the stack trace, as it might contain sensitive information.",
                SimulationId = simulationId,
                Status = (int)SimulationLogStatus.Error,
                Subject = (int)SimulationLogSubject.ExceptionThrown,
            };
            return returnValue;
        }
    }
}
