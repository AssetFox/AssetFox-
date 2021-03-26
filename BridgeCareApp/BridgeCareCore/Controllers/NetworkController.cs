using System;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Logging;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NetworkController : ControllerBase
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;
        private readonly IEsecSecurity _esecSecurity;
        private readonly ILog _log;

        public NetworkController(UnitOfDataPersistenceWork unitOfDataPersistenceWork, ILog log, IEsecSecurity esecSecurity)
        {
            _unitOfWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _esecSecurity = esecSecurity ?? throw new ArgumentNullException(nameof(esecSecurity));
        }

        [HttpGet]
        [Route("GetAllNetworks")]
        [Authorize]
        public async Task<IActionResult> AllNetworks()
        {
            try
            {
                var result = await _unitOfWork.NetworkRepo.Networks();
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
        [Authorize]
        public IActionResult CreateNetwork(string networkName)
        {
            try
            {
                var userInfo = _esecSecurity.GetUserInformation(Request).ToDto();
                // get network definition attribute from json file
                var attribute = _unitOfWork.AttributeMetaDataRepo.GetNetworkDefinitionAttribute();

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
                _unitOfWork.NetworkRepo.CreateNetwork(network, userInfo);

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

        [HttpPost]
        [Route("UpsertBenefitQuantifier")]
        [Authorize]
        public async Task<IActionResult> UpsertBenefitQuantifier([FromBody] BenefitQuantifierDTO dto)
        {
            try
            {
                var userInfo = _esecSecurity.GetUserInformation(Request).ToDto();
                _unitOfDataPersistenceWork.BeginTransaction();
                await Task.Factory.StartNew(() =>
                {
                    _unitOfDataPersistenceWork.BenefitQuantifierRepo.UpsertBenefitQuantifier(dto, userInfo);
                });

                _unitOfDataPersistenceWork.Commit();
                return Ok();
            }
            catch (Exception e)
            {
                _unitOfDataPersistenceWork.Rollback();
                Console.WriteLine(e);
                return BadRequest(e);
            }
        }
    }
}
