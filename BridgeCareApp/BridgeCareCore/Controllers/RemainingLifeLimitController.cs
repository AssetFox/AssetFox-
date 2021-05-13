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
    public class RemainingLifeLimitController : BridgeCareCoreBaseController
    {
        public RemainingLifeLimitController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor) { }

        [HttpGet]
        [Route("GetRemainingLifeLimitLibraries")]
        [Authorize(Policy = SecurityConstants.Policy.Admin)]
        public async Task<IActionResult> RemainingLifeLimitLibraries()
        {
            try
            {
                var result = await Task.Factory.StartNew(() => UnitOfWork.RemainingLifeLimitRepo
                    .RemainingLifeLimitLibrariesWithRemainingLifeLimits());
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Remaining Life Limit error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertRemainingLifeLimitLibrary/{simulationId}")]
        [Authorize(Policy = SecurityConstants.Policy.Admin)]
        public async Task<IActionResult> UpsertRemainingLifeLimitLibrary(Guid simulationId, RemainingLifeLimitLibraryDTO dto)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    UnitOfWork.RemainingLifeLimitRepo
                        .UpsertRemainingLifeLimitLibrary(dto, simulationId);
                    UnitOfWork.RemainingLifeLimitRepo
                        .UpsertOrDeleteRemainingLifeLimits(dto.RemainingLifeLimits, dto.Id);
                    UnitOfWork.Commit();
                });


                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Remaining Life Limit error::{e.Message}");
                throw;
            }
        }

        [HttpDelete]
        [Route("DeleteRemainingLifeLimitLibrary/{libraryId}")]
        [Authorize(Policy = SecurityConstants.Policy.Admin)]
        public async Task<IActionResult> DeleteRemainingLifeLimitLibrary(Guid libraryId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    UnitOfWork.RemainingLifeLimitRepo.DeleteRemainingLifeLimitLibrary(libraryId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Remaining Life Limit error::{e.Message}");
                throw;
            }
        }
    }
}
