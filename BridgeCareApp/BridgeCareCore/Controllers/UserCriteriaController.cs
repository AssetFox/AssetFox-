using System;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Controllers.BaseController;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserCriteriaController : BridgeCareCoreBaseController
    {
        public const string UserCriteriaError = "User Criteria Error";

        public UserCriteriaController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor) { }

        [HttpGet]
        [Route("GetUserCriteria")]
        [ClaimAuthorize("UserCriteriaViewAccess")]
        public async Task<IActionResult> GetUserCriteria()
        {
            try
            {
                var result = await Task.Factory.StartNew(() =>
                {
                    var userCriteria = UnitOfWork.UserCriteriaRepo.GetOwnUserCriteria(UserInfo.ToDto());
                    return userCriteria;
                });

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{UserCriteriaError}::GetUserCriteria - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetAllUserCriteria")]
        [ClaimAuthorize("UserCriteriaViewAccess")]
        public async Task<IActionResult> GetAllUserCriteria()
        {
            try
            {
                var result = await Task.Factory.StartNew(() => UnitOfWork.UserCriteriaRepo.GetAllUserCriteria());
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{UserCriteriaError}::GetAllUserCriteria - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertUserCriteria")]
        [ClaimAuthorize("UserCriteriaModifyAccess")]
        public async Task<IActionResult> UpsertUserCriteria([FromBody] UserCriteriaDTO userCriteria)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.UserCriteriaRepo.UpsertUserCriteria(userCriteria);
                });

                return Ok();
            }
            catch (Exception e)
            {
                var username = userCriteria?.UserName ?? "null";
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{UserCriteriaError}::UpsertUserCriteria for {username} - {e.Message}");
                throw;
            }
        }

        [HttpDelete]
        [Route("RevokeUserAccess/{userCriteriaId}")]
        [ClaimAuthorize("UserCriteriaModifyAccess")]
        public async Task<IActionResult> RevokeUserAccess(Guid userCriteriaId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.UserCriteriaRepo.RevokeUserAccess(userCriteriaId);
                });

                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{UserCriteriaError}::RevokeUserAccess - {e.Message}");
                throw;
            }
        }

        [HttpDelete]
        [Route("DeactivateUser/{userId}")]
        [ClaimAuthorize("UserCriteriaModifyAccess")]
        public async Task<IActionResult> DeactivateUser(Guid userId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.UserCriteriaRepo.DeactivateUser(userId);
                });

                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{UserCriteriaError}::DeactivateUser - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("ReactivateUser/{userId}")]
        [ClaimAuthorize("UserCriteriaModifyAccess")]
        public async Task<IActionResult> ReactivateUser(Guid userId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.UserCriteriaRepo.ReactivateUser(userId);
                });

                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{UserCriteriaError}::ReactivateUser - {e.Message}");
                throw;
            }
        }
    }
}
