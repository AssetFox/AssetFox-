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
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{UserCriteriaError}::GetUserCriteria - {e.Message}", e);
            }
            return Ok();
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
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{UserCriteriaError}::GetAllUserCriteria - {e.Message}", e);
            }
            return Ok();
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
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{UserCriteriaError}::UpsertUserCriteria for {username} - {e.Message}", e);
            }
            return Ok();
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
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{UserCriteriaError}::RevokeUserAccess - {e.Message}", e);
            }
            return Ok();
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
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{UserCriteriaError}::DeactivateUser - {e.Message}", e);
            }
            return Ok();
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
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{UserCriteriaError}::ReactivateUser - {e.Message}", e);
            }
            return Ok();
        }
    }
}
