using System;
using System.Threading.Tasks;

namespace BridgeCareCore.Interfaces.Simulation
{
    public interface ISimulationAnalysis
    {
        Task CreateAndRun(Guid networkId,Guid simulationId);
    }
}
