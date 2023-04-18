using System;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

public class SimulationLogMessageBuilder
{
    public string Message { get; set; }

    public Guid SimulationId { get; set; }

    public SimulationLogStatus Status { get; set; }

    public SimulationLogSubject Subject { get; set; }

    public override string ToString() => $"[Simulation ID {SimulationId}, Status \"{Status}\", Subject \"{Subject}\"] {Message}";
}
