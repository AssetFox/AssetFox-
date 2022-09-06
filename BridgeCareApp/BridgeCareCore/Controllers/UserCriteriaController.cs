using System;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Controllers.BaseController;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserCriteriaController : BridgeCareCoreBaseController
    {

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
                    UnitOfWork.BeginTransaction();
                    var userCriteria = UnitOfWork.UserCriteriaRepo
                        .GetOwnUserCriteria(UserInfo.ToDto(), SecurityConstants.Role.BAMSAdmin);
                    UnitOfWork.Commit();
                    return userCriteria;
                });

                return Ok(result);
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"User Criteria error::{e.Message}");
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"User Criteria error::{e.Message}");
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
                    UnitOfWork.BeginTransaction();
                    UnitOfWork.UserCriteriaRepo.UpsertUserCriteria(userCriteria);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"User Criteria error::{e.Message}");
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
                    UnitOfWork.BeginTransaction();
                    UnitOfWork.UserCriteriaRepo.RevokeUserAccess(userCriteriaId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"User Criteria error::{e.Message}");
                throw;
            }
        }

        [HttpDelete]
        [Route("DeleteUser/{userId}")]
        [ClaimAuthorize("UserCriteriaModifyAccess")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    UnitOfWork.UserCriteriaRepo.DeleteUser(userId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"User Criteria error::{e.Message}");
                throw;
            }
        }
    }
}
