using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NetworkController : ControllerBase
    {
        private readonly IAttributeMetaDataRepository _attributeMetaDataFileRepo;
        private readonly INetworkRepository _networkRepo;
        private readonly IAttributeRepository _attributeRepo;
        private readonly ILogger<NetworkController> _logger;

        public NetworkController(IAttributeMetaDataRepository attributeMetaDataFileRepo,
            INetworkRepository networkRepo,
            IAttributeRepository attributeRepo,
            ILogger<NetworkController> logger)
        {
            _attributeMetaDataFileRepo = attributeMetaDataFileRepo ?? throw new ArgumentNullException(nameof(attributeMetaDataFileRepo));
            _networkRepo = networkRepo ?? throw new ArgumentNullException(nameof(networkRepo));
            _attributeRepo = attributeRepo ?? throw new ArgumentNullException(nameof(attributeRepo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        [Route("CreateNetwork")]
        public IActionResult CreateNetwork([FromBody] string name)
        {
            try
            {
                // get attribute meta data from json file
                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? string.Empty,
                    "MetaData//AttributeMetaData", "networkDefinitionRules.json");
                var attributeMetaDatum = _attributeMetaDataFileRepo.All(filePath).FirstOrDefault();

                if(attributeMetaDatum == null)
                {
                    throw new InvalidOperationException("No attribute meta data found");
                }

                // create attribute from meta datum
                var attribute = AttributeFactory.Create(attributeMetaDatum);

                // get attribute data
                var attributeData = AttributeDataBuilder.GetData(AttributeConnectionBuilder.Build(attribute));

                // create network domain from attribute data
                var network = NetworkFactory.CreateNetworkFromAttributeDataRecords(attributeData);
                network.Name = name;

                // insert network domain data into data source
                _networkRepo.CreateNetwork(network);

                _logger.LogInformation($"a network with name : {network.Name} has been created");

                // [TODO] Create DTO to return network information necessary to be stored in the UI for future reference.
                return Ok($"Network {network.Name} with ID {network.Id} and {network.MaintainableAssets.Count()} maintainable assets was created successfully.");
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }
    }
}
