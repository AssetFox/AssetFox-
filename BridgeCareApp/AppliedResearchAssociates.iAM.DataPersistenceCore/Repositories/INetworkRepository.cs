using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface INetworkRepository
    {
        void CreateNetwork(DataAssignment.Networking.Network network);

        void CreateNetwork(Network network);

        List<DataAssignment.Networking.Network> GetAllNetworks();

        NetworkEntity GetPennDotNetwork();

        bool CheckPennDotNetworkHasData();

        Domains.Network GetSimulationAnalysisNetwork(Guid networkId, Explorer explorer, bool areFacilitiesRequired = true);

        void DeleteNetworkData();
    }
}
