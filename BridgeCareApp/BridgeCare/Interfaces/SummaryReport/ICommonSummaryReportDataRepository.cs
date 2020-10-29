using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface ICommonSummaryReportDataRepository
    {
        SimulationYearsModel GetSimulationYearsData(int simulationId);        
    }
}
