using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.Data;
using AppliedResearchAssociates.iAM.Data.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Controllers.BaseController;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Security.Interfaces;
using BridgeCareCore.Services;
using BridgeCareCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using BridgeCareCore.Utils;
using BridgeCareCore.Security;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NetworkController : BridgeCareCoreBaseController
    {
        public NetworkController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor) { }

        [HttpGet]
        [Route("GetAllNetworks")]
        //[Authorize]
        [ClaimAuthorize("NetworkViewAccess")]
        public async Task<IActionResult> AllNetworks()
        {
            try
            {
                var result = await UnitOfWork.NetworkRepo.Networks();
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Network error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("CreateNetwork/{networkName}")]
        //[Authorize]
        [ClaimAuthorize("NetworkAddAccess")]
        public async Task<IActionResult> CreateNetwork(string networkName, NetworkCreationParameters parameters)
        {
            try
            {
                var idAttribute = AttributeService.ConvertAllAttribute(parameters.NetworkDefinitionAttribute);

                var attribute = AttributeMapper.ToDomain(idAttribute);
                var result = await Task.Factory.StartNew(() =>
                {
                    // throw an exception if not network definition attribute is present
                    if (attribute == null || string.IsNullOrEmpty(parameters.DefaultEquation))
                    {
                        throw new InvalidOperationException("Network definition rules do not exist, or the default equation is not specified");
                    }

                    // create network domain model from attribute data created from the network attribute
                    var allDataSource = parameters.NetworkDefinitionAttribute.DataSource;
                    var mappedDataSource = AllDataSourceMapper.ToSpecificDto(allDataSource);
                    var network = NetworkFactory.CreateNetworkFromAttributeDataRecords(
                        AttributeDataBuilder.GetData(AttributeConnectionBuilder.Build(attribute, parameters.NetworkDefinitionAttribute.DataSource, UnitOfWork)), parameters.DefaultEquation);
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Network error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("GetCompatibleNetworks/{networkId}")]
        //[Authorize]
        [ClaimAuthorize("NetworkViewAccess")]
        public async Task<IActionResult> GetCompatibleNetworks(Guid networkId)
        {
            try
            {
                var attributesForOriginalNetwork = UnitOfWork.AttributeRepo.GetAttributeIdsInNetwork(networkId);
                var networks = await UnitOfWork.NetworkRepo.Networks();

                var compatibleNetworks = new List<NetworkDTO>();

                foreach (var network in networks)
                {
                    var attributesForNetwork = UnitOfWork.AttributeRepo.GetAttributeIdsInNetwork(network.Id);

                    if (attributesForOriginalNetwork.TrueForAll(_ => attributesForNetwork.Any(__ => _ == __))) {
                        compatibleNetworks.Add(network);
                    }
                }

                return Ok(compatibleNetworks);

            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Network error::{e.Message}");
                throw;
            }
        }
        [HttpPost]
        [Route("UpsertBenefitQuantifier")]
        //[Authorize]
        [ClaimAuthorize("NetworkAggregateAccess")]
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Network error::{e.Message}");
                throw;
            }
        }
    }
}
