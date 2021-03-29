using System;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Logging;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NetworkController : HubControllerBase
    {
        private readonly IEsecSecurity _esecSecurity;
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public NetworkController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork,
            IHubService hubService) : base(hubService)
        {
            _esecSecurity = esecSecurity ?? throw new ArgumentNullException(nameof(esecSecurity));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
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
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Network error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("CreateNetwork/{networkName}")]
        [Authorize]
        public async Task<IActionResult> CreateNetwork(string networkName)
        {
            try
            {
                _unitOfWork.SetUser(_esecSecurity.GetUserInformation(Request).Name);

               var result = await Task.Factory.StartNew(() =>
                {
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
                    _unitOfWork.NetworkRepo.CreateNetwork(network);

                    // [TODO] Create DTO to return network information necessary to be stored in the UI
                    // for future reference.
                    return network.Id;
                });

                return Ok(result);
            }
            catch (Exception e)
            {
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Network error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertBenefitQuantifier")]
        [Authorize]
        public async Task<IActionResult> UpsertBenefitQuantifier([FromBody] BenefitQuantifierDTO dto)
        {
            try
            {
                _unitOfWork.SetUser(_esecSecurity.GetUserInformation(Request).Name);

                await Task.Factory.StartNew(() =>
                {
                    _unitOfWork.BeginTransaction();
                    _unitOfWork.BenefitQuantifierRepo.UpsertBenefitQuantifier(dto);
                    _unitOfWork.Commit();
                });


                return Ok();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Network error::{e.Message}");
                throw;
            }
        }
    }
}
