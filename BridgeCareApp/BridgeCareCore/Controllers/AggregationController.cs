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
                foreach (var attributeMetaDatum in attributeMetaData)
                {
                    // Note that attributes have NEW guids assigned as part of this process. All
                    // attributes referenced later in the application will NOT have these Guids.
                    // This means each network data assignment creates a UNIQUE network. This is NOT
                    // the desired behavior. I think we'll need to save the GUIDs out to the JSON
                    // file and then check back against those GUIDS.
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
