﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using BridgeCareCore.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AggregationController : ControllerBase
    {
        private readonly IAttributeMetaDataRepository _attributeMetaDataRepo;
        private readonly IMaintainableAssetRepository _maintainableAssetRepo;
        private readonly IAggregatedResultRepository _aggregatedResultRepo;
        private readonly IAttributeDatumRepository _attributeDatumRepo;
        private readonly ILogger<NetworkController> _logger;
        private readonly IHubContext<BridgeCareHub> HubContext;

        public AggregationController(IAttributeMetaDataRepository attributeMetaDataRepo,
            IMaintainableAssetRepository maintainableAssetRepo,
            IAttributeDatumRepository attributeDatumRepo,
            ILogger<NetworkController> logger, IHubContext<BridgeCareHub> hub,
            IAggregatedResultRepository aggregatedResultRepo)
        {
            _attributeMetaDataRepo = attributeMetaDataRepo ?? throw new ArgumentNullException(nameof(attributeMetaDataRepo));
            _maintainableAssetRepo = maintainableAssetRepo ?? throw new ArgumentNullException(nameof(maintainableAssetRepo));
            _attributeDatumRepo = attributeDatumRepo ?? throw new ArgumentNullException(nameof(attributeDatumRepo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            HubContext = hub ?? throw new ArgumentNullException(nameof(hub));
            _aggregatedResultRepo = aggregatedResultRepo ?? throw new ArgumentNullException(nameof(aggregatedResultRepo));
        }

        [HttpPost]
        [Route("AggregateNetworkData/{networkId}")]
        public async Task<IActionResult> AggregateNetworkData(Guid networkId)
        {
            var broadcastingMessage = "Starting data assignment";
            var percentage = 0.0;
            try
            {
                await HubContext
                    .Clients
                    .All
                    .SendAsync("BroadcastAssignDataStatus", broadcastingMessage, percentage);

                // Get/create configurable attributes
                var configurationAttributes = _attributeMetaDataRepo.GetAllAttributes().ToList();

                // get all maintainable assets in the network with their assigned data (if any) and locations
                var maintainableAssets = _maintainableAssetRepo.GetAllInNetworkWithAssignedDataAndLocations(networkId)
                    .ToList();

                // Create list of attribute ids we are allowed to update with assigned data.
                var networkAttributeIds = maintainableAssets.Where(_ => _.AssignedData != null && _.AssignedData.Any())
                    .SelectMany(_ => _.AssignedData.Select(__ => __.Attribute.Id).Distinct()).ToList();

                // create list of attribute data from configuration attributes (exclude attributes that don't have command text as there
                // will be no way to select data for them from the data source)
                var attributeData = configurationAttributes.Where(_ => !string.IsNullOrEmpty(_.Command))
                    .Select(AttributeConnectionBuilder.Build)
                    .SelectMany(AttributeDataBuilder.GetData).ToList();

                // get the attribute ids for assigned data that can be deleted (attribute is present
                // in the data source and meta data file)
                var attributeIdsToBeUpdatedWithAssignedData = configurationAttributes.Select(_ => _.Id)
                    .Intersect(networkAttributeIds).Distinct().ToList();

                var totalAssests = (double)maintainableAssets.Count;
                var i = 0.0;
                // loop over maintainable assets and remove assigned data that has an attribute id
                // in attributeIdsToBeUpdatedWithAssignedData then assign the new attribute data
                // that was created
                foreach (var maintainableAsset in maintainableAssets)
                {
                    if (i % 500 == 0)
                    {
                        broadcastingMessage = $"Assigning attribute data";
                        percentage = Math.Round((i / totalAssests) * 100, 1);
                        _ = HubContext
                       .Clients
                       .All
                       .SendAsync("BroadcastAssignDataStatus", broadcastingMessage, percentage);
                    }
                    i++;
                    maintainableAsset.AssignedData.RemoveAll(_ => attributeIdsToBeUpdatedWithAssignedData.Contains(_.Attribute.Id));
                    maintainableAsset.AssignAttributeData(attributeData);
                }

                broadcastingMessage = $"Finished assigning attribute data. Saving it to the datasource...";
                await HubContext
                        .Clients
                        .All
                        .SendAsync("BroadcastAssignDataStatus", broadcastingMessage, percentage);
                // update the maintainable assets assigned data in the data source
                var updatedRecordsCount = _attributeDatumRepo.UpdateAssignedData(maintainableAssets);

                AggregateData(networkId, maintainableAssets);

                _logger.LogInformation("Attribute data have been aggregated to maintenance assets.");
                return Ok($"Updated {updatedRecordsCount} records.");
            }
            catch (Exception e)
            {
                broadcastingMessage = "An error has occured while assigning data";
                await HubContext
                            .Clients
                            .All
                            .SendAsync("BroadcastAssignDataStatus", broadcastingMessage, percentage);
                return StatusCode(500, e);
            }
        }

        private void AggregateData(Guid networkId, List<MaintainableAsset> maintainableAssets)
        {
            var broadcastingMessage = "Starting data aggregation";
            var percentage = 0.0;
            try
            {
                 HubContext
                    .Clients
                    .All
                    .SendAsync("BroadcastAssignDataStatus", broadcastingMessage, percentage);


                var aggregatedResults = new List<IAggregatedResult>();

                var totalAssests = (double)maintainableAssets.Count;
                var i = 0.0;
                // loop over the maintainable assets and aggregate the assigned data as numeric or
                // text based on assigned data attribute data type
                foreach (var maintainableAsset in maintainableAssets)
                {
                    if (i % 500 == 0)
                    {
                        broadcastingMessage = $"Aggregating data";
                        percentage = Math.Round((i / totalAssests) * 100, 1);
                        _ = HubContext
                       .Clients
                       .All
                       .SendAsync("BroadcastAssignDataStatus", broadcastingMessage, percentage);
                    }
                    i++;
                    // aggregate numeric data
                    if (maintainableAsset.AssignedData.Any(_ => _.Attribute.DataType == "NUMERIC"))
                    {
                        aggregatedResults.AddRange(maintainableAsset.AssignedData
                            .Where(_ => _.Attribute.DataType == "NUMERIC")
                            .Select(_ => _.Attribute)
                            .Select(_ => maintainableAsset.GetAggregatedValuesByYear(_, AggregationRuleFactory.CreateNumericRule(_)))
                            .ToList());
                    }

                    //aggregate text data
                    if (maintainableAsset.AssignedData.Any(_ => _.Attribute.DataType == "TEXT"))
                    {
                        aggregatedResults.AddRange(maintainableAsset.AssignedData
                            .Where(_ => _.Attribute.DataType == "TEXT")
                            .Select(_ => _.Attribute)
                            .Select(_ => maintainableAsset.GetAggregatedValuesByYear(_, AggregationRuleFactory.CreateTextRule(_)))
                            .ToList());
                    }
                }

                broadcastingMessage = $"Finished aggregating attribute data. Saving it to the datasource...";
                 HubContext
                        .Clients
                        .All
                        .SendAsync("BroadcastAssignDataStatus", broadcastingMessage, percentage);
                // create aggregated data records in the data source
                var createdRecordsCount = _aggregatedResultRepo.CreateAggregatedResults(aggregatedResults);

                broadcastingMessage = $"Successfully aggregated the data";
                HubContext
                       .Clients
                       .All
                       .SendAsync("BroadcastAssignDataStatus", broadcastingMessage, percentage);
            }
            catch (Exception e)
            {
                broadcastingMessage = "An error has occured while aggregating data";
                 HubContext
                            .Clients
                            .All
                            .SendAsync("BroadcastAssignDataStatus", broadcastingMessage, percentage);
            }
        }
    }
}