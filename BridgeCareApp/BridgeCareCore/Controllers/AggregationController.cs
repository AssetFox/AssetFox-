using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataAssignment.Segmentation;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AggregationController : ControllerBase
    {
        private readonly IRepository<Network> NetworkRepository;
        private readonly IRepository<Segment> SegmentRepository;
        private readonly ILogger<SegmentationController> _logger;
        private readonly IRepository<AttributeMetaDatum> AttributeMetaDataRepository;
        private readonly INetworkDataRepository NetorkRepository;

        public AggregationController(ILogger<SegmentationController> logger, IRepository<Network> networkRepository,
            IRepository<Segment> segmentRepository,
            IRepository<AttributeMetaDatum> attributeRepo, INetworkDataRepository partialNetworkRepo)
        {
            NetworkRepository = networkRepository;
            SegmentRepository = segmentRepository;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            AttributeMetaDataRepository = attributeRepo ?? throw new ArgumentNullException(nameof(attributeRepo));
            NetorkRepository = partialNetworkRepo ?? throw new ArgumentNullException(nameof(partialNetworkRepo));
        }

        [HttpPost]
        [Route("AssignNetworkData")]
        public async Task<IActionResult> AssignNetworkData([FromBody] Guid networkId)
        {
            var network = NetorkRepository.GetNetworkWithNoAttributeData(networkId);

            var attributeMetaData = AttributeMetaDataRepository.All();
            //var attributeJsonText = System.IO.File.ReadAllText("attributeMetaData.json");
            //var attributeMetaData = JsonConvert.DeserializeAnonymousType(attributeJsonText, new { AttributeMetaData = default(List<AttributeMetaDatum>) }).AttributeMetaData;

            var attributeData = new List<IAttributeDatum>();
            var attributes = new List<Attribute>();

            // Create the list of attributes
            foreach (var attributeMetaDatum in attributeMetaData)
            {
                var attribute = AttributeFactory.Create(attributeMetaDatum);
                attributes.Add(attribute);
            }

            // Create the attribute data for each attribute
            foreach (var attribute in attributes)
            {
                attributeData.AddRange(AttributeDataBuilder.GetData(AttributeConnectionBuilder.Build(attribute)));
            }

            foreach (var segment in network.Segments)
            {
                segment.AssignAttributeData(attributeData);
            }

            NetworkRepository.Update(network);
            
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


            return Ok();
        }
    }
}
