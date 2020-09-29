﻿
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Aggregation;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Segmentation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace BridgeCareCore.Controllers
{
    public class AggregationController : ControllerBase
    {
        private readonly IRepository<NetworkEntity> NetworkRepository;
        private readonly IRepository<SegmentEntity> SegmentRepository;
        private readonly ILogger<SegmentationController> _logger;

        public AggregationController(ILogger<SegmentationController> logger, IRepository<NetworkEntity> networkRepository,
            IRepository<SegmentEntity> segmentRepository)
        {
            NetworkRepository = networkRepository;
            SegmentRepository = segmentRepository;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        [Route("AssignNetworkData")]
        public async Task<IActionResult> AssignNetworkData(Guid networkGuid)
        {
            // Mapping
            var networkEntity = NetworkRepository.Get(networkGuid);
            var segments = new List<Segment>();

            foreach (var segmentEntity in networkEntity.SegmentEntities)
            {
                segments.Add(new Segment(LocationBuilder.CreateFromEntity(segmentEntity.LocationEntity)));
            }

            var network = new Network(segments, networkGuid, networkEntity.Name);

            var attributeJsonText = System.IO.File.ReadAllText("attributeMetaData.json");
            var attributeMetaData = JsonConvert.DeserializeAnonymousType(attributeJsonText, new { AttributeMetaData = default(List<AttributeMetaDatum>) }).AttributeMetaData;

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

            return Ok(Aggregator.AssignAttributeDataToSegments(attributeData.AsEnumerable(), network.Segments));
        }
    }
}
