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
    public class AnnouncementController : ControllerBase
    {
        protected readonly UnitOfDataPersistenceWork UnitOfWork;

        protected readonly IHubService HubService;

        protected readonly IHttpContextAccessor ContextAccessor;
        public AnnouncementController(UnitOfDataPersistenceWork unitOfWork,
            IHubService hubService, IHttpContextAccessor contextAccessor)
        {
            UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            HubService = hubService ?? throw new ArgumentNullException(nameof(hubService));
            ContextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
        }

        [HttpGet]
        [Route("GetAnnouncements")]
        //[Authorize]
        public async Task<IActionResult> Announcements()
        {
            try
            {
                var result = await Task.Factory.StartNew(() => UnitOfWork.AnnouncementRepo.Announcements());
                return Ok(result);
            }
            catch (Exception e)
            {
                //HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Announcement error::{e.Message}");
                throw new Exception(e.Message);
            }
        }

        [HttpPost]
        [Route("UpsertAnnouncement")]
        //[Authorize(Policy = SecurityConstants.Policy.Admin)]
        public async Task<IActionResult> UpsertAnnouncement(AnnouncementDTO dto)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    UnitOfWork.AnnouncementRepo.UpsertAnnouncement(dto);
                    UnitOfWork.Commit();
                });


                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                //HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Announcement error::{e.Message}");
                throw new Exception(e.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteAnnouncement/{announcementId}")]
        //[Authorize(Policy = SecurityConstants.Policy.Admin)]
        public async Task<IActionResult> DeleteAnnouncement(Guid announcementId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    UnitOfWork.AnnouncementRepo.DeleteAnnouncement(announcementId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                //HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Announcement error::{e.Message}");
                throw new Exception(e.Message);
            }
        }
    }
}
