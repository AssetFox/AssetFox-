using System;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NetworkController : ControllerBase
    {
        private readonly IRepository<AttributeMetaDatum> networkDefinitionFileRepository;
        private readonly INetworkRepository NetworkRepo;
        private readonly ISaveChanges Repos;

        private readonly ILogger<NetworkController> Logger;

        public NetworkController(IRepository<AttributeMetaDatum> maintainableAssetFileRepo,
            INetworkRepository networkRepo,            
            ISaveChanges repos,
            ILogger<NetworkController> logger)
        {
            networkDefinitionFileRepository = maintainableAssetFileRepo ?? throw new ArgumentNullException(nameof(maintainableAssetFileRepo));
            NetworkRepo = networkRepo ?? throw new ArgumentNullException(nameof(networkRepo));
            Repos = repos ?? throw new ArgumentNullException(nameof(repos));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        [Route("CreateNetwork")]
        public async Task<IActionResult> CreateNetwork([FromBody] string name)
        {
            try
            {
                // get attribute meta data from maintainable asset json file
                var attributeMetaData = networkDefinitionFileRepository.All().ToList();
                if(attributeMetaData.Count() != 1)
                {
                    throw new InvalidOperationException("Segmentation supports only a single meta data attribute.");
                }

                var attribute = AttributeFactory.Create(attributeMetaData.FirstOrDefault());
                var attributeData = AttributeDataBuilder.GetData(AttributeConnectionBuilder.Build(attribute));
                var network = NetworkFactory.CreateNetworkFromAttributeDataRecords(attributeData);
                network.Name = name;

                NetworkRepo.Add(network);

                // [TODO] This call is dependent on a MSSQL repo and doesn't go in the controller. Need to remove it.
                Repos.SaveChanges();

                Logger.LogInformation($"a network with name : {network.Name} has been created");
                return Ok($"Network {network.Name} with {network.MaintainableAssets.Count()} maintainable assets was created successfully.");
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }
    }
}
