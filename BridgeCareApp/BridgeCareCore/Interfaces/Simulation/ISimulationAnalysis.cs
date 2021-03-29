using System;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;

namespace BridgeCareCore.Interfaces.Simulation
{
    public interface ISimulationAnalysis
    {
        Task CreateAndRunPermitted(Guid networkId, Guid simulationId);

        Task CreateAndRun(Guid networkId, Guid simulationId);
    }
}
