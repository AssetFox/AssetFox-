using System;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Controllers.BaseController;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BenefitQuantifierController : BridgeCareCoreBaseController
    {
        public const string BenefitQuantifierError = "Benefit Quantifier Error";
        public BenefitQuantifierController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor) { }

        [HttpGet]
        [Route("GetBenefitQuantifier/{networkId}")]
        [Authorize]
        public async Task<IActionResult> GetBenefitQuantifier(Guid networkId)
        {
            try
            {
                var result = await Task.Factory.StartNew(() =>
                    UnitOfWork.BenefitQuantifierRepo.GetBenefitQuantifier(networkId));

                return Ok(result);
            }
            catch (Exception e)
            {
                var networkName = UnitOfWork.NetworkRepo.GetNetworkNameOrId(networkId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{BenefitQuantifierError}::GetBenefitQuantifier for {networkName} - {e.Message}");
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
                    UnitOfWork.BenefitQuantifierRepo.UpsertBenefitQuantifierAtomically(dto);
                });

                return Ok();
            }
            catch (Exception e)
            {
                var expression = dto?.Equation?.Expression ?? "null";
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{BenefitQuantifierError}::UpsertBenefitQuantifier {expression} - {e.Message}");
                throw;
            }
        }

        [HttpDelete]
        [Route("DeleteBenefitQuantifier/{networkId}")]
        [Authorize]
        public async Task<IActionResult> DeleteBenefitQuantifier(Guid networkId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    UnitOfWork.BenefitQuantifierRepo.DeleteBenefitQuantifier(networkId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                var networkName = UnitOfWork.NetworkRepo.GetNetworkNameOrId(networkId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{BenefitQuantifierError}::DeleteBenefitQuantifier {networkName} - {e.Message}");
                throw;
            }
        }
    }
}
