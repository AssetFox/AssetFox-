using System;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Controllers.BaseController;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"User error::{e.Message}");
                throw;
            }
        }

        [HttpPut]
        [Route("UpdateUser")]
        [Authorize(Policy = SecurityConstants.Policy.Admin)]
        public async Task<IActionResult> UpdateUser([FromBody] UserDTO dto)
        {
            try
            {
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"User error::{e.Message}");
                throw;
            }
        }

        [HttpDelete]
        [Route("DeleteUser/{username}")]
        [Authorize(Policy = SecurityConstants.Policy.Admin)]
        public async Task<IActionResult> DeleteUser(string username)
        {
            try
            {
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"User error::{e.Message}");
                throw;
            }
        }
    }
}
