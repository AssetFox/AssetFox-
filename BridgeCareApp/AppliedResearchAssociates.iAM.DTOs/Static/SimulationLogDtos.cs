using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.iAM.Analysis;

namespace AppliedResearchAssociates.iAM.DTOs.Static
{
    public static class SimulationLogDtos
    {
        public static SimulationLogDTO GenericException(Guid simulationId, Exception exception)
        {
            var r = new SimulationLogDTO
            {
                Id = Guid.NewGuid(),
                Message = $"An exception was thrown. {exception.Message}. This log entry does not include the stack trace, as it might contain sensitive information.",
                SimulationId = simulationId,
                Status = (int)SimulationLogStatus.Error,
                Subject = (int)SimulationLogSubject.ExceptionThrown,
            };
            return r;
        }
    }
}
