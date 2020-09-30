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
        private readonly IRepository<Network> NetworkRepository;
        private readonly IRepository<Segment> SegmentRepository;
        private readonly ILogger<SegmentationController> _logger;
        private readonly IRepository<AttributeMetaDatum> _SegmentationFileRepository;

        public SegmentationController(ILogger<SegmentationController> logger, IRepository<Network> networkRepository,
            IRepository<Segment> segmentRepository, IRepository<AttributeMetaDatum> segmentFileRepository)
        {
            NetworkRepository = networkRepository;
            SegmentRepository = segmentRepository;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _SegmentationFileRepository = segmentFileRepository;
        }

        [HttpPost]
        [Route("CreateNetwork")]
        public async Task<IActionResult> CreateNetwork([FromBody] string name)
        {
            // Domain logic
            var attributeMetaData = _SegmentationFileRepository.All();
            //var segmentationRulesJsonText = System.IO.File.ReadAllText("segmentationMetaData.json");
            //var attributeMetaDatum = JsonConvert.DeserializeAnonymousType(segmentationRulesJsonText,
            //    new { AttributeMetaDatum = default(AttributeMetaDatum) }).AttributeMetaDatum;

            var attribute = AttributeFactory.Create(attributeMetaData.FirstOrDefault());
            var attributeData = AttributeDataBuilder.GetData(AttributeConnectionBuilder.Build(attribute));

            var network = Segmenter.CreateNetworkFromAttributeDataRecords(attributeData);

            network.Name = name;
            var newNetwork = NetworkRepository.Add(network);
            SegmentRepository.AddAll(network.Segments.ToList());

            NetworkRepository.SaveChanges(); // this will save all of the data in the IAMContext object
            _logger.LogInformation($"a network with name : {newNetwork.Name} has been created");
            return Ok(newNetwork);
        }
    }
}
