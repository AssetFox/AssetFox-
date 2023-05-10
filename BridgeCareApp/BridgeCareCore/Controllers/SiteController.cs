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
    public class SiteController : BridgeCareCoreBaseController
    {
        public const string SiteError = "Site Error";

        public SiteController(IEsecSecurity esecSecurity, IUnitOfWork unitOfWork, IHubService hubService, IHttpContextAccessor contextAccessor):
                         base(esecSecurity, unitOfWork, hubService, contextAccessor) { }

        [HttpGet]
        [Route("GetImplementationName")]
        [Authorize]
        public async Task<IActionResult> GetImplementationName()
        {
            try
            {
                var name = UnitOfWork.SiteRepo.GetImplementationName();
                return Ok(name);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SiteError}::GetImplementationName - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("SetImplementationName/{name}")]
        [ClaimAuthorize("AdminAccess")]
        public async Task<IActionResult> SetImplementationName(string name)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.SiteRepo.SetImplementationName(name);
                });
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SiteError}::SetImplementationName - {e.Message}");
                throw;
            }
        }

        //[HttpPost]
        [HttpPut]
        [Route("SetAgencyLogo/{agencyLogo}")]
        [Authorize]
        public async Task<IActionResult> SetAgencyLogo(string agencyLogo)
        {
            try
            {
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SiteError}::SetAgencyLogo - {e.Message}");
                throw;
            }
        }

        //[HttpPost]
        [HttpPut]
        [Route("SetImplementationLogo/{productLogo}")]
        [Authorize]
        public async Task<IActionResult> SetImplementationLogo(string productLogo)
        {
            try
            {
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SiteError}::SetImplementationLogo - {e.Message}");
                throw;
            }
        }
    }
}
