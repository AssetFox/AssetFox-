using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Services.SimulationCloning;

internal class SimulationAnalysisDetailCloner
{
    internal static SimulationAnalysisDetailDTO Clone(SimulationAnalysisDetailDTO simulationAnalysisDetail)
    {
        var clone = new SimulationAnalysisDetailDTO
        {
            LastRun = simulationAnalysisDetail.LastRun,
            ReportType = simulationAnalysisDetail.ReportType,
            RunTime = simulationAnalysisDetail.RunTime,            
            Status = simulationAnalysisDetail.Status            
        };
        
        return clone;
    }
}
