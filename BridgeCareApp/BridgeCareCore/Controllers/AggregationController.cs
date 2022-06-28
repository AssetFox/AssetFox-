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
using BridgeCareCore.Services;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AggregationController : BridgeCareCoreBaseController
    {
        private int _count;
        private string _status = string.Empty;
        private double _percentage;
        private Guid _networkId = Guid.Empty;
        private readonly ILog _log;
        private readonly IAggregationService _aggregationService;

        public AggregationController(
            IAggregationService aggregationService
            )
        {
            _aggregationService = aggregationService;
        }

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
        }
        [HttpPost]
        [Route("AggregateNetworkData/{networkId}")]
        [Authorize(Policy = SecurityConstants.Policy.Admin)]
        public async Task<IActionResult> AggregateNetworkData(Guid networkId)
        {

            catch (Exception e)
            {
                UnitOfWork.Rollback();
                _status = "Aggregation failed";
                UnitOfWork.NetworkRepo.UpsertNetworkRollupDetail(_networkId, _status);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastAssignDataStatus,
                    new NetworkRollupDetailDTO { NetworkId = _networkId, Status = _status }, 0.0);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Aggregation error::{e.Message}");
                throw;
            }
        }

        private void CheckCurrentLongRunningTask(Task currentLongRunningTask)
        {
            _count = 0;
            var cts = new CancellationTokenSource();

            var currentStatusMessageTask = CreateCurrentStatusMessageTask(cts.Token);

            while (!currentLongRunningTask.IsCompleted)
            {
                if (currentStatusMessageTask.IsCompleted)
                {
                    currentStatusMessageTask = CreateCurrentStatusMessageTask(cts.Token);
                }
            }

            if (!currentStatusMessageTask.IsCompleted)
            {
                cts.Cancel();
            }
        }

        private Task CreateCurrentStatusMessageTask(CancellationToken token) =>
            Task.Run(async () =>
            {
                await Task.Delay(3000, token);
                if (_count > 3)
                {
                    _count = 0;
                }
                SendCurrentStatusMessage();
                _count++;
            }, token);

        private void SendCurrentStatusMessage()
        {
            var message = _count switch
            {
                0 => $"{_status}.",
                1 => $"{_status}..",
                2 => $"{_status}...",
                _ => _status
            };

            HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastAssignDataStatus,
                new NetworkRollupDetailDTO { NetworkId = _networkId, Status = message }, _percentage);
        }
    }
}
