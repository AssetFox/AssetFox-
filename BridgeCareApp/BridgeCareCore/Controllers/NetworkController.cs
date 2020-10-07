using System;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataAssignment.Segmentation;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NetworkController : ControllerBase
    {
        private readonly IMaintainableAssetRepository SegmentRepository;
        private readonly ILogger<NetworkController> _logger;
        private readonly IRepository<AttributeMetaDatum> _SegmentationFileRepository;
        private readonly INetworkDataRepository NetworkRepository;
        private readonly ISaveChanges Repositories;

        public NetworkController(ILogger<NetworkController> logger,
            IMaintainableAssetRepository segmentRepo,
            IRepository<AttributeMetaDatum> segmentFileRepository,
            INetworkDataRepository networkRepo, ISaveChanges saveAll)
        {
            SegmentRepository = segmentRepo;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _SegmentationFileRepository = segmentFileRepository;
            NetworkRepository = networkRepo ?? throw new ArgumentNullException(nameof(networkRepo));
            Repositories = saveAll ?? throw new ArgumentNullException(nameof(networkRepo));
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

            var network = NetworkFactory.CreateNetworkFromAttributeDataRecords(attributeData);
            network.Name = name;

            NetworkRepository.AddNetworkWithoutAnyData(network);
            SegmentRepository.AddNetworkMaintainableAssets(network.MaintainableAssets, network.Id);

            Repositories.SaveChanges(); // this will save all of the data in the IAMContext object
            _logger.LogInformation($"a network with name : {network.Name} has been created");
            return Ok(network);
        }
    }
}
