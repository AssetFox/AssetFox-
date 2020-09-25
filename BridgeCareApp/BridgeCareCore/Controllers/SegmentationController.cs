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
        public SegmentationController(NetworkRepository networkRepository)
        {
            NetworkRepository = networkRepository;
        }

        [HttpPost]
        [Route("CreateNetwork")]
        public async Task<IActionResult> CreateNetwork()
        {
            var apiObject = new ApiExamples();

            var network = apiObject.CreateNewNetwork();
            var entity = new NetworkEntity { Id = network.Guid, Name = network.Name };

            return Ok(NetworkRepository.Add(entity));

        }
    }
}
