using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;
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
        private readonly ILogger<NetworkController> _logger;
        private readonly IRepository<AttributeMetaDatum> AttributeMetaDataRepository;
        private readonly IAggregatedResultDataRepository AggregatedResultRepository;
        private readonly INetworkDataRepository NetworkRepository;
        private readonly ISegmentDataRepository SegmentRepository;
        private readonly IAttributeDataRepository AttributeRepository;
        private readonly IAttributeDatumDataRepository AttributeDatumDataRepository;
        private readonly ISaveChanges Repositories;

        public AggregationController(ILogger<NetworkController> logger,
            IRepository<AttributeMetaDatum> attributeRepo,
            INetworkDataRepository partialNetworkRepo,
            ISegmentDataRepository segmentRepository,
            IAttributeDatumDataRepository attributeDatumDataRepository,
            IAttributeDataRepository attributeRepository,
            IAggregatedResultDataRepository aggregatedResultRepository,
            ISaveChanges repositories)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            AttributeMetaDataRepository = attributeRepo ?? throw new ArgumentNullException(nameof(attributeRepo));
            NetworkRepository = partialNetworkRepo ?? throw new ArgumentNullException(nameof(partialNetworkRepo));
            SegmentRepository = segmentRepository ?? throw new ArgumentNullException(nameof(segmentRepository));
            AttributeRepository = attributeRepository ?? throw new ArgumentNullException(nameof(attributeRepository));
            AttributeDatumDataRepository = attributeDatumDataRepository ?? throw new ArgumentNullException(nameof(attributeDatumDataRepository));
            Repositories = repositories ?? throw new ArgumentNullException(nameof(repositories));
            AggregatedResultRepository = aggregatedResultRepository ?? throw new ArgumentNullException(nameof(aggregatedResultRepository));
        }

        [HttpPost]
        [Route("AssignNetworkData")]
        public async Task<IActionResult> AssignNetworkData([FromBody] Guid networkId)
        {
            var network = NetworkRepository.GetNetworkWithNoAttributeData(networkId);

            var attributeMetaData = AttributeMetaDataRepository.All();

            var attributeData = new List<IAttributeDatum>();

            // Create the list of attributes
            foreach (var attributeMetaDatum in attributeMetaData)
            {
                var attribute = AttributeFactory.Create(attributeMetaDatum);
                attributeData.AddRange(AttributeDataBuilder.GetData(AttributeConnectionBuilder.Build(attribute)));
                AttributeRepository.AddAttribute(attribute);
            }

            if (attributeData.Any())
            {
                foreach (var segment in network.MaintainableAssets)
                {
                    // assign attribute data to segment
                    segment.AssignAttributeData(attributeData);
                    // add assigned attribute data to db context
                    if (segment.AssignedData.Any(a => a.Attribute.DataType == "NUMERIC"))
                    {
                        var numericAttributeData = segment.AssignedData
                            .Where(a => a.Attribute.DataType == "NUMERIC")
                            .Select(a => (AttributeDatum<double>)Convert.ChangeType(a, typeof(AttributeDatum<double>)));

                        AttributeDatumDataRepository.AddAttributeData(numericAttributeData, segment.Id);
                    }

                    if (segment.AssignedData.Any(a => a.Attribute.DataType == "TEXT"))
                    {
                        var textAttributeData = segment.AssignedData
                            .Where(a => a.Attribute.DataType == "TEXT")
                            .Select(a => (AttributeDatum<string>)Convert.ChangeType(a, typeof(AttributeDatum<string>)));

                        AttributeDatumDataRepository.AddAttributeData(textAttributeData, segment.Id);
                    }
                }
            }

            Repositories.SaveChanges();
            _logger.LogInformation("Attributes & attribute data have been created");
            return Ok();
        }

        [HttpPost]
        [Route("AggregateNetworkData")]
        public async Task<IActionResult> AggregateNetworkData([FromBody] Guid networkId)
        {
            var segments = SegmentRepository.GetNetworkSegmentsWithAssignedData(networkId);

            foreach (var segment in segments)
            {
                if (segment.AssignedData.Any(a => a.Attribute.DataType == "NUMERIC"))
                {
                    var aggregatedNumericResults = segment.AssignedData
                        .Where(a => a.Attribute.DataType == "NUMERIC")
                        .Select(a => a.Attribute)
                        .Select(a => segment.GetAggregatedValuesByYear(a, AggregationRuleFactory.CreateNumericRule(a)))
                        .ToList();

                    AggregatedResultRepository.AddAggregatedResults(aggregatedNumericResults, segment.Id);
                }

                if (segment.AssignedData.Any(a => a.Attribute.DataType == "TEXT"))
                {
                    var aggregatedTextResults = segment.AssignedData
                        .Where(a => a.Attribute.DataType == "TEXT")
                        .Select(a => a.Attribute)
                        .Select(a => segment.GetAggregatedValuesByYear(a, AggregationRuleFactory.CreateTextRule(a)))
                        .ToList();


                    AggregatedResultRepository.AddAggregatedResults(aggregatedTextResults, segment.Id);
                }
            }

            return Ok();
        }
    }
}
