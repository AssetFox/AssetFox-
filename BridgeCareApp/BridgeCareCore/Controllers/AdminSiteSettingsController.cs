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
using System.Data;
using System.Drawing;
using static BridgeCareCore.Security.SecurityConstants;

namespace BridgeCareCore.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AdminSiteSettingsController : BridgeCareCoreBaseController
    {
        public const string AdminSiteSettingsError = "Admin Site Settings Error";

        public AdminSiteSettingsController(IEsecSecurity esecSecurity, IUnitOfWork unitOfWork, IHubService hubService, IHttpContextAccessor contextAccessor):
                         base(esecSecurity, unitOfWork, hubService, contextAccessor) { }

        [HttpGet]
        [Route("GetImplementationName")]
        public async Task<IActionResult> GetImplementationName()
        {
            try
            {
                var name = UnitOfWork.AdminSettingsRepo.GetImplementationName();
                return Ok(name);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{AdminSiteSettingsError}::GetImplementationName - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("SetImplementationName/{name}")]
        [Authorize(Policy = Policy.ModifyAdminSiteSettings)]
        public async Task<IActionResult> SetImplementationName(string name)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.AdminSettingsRepo.SetImplementationName(name);
                });
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{AdminSiteSettingsError}::SetImplementationName - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetAgencyLogo")]
        public async Task<IActionResult> GetAgencyLogo()
        {
            try
            {
                var result = await Task.Factory.StartNew(() => UnitOfWork.AdminSettingsRepo.GetAgencyLogo());
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{AdminSiteSettingsError}::GetAgencyLogo - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("SetAgencyLogo")]
        [Authorize(Policy = Policy.ModifyAdminSiteSettings)]
        public async Task<IActionResult> SetAgencyLogo()
        {
            try
            {
                if (!ContextAccessor.HttpContext.Request.HasFormContentType)
                    throw new ConstraintException("Request MIME type is invalid.");
                if (ContextAccessor.HttpContext.Request.Form.Files.Count < 1)
                    throw new ConstraintException("Attributes file not found.");
                //https://stackoverflow.com/questions/8848725/asp-net-c-sharp-convert-filestream-to-image
                Image logo = Image.FromStream(ContextAccessor.HttpContext.Request.Form.Files[0].OpenReadStream());
                await Task.Factory.StartNew(() => UnitOfWork.AdminSettingsRepo.SetAgencyLogo(logo));
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{AdminSiteSettingsError}::SetAgencyLogo - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetImplementationLogo")]
        public async Task<IActionResult> GetImplementationLogo()
        {
            try
            {
                var result = await Task.Factory.StartNew(() => UnitOfWork.AdminSettingsRepo.GetImplementationLogo());
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{AdminSiteSettingsError}::GetImplementationLogo - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("SetImplementationLogo")]
        [Authorize(Policy = Policy.ModifyAdminSiteSettings)]
        public async Task<IActionResult> SetImplementationLogo()
        {
            try
            {
                if (!ContextAccessor.HttpContext.Request.HasFormContentType)
                    throw new ConstraintException("Request MIME type is invalid.");
                if (ContextAccessor.HttpContext.Request.Form.Files.Count < 1)
                    throw new ConstraintException("Attributes file not found.");
                Image logo = Image.FromStream(ContextAccessor.HttpContext.Request.Form.Files[0].OpenReadStream());
                await Task.Factory.StartNew( () => UnitOfWork.AdminSettingsRepo.SetImplementationLogo(logo) );
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{AdminSiteSettingsError}::SetImplementationLogo - {e.Message}");
                throw;
            }
        }
    }
}
