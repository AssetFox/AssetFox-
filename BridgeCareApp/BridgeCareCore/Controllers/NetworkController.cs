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
        public const string NetworkError = "Network Error";
        public NetworkController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor) { }

        [HttpGet]
        [Route("GetAllNetworks")]
        [ClaimAuthorize("NetworkViewAccess")]
        public async Task<IActionResult> AllNetworks()
        {
            List<NetworkDTO> result;
            try
            {
                result = await UnitOfWork.NetworkRepo.Networks();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{NetworkError}::AllNetworks - {e.Message}");
                throw;
            }


            foreach (var network in result)
            {
                try
                {
                    network.DefaultSpatialWeighting = UnitOfWork.MaintainableAssetRepo.GetPredominantAssetSpatialWeighting(network.Id);
                }
                catch
                {
                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastWarning, $"Unable to get spatial weightings for network {network.Name}");
                    // No throw here.  It is OK if this DTO remains null
                }
            }            

            return Ok(result);
        }

        [HttpPost]
        [Route("CreateNetwork/{networkName}")]
        [ClaimAuthorize("NetworkAddAccess")]
        public async Task<IActionResult> CreateNetwork(string networkName, NetworkCreationParameters parameters)
        {
            try
            {
                var idAttribute = AttributeService.ConvertAllAttribute(parameters.NetworkDefinitionAttribute);

                var attribute = AttributeMapper.ToDomain(idAttribute, UnitOfWork.EncryptionKey);
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
                        AttributeDataBuilder.GetData(AttributeConnectionBuilder.Build(attribute, mappedDataSource, UnitOfWork)), parameters.DefaultEquation);
                    network.Name = networkName;
                    network.KeyAttributeId = parameters.NetworkDefinitionAttribute.Id;

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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{NetworkError}::CreateNetwork {networkName} - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("GetCompatibleNetworks/{networkId}")]
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
                    //TODO: Investigate case where networks have separate key attributes. Disable until handled in 3.1
                    /*
                    var attributesForNetwork = UnitOfWork.AttributeRepo.GetAttributeIdsInNetwork(network.Id);

                    if (attributesForOriginalNetwork.TrueForAll(_ => attributesForNetwork.Any(__ => _ == __))) {
                        compatibleNetworks.Add(network);
                    }
                    */

                    //Placeholder until above enabled
                    if (network.Id == networkId)
                    {
                        compatibleNetworks.Add(network);
                    }
                }
                


                return Ok(compatibleNetworks);

            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{NetworkError}::GetCompatibleNetworks - {e.Message}");
                throw;
            }
        }
        [HttpPost]
        [Route("UpsertBenefitQuantifier")]
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{NetworkError}::UpsertBenefitQuantifier - {e.Message}");
                throw;
            }
        }
    }
}
