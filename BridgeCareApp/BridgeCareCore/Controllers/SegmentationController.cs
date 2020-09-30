using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Aggregation;
using AppliedResearchAssociates.iAM.DataAccess;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Segmentation;
using Microsoft.AspNetCore.Http;
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
        private readonly IRepository<AttributeMetaDatum> _SegmentationFileRepository;

        public SegmentationController(ILogger<SegmentationController> logger, IRepository<NetworkEntity> networkRepository,
            IRepository<SegmentEntity> segmentRepository, IRepository<AttributeMetaDatum> segmentFileRepository)
        {
            NetworkRepository = networkRepository;
            SegmentRepository = segmentRepository;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _SegmentationFileRepository = segmentFileRepository;
        }

        [HttpPost]
        [Route("CreateNetwork")]
        public async Task<IActionResult> CreateNetwork()
        {
            // Domain logic
            var attributeMetaData = _SegmentationFileRepository.All();
            //var segmentationRulesJsonText = System.IO.File.ReadAllText("segmentationMetaData.json");
            //var attributeMetaDatum = JsonConvert.DeserializeAnonymousType(segmentationRulesJsonText,
            //    new { AttributeMetaDatum = default(AttributeMetaDatum) }).AttributeMetaDatum;

            var attribute = AttributeFactory.Create(attributeMetaData.FirstOrDefault());
            var attributeData = AttributeDataBuilder.GetData(AttributeConnectionBuilder.Build(attribute));

            var network = Segmenter.CreateNetworkFromAttributeDataRecords(attributeData);

            // Mapping
            var networkEntity = new NetworkEntity { Id = network.Guid, Name = network.Name };

            var newNetwork = NetworkRepository.Add(networkEntity);
            NetworkRepository.SaveChanges();
            _logger.LogInformation($"a network with name : {newNetwork.Name} has been created");
            return Ok(newNetwork);
        }

        
    }
}
