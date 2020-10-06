using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;
using AppliedResearchAssociates.iAM.DataAssignment.Segmentation;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using DataMinerAttribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AggregationController : ControllerBase
    {
        private readonly ILogger<SegmentationController> _logger;
        private readonly IRepository<AttributeMetaDatum> AttributeMetaDataRepository;
        private readonly INetworkDataRepository NetworkRepository;
        private readonly IAttributeDataRepository AttributeRepository;
        private readonly IAttributeDatumDataRepository AttributeDatumDataRepository;
        private readonly ISaveChanges Repositories;

        public AggregationController(ILogger<SegmentationController> logger,
            IRepository<AttributeMetaDatum> attributeRepo,
            INetworkDataRepository partialNetworkRepo,
            IAttributeDatumDataRepository attributeDatumDataRepository,
            IAttributeDataRepository attributeRepository,
            ISaveChanges repositories)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            AttributeMetaDataRepository = attributeRepo ?? throw new ArgumentNullException(nameof(attributeRepo));
            NetworkRepository = partialNetworkRepo ?? throw new ArgumentNullException(nameof(partialNetworkRepo));
            AttributeRepository = attributeRepository ?? throw new ArgumentNullException(nameof(attributeRepository));
            AttributeDatumDataRepository = attributeDatumDataRepository ?? throw new ArgumentNullException(nameof(attributeDatumDataRepository));
            Repositories = repositories ?? throw new ArgumentNullException(nameof(repositories));

        }

        [HttpPost]
        [Route("AssignNetworkData")]
        public async Task<IActionResult> AssignNetworkData([FromBody] Guid networkId)
        {
            var network = NetworkRepository.GetNetworkWithNoAttributeData(networkId);

            var attributeMetaData = AttributeMetaDataRepository.All();
            //var attributeJsonText = System.IO.File.ReadAllText("attributeMetaData.json");
            //var attributeMetaData = JsonConvert.DeserializeAnonymousType(attributeJsonText, new { AttributeMetaData = default(List<AttributeMetaDatum>) }).AttributeMetaData;

            var attributeData = new List<IAttributeDatum>();
            var attributes = new List<DataMinerAttribute>();

            // Create the list of attributes
            foreach (var attributeMetaDatum in attributeMetaData)
            {
                var attribute = AttributeFactory.Create(attributeMetaDatum);
                attributes.Add(attribute);
            }
            // add the attributes to db context
            if (attributes.Any())
            {
                foreach (var attribute in attributes)
                {
                    AttributeRepository.AddAttribute(attribute);
                }
            }

            // Create the attribute data for each attribute
            foreach (var attribute in attributes)
            {
                attributeData.AddRange(AttributeDataBuilder.GetData(AttributeConnectionBuilder.Build(attribute)));
            }

            foreach (var segment in network.Segments)
            {
                // assign attribute data to segments
                segment.AssignAttributeData(attributeData);
                // add attribute data to db context
                foreach (var attributeDatum in segment.AssignedData)
                {
                    if (attributeDatum.Attribute.DataType == "NUMERIC")
                    {
                        var numericAttributeDatum = (AttributeDatum<double>)Convert.ChangeType(attributeDatum, typeof(AttributeDatum<double>));
                        AttributeDatumDataRepository.AddAttributeDatum(numericAttributeDatum, segment.Location.UniqueIdentifier);
                    }
                    else
                    {
                        var textAttributeDatum = (AttributeDatum<string>)Convert.ChangeType(attributeDatum, typeof(AttributeDatum<string>));
                        AttributeDatumDataRepository.AddAttributeDatum(textAttributeDatum, segment.Location.UniqueIdentifier);
                    }
                }
            }

            //foreach (var segmentEntity in networkEntity.SegmentEntities)
            //{
            //    var segment = new Segment(LocationEntityToLocation.CreateFromEntity(segmentEntity.LocationEntity));
            //    segment.AssignAttributeData(attributeData);

            //    segments.Add(new SegmentEntity
            //    {
            //        AttributeData = (ICollection<AttributeDatumEntity>)segment.AssignedData,

            //    });
            //}

            //SegmentRepository.AddAll()
            //var network = new Network(segments, networkGuid, networkEntity.Name);

            Repositories.SaveChanges();
            _logger.LogInformation("Attributes & attribute data have been created");
            return Ok(network);
        }

        [HttpPost]
        [Route("AggregateNetworkData")]
        public async Task<IActionResult> AggregateNetworkData([FromBody] Guid networkId)
        {
            var attributeMetaData = AttributeMetaDataRepository.All();

            var attributes = new List<DataMinerAttribute>();

            // Create the list of attributes
            foreach (var attributeMetaDatum in attributeMetaData)
            {
                var attribute = AttributeFactory.Create(attributeMetaDatum);
                attributes.Add(attribute);
            }

            var network = NetworkRepository.GetNetworkWillAllData(networkId);

            var aggregatedNumericResults = new List<(DataMinerAttribute attribute, (int year, double value))>();
            var aggregatedTextResults = new List<(DataMinerAttribute attribute, (int year, string value))>();
            foreach (var attribute in attributes)
            {
                foreach (var segment in network.Segments)
                {
                    switch (attribute.DataType)
                    {
                    case "NUMERIC":
                        aggregatedNumericResults.AddRange(segment.GetAggregatedValuesByYear(attribute, AggregationRuleFactory.CreateNumericRule(attribute)));
                        break;
                    case "TEXT":
                        aggregatedTextResults.AddRange(segment.GetAggregatedValuesByYear(attribute, AggregationRuleFactory.CreateTextRule(attribute)));
                        break;
                    }
                }
            }

            return Ok();
        }
    }
}
