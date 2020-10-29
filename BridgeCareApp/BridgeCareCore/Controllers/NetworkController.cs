using System;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NetworkController : ControllerBase
    {
        private readonly IAttributeMetaDataRepository _attributeMetaDataFileRepo;
        private readonly INetworkRepository _networkRepo;
        private readonly ILogger<NetworkController> _logger;

        public NetworkController(IAttributeMetaDataRepository attributeMetaDataFileRepo,
            INetworkRepository networkRepo,
            ILogger<NetworkController> logger)
        {
            _attributeMetaDataFileRepo = attributeMetaDataFileRepo ?? throw new ArgumentNullException(nameof(attributeMetaDataFileRepo));
            _networkRepo = networkRepo ?? throw new ArgumentNullException(nameof(networkRepo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        [Route("CreateNetwork")]
        public IActionResult CreateNetwork([FromBody] string name)
        {
            try
            {
                // get network definition attribute from json file
                var attribute = _attributeMetaDataFileRepo.GetNetworkAttribute();

                // throw an exception if not network definition attribute is present
                if (attribute == null)
                {
                    throw new InvalidOperationException("Network definition rules do not exist.");
                }

                // create network domain model from attribute data created from the network attribute
                var network = NetworkFactory.CreateNetworkFromAttributeDataRecords(
                    AttributeDataBuilder.GetData(AttributeConnectionBuilder.Build(attribute)));
                network.Name = name;

                // insert network domain data into the data source
                _networkRepo.CreateNetwork(network);

                _logger.LogInformation($"A network with name : {network.Name} has been created.");

                // [TODO] Create DTO to return network information necessary to be stored in the UI
                // for future reference.
                return Ok($"Network {network.Name} with ID {network.Id} and {network.MaintainableAssets.Count()} maintainable assets was created successfully.");
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }
    }
}
