using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Aggregation;
using AppliedResearchAssociates.iAM.DataAccess;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Segmentation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
        public async Task<IActionResult> CreateNetwork()
        {
            var api = new ApiExamples();

            // Domain logic
            var network = api.CreateNetwork();

            // Mapping
            var entity = new NetworkEntity { Id = network.Guid, Name = network.Name };
            var newNetwork = NetworkRepository.Add(entity);
            
            NetworkRepository.SaveChanges();
            _logger.LogInformation($"a network with name : {newNetwork.Name} has been created");
            return Ok(newNetwork);
        }

        
    }
}
