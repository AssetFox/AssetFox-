using System;

namespace BridgeCareCore.Services;

public sealed record AnalysisEventLogEntry(DateTimeOffset Timestamp, Guid SimulationId, string ScenarioName, string Message)
{
    public AnalysisEventLogEntry(Guid simulationId, string scenarioName, string message)
        : this(DateTimeOffset.UtcNow, simulationId, scenarioName, message)
    {
    }

    public string FormatForLog() => $"{Timestamp:O}\t{SimulationId}\t{ScenarioName}\t{Message}";
}
