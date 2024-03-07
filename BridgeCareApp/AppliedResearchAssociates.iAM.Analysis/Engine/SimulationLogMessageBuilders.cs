using System;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

public static class SimulationLogMessageBuilders
{
    public static SimulationLogMessageBuilder CalculationFatal(
        string message, Guid simulationId)
        => new SimulationLogMessageBuilder
        {
            Message = message,
            SimulationId = simulationId,
            Status = SimulationLogStatus.Fatal,
            Subject = SimulationLogSubject.Calculation,
        };

    internal static SimulationLogMessageBuilder InvalidTreatmentCost(AnalysisMaintainableAsset asset, Treatment treatment, double cost, Guid simulationId) => new SimulationLogMessageBuilder
    {
        SimulationId = simulationId,
        Status = SimulationLogStatus.Error,
        Subject = SimulationLogSubject.Calculation,
        Message = $"Invalid cost {cost} for treatment {treatment.Name} on asset ({asset.AssetName} {asset.Id})",
    };

    internal static SimulationLogMessageBuilder RuntimeWarning(SimulationMessageBuilder innerBuilder, Guid simulationId)
        => new SimulationLogMessageBuilder
        {
            SimulationId = simulationId,
            Status = SimulationLogStatus.Warning,
            Subject = SimulationLogSubject.Runtime,
            Message = innerBuilder.ToString(),
        };


    internal static SimulationLogMessageBuilder RuntimeFatal(SimulationMessageBuilder innerBuilder, Guid simulationId)
        => new SimulationLogMessageBuilder
        {
            SimulationId = simulationId,
            Status = SimulationLogStatus.Fatal,
            Subject = SimulationLogSubject.Runtime,
            Message = innerBuilder.ToString(),
        };

    internal static SimulationLogMessageBuilder HasValidationErrors(SimulationMessageBuilder messageBuilder, Guid simulationId) =>
        new SimulationLogMessageBuilder
        {
            Message = messageBuilder.ToString(),
            SimulationId = simulationId,
            Status = SimulationLogStatus.Fatal,
            Subject = SimulationLogSubject.Validation,
        };

    internal static SimulationLogMessageBuilder Exception(Exception e, Guid simulationId)
        => new SimulationLogMessageBuilder
    {
        Message = $"Exception thrown: {e.Message}",
        SimulationId = simulationId,
        Status = SimulationLogStatus.Error,
        Subject = SimulationLogSubject.ExceptionThrown,
    };
}
