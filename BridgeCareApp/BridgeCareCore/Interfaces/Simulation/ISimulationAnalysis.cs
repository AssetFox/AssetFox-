using System;

namespace BridgeCareCore.Interfaces.Simulation
{
    public interface ISimulationAnalysis
    {
        public void CreateAndRun(Guid networkId,Guid simulationId);
        void GetAllSimulations(Guid networkId);
        void CreateSimulation(string simulationName);
    }
}
