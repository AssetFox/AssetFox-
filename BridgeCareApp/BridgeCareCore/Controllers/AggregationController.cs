﻿using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;
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
        private readonly IMaintainableAssetRepository _maintainableAssetRepo;
        private readonly IAggregatedResultRepository _aggregatedResultRepo;
        private readonly IHubContext<BridgeCareHub> HubContext;

        public AggregationController(IMaintainableAssetRepository maintainableAssetRepo, IAggregatedResultRepository aggregatedResultRepo, IHubContext<BridgeCareHub> hub)
        {
            _maintainableAssetRepo = maintainableAssetRepo ?? throw new ArgumentNullException(nameof(maintainableAssetRepo));
            _aggregatedResultRepo = aggregatedResultRepo ?? throw new ArgumentNullException(nameof(aggregatedResultRepo));
            HubContext = hub;
        }

        [HttpPost]
        [Route("AggregateNetworkData/{networkId}")]
        public async Task<IActionResult> AggregateNetworkData([FromBody] Guid networkId)
        {
            var broadcastingMessage = "Starting data assignment";
            var percentage = 0.0;
            try
            {
                // get all maintainable assets in the network with assigned data and locations
                var maintainableAssets = _maintainableAssetRepo.GetAllInNetworkWithAssignedDataAndLocations(networkId).ToList();
                await HubContext
                    .Clients
                    .All
                    .SendAsync("BroadcastAssignDataStatus", broadcastingMessage, percentage);


                var aggregatedResults = new List<IAggregatedResult>();

                // loop over the maintainable assets and aggregate the assigned data as numeric or
                // text based on assigned data attribute data type
                foreach (var maintainableAsset in maintainableAssets)
                {
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
                    broadcastingMessage = $"Finished assigning attribute data. Saving it to the datasource...";
                    await HubContext
                            .Clients
                            .All
                            .SendAsync("BroadcastAssignDataStatus", broadcastingMessage, percentage);
                }

                // create aggregated data records in the data source
                var createdRecordsCount = _aggregatedResultRepo.CreateAggregatedResults(aggregatedResults);

                return Ok($"{createdRecordsCount} aggregated result records added.");
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

        [HttpGet]
        [Route("GetAggregatedNetworkData")]
        public IActionResult GetAggregatedNetworkData([FromHeader] Guid networkId)
        {
            var results = _aggregatedResultRepo.GetAggregatedResults(networkId);
            return Ok($"Retrieved {results.Count()} results successfully.");
        }
    }
}
