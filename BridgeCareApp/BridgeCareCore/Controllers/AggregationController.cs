using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
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

                Debug.WriteLine($"Aggregation started at {DateTime.Now}");
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastWorkQueueUpdate, networkId.ToString());

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name,  $"{AggregationError}::NetworkAggregateAccess - {HubService.errorList["Unauthorized"]}", e);
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{AggregationError}::NetworkAggregateAccess - {e.Message}", e);
                return Ok();
            }
        }

    }
}
