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
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

                // get attribute meta data from json file
                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? string.Empty,
                    "MetaData//AttributeMetaData", "attributeMetaData.json");
                var attributeMetaData = _attributeMetaDataRepo.All(filePath);

                var network = _networkRepo.GetNetworkWithAssetsAndLocations(networkId);
                var numberUpdatedRecords = 0;

                var newAttributes = new List<Attribute>();
                var attributes = new List<Attribute>();
                
                // Check to see if the GUIDs in the meta data repo are blank. A blank GUID requires
                // that the attribute has never been assigned in a network previously.
                if (attributeMetaData.Any(_ => string.IsNullOrEmpty(_.Id.ToString())))
                {
                    //newAttributes.AddRange(attributeMetaData.Where(_ => string.IsNullOrEmpty(_.Id.ToString())));
                    attributeMetaData = attributeMetaData.Where(_ => !string.IsNullOrEmpty(_.Id.ToString())).ToList();
                }

                // If a GUID is present in the attribute meta data repo then we need to see if we
                // can match it with an attribute in the current network if we cannot match it, the
                // attribute is new to the network and we proceed normally.

                // If a GUID is present for an attribute from the network, but NOT in the meta data
                // repository, then the attribute is simply skipped during data assignment and
                // aggregation. The existing data for that attribute must be PRESERVED in the
                // network so it can be utilized during analysis
                var networkAttributeMetaData = _attributeRepo.GetAttributesFromNetwork(networkId).ToList();
                foreach (var attributeMetaDatum in attributeMetaData)
                {
                    if (attributeMetaDatum.Id.ToString() == "")
                    {
                        // No ID for this attribute found. Create a new attribute with a new GUID.
                        attributeMetaDatum.Id = Guid.NewGuid();
                    }
                    // Persist any changes back out to the attribute meta data repository.
                    //AttributeMetaDataRepository.UpdateAll(attributeMetaData);
                    attributes.Add(AttributeFactory.Create(attributeMetaDatum));
                }

                var attributeData = new List<IAttributeDatum>();
                foreach (var attribute in attributes)
                {
                    attributeData.AddRange(AttributeDataBuilder.GetData(AttributeConnectionBuilder.Build(attribute)));
                }

                if (attributeData.Any())
                {
                    _attributeDatumRepo.DeleteAssignedDataFromNetwork(network.Id);

                    foreach (var maintainableAsset in network.MaintainableAssets)
                    {
                        maintainableAsset.AssignAttributeData(attributeData);
                    }
                    
                    numberUpdatedRecords = _attributeDatumRepo.UpdateAssignedData(network);
                }

                _logger.LogInformation("Attribute data have been assigned to maintenance assets.");
                return Ok($"Updated {numberUpdatedRecords} records.");
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
            var maintainableAssets = _maintainableAssetRepo.GetAllInNetwork(networkId);
            var aggregatedNumericResults = new List<AggregatedResult<double>>();
            var aggregatedTextResults = new List<AggregatedResult<string>>();
            foreach (var maintainableAsset in maintainableAssets)
            {
                if (maintainableAsset.AssignedData.Any(a => a.Attribute.DataType == "NUMERIC"))
                {
                    aggregatedNumericResults.AddRange(maintainableAsset.AssignedData
                        .Where(a => a.Attribute.DataType == "NUMERIC")
                        .Select(a => a.Attribute)
                        .Select(a => maintainableAsset.GetAggregatedValuesByYear(a, AggregationRuleFactory.CreateNumericRule(a))));
                }

                if (maintainableAsset.AssignedData.Any(a => a.Attribute.DataType == "TEXT"))
                {
                    aggregatedTextResults.AddRange(maintainableAsset.AssignedData
                        .Where(a => a.Attribute.DataType == "TEXT")
                        .Select(a => a.Attribute)
                        .Select(a => maintainableAsset.GetAggregatedValuesByYear(a, AggregationRuleFactory.CreateTextRule(a))));
                }
            }
            var numberDeletedEntries = _aggregatedResultRepo.DeleteAggregatedResults(networkId);
            var numberNumericResults = _aggregatedResultRepo.CreateAggregatedResults(aggregatedNumericResults);
            var numberTextResults = _aggregatedResultRepo.CreateAggregatedResults(aggregatedTextResults);

            return Ok($"{numberDeletedEntries} removed from database. {numberNumericResults + numberTextResults} added.");
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
