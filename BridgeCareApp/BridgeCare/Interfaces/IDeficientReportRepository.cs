using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface IDeficientReportRepository
    {
        DeficientResult GetData(SimulationModel data, int[] totalYears);
    }
}
