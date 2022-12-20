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

using Policy = BridgeCareCore.Security.SecurityConstants.Policy;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BridgeCareCoreBaseController
    {
        public UserController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor) { }

        [HttpGet]
        [Route("GetAllUsers")]
        [Authorize]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var result = await UnitOfWork.UserRepo.GetAllUsers();
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"User error::GetAllUsers - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetUserByUserName/{userName}")]
        [Authorize]
        public async Task<IActionResult> GetUserByUserName( string userName)
        {
            try
            {
                var result = await UnitOfWork.UserRepo.GetUserByUserName(userName);
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"User error::GetUserByUserName - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetUserById/{userName}")]
        [Authorize]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            try
            {
                var result = await UnitOfWork.UserRepo.GetUserById(id);
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"User error::GetUserById - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpdateLastNewsAccessDate")]
        [Authorize]
        public async Task<IActionResult> UpdateLastNewsAccessDate([FromBody] LastNewsAccessDateDTO dto)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    UnitOfWork.UserRepo.UpdateLastNewsAccessDate(dto.Id, dto.LastNewsAccessDate);
                    UnitOfWork.Commit();
                });
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"User error::UpdateLastNewsAccessDate - {e.Message}");
                throw;
            }
        }

        [HttpPut]
        [Route("UpdateUser")]
        [ClaimAuthorize("AdminAccess")]
        public async Task<IActionResult> UpdateUser([FromBody] UserDTO dto)
        {
            var username = dto?.Username ?? "null";
            try
            {
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"User error::UpdateUser {username} - {e.Message}");
                throw;
            }
        }

        [HttpDelete]
        [Route("DeleteUser/{username}")]
        [ClaimAuthorize("AdminAccess")]
        public async Task<IActionResult> DeleteUser(string username)
        {
            try
            {
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"User error::DeleteUser {username} - {e.Message}");
                throw;
            }
        }
    }
}
