using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataAssignment.Segmentation;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SegmentationController : ControllerBase
    {
        private readonly IRepository<NetworkEntity> NetworkRepository;
        private readonly IRepository<SegmentEntity> SegmentRepository;
        private readonly ILogger<SegmentationController> _logger;

        public SegmentationController(ILogger<SegmentationController> logger, IRepository<NetworkEntity> networkRepository,
            IRepository<SegmentEntity> segmentRepository)
        {
            NetworkRepository = networkRepository;
            SegmentRepository = segmentRepository;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        [Route("CreateNetwork")]
        public async Task<IActionResult> CreateNetwork([FromBody] string name)
        {
            // Domain logic
            var segmentationRulesJsonText = System.IO.File.ReadAllText("segmentationMetaData.json");
            var attributeMetaDatum = JsonConvert.DeserializeAnonymousType(segmentationRulesJsonText,
                new { AttributeMetaDatum = default(AttributeMetaDatum) }).AttributeMetaDatum;

            var attribute = AttributeFactory.Create(attributeMetaDatum);
            var attributeData = AttributeDataBuilder.GetData(AttributeConnectionBuilder.Build(attribute));

            var network = Segmenter.CreateNetworkFromAttributeDataRecords(attributeData);
            ICollection<SegmentEntity> segmentEntities = new List<SegmentEntity>();
            foreach (var segment in network.Segments)
            {
                segmentEntities.Add(new SegmentEntity
                {
                });
            }

            // Mapping
            var networkEntity = new NetworkEntity { Id = network.Guid, Name = name };

            var newNetwork = NetworkRepository.Add(networkEntity);
            SegmentRepository.AddAll(networkEntity.SegmentEntities.ToList());

            NetworkRepository.SaveChanges();
            _logger.LogInformation($"a network with name : {newNetwork.Name} has been created");
            return Ok(newNetwork);
        }
    }
}
