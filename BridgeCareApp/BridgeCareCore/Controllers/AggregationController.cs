using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Data;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Controllers.BaseController;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
using BridgeCareCore.Services.Aggregation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BridgeCareCore.Models;
using BridgeCareCore.Services;
using AppliedResearchAssociates.iAM.Common;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AggregationController : BridgeCareCoreBaseController
    {
        public const string AggregationError = "Aggregation Error";
        public const bool UpdateAttributes = false;
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

        private static bool FalseButCompilerDoesNotKnowThat => Guid.NewGuid() == Guid.Empty;

        [HttpPost]
        [Route("AggregateNetworkData/{networkId}")]
        [ClaimAuthorize("NetworkAggregateAccess")]
        public async Task<IActionResult> AggregateNetworkData(Guid networkId, List<AllAttributeDTO> attributes)
        {
            if (FalseButCompilerDoesNotKnowThat || UpdateAttributes)
            {
                var dataSources = UnitOfWork.DataSourceRepo.GetDataSources();
                var metadataDataSource = dataSources.FirstOrDefault(ds => ds.Name == "MetaData.Json");
                var metadataDataSourceId = metadataDataSource.Id;
                var metadataAttributes = UnitOfWork.AttributeMetaDataRepo.GetAllAttributes(metadataDataSourceId);
                var dbAttributes = UnitOfWork.AttributeRepo.GetAttributes();
                UnitOfWork.AttributeRepo.UpsertAttributes(metadataAttributes);
                var dbAttributesAfter = UnitOfWork.AttributeRepo.GetAttributes();
                var dbAttributeIdsAfter = dbAttributesAfter.Select(a => a.Id).ToList();
                var metadataAttributeIds = metadataAttributes.Select(a => a.Id).ToList();
                var attributeIdsToDelete = dbAttributeIdsAfter.Except(metadataAttributeIds).ToList();
                UnitOfWork.AttributeRepo.DeleteAttributesShouldNeverBeNeededButSometimesIs(attributeIdsToDelete);
                var dbAttributesAfterDeletion = UnitOfWork.AttributeRepo.GetAttributes();
                var dbAttributeIdsAfterDeletion = dbAttributesAfterDeletion.Select(a => a.Id).ToList();
                var attributeIdsNotDeleted = dbAttributeIdsAfterDeletion.Except(metadataAttributeIds).ToList();
                if (attributeIdsNotDeleted.Any())
                {
                    throw new Exception("Failed to delete attributes we don't want");
                }
            }


            var channel = Channel.CreateUnbounded<AggregationStatusMemo>();
            var state = new AggregationState();
            var timer = KeepUserInformedOfState(state);
            var readTask = Task.Run(() => ReadMessages(channel.Reader));
            try
            {
                return Ok();
            }
            finally
            {
                Response.OnCompleted(async () =>
                {
                    try
                    {
                        var specificAttributes = AttributeService.ConvertAllAttributeList(attributes);
                        var result = await _aggregationService.AggregateNetworkData(channel.Writer, networkId, state, specificAttributes);
                    }
                    catch (Exception e)
                    {
                        state.Status = "Aggregation failed";
                        UnitOfWork.NetworkRepo.UpsertNetworkRollupDetail(networkId, state.Status);
                        HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastAssignDataStatus,
                            new NetworkRollupDetailDTO { NetworkId = networkId, Status = state.Status }, 0.0);
                        HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{AggregationError}::AggregateNetworkData - { e.Message }");
                        throw;
                    }
                    finally
                    {
                        NotifyUserOfState(state);
                        timer.Stop();
                        timer.Close();
                        channel.Writer.Complete();
                        //readTask.Dispose();
                    }
                });
            }
        }

        private async Task ReadMessages(ChannelReader<AggregationStatusMemo> reader)
        {
            while (await reader.WaitToReadAsync())
            {
                while (reader.TryRead(out AggregationStatusMemo status))
                {
                    var error = status.ErrorMessage;
                    if(error != null)
                    {
                        DoublyBroadcastError(error);
                    }
                }
            }
        }

        private void NotifyUserOfState(AggregationState state)
        {
            try
            {
                SendCurrentStatusMessage(state);
            } catch
            {
                // ingore the error
            }
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
            var count = state.Count % 10;
            var periods = StringWithPeriods(count + 1);
            var message = $"{state.Status}{periods}";

            HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastAssignDataStatus,
                new NetworkRollupDetailDTO { NetworkId = state.NetworkId, Status = message }, state.Percentage);
            state.Count++;
        }

        private static string StringWithPeriods(int count)
        {
            var builder = new StringBuilder();
            for (int i=0; i<count; i++)
            {
                builder.Append('.');
            }
            return builder.ToString();
        }
    }
}
