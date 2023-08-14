using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.Data.SimulationCloning
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
        internal static List<ScenarioPerformanceCurvesImportResultDTO> CloneList(IEnumerable<ScenarioPerformanceCurvesImportResultDTO> scenarioPerformanceCurvesImportResult)
        {
            var clone = new List<ScenarioPerformanceCurvesImportResultDTO>();
            foreach (var scenarioPerformance in scenarioPerformanceCurvesImportResult)
            {
                var childClone = Clone(scenarioPerformance);
                clone.Add(childClone);
            }
            return clone;

        }
        internal static List<ScenarioPerformanceCurvesImportResultDTO> CloneListNullPropagating(IEnumerable<ScenarioPerformanceCurvesImportResultDTO> scenarioPerformanceCurvesImportResult)
        {
           if (scenarioPerformanceCurvesImportResult == null) { return null; }
           return CloneList(scenarioPerformanceCurvesImportResult);
        }
    }
}
