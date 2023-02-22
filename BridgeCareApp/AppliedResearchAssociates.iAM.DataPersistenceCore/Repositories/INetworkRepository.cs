using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;
using Network = AppliedResearchAssociates.iAM.Data.Networking.Network;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface INetworkRepository
    {
        void CreateNetwork(Network network);

        void CreateNetwork(Analysis.Network network);

        Task<List<NetworkDTO>> Networks();

        List<Network> GetAllNetworks();

        NetworkEntity GetMainNetwork();

        Analysis.Network GetSimulationAnalysisNetwork(Guid networkId, Explorer explorer, bool areFacilitiesRequired = true);

        void DeleteNetworkData();

        void DeleteNetwork(Guid networkId);

        void UpsertNetworkRollupDetail(Guid networkId, string status);

        string GetNetworkName(Guid networkId);
    }
}
