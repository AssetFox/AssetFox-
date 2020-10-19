using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;
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
        private readonly INetworkRepository NetworkRepository;
        private readonly IRepository<AttributeMetaDatum> AttributeMetaDataRepository;
        private readonly IMaintainableAssetRepository MaintainableAssetRepository;
        private readonly IAttributeDatumRepository AttributeDatumRepository;
        private readonly IAggregatedResultRepository AggregatedResultRepository;
        private readonly ISaveChanges SaveChangesRepository;

        private readonly ILogger<NetworkController> Logger;

        public AggregationController(
            INetworkRepository networkRepository,
            IRepository<AttributeMetaDatum> attributeMetaDataRepository,
            IMaintainableAssetRepository maintainableAssetRepository,
            IAttributeDatumRepository attributeDatumRepository,
            IAggregatedResultRepository aggregatedResultRepository,
            ISaveChanges saveChangesRepository,
            ILogger<NetworkController> logger)
        {
            NetworkRepository = networkRepository ?? throw new ArgumentNullException(nameof(networkRepository));
            AttributeMetaDataRepository = attributeMetaDataRepository ?? throw new ArgumentNullException(nameof(attributeMetaDataRepository));
            MaintainableAssetRepository = maintainableAssetRepository ?? throw new ArgumentNullException(nameof(maintainableAssetRepository));
            AttributeDatumRepository = attributeDatumRepository ?? throw new ArgumentNullException(nameof(attributeDatumRepository));
            AggregatedResultRepository = aggregatedResultRepository ?? throw new ArgumentNullException(nameof(aggregatedResultRepository));
            SaveChangesRepository = saveChangesRepository ?? throw new ArgumentNullException(nameof(saveChangesRepository));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        [Route("AssignNetworkData")]
        public async Task<IActionResult> AssignNetworkData([FromBody] Guid networkId)
        {
            try
            {
                var network = NetworkRepository.Get(networkId);
                var attributeData = new List<IAttributeDatum>();
                var numberUpdatedRecords = 0;

                var attributes = new List<Attribute>();
                var attributeMetaData = AttributeMetaDataRepository.All();

                // Check to see if the GUIDs in the meta data repo are blank. A blank GUID requires
                // that the attribute has never been assigned in a network previously.

                // If a GUID is present in the attribute meta data repo then we need to see if we
                // can match it with an attribute in the current network if we cannot match it, the
                // attribute is new to the network and we proceed normally.

                // If a GUID is present for an attribute from the network, but NOT in the meta data
                // repository, then the attribute is simply skipped during data assignment and
                // aggregation. The existing data for that attribute must be PRESERVED in the
                // network so it can be utilized during analysis
                var networkAttributeMetaData = AttributeDatumRepository.GetAttributesFromNetwork(networkId).ToList();
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

                foreach (var attribute in attributes)
                {
                    attributeData.AddRange(AttributeDataBuilder.GetData(AttributeConnectionBuilder.Build(attribute)));
                }

                if (attributeData.Any())
                {
                    foreach (var maintainableAsset in network.MaintainableAssets)
                    {
                        maintainableAsset.AssignAttributeData(attributeData);
                    }
                    numberUpdatedRecords = AttributeDatumRepository.UpdateAssignedData(network);
                }

                SaveChangesRepository.SaveChanges();
                Logger.LogInformation("Attribute data have been assigned to maintenance assets.");
                return Ok($"Updated {numberUpdatedRecords} records.");
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        [HttpPost]
        [Route("AggregateNetworkData")]
        public async Task<IActionResult> AggregateNetworkData([FromBody] Guid networkId)
        {
            var maintainableAssets = MaintainableAssetRepository.GetAllInNetwork(networkId);
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
            var numberDeletedEntries = AggregatedResultRepository.DeleteAggregatedResults(networkId);
            var numberNumericResults = AggregatedResultRepository.AddAggregatedResults(aggregatedNumericResults);
            var numberTextResults = AggregatedResultRepository.AddAggregatedResults(aggregatedTextResults);

            return Ok($"{numberDeletedEntries} removed from database. {numberNumericResults + numberTextResults} added.");
        }

        [HttpGet]
        [Route("GetAggregatedNetworkData")]
        public async Task<IActionResult> GetAggregatedNetworkData([FromHeader] Guid networkId)
        {
            var results = AggregatedResultRepository.GetAggregatedResults(networkId);
            return Ok($"Retrieved {results.Count()} results successfully.");
        }
    }
}
