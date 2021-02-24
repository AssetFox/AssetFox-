using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NetworkController : ControllerBase
    {
        private readonly UnitOfDataPersistenceWork _unitOfDataPersistenceWork;
        private readonly ILog _log;

        public NetworkController(UnitOfDataPersistenceWork unitOfDataPersistenceWork, ILog log)
        {
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        [HttpGet]
        [Route("GetAllNetworks")]
        public async Task<IActionResult> AllNetworks()
        {
            try
            {
                var result = await _unitOfDataPersistenceWork.NetworkRepo.Networks();
                return Ok(result);
            }
            catch (Exception e)
            {
                _log.Error($"GetAllNetworks Error => {e.Message}::{e.StackTrace}");
                return BadRequest(e);
            }
        }

        [HttpPost]
        [Route("CreateNetwork/{networkName}")]
        public IActionResult CreateNetwork(string networkName)
        {
            try
            {
                // get network definition attribute from json file
                var attribute = _unitOfDataPersistenceWork.AttributeMetaDataRepo.GetNetworkDefinitionAttribute();

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
                _unitOfDataPersistenceWork.NetworkRepo.CreateNetwork(network);

                // [TODO] Create DTO to return network information necessary to be stored in the UI
                // for future reference.
                return Ok(network.Id);
            }
            catch (Exception e)
            {
                _log.Error($"CreateNetwork Error => { e.Message}::{ e.StackTrace}");
                return StatusCode(500, $"{e.Message}::{e.StackTrace}");
            }
        }
        public class NetworkName
        {
            internal string name { get; set; }
        }
    }
}
