using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserCriteriaController : HubControllerBase
    {
        private readonly IEsecSecurity _esecSecurity;
        private readonly UnitOfDataPersistenceWork _unitOfWork;


        public UserCriteriaController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork,
            IHubService hubService, IUserCriteriaRepository repo) : base(hubService)
        {
            _esecSecurity = esecSecurity;
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [HttpGet]
        [Route("GetUserCriteria")]
        [Authorize]
        public async Task<IActionResult> GetUserCriteria()
        {
            try
            {
                var userInfo = _esecSecurity.GetUserInformation(Request);

                var result = await Task.Factory.StartNew(() =>
                {
                    _unitOfWork.BeginTransaction();
                    var userCriteria =
                        _unitOfWork.UserCriteriaRepo.GetOwnUserCriteria(userInfo.ToDto(),
                            SecurityConstants.Role.BAMSAdmin);
                    _unitOfWork.Commit();
                    return userCriteria;
                });

                return Ok(result);
            }
            catch (Exception e)
            {
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"User Criteria error::{e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetAllUserCriteria")]
        [Authorize(Policy = SecurityConstants.Policy.Admin)]
        public async Task<IActionResult> GetAllUserCriteria()
        {
            try
            {
                var result = await Task.Factory.StartNew(() => _unitOfWork.UserCriteriaRepo.GetAllUserCriteria());
                return Ok(result);
            }
            catch (Exception e)
            {
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"User Criteria error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertUserCriteria")]
        [Authorize(Policy = SecurityConstants.Policy.Admin)]
        public async Task<IActionResult> UpsertUserCriteria([FromBody] UserCriteriaDTO userCriteria)
        {
            try
            {
                _unitOfWork.SetUser(_esecSecurity.GetUserInformation(Request).Name);

                await Task.Factory.StartNew(() =>
                {
                    _unitOfWork.BeginTransaction();
                    _unitOfWork.UserCriteriaRepo.UpsertUserCriteria(userCriteria);
                    _unitOfWork.Commit();
                });

                return Ok();
            }
            catch (Exception e)
            {
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"User Criteria error::{e.Message}");
                throw;
            }
        }

        [HttpDelete]
        [Route("RevokeUserAccess/{userCriteriaId}")]
        [Authorize(Policy = SecurityConstants.Policy.Admin)]
        public async Task<IActionResult> RevokeUserAccess(Guid userCriteriaId)
        {
            try
            {
                _unitOfWork.SetUser(_esecSecurity.GetUserInformation(Request).Name);

                await Task.Factory.StartNew(() =>
                {
                    _unitOfWork.BeginTransaction();
                    _unitOfWork.UserCriteriaRepo.RevokeUserAccess(userCriteriaId);
                    _unitOfWork.Commit();
                });

                return Ok();
            }
            catch (Exception e)
            {
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"User Criteria error::{e.Message}");
                throw;
            }
        }

        [HttpDelete]
        [Route("DeleteUser/{userId}")]
        [Authorize(Policy = SecurityConstants.Policy.Admin)]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    _unitOfWork.BeginTransaction();
                    _unitOfWork.UserCriteriaRepo.DeleteUser(userId);
                    _unitOfWork.Commit();
                });

                return Ok();
            }
            catch (Exception e)
            {
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"User Criteria error::{e.Message}");
                throw;
            }
        }
    }
}
