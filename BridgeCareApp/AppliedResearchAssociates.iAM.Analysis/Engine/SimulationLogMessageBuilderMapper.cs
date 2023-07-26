using System;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

public class SimulationLogMessageBuilderMapper
{
    public static SimulationLogDTO ToDTO(SimulationLogMessageBuilder builder)
        => new SimulationLogDTO
        {
            Id = Guid.NewGuid(),
            Message = builder.Message,
            SimulationId = builder.SimulationId,
            Status = (int)builder.Status,
            Subject = (int)builder.Subject,
        };
}
