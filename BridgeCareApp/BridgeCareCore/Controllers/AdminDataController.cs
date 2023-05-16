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
using BridgeCareCore.Security;
using Humanizer;

namespace BridgeCareCore.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AdminDataController : BridgeCareCoreBaseController
    {
        public const string SiteError = "Site Error";

        public AdminDataController(IEsecSecurity esecSecurity, IUnitOfWork unitOfWork, IHubService hubService, IHttpContextAccessor contextAccessor) :
                         base(esecSecurity, unitOfWork, hubService, contextAccessor)
        { }

        [HttpGet]
        [Route("GetPrimaryNetwork")]
        [Authorize]
        public async Task<IActionResult> GetPrimaryNetwork()
        {
            try
            {
                var name = UnitOfWork.AdminDataRepo.GetPrimaryNetwork();
                return Ok(name);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SiteError}::GetPrimaryNetwork - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("SetPrimaryNetwork/{name}")]
        [ClaimAuthorize("AdminAccess")]
        public async Task<IActionResult> SetPrimaryNetwork(string name)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.AdminDataRepo.SetPrimaryNetwork(name);
                });
                    return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SiteError}::SetPrimaryNetwork - {e.Message}");
                return BadRequest($"{SiteError}::SetPrimaryNetwork - {e.Message}");
            }
        }

    }
}
