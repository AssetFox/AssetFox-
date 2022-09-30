using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
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
using BridgeCareCore.Utils.Interfaces;

using Policy = BridgeCareCore.Security.SecurityConstants.Policy;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RemainingLifeLimitController : BridgeCareCoreBaseController
    {
        private Guid UserId => UnitOfWork.CurrentUser?.Id ?? Guid.Empty;
        private readonly IClaimHelper _claimHelper;

        public RemainingLifeLimitController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor, IClaimHelper claimHelper) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            _claimHelper = claimHelper ?? throw new ArgumentNullException(nameof(claimHelper));
        }        

        [HttpGet]
        [Route("GetRemainingLifeLimitLibraries")]
        [Authorize(Policy = Policy.ViewRemainingLifeLimitFromLibrary)]
        public async Task<IActionResult> RemainingLifeLimitLibraries()
        {
            try
            {
                var result = new List<RemainingLifeLimitLibraryDTO>();
                await Task.Factory.StartNew(() =>
                {
                    result = GetAllRemainingLifeLimitLibrariesWithRemainingLifeLimits();
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        result = result.Where(_ => _.Owner == UserId || _.IsShared == true).ToList();
                    }
                });

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Remaining Life Limit error::{e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetScenarioRemainingLifeLimits/{simulationId}")]
        [Authorize(Policy = Policy.ViewRemainingLifeLimitFromScenario)]
        public async Task<IActionResult> GetScenarioRemainingLifeLimits(Guid simulationId)
        {
            try
            {
                var result = new List<RemainingLifeLimitDTO>();
                await Task.Factory.StartNew(() =>
                {
                    _claimHelper.CheckUserSimulationReadAuthorization(simulationId, UserId);
                    result = UnitOfWork.RemainingLifeLimitRepo.GetScenarioRemainingLifeLimits(simulationId);
                });

                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Remaining life limit error::{e.Message}");
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Remaining life limit error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertRemainingLifeLimitLibrary/")]
        [Authorize(Policy = Policy.ModifyRemainingLifeLimitFromLibrary)]
        public async Task<IActionResult> UpsertRemainingLifeLimitLibrary(RemainingLifeLimitLibraryDTO dto)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    var currentRecord = UnitOfWork.RemainingLifeLimitRepo
                   .RemainingLifeLimitLibrariesWithRemainingLifeLimits().FirstOrDefault(_ => _.Id == dto.Id);
                    // by pass owner check if no record
                    if (currentRecord != null)
                    {
                        _claimHelper.CheckUserLibraryModifyAuthorization(currentRecord.Owner, UserId);
                    }
                    UnitOfWork.RemainingLifeLimitRepo.UpsertRemainingLifeLimitLibrary(dto);
                    UnitOfWork.RemainingLifeLimitRepo.UpsertOrDeleteRemainingLifeLimits(dto.RemainingLifeLimits, dto.Id);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Remaining life limit error::{e.Message}");
                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Remaining Life Limit error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertScenarioRemainingLifeLimits/{simulationId}")]
        [Authorize(Policy = Policy.ModifyRemainingLifeLimitFromScenario)]
        public async Task<IActionResult> UpsertScenarioRemainingLifeLimits(Guid simulationId, List<RemainingLifeLimitDTO> dtos)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    _claimHelper.CheckUserSimulationModifyAuthorization(simulationId, UserId);
                    UnitOfWork.RemainingLifeLimitRepo.UpsertOrDeleteScenarioRemainingLifeLimits(dtos, simulationId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Remaining life limit error::{e.Message}");
                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Target condition goal error::{e.Message}");
                throw;
            }
        }

        [HttpDelete]
        [Route("DeleteRemainingLifeLimitLibrary/{libraryId}")]
        [Authorize(Policy = Policy.DeleteRemainingLifeLimitFromLibrary)]
        public async Task<IActionResult> DeleteRemainingLifeLimitLibrary(Guid libraryId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        var dto = GetAllRemainingLifeLimitLibrariesWithRemainingLifeLimits()
                        .FirstOrDefault(_ => _.Id == libraryId);
                        if (dto == null) return;
                        _claimHelper.CheckUserLibraryModifyAuthorization(dto.Owner, UserId);
                    }
                    UnitOfWork.RemainingLifeLimitRepo.DeleteRemainingLifeLimitLibrary(libraryId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Remaining life limit error::{e.Message}");
                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Remaining Life Limit error::{e.Message}");
                throw;
            }
        }

        private List<RemainingLifeLimitLibraryDTO> GetAllRemainingLifeLimitLibrariesWithRemainingLifeLimits()
        {
            return UnitOfWork.RemainingLifeLimitRepo.RemainingLifeLimitLibrariesWithRemainingLifeLimits();
        }
    }
}
