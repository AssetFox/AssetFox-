using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Models;

namespace BridgeCareCore.Services.Aggregation
{
    public interface IAggregationService
    {
        /// <summary>Returns true if the aggregation succeeded</summary> 
        Task<bool> AggregateNetworkData(
            ChannelWriter<AggregationStatusMemo> writer,
            Guid networkId,
            AggregationState state,
            UserInfo userInfo,
            List<AttributeDTO> attributes);
    }
}
