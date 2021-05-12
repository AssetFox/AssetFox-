using System;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Controllers.BaseController;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NetworkController : BridgeCareCoreBaseController
    {
        private readonly IEsecSecurity _esecSecurity;


        public NetworkController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor) { }

        [HttpGet]
        [Route("GetAllNetworks")]
        [Authorize]
        public async Task<IActionResult> AllNetworks()
        {
            try
            {
                var result = await UnitOfWork.NetworkRepo.Networks();
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Network error::{e.Message}");
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
                var result = await Task.Factory.StartNew(() =>
                {
                    // get network definition attribute from json file
                    var networkSettings = UnitOfWork.AttributeMetaDataRepo.GetNetworkDefinitionAttribute();

                    // throw an exception if not network definition attribute is present
                    if (networkSettings.Attribute == null || string.IsNullOrEmpty(networkSettings.DefaultEquation))
                    {
                        throw new InvalidOperationException("Network definition rules do not exist, or the default equation is not specified");
                    }

                    // create network domain model from attribute data created from the network attribute
                    var network = NetworkFactory.CreateNetworkFromAttributeDataRecords(
                        AttributeDataBuilder.GetData(AttributeConnectionBuilder.Build(networkSettings.Attribute)), networkSettings.DefaultEquation);
                    network.Name = networkName;

                    // insert network domain data into the data source
                    UnitOfWork.NetworkRepo.CreateNetwork(network);

                    // [TODO] Create DTO to return network information necessary to be stored in the UI
                    // for future reference.
                    return network.Id;
                });

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Network error::{e.Message}");
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
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    UnitOfWork.BenefitQuantifierRepo.UpsertBenefitQuantifier(dto);
                    UnitOfWork.Commit();
                });


                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Network error::{e.Message}");
                throw;
            }
        }
    }
}
