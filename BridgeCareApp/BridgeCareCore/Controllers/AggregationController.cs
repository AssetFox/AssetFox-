using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.Data;
using AppliedResearchAssociates.iAM.Data.Aggregation;
using AppliedResearchAssociates.iAM.Data.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Controllers.BaseController;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Logging;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Channels;
using BridgeCareCore.Services.Aggregation;
using System.Timers;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AggregationController : BridgeCareCoreBaseController
    {

        private readonly ILog _log;
        private readonly IAggregationService _aggregationService;

        private void DoublyBroadcastError(string broadcastError)
        {
            _log.Error(broadcastError);
            HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, broadcastError);
        }

        public AggregationController(ILog log, IAggregationService aggregationService,
            IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork,
            IHubService hubService, IHttpContextAccessor httpContextAccessor) :
            base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _aggregationService = aggregationService ?? throw new ArgumentNullException(nameof(aggregationService));
        }
        [HttpPost]
        [Route("AggregateNetworkData/{networkId}")]
        [Authorize(Policy = SecurityConstants.Policy.Admin)]
        public async Task<IActionResult> AggregateNetworkData(Guid networkId)
        {
            var channel = Channel.CreateUnbounded<AggregationStatusMemo>();
            var state = new AggregationState();
            try
            {
                var result = await _aggregationService.AggregateNetworkData(channel.Writer, networkId, state, UserInfo);
                if (result)
                {
                    return Ok();
                } else
                {
                    return StatusCode(500, state.ErrorMessage);
                }
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                state.Status = "Aggregation failed";
                UnitOfWork.NetworkRepo.UpsertNetworkRollupDetail(networkId, state.Status);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastAssignDataStatus,
                    new NetworkRollupDetailDTO { NetworkId = networkId, Status = state.Status}, 0.0);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Aggregation error::{e.Message}");
                throw;
            }
        }

        private void NotifyUserOfState(AggregationState state)
        {

        }

        private System.Timers.Timer KeepUserInformedOfState(AggregationState state)
        {
            var timer = new System.Timers.Timer(3000);
            timer.Elapsed += delegate
            {
                SendCurrentStatusMessage(state);
            };
            timer.Start();
            return timer;
        }
        private void SendCurrentStatusMessage(AggregationState state)
        {
            var count = state.Count % 3;
            var message =  count switch
            {
                0 => $"{state.Status}.",
                1 => $"{state.Status}..",
                2 => $"{state.Status}...",
                _ => state.Status
            };

            HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastAssignDataStatus,
                new NetworkRollupDetailDTO { NetworkId = state.NetworkId, Status = message }, state.Percentage);
        }
    }
}
