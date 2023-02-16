using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Controllers.BaseController;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BridgeCareCore.Utils.Interfaces;
using Policy = BridgeCareCore.Security.SecurityConstants.Policy;
using BridgeCareCore.Models;
using BridgeCareCore.Interfaces;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RemainingLifeLimitController : BridgeCareCoreBaseController
    {
        public const string RemainingLifeLimitError = "Remaining Life Limit Error";
        private Guid UserId => UnitOfWork.CurrentUser?.Id ?? Guid.Empty;
        private readonly IClaimHelper _claimHelper;
        private readonly IRemainingLifeLimitPagingService _remainingLIfeLimitService;

        public RemainingLifeLimitController(IEsecSecurity esecSecurity, IUnitOfWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor, IClaimHelper claimHelper,
            IRemainingLifeLimitPagingService remainingLifeService) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            _claimHelper = claimHelper ?? throw new ArgumentNullException(nameof(claimHelper));
            _remainingLIfeLimitService = remainingLifeService ?? throw new ArgumentNullException(nameof(remainingLifeService));
        }


        [HttpPost]
        [Route("GetScenarioRemainingLifeLimitPage/{simulationId}")]
        [Authorize(Policy = Policy.ModifyRemainingLifeLimitFromScenario)]
        public async Task<IActionResult> GetScenarioRemainingLifeLimitPage(Guid simulationId, PagingRequestModel<RemainingLifeLimitDTO> pageRequest)
        {
            try
            {
                var result = new PagingPageModel<RemainingLifeLimitDTO>();
                await Task.Factory.StartNew(() =>
                {
                    _claimHelper.CheckUserSimulationReadAuthorization(simulationId, UserId);
                    result = _remainingLIfeLimitService.GetScenarioPage(simulationId, pageRequest);
                });

                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{RemainingLifeLimitError}::GetScenarioRemainingLifeLimitPage for {simulationName} - {e.Message}");
                throw;
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{RemainingLifeLimitError}::GetScenarioRemainingLifeLimitPage for {simulationName} - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("GetLibraryRemainingLifeLimitPage/{libraryId}")]
        [Authorize(Policy = Policy.ViewRemainingLifeLimitFromLibrary)]
        public async Task<IActionResult> GetLibraryRemainingLifeLimitPage(Guid libraryId, PagingRequestModel<RemainingLifeLimitDTO> pageRequest)
        {
            try
            {
                var result = new PagingPageModel<RemainingLifeLimitDTO>();
                await Task.Factory.StartNew(() =>
                {
                    result = _remainingLIfeLimitService.GetLibraryPage(libraryId, pageRequest);
                });

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{RemainingLifeLimitError} ::GetLibraryRemainingLifeLimitPage - {e.Message}");
                throw;
            }
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
                    result = UnitOfWork.RemainingLifeLimitRepo.GetAllRemainingLifeLimitLibrariesNoChildren();
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        result = result.Where(_ => _.Owner == UserId || _.IsShared == true).ToList();
                    }
                });

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{RemainingLifeLimitError}::RemainingLifeLimitLibraries - {e.Message}");
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
            catch (UnauthorizedAccessException)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{RemainingLifeLimitError}::GetScenarioRemainingLifeLimits for {simulationName} - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{RemainingLifeLimitError}::GetScenarioRemainingLifeLimits for {simulationName} - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertRemainingLifeLimitLibrary/")]
        [Authorize(Policy = Policy.ModifyRemainingLifeLimitFromLibrary)]
        public async Task<IActionResult> UpsertRemainingLifeLimitLibrary(LibraryUpsertPagingRequestModel<RemainingLifeLimitLibraryDTO, RemainingLifeLimitDTO> upsertRequest)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    var items = _remainingLIfeLimitService.GetSyncedLibraryDataset(upsertRequest);                   
                    var dto = upsertRequest.Library;
                    if (dto != null)
                    {
                        _claimHelper.CheckIfAdminOrOwner(dto.Owner, UserId);
                        dto.RemainingLifeLimits = items;
                    }
                    UnitOfWork.RemainingLifeLimitRepo.UpsertRemainingLifeLimitLibrary(dto);
                    UnitOfWork.RemainingLifeLimitRepo.UpsertOrDeleteRemainingLifeLimits(dto.RemainingLifeLimits, dto.Id);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{RemainingLifeLimitError}::UpsertRemainingLifeLimitLibrary - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{RemainingLifeLimitError}::UpsertRemainingLifeLimitLibrary - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertScenarioRemainingLifeLimits/{simulationId}")]
        [Authorize(Policy = Policy.ModifyRemainingLifeLimitFromScenario)]
        public async Task<IActionResult> UpsertScenarioRemainingLifeLimits(Guid simulationId, PagingSyncModel<RemainingLifeLimitDTO> pagingSync)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    var dtos = _remainingLIfeLimitService.GetSyncedScenarioDataSet(simulationId, pagingSync);
                    _claimHelper.CheckUserSimulationModifyAuthorization(simulationId, UserId);
                    UnitOfWork.RemainingLifeLimitRepo.UpsertOrDeleteScenarioRemainingLifeLimits(dtos, simulationId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                UnitOfWork.Rollback();
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{RemainingLifeLimitError}::UpsertScenarioRemainingLifeLimits for {simulationName} - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{RemainingLifeLimitError}::UpsertScenarioRemainingLifeLimits for {simulationName} - {e.Message}");
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
                        _claimHelper.CheckIfAdminOrOwner(dto.Owner, UserId);
                    }
                    UnitOfWork.RemainingLifeLimitRepo.DeleteRemainingLifeLimitLibrary(libraryId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{RemainingLifeLimitError}::DeleteRemainingLifeLimitLibrary - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{RemainingLifeLimitError}::DeleteRemainingLifeLimitLibrary - {e.Message}");
                throw;
            }
        }

        private List<RemainingLifeLimitLibraryDTO> GetAllRemainingLifeLimitLibrariesWithRemainingLifeLimits()
        {
            return UnitOfWork.RemainingLifeLimitRepo.GetAllRemainingLifeLimitLibrariesWithRemainingLifeLimits();
        }
    }
}
