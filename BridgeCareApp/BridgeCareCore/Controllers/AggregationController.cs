using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;
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
                // Get configurable attribute meta data.
                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? string.Empty,
                    "MetaData//AttributeMetaData", "attributeMetaData.json");
                var attributeMetaData = _attributeMetaDataRepo.All(filePath);

                // Create attributes from the meta data.
                var configurationAttributes = attributeMetaData.Select(_ => AttributeFactory.Create(_));

                // Create list of attributes we are allowed to update with assigned data.
                var networkAttributeIds = _attributeRepo.GetAttributesFromNetwork(networkId).Select(_ => _.Id);

                var maintainableAssets = _maintainableAssetRepo.GetAllInNetworkWithAssignedData(networkId);

                var attributeData = new List<IAttributeDatum>();
                foreach (var attribute in configurationAttributes)
                {
                    attributeData.AddRange(AttributeDataBuilder.GetData(AttributeConnectionBuilder.Build(attribute)));
                }

                foreach (var maintainableAsset in maintainableAssets)
                {
                    maintainableAsset.AssignAttributeData(attributeData);
                }

                // Attribute Ids for clearing assigned data
                var attributeIdsToBeUpdatedWithAssignedData = configurationAttributes.Select(_ => _.Id).Union(networkAttributeIds).Distinct();

                var updatedRecordsCount = _attributeDatumRepo.UpdateAssignedDataByAttributeId(networkId, attributeIdsToBeUpdatedWithAssignedData, maintainableAssets);

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
                // get attribute meta data from json file
                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? string.Empty,
                    "MetaData//AttributeMetaData", "attributeMetaData.json");

                // get the network attributes
                // var networkAttributes = _attributeRepo.GetAttributesFromNetwork(networkId).ToList();

                // delete aggregated data with an attribute id in the database that has a matching attribute id in the meta data file
                //var numberDeletedEntries = _aggregatedResultRepo.DeleteAggregatedResults(networkId,
                //    attributeDictionary.Keys.ToList(), networkAttributes.Select(_ => _.Id).ToList());

                // 
                //var metaDataAttributes = attributeDictionary.Values.
                //    Where(_ => networkAttributes.Select(__ => __.Id).Contains(_.Id))
                //    .ToList();

                //var databaseAttributes = networkAttributes
                //    .Where(_ => attributeDictionary.Keys.Contains(_.Id))
                //    .ToList();

                //var attributeIds = metaDataAttributes.Concat(databaseAttributes)
                //    .DistinctBy(_ => _.Id)
                //    .Select(_ => _.Id)
                //    .ToList();

                // get all maintainable assets in the network
                var maintainableAssets = _maintainableAssetRepo.GetAllInNetworkWithAssignedData(networkId);

                var aggregatedNumericResults = new List<AggregatedResult<double>>();
                var aggregatedTextResults = new List<AggregatedResult<string>>();
                foreach (var maintainableAsset in maintainableAssets)
                {
                    //// aggregate numeric data
                    //if (maintainableAsset.AssignedData.Any(_ => _.Attribute.DataType == "NUMERIC" && attributeIds.Contains(_.Attribute.Id)))
                    //{
                    //    aggregatedNumericResults.AddRange(maintainableAsset.AssignedData
                    //        .Where(_ => _.Attribute.DataType == "NUMERIC" && attributeIds.Contains(_.Attribute.Id))
                    //        .Select(_ => _.Attribute)
                    //        .Select(_ => maintainableAsset.GetAggregatedValuesByYear(_, AggregationRuleFactory.CreateNumericRule(_))));
                    //}

                    ////aggregate text data
                    //if (maintainableAsset.AssignedData.Any(_ => _.Attribute.DataType == "TEXT"))
                    //{
                    //    aggregatedTextResults.AddRange(maintainableAsset.AssignedData
                    //        .Where(_ => _.Attribute.DataType == "TEXT" && attributeIds.Contains(_.Attribute.Id))
                    //        .Select(_ => _.Attribute)
                    //        .Select(_ => maintainableAsset.GetAggregatedValuesByYear(_, AggregationRuleFactory.CreateTextRule(_))));
                    //}
                }

                // create aggregated data records in the data source
                var numberNumericResults = _aggregatedResultRepo.CreateAggregatedResults(aggregatedNumericResults);
                var numberTextResults = _aggregatedResultRepo.CreateAggregatedResults(aggregatedTextResults);

                //return Ok($"{numberDeletedEntries} removed from database. {numberNumericResults + numberTextResults} added.");
                return Ok();
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
