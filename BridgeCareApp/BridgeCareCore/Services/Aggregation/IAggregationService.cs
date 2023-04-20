using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Services.Aggregation
{
    public interface IAggregationService
    {
        /// <summary>Returns true if the aggregation succeeded</summary> 
        Task<bool> AggregateNetworkData(
            ChannelWriter<AggregationStatusMemo> writer,
            Guid networkId,
            AggregationState state,
            List<AttributeDTO> attributes, CancellationToken? cancellationToken = null);
    }
}
