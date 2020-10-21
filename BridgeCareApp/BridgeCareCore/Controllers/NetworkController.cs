using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataAssignment.Segmentation;
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
        private readonly IRepository<AttributeMetaDatum> MaintainableAssetFileRepo;
        private readonly IRepository<Network> NetworkRepo;

        //private readonly IRepository<MaintainableAsset> MaintainableAssetRepo;
        private readonly ISaveChanges Repos;

        private readonly ILogger<NetworkController> Logger;

        public NetworkController(IRepository<AttributeMetaDatum> maintainableAssetFileRepo,
            IRepository<Network> networkRepo,
            //IRepository<MaintainableAsset> maintainableAssetRepo,
            ISaveChanges repos,
            ILogger<NetworkController> logger)
        {
            MaintainableAssetFileRepo = maintainableAssetFileRepo ?? throw new ArgumentNullException(nameof(maintainableAssetFileRepo));
            NetworkRepo = networkRepo ?? throw new ArgumentNullException(nameof(networkRepo));
            //MaintainableAssetRepo = maintainableAssetRepo ?? throw new ArgumentNullException(nameof(maintainableAssetRepo));
            Repos = repos ?? throw new ArgumentNullException(nameof(repos));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [Route("GetAllNetworks")]
        public IActionResult GetAllNetworks()
        {
            try
            {
                var networks = NetworkRepo.All();
                // Sending the first network because PennDOT will always have only 1 network
                var filteredNetworks = new List<Network> { networks.FirstOrDefault() };
                return Ok(filteredNetworks);
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        [HttpPost]
        [Route("CreateNetwork")]
        public async Task<IActionResult> CreateNetwork([FromBody] string name)
        {
            try
            {
                // get attribute meta data from maintainable asset json file
                var attributeMetaData = MaintainableAssetFileRepo.All();
                // create an attribute from the meta data
                var attribute = AttributeFactory.Create(attributeMetaData.FirstOrDefault());
                // build a connection from the attribute to create attribute data from attribute
                // data in the data source
                var attributeData = AttributeDataBuilder.GetData(AttributeConnectionBuilder.Build(attribute));
                // create a network domain from the attribute data
                var network = NetworkFactory.CreateNetworkFromAttributeDataRecords(attributeData);
                network.Name = name;
                // add the network to our data source
                var newlyGeneratedId = NetworkRepo.Add(network);
                // add the maintainable assets to our data source
                //MaintainableAssetRepo.AddAll(network.MaintainableAssets, network.Id);

                Repos.SaveChanges(); // this will save all of the data in the IAMContext object
                Logger.LogInformation($"a network with name : {network.Name} has been created");
                return Ok(newlyGeneratedId);
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }
    }
}
