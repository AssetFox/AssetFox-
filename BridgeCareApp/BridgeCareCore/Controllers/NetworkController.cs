﻿using System;
using System.Collections.Generic;
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

        [HttpGet]
        [Route("GetAllNetworks")]
        public IActionResult GetAllNetworks()
        {
            try
            {
                var networks = _networkRepo.GetAllNetworks();
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
        [Route("CreateNetwork/{networkName}")]
        public IActionResult CreateNetwork(string networkName)
        {
            try
            {
                // get network definition attribute from json file
                var attribute = _attributeMetaDataFileRepo.GetNetworkDefinitionAttribute();

                // throw an exception if not network definition attribute is present
                if (attribute == null)
                {
                    throw new InvalidOperationException("Network definition rules do not exist.");
                }

                // create network domain model from attribute data created from the network attribute
                var network = NetworkFactory.CreateNetworkFromAttributeDataRecords(
                    AttributeDataBuilder.GetData(AttributeConnectionBuilder.Build(attribute)));
                network.Name = networkName;

                // insert network domain data into the data source
                _networkRepo.CreateNetwork(network);

                _logger.LogInformation($"A network with name : {network.Name} has been created.");

                // [TODO] Create DTO to return network information necessary to be stored in the UI
                // for future reference.
                return Ok(network.Id);
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }
        public class NetworkName
        {
            internal string name { get; set; }
        }
    }
}
