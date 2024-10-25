using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Data;
using AppliedResearchAssociates.iAM.Data.Attributes;
using AppliedResearchAssociates.iAM.Data.Mappers;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Controllers.BaseController;
using BridgeCareCore.Models;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
using BridgeCareCore.Services;
using BridgeCareCore.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BridgeCareCore.Interfaces;
using Microsoft.SqlServer.Dac.Model;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NetworkController : BridgeCareCoreBaseController
    {
        public const string NetworkError = "Network Error";
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        private readonly IGeneralWorkQueueService _workQueueService;
        public NetworkController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor, IGeneralWorkQueueService workQueueService) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            _workQueueService = workQueueService ?? throw new ArgumentNullException(nameof(workQueueService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

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
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{NetworkError}::AllNetworks - {e.Message}", e);
                return Ok();
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
        public IActionResult CreateNetwork(string networkName, NetworkCreationParameters parameters)
        {
            try
            {
                var idAttribute = AttributeService.ConvertAllAttribute(parameters.NetworkDefinitionAttribute);

                var attribute = AttributeDtoDomainMapper.ToDomain(idAttribute, UnitOfWork.EncryptionKey);
                // throw an exception if not network definition attribute is present
                if (attribute == null || string.IsNullOrEmpty(parameters.DefaultEquation))
                {
                    throw new InvalidOperationException("Network definition rules do not exist, or the default equation is not specified");
                }

                // create network domain model from attribute data created from the network attribute
                var allDataSource = parameters.NetworkDefinitionAttribute.DataSource;
                var mappedDataSource = AllDataSourceMapper.ToSpecificDto(allDataSource);
                var attributeConnection = AttributeConnectionBuilder.Build(attribute, mappedDataSource, UnitOfWork);
                var attributeData = AttributeDataBuilder.GetData(attributeConnection);

                if (!attributeData.Any())
                {
                    // Send an error message with HubService if attributeData is empty
                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError,
                        $"{NetworkError}::CreateNetwork {networkName} - No data found for the network attributes.");

                    // Return a BadRequest to indicate that network creation is not possible due to missing data
                    return BadRequest("No data found for the network attributes.");
                }

                var network = NetworkFactory.CreateNetworkFromAttributeDataRecords(attributeData, parameters.DefaultEquation);
                network.Name = networkName;
                network.KeyAttributeId = parameters.NetworkDefinitionAttribute.Id;

                // insert network domain data into the data source
                UnitOfWork.NetworkRepo.CreateNetwork(network);

                // [TODO] Create DTO to return network information necessary to be stored in the UI
                // for future reference.
                return Ok(network.Id);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{NetworkError}::CreateNetwork {networkName} - {e.Message}", e);
            }
            return Ok();
        }

        [HttpPost]
        [Route("DeleteNetwork/{networkId}")]
        [ClaimAuthorize("NetworkDeleteAccess")]
        public async Task<IActionResult> DeleteNetwork(Guid networkId)
        {
            try
            {
                var networkName = "";
                await Task.Factory.StartNew(() =>
                {
                    networkName = UnitOfWork.NetworkRepo.GetNetworkName(networkId);
                });
                DeleteNetworkWorkitem workItem = new DeleteNetworkWorkitem(networkId, UserInfo.Name, networkName);
                var analysisHandle = _workQueueService.CreateAndRun(workItem);
                
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastWorkQueueUpdate, null);

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{NetworkError}::DeleteNetwork - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{NetworkError}::DeleteNetwork - {e.Message}", e);
            }
            return Ok();
        }

        [HttpPost]
        [Route("EditNetworkName/{networkId}/{newNetworkName}")]
        [ClaimAuthorize("EditNetworkNameAccess")]
        public async Task<IActionResult> EditNetworkName(Guid networkId, string newNetworkName)
        {
            // Get the correct entity
            var entity = _unitOfWork.Context.Network.SingleOrDefault(n => n.Id == networkId);

            // Check if the entity exists
            if (entity == null)
            {
                return NotFound("Network not found");
            }

            // Update the network's name
            entity.Name = newNetworkName;

            // Save changes to the database
            await _unitOfWork.Context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        [Route("GetCompatibleNetworks/{networkId}")]
        [ClaimAuthorize("NetworkViewAccess")]
        public async Task<IActionResult> GetCompatibleNetworks(Guid networkId)
        {
             
            try
            {
                // var attributesForOriginalNetwork = UnitOfWork.AttributeRepo.GetAttributeIdsInNetwork(networkId);
                var networks = await UnitOfWork.NetworkRepo.Networks();
                var originalNetwork = networks.First(_ => _.Id == networkId);
                var compatibleNetworks = new List<NetworkDTO>();
                compatibleNetworks.Add(originalNetwork);

            /* TODO: Support cross network cloning. Disabling check until implemented.
                foreach (var network in networks)
                {
                    if(network.Id == networkId)
                        continue;
                    if (network.KeyAttribute != originalNetwork.KeyAttribute)
                        continue;
                    var attributesForNetwork = UnitOfWork.AttributeRepo.GetAttributeIdsInNetwork(network.Id);

                        if (attributesForOriginalNetwork.TrueForAll(_ => attributesForNetwork.Any(__ => _ == __))) {
                            compatibleNetworks.Add(network);
                        }

                    }
            */

                return Ok(compatibleNetworks);

            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{NetworkError}::GetCompatibleNetworks - {e.Message}", e);
            }
            return Ok();
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
                    UnitOfWork.BenefitQuantifierRepo.UpsertBenefitQuantifier(dto);
                });

                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{NetworkError}::UpsertBenefitQuantifier - {e.Message}", e);
            }
            return Ok();
        }
    }
}
