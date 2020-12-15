using System;
using System.Threading.Tasks;

namespace BridgeCareCore.Interfaces.Simulation
{
    public interface ISimulationAnalysis
    {
        void CreateAndRun(Guid networkId,Guid simulationId);
        void GetAllSimulations(Guid networkId);
        void CreateSimulation(string simulationName);
    }
}
