using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Common;
using AppliedResearchAssociates.iAM.Data;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Controllers.BaseController;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
using BridgeCareCore.Services;
using BridgeCareCore.Services.Aggregation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        private readonly IGeneralWorkQueueService _generalWorkQueueService;


        public AggregationController(ILog log, IAggregationService aggregationService,
            IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork,
            IHubService hubService, IHttpContextAccessor httpContextAccessor, IGeneralWorkQueueService generalWorkQueueService) :
            base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _aggregationService = aggregationService ?? throw new ArgumentNullException(nameof(aggregationService));
            _generalWorkQueueService = generalWorkQueueService ?? throw new ArgumentNullException(nameof(generalWorkQueueService));
        }

        private static bool FalseButCompilerDoesNotKnowThat => Guid.NewGuid() == Guid.Empty;


        [HttpPost]
        [Route("AggregateNetworkData/{networkId}")]
        [ClaimAuthorize("NetworkAggregateAccess")]
        public async Task<IActionResult> AggregateNetworkData(Guid networkId, List<AllAttributeDTO> attributes)
        {
            try
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
                var networkName = "";
                var specificAttributes = new List<AttributeDTO>();
                await Task.Factory.StartNew(() =>
                {
                    specificAttributes = AttributeService.ConvertAllAttributeList(attributes);
                    networkName = UnitOfWork.NetworkRepo.GetNetworkName(networkId);
                });
                AggregationWorkitem workItem = new AggregationWorkitem(networkId, UserInfo.Name, networkName, specificAttributes);
                var analysisHandle = _generalWorkQueueService.CreateAndRun(workItem);
                // Before sending a "queued" message that may overwrite early messages from the run,
                // allow a brief moment for an empty queue to start running the submission.
                await Task.Delay(500);
                if (!analysisHandle.WorkHasStarted)
                {
                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastWorkQueueStatusUpdate, new QueuedWorkStatusUpdateModel() {Id = networkId, Status = analysisHandle.MostRecentStatusMessage });
                }

                //await analysisHandle.WorkCompletion;
                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{AggregationError}::NetworkAggregateAccess - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{AggregationError}::NetworkAggregateAccess - {e.Message}");
                throw;
            }
        }

        [HttpDelete]
        [Route("CancelNetworkAggregation/{networkId}")]
        [Authorize]
        public async Task<IActionResult> CancelNetworkAggregation(Guid networkId)
        {
            try
            {
                var hasBeenRemovedFromQueue = _generalWorkQueueService.Cancel(networkId);
                await Task.Delay(125);

                if (hasBeenRemovedFromQueue)
                {
                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastWorkQueueStatusUpdate, new QueuedWorkStatusUpdateModel() { Id = networkId, Status = "Canceled" });
                }
                else
                {
                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastWorkQueueStatusUpdate, new QueuedWorkStatusUpdateModel() { Id = networkId, Status = "Canceling network deletion..." });

                }
                return Ok();
            }
            catch (Exception e)
            {
                var networkName = UnitOfWork.NetworkRepo.GetNetworkName(networkId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Error canceling network deltion for {networkName}::{e.Message}");
                throw;
            }
        }

    }
}
