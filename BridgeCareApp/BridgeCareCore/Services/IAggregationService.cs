using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;

namespace BridgeCareCore.Services
{
    public interface IAggregationService
    {
        /// <summary>Returns true if the aggregation succeeded</summary> 
        Task<bool> AggregateNetworkData(ChannelWriter<NetworkRollupDetailDTO> writer, Guid networkId);
    }
}
