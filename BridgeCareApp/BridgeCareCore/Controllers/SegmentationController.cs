using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataAccess;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Segmentation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SegmentationController : ControllerBase
    {
        private readonly IRepository<NetworkEntity> NetworkRepository;
        private readonly IRepository<SegmentEntity> SegmentRepository;
        public SegmentationController(NetworkRepository networkRepository, SegmentRepository segmentRepository)
        {
            NetworkRepository = networkRepository;
            SegmentRepository = segmentRepository;
        }

        [HttpPost]
        [Route("CreateNetwork")]
        public async Task<IActionResult> CreateNetwork()
        {
            var apiObject = new ApiExamples();

            var network = apiObject.CreateNewNetwork();
            var entity = new NetworkEntity { Id = network.Guid, Name = network.Name };
            var newNetwork = NetworkRepository.Add(entity);
            var segmentEntity = new SegmentEntity
            {
                
            };
            /*var segmentEntities = new List<SegmentEntity> { network..Segments };
            SegmentRepository.AddAll()*/
            NetworkRepository.SaveChanges();
            return Ok(newNetwork);

        }
    }
}
