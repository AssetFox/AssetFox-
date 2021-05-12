using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface INetworkRepository
    {
        void CreateNetwork(DataAssignment.Networking.Network network);

        void CreateNetwork(Network network);

        Task<List<NetworkDTO>> Networks();

        List<DataAssignment.Networking.Network> GetAllNetworks();

        NetworkEntity GetPennDotNetwork();

        bool CheckPennDotNetworkHasData();

        Domains.Network GetSimulationAnalysisNetwork(Guid networkId, Explorer explorer, bool areFacilitiesRequired = true);

        void DeleteNetworkData();

        void UpsertNetworkRollupDetail(Guid networkId, string status);
    }
}
