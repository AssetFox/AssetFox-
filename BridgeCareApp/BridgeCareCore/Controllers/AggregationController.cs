using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MoreLinq.Extensions;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AggregationController : ControllerBase
    {
        private readonly INetworkRepository _networkRepo;
        private readonly IAttributeMetaDataRepository _attributeMetaDataRepo;
        private readonly IMaintainableAssetRepository _maintainableAssetRepo;
        private readonly IAttributeDatumRepository _attributeDatumRepo;
        private readonly IAggregatedResultRepository _aggregatedResultRepo;
        private readonly IAttributeRepository _attributeRepo;
        private readonly ILogger<NetworkController> _logger;

        public AggregationController(
            INetworkRepository networkRepo,
            IAttributeMetaDataRepository attributeMetaDataRepo,
            IMaintainableAssetRepository maintainableAssetRepo,
            IAttributeDatumRepository attributeDatumRepo,
            IAggregatedResultRepository aggregatedResultRepo,
            IAttributeRepository attributeRepo,
            ILogger<NetworkController> logger)
        {
            _networkRepo = networkRepo ?? throw new ArgumentNullException(nameof(networkRepo));
            _attributeMetaDataRepo = attributeMetaDataRepo ?? throw new ArgumentNullException(nameof(attributeMetaDataRepo));
            _maintainableAssetRepo = maintainableAssetRepo ?? throw new ArgumentNullException(nameof(maintainableAssetRepo));
            _attributeDatumRepo = attributeDatumRepo ?? throw new ArgumentNullException(nameof(attributeDatumRepo));
            _aggregatedResultRepo = aggregatedResultRepo ?? throw new ArgumentNullException(nameof(aggregatedResultRepo));
            _attributeRepo = attributeRepo ?? throw new ArgumentNullException(nameof(attributeRepo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        [Route("AssignNetworkData")]
        public IActionResult AssignNetworkData([FromBody] Guid networkId)
        {
            try
            {
                // Get/create configurable attributes
                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? string.Empty,
                    "MetaData//AttributeMetaData", "AttributemetaData.json");
                var configurationAttributes = _attributeMetaDataRepo.GetAllAttributes(filePath).ToList();

                // get all maintainable assets in the network with their assigned data (if any) and locations
                var maintainableAssets = _maintainableAssetRepo.GetAllInNetworkWithAssignedDataAndLocations(networkId)
                    .ToList();

                // Create list of attribute ids we are allowed to update with assigned data.
                var networkAttributeIds = maintainableAssets.Where(_ => _.AssignedData != null && _.AssignedData.Any())
                    .SelectMany(_ => _.AssignedData.Select(__ => __.Attribute.Id).Distinct()).ToList();

                // create list of attribute data from configuration attributes
                var attributeData = configurationAttributes.Select(AttributeConnectionBuilder.Build)
                    .SelectMany(AttributeDataBuilder.GetData).ToList();

                // get the attribute ids for assigned data that can be deleted (attribute is present in the data source and meta data file)
                var attributeIdsToBeUpdatedWithAssignedData = configurationAttributes.Select(_ => _.Id)
                    .Intersect(networkAttributeIds).Distinct().ToList();

                // loop over maintainable assets and remove assigned data that has an attribute id in attributeIdsToBeUpdatedWithAssignedData
                // then assign the new attribute data that was created
                foreach (var maintainableAsset in maintainableAssets)
                {
                    maintainableAsset.AssignedData.RemoveAll(_ => attributeIdsToBeUpdatedWithAssignedData.Contains(_.Attribute.Id));
                    maintainableAsset.AssignAttributeData(attributeData);
                }

                // update the maintainable assets assigned data in the data source
                var updatedRecordsCount = _attributeDatumRepo.UpdateMaintainableAssetsAssignedData(maintainableAssets);

                _logger.LogInformation("Attribute data have been assigned to maintenance assets.");
                return Ok($"Updated {updatedRecordsCount} records.");
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        [HttpPost]
        [Route("AggregateNetworkData")]
        public IActionResult AggregateNetworkData([FromBody] Guid networkId)
        {
            try
            {
                // get all maintainable assets in the network with assigned data and locations
                var maintainableAssets = _maintainableAssetRepo.GetAllInNetworkWithAssignedDataAndLocations(networkId).ToList();

                var aggregatedResults = new List<IAggregatedResult>();

                // loop over the maintainable assets and aggregate the assigned data as numeric or text based on assigned data attribute data type
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
                }

                // create aggregated data records in the data source
                var createdRecordsCount = _aggregatedResultRepo.CreateAggregatedResults(aggregatedResults);

                return Ok($"{createdRecordsCount} aggregated result records added.");
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        [HttpGet]
        [Route("GetAggregatedNetworkData")]
        public async Task<IActionResult> GetAggregatedNetworkData([FromHeader] Guid networkId)
        {
            var results = _aggregatedResultRepo.GetAggregatedResults(networkId);
            return Ok($"Retrieved {results.Count()} results successfully.");
        }
    }
}
