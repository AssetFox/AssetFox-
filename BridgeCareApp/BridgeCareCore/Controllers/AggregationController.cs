using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Interfaces;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IAggregatedResultDataRepository AggregatedResultRepo;
        private readonly ISaveChanges Repos;
        private readonly ILogger<NetworkController> Logger;

        public AggregationController(IRepository<Network> networkRepo,
            IRepository<AttributeMetaDatum> attributeMetaDataRepo,
            IRepository<DataMinerAttribute> attributeRepo,
            IRepository<AttributeDatum<double>> numericAttributeDatumRepo,
            IRepository<AttributeDatum<string>> textAttributeDatumRepo,
            IRepository<MaintainableAsset> maintainableAssetRepo,
            IAggregatedResultDataRepository aggregatedResultRepo,
            ISaveChanges repos,
            ILogger<NetworkController> logger)
        {
            NetworkRepo = networkRepo ?? throw new ArgumentNullException(nameof(networkRepo));
            AttributeMetaDataRepo = attributeMetaDataRepo ?? throw new ArgumentNullException(nameof(attributeMetaDataRepo));
            AttributeRepo = attributeRepo ?? throw new ArgumentNullException(nameof(attributeRepo));
            NumericAttributeDatumRepo = numericAttributeDatumRepo ?? throw new ArgumentNullException(nameof(numericAttributeDatumRepo));
            TextAttributeDatumRepo = textAttributeDatumRepo ?? throw new ArgumentNullException(nameof(textAttributeDatumRepo));
            MaintainableAssetRepo = maintainableAssetRepo ?? throw new ArgumentNullException(nameof(maintainableAssetRepo));
            AggregatedResultRepo = aggregatedResultRepo ?? throw new ArgumentNullException(nameof(aggregatedResultRepo));
            Repos = repos ?? throw new ArgumentNullException(nameof(repos));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        [Route("AssignNetworkData")]
        public async Task<IActionResult> AssignNetworkData([FromBody] Guid networkId)
        {
            try
            {
                var network = NetworkRepo.Get(networkId);

                var attributeMetaData = AttributeMetaDataRepo.All();

                var attributeData = new List<IAttributeDatum>();

                // Create the list of attributes
                foreach (var attributeMetaDatum in attributeMetaData)
                {
                    var attribute = AttributeFactory.Create(attributeMetaDatum);
                    attributeData.AddRange(AttributeDataBuilder.GetData(AttributeConnectionBuilder.Build(attribute)));
                    //AttributeRepo.Add(attribute);
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

                            NumericAttributeDatumRepo.AddAll(numericAttributeData);
                        }

                        if (maintainableAsset.AssignedData.Any(a => a.Attribute.DataType == "TEXT"))
                        {
                            var textAttributeData = maintainableAsset.AssignedData
                                .Where(a => a.Attribute.DataType == "TEXT")
                                .Select(a => (AttributeDatum<string>)Convert.ChangeType(a, typeof(AttributeDatum<string>)));

                            TextAttributeDatumRepo.AddAll(textAttributeData);
                        }
                    }
                }

                Repos.SaveChanges();
                Logger.LogInformation("Attributes & attribute data have been created");
                return Ok();
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
            var maintainableAssets = MaintainableAssetRepo.Find(networkId);

            foreach (var maintainableAsset in maintainableAssets)
            {
                if (maintainableAsset.AssignedData.Any(a => a.Attribute.DataType == "NUMERIC"))
                {
                    var aggregatedNumericResults = maintainableAsset.AssignedData
                        .Where(a => a.Attribute.DataType == "NUMERIC")
                        .Select(a => a.Attribute)
                        .Select(a => maintainableAsset.GetAggregatedValuesByYear(a, AggregationRuleFactory.CreateNumericRule(a)))
                        .ToList();

                    AggregatedResultRepo.AddAggregatedResults<double>(aggregatedNumericResults);
                }

                if (maintainableAsset.AssignedData.Any(a => a.Attribute.DataType == "TEXT"))
                {
                    var aggregatedTextResults = maintainableAsset.AssignedData
                        .Where(a => a.Attribute.DataType == "TEXT")
                        .Select(a => a.Attribute)
                        .Select(a => maintainableAsset.GetAggregatedValuesByYear(a, AggregationRuleFactory.CreateTextRule(a)))
                        .ToList();

                    AggregatedResultRepo.AddAggregatedResults(aggregatedTextResults);
                }
            }

            return Ok();
        }
    }
}
