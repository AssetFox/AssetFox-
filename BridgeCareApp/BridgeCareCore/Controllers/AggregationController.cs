using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataAssignment.Segmentation;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using BridgeCareCore.Profile;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace BridgeCareCore.Controllers
{
    public class AggregationController : ControllerBase
    {
        private readonly IRepository<NetworkEntity> NetworkRepository;
        private readonly IRepository<SegmentEntity> SegmentRepository;
        private readonly ILogger<SegmentationController> _logger;
        private readonly IRepository<AttributeMetaDatum> AttributeMetaRepository;

        public AggregationController(ILogger<SegmentationController> logger, IRepository<NetworkEntity> networkRepository,
            IRepository<SegmentEntity> segmentRepository, IRepository<AttributeMetaDatum> attributeRepo)
        {
            NetworkRepository = networkRepository;
            SegmentRepository = segmentRepository;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            AttributeMetaRepository = attributeRepo ?? throw new ArgumentNullException(nameof(attributeRepo));
        }

        [HttpPost]
        [Route("AssignNetworkData")]
        public async Task<IActionResult> AssignNetworkData(Guid networkGuid)
        {
            var networkEntity = NetworkRepository.Get(networkGuid);

            var attributeMetaData = AttributeMetaRepository.All();
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

            var segments = new List<SegmentEntity>();
            foreach (var segmentEntity in networkEntity.SegmentEntities)
            {
                var segment = new Segment(LocationEntityToLocation.CreateFromEntity(segmentEntity.LocationEntity));
                segment.AssignAttributeData(attributeData);

                segments.Add(new SegmentEntity
                {
                    AttributeData = (ICollection<AttributeDatumEntity>)segment.AssignedData,

                });
            }

            SegmentRepository.AddAll(seg)
            var network = new Network(segments, networkGuid, networkEntity.Name);


            return Ok();
        }
    }
}
