using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs.Static;

namespace AppliedResearchAssociates.iAM.Analysis
{
    public static class SimulationLogMessageBuilders
    {
        public static SimulationLogMessageBuilder CalculationError(
            string message, Guid simulationId)
            => new SimulationLogMessageBuilder
            {
                Message = message,
                SimulationId = simulationId,
                Status = SimulationLogStatus.Error,
                Subject = SimulationLogSubject.Calculation,
            };

        internal static SimulationLogMessageBuilder InvalidTreatmentCost(Section section, SelectableTreatment treatment, double cost, Guid simulationId) => new SimulationLogMessageBuilder
        {
            SimulationId = simulationId,
            Status = SimulationLogStatus.Error,
            Subject = SimulationLogSubject.Calculation,
            Message = $"Invalid cost {cost} for treatment {treatment.Name} on section ({section.Name} {section.Id})",
        };
    }
}
