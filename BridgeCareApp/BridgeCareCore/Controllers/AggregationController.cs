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

                // get attribute meta data from json file
                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? string.Empty,
                    "MetaData//AttributeMetaData", "attributeMetaData.json");
                var attributeMetaData = _attributeMetaDataRepo.All(filePath);

                // Check to see if the GUIDs in the meta data repo are blank. A blank GUID requires
                // that the attribute has never been assigned in a network previously.
                if (attributeMetaData.Any(_ => string.IsNullOrEmpty(_.Id.ToString())))
                {
                    // give new meta data a guid
                    attributeMetaData = attributeMetaData.Select(_ =>
                    {
                        if (string.IsNullOrEmpty(_.Id.ToString()))
                        {
                            _.Id = Guid.NewGuid();
                        }

                        return _;
                    }).ToList();

                    // update the meta data file
                    _attributeMetaDataRepo.UpdateAll(filePath, attributeMetaData);
                }

                // get attribute dictionary containing all attributes created from the meta data
                var attributeDictionary = _attributeRepo.GetAttributeDictionary(filePath);

                // create database records for any missing attributes
                _attributeRepo.CreateMissingAttributes(attributeDictionary.Values.ToList());

                // get the network attributes
                var networkAttributes = _attributeRepo.GetAttributesFromNetwork(networkId).ToList();

                // delete any assigned data where a network attribute has a matching attribute from the meta data file
                _attributeDatumRepo.DeleteAssignedDataFromNetwork(networkId, attributeDictionary.Keys.ToList(),
                    networkAttributes.Select(_ => _.Id).ToList());

                // If a GUID is present in the attribute meta data repo then we need to see if we
                // can match it with an attribute in the current network if we cannot match it, the
                // attribute is new to the network and we proceed normally.

                // If a GUID is present for an attribute from the network, but NOT in the meta data
                // repository, then the attribute is simply skipped during data assignment and
                // aggregation. The existing data for that attribute must be PRESERVED in the
                // network so it can be utilized during analysis

                // do not skip attributes that didn't have an id
                var metaDataAttributes = attributeDictionary.Values.
                    Where(_ => networkAttributes.Select(__ => __.Id).Contains(_.Id))
                    .ToList();

                var databaseAttributes = networkAttributes
                    .Where(_ => attributeDictionary.Keys.Contains(_.Id))
                    .ToList();

                var attributes = metaDataAttributes.Concat(databaseAttributes).DistinctBy(_ => _.Id);

                var attributeData = new List<IAttributeDatum>();
                foreach (var attribute in attributes)
                {
                    attributeData.AddRange(AttributeDataBuilder.GetData(AttributeConnectionBuilder.Build(attribute)));
                }

                var updatedRecordsCount = 0;
                if (attributeData.Any())
                {
                    var network = _networkRepo.GetNetworkWithAssetsAndLocations(networkId);
                    foreach (var maintainableAsset in network.MaintainableAssets)
                    {
                        maintainableAsset.AssignAttributeData(attributeData);
                    }
                    
                    updatedRecordsCount = _attributeDatumRepo.UpdateAssignedData(network);
                }

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

                // get the attribute dictionary
                var attributeDictionary = _attributeRepo.GetAttributeDictionary(filePath);

                // get the network attributes
                var networkAttributes = _attributeRepo.GetAttributesFromNetwork(networkId).ToList();

                // delete aggregated data with an attribute id in the database that has a matching attribute id in the meta data file
                var numberDeletedEntries = _aggregatedResultRepo.DeleteAggregatedResults(networkId,
                    attributeDictionary.Keys.ToList(), networkAttributes.Select(_ => _.Id).ToList());

                // 
                var metaDataAttributes = attributeDictionary.Values.
                    Where(_ => networkAttributes.Select(__ => __.Id).Contains(_.Id))
                    .ToList();

                var databaseAttributes = networkAttributes
                    .Where(_ => attributeDictionary.Keys.Contains(_.Id))
                    .ToList();

                var attributeIds = metaDataAttributes.Concat(databaseAttributes)
                    .DistinctBy(_ => _.Id)
                    .Select(_ => _.Id)
                    .ToList();

                // get all maintainable assets in the network
                var maintainableAssets = _maintainableAssetRepo.GetAllInNetworkWithAssignedData(networkId);

                var aggregatedNumericResults = new List<AggregatedResult<double>>();
                var aggregatedTextResults = new List<AggregatedResult<string>>();
                foreach (var maintainableAsset in maintainableAssets)
                {
                    // aggregate numeric data
                    if (maintainableAsset.AssignedData.Any(_ => _.Attribute.DataType == "NUMERIC" && attributeIds.Contains(_.Attribute.Id)))
                    {
                        aggregatedNumericResults.AddRange(maintainableAsset.AssignedData
                            .Where(_ => _.Attribute.DataType == "NUMERIC" && attributeIds.Contains(_.Attribute.Id))
                            .Select(_ => _.Attribute)
                            .Select(_ => maintainableAsset.GetAggregatedValuesByYear(_, AggregationRuleFactory.CreateNumericRule(_))));
                    }

                    //aggregate text data
                    if (maintainableAsset.AssignedData.Any(_ => _.Attribute.DataType == "TEXT"))
                    {
                        aggregatedTextResults.AddRange(maintainableAsset.AssignedData
                            .Where(_ => _.Attribute.DataType == "TEXT" && attributeIds.Contains(_.Attribute.Id))
                            .Select(_ => _.Attribute)
                            .Select(_ => maintainableAsset.GetAggregatedValuesByYear(_, AggregationRuleFactory.CreateTextRule(_))));
                    }
                }

                // create aggregated data records in the data source
                var numberNumericResults = _aggregatedResultRepo.CreateAggregatedResults(aggregatedNumericResults);
                var numberTextResults = _aggregatedResultRepo.CreateAggregatedResults(aggregatedTextResults);

                return Ok($"{numberDeletedEntries} removed from database. {numberNumericResults + numberTextResults} added.");
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
