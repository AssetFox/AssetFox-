using System;
using System.Threading.Tasks;

namespace BridgeCareCore.Interfaces
{
    public interface ISimulationAnalysis
    {
        Task CreateAndRunPermitted(Guid networkId, Guid simulationId);

        Task CreateAndRun(Guid networkId, Guid simulationId);
    }
}
