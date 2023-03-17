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
    public class AnnouncementController : BridgeCareCoreBaseController
    {
        public const string AnnouncementError = "Announcement Error";
        public AnnouncementController(IEsecSecurity esecSecurity, IUnitOfWork unitOfWork,
            IHubService hubService, IHttpContextAccessor httpContextAccessor) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
        }

        [HttpGet]
        [Route("GetAnnouncements")]
        [ClaimAuthorize("AnnouncementViewAccess")]
        public async Task<IActionResult> Announcements()
        {
            try
            {
                var result = await Task.Factory.StartNew(() => UnitOfWork.AnnouncementRepo.Announcements());
                return Ok(result);
            }
            catch (Exception e)
            {
                var t = e.InnerException;
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{AnnouncementError}::GetAnnouncements - {e.Message}");
                throw new Exception(e.Message);
            }
        }

        [HttpPost]
        [Route("UpsertAnnouncement")]
        [ClaimAuthorize("AnnouncementModifyAccess")]
        public async Task<IActionResult> UpsertAnnouncement(AnnouncementDTO dto)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.AnnouncementRepo.UpsertAnnouncement(dto);
                });


                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{AnnouncementError}::UpsertAnnouncement - {e.Message}");
                throw new Exception(e.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteAnnouncement/{announcementId}")]
        [ClaimAuthorize("AnnouncementModifyAccess")]
        public async Task<IActionResult> DeleteAnnouncement(Guid announcementId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.AnnouncementRepo.DeleteAnnouncement(announcementId);
                });

                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{AnnouncementError}::DeleteAnnouncement - {e.Message}");
                throw new Exception(e.Message);
            }
        }
    }
}
