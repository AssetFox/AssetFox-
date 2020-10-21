using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;
using AppliedResearchAssociates.iAM.DataAssignment.Segmentation;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using BridgeCareCore.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using DataMinerAttribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AggregationController : ControllerBase
    {
        private readonly IRepository<Network> NetworkRepo;
        private readonly IRepository<AttributeMetaDatum> AttributeMetaDataRepo;
        private readonly IRepository<DataMinerAttribute> AttributeRepo;
        private readonly IRepository<AttributeDatum<double>> NumericAttributeDatumRepo;
        private readonly IRepository<AttributeDatum<string>> TextAttributeDatumRepo;
        private readonly IRepository<MaintainableAsset> MaintainableAssetRepo;
        private readonly IHubContext<BridgeCareHub> HubContext;

        private readonly IRepository<IEnumerable<(DataMinerAttribute attribute, (int year, double value))>>
            NumericAggregatedResultRepo;

        private readonly IRepository<IEnumerable<(DataMinerAttribute attribute, (int year, string value))>>
            TextAggregatedResultRepo;

        private readonly ISaveChanges Repos;
        private readonly ILogger<NetworkController> Logger;

        public AggregationController(IRepository<Network> networkRepo,
            IRepository<AttributeMetaDatum> attributeMetaDataRepo,
            IRepository<DataMinerAttribute> attributeRepo,
            IRepository<AttributeDatum<double>> numericAttributeDatumRepo,
            IRepository<AttributeDatum<string>> textAttributeDatumRepo,
            IRepository<MaintainableAsset> maintainableAssetRepo,
            IRepository<IEnumerable<(DataMinerAttribute attribute, (int year, double value))>> numericAggregatedResultRepo,
            IRepository<IEnumerable<(DataMinerAttribute attribute, (int year, string value))>> textAggregatedResultRepo,
            ISaveChanges repos,
            ILogger<NetworkController> logger, IHubContext<BridgeCareHub> hub)
        {
            NetworkRepo = networkRepo ?? throw new ArgumentNullException(nameof(networkRepo));
            AttributeMetaDataRepo = attributeMetaDataRepo ?? throw new ArgumentNullException(nameof(attributeMetaDataRepo));
            AttributeRepo = attributeRepo ?? throw new ArgumentNullException(nameof(attributeRepo));
            NumericAttributeDatumRepo = numericAttributeDatumRepo ?? throw new ArgumentNullException(nameof(numericAttributeDatumRepo));
            TextAttributeDatumRepo = textAttributeDatumRepo ?? throw new ArgumentNullException(nameof(textAttributeDatumRepo));
            MaintainableAssetRepo = maintainableAssetRepo ?? throw new ArgumentNullException(nameof(maintainableAssetRepo));
            NumericAggregatedResultRepo = numericAggregatedResultRepo ?? throw new ArgumentNullException(nameof(numericAggregatedResultRepo));
            TextAggregatedResultRepo = textAggregatedResultRepo ?? throw new ArgumentNullException(nameof(textAggregatedResultRepo));
            Repos = repos ?? throw new ArgumentNullException(nameof(repos));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            HubContext = hub;
        }

        [HttpPost]
        [Route("AssignNetworkData/{networkId}")]
        public async Task<IActionResult> AssignNetworkData(Guid networkId)
        {
            try
            {
                await HubContext
                    .Clients
                    .All
                    .SendAsync("BroadcastMessage", "from controller");

                var network = NetworkRepo.Get(networkId);

                var attributeMetaData = AttributeMetaDataRepo.All();

                var attributeData = new List<IAttributeDatum>();

                // Create the list of attributes
                foreach (var attributeMetaDatum in attributeMetaData)
                {
                    var attribute = AttributeFactory.Create(attributeMetaDatum);
                    attributeData.AddRange(AttributeDataBuilder.GetData(AttributeConnectionBuilder.Build(attribute)));
                    AttributeRepo.Add(attribute);
                }

                if (attributeData.Any())
                {
                    foreach (var maintainableAsset in network.MaintainableAssets)
                    {
                        // assign attribute data to maintainable asset
                        maintainableAsset.AssignAttributeData(attributeData);
                        // add assigned attribute data to db context
                        if (maintainableAsset.AssignedData.Any(a => a.Attribute.DataType == "NUMERIC"))
                        {
                            var numericAttributeData = maintainableAsset.AssignedData
                                .Where(a => a.Attribute.DataType == "NUMERIC")
                                .Select(a => (AttributeDatum<double>)Convert.ChangeType(a, typeof(AttributeDatum<double>)));

                            NumericAttributeDatumRepo.AddAll(numericAttributeData, maintainableAsset.Id);
                        }

                        if (maintainableAsset.AssignedData.Any(a => a.Attribute.DataType == "TEXT"))
                        {
                            var textAttributeData = maintainableAsset.AssignedData
                                .Where(a => a.Attribute.DataType == "TEXT")
                                .Select(a => (AttributeDatum<string>)Convert.ChangeType(a, typeof(AttributeDatum<string>)));

                            TextAttributeDatumRepo.AddAll(textAttributeData, maintainableAsset.Id);
                        }
                    }
                }

                Repos.SaveChanges();
                Logger.LogInformation("Attributes & attribute data have been created");
                return Ok("Successfully assigned network data");
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
            try
            {
                var maintainableAssets = MaintainableAssetRepo.Find(networkId);

                foreach (var maintainableAsset in maintainableAssets)
                {
                    if (maintainableAsset.AssignedData.Any(a => a.Attribute.DataType == "NUMERIC"))
                    {
                        var aggregatedNumericResults = maintainableAsset.AssignedData
                            .Where(a => a.Attribute.DataType == "NUMERIC")
                            .Select(a => a.Attribute)
                            .Select(a => maintainableAsset.GetAggregatedValuesByYear(a, AggregationRuleFactory.CreateNumericRule(a)).ToList());

                        NumericAggregatedResultRepo.AddAll(aggregatedNumericResults, maintainableAsset.Id);
                    }

                    if (maintainableAsset.AssignedData.Any(a => a.Attribute.DataType == "TEXT"))
                    {
                        var aggregatedTextResults = maintainableAsset.AssignedData
                            .Where(a => a.Attribute.DataType == "TEXT")
                            .Select(a => a.Attribute)
                            .Select(a => maintainableAsset.GetAggregatedValuesByYear(a, AggregationRuleFactory.CreateTextRule(a)).ToList());

                        TextAggregatedResultRepo.AddAll(aggregatedTextResults, maintainableAsset.Id);
                    }
                }

                Repos.SaveChanges();
                Logger.LogInformation("Attributes & attribute data have been created");
                return Ok("Successfully aggregated network data");
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }
    }
}
