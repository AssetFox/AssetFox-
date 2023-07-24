using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Services.SimulationCloning
{
    internal class ScenarioPerformanceCurvesImportResultCloner
    {
        internal static ScenarioPerformanceCurvesImportResultDTO Clone(ScenarioPerformanceCurvesImportResultDTO scenarioPerformanceCurvesImportResult)
        {
            var clone = new ScenarioPerformanceCurvesImportResultDTO
            {
                PerformanceCurves = scenarioPerformanceCurvesImportResult.PerformanceCurves,
                WarningMessage = scenarioPerformanceCurvesImportResult.WarningMessage,
            };
            return clone;
        }

    }
}
