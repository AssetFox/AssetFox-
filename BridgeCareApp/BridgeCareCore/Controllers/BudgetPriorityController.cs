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
    public class BudgetPriorityController : BridgeCareCoreBaseController
    {
        public const string BudgetPriorityError = "Budget Priority Error";
        private readonly IClaimHelper _claimHelper;

        private Guid UserId => UnitOfWork.CurrentUser?.Id ?? Guid.Empty;

        private IBudgetPriortyPagingService _budgetPriortyService;

        public BudgetPriorityController(IEsecSecurity esecSecurity, IUnitOfWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor, IClaimHelper claimHelper,
            IBudgetPriortyPagingService budgetPriortyService) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            _claimHelper = claimHelper ?? throw new ArgumentNullException(nameof(claimHelper));
            _budgetPriortyService = budgetPriortyService;
        }

        [HttpPost]
        [Route("GetScenarioBudgetPriorityPage/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> GetScenarioBudgetPriorityPage(Guid simulationId, PagingRequestModel<BudgetPriorityDTO> pageRequest)
        {
            try
            {
                var result = await Task.Factory.StartNew(() => _budgetPriortyService.GetScenarioPage(simulationId, pageRequest));
                return Ok(result);
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{BudgetPriorityError}::GetScenarioBudgetPriorityPage for {simulationName} - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("GetLibraryBudgetPriorityPage/{libraryId}")]
        [Authorize]
        public async Task<IActionResult> GetLibraryBudgetPriortyPage(Guid libraryId, PagingRequestModel<BudgetPriorityDTO> pageRequest)
        {
            try
            {
                var result = await Task.Factory.StartNew(() => _budgetPriortyService.GetLibraryPage(libraryId, pageRequest));
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{BudgetPriorityError}::GetLibraryBudgetPriorityPage - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetBudgetPriorityLibraries")]
        [Authorize(Policy = Policy.ViewBudgetPriorityFromLibrary)]
        public async Task<IActionResult> GetBudgetPriorityLibraries()
        {
            try
            {
                var result = new List<BudgetPriorityLibraryDTO>();
                await Task.Factory.StartNew(() =>
                {
                    result = UnitOfWork.BudgetPriorityRepo.GetBudgetPriortyLibrariesNoChildren();
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        result = result.Where(_ => _.Owner == UserId || _.IsShared == true).ToList();
                    }
                });

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{BudgetPriorityError}::GetBudgetPriorityLibraries - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertBudgetPriorityLibrary")]
        [Authorize(Policy = Policy.ModifyBudgetPriorityFromLibrary)]
        public async Task<IActionResult> UpsertBudgetPriorityLibrary(LibraryUpsertPagingRequestModel<BudgetPriorityLibraryDTO, BudgetPriorityDTO> upsertRequest)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    var items = _budgetPriortyService.GetSyncedLibraryDataset(upsertRequest);
                    var dto = upsertRequest.Library;
                    if (dto != null)
                    {
                        _claimHelper.OldWayCheckUserLibraryModifyAuthorization(dto.Owner, UserId);
                        dto.BudgetPriorities = items;
                    }
                    UnitOfWork.BudgetPriorityRepo.UpsertBudgetPriorityLibrary(dto);
                    UnitOfWork.BudgetPriorityRepo.UpsertOrDeleteBudgetPriorities(dto.BudgetPriorities, dto.Id);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{BudgetPriorityError}::UpsertBudgetPriorityLibrary - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{BudgetPriorityError}::UpsertBudgetPriorityLibrary - {e.Message}");
                throw;
            }
        }

        [HttpDelete]
        [Route("DeleteBudgetPriorityLibrary/{libraryId}")]
        [Authorize(Policy = Policy.DeleteBudgetPriorityFromLibrary)]
        public async Task<IActionResult> DeleteBudgetPriorityLibrary(Guid libraryId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        var dto = GetAllBudgetPriorityLibraries().FirstOrDefault(_ => _.Id == libraryId);
                        if (dto == null) return;
                        _claimHelper.OldWayCheckUserLibraryModifyAuthorization(dto.Owner, UserId);
                    }
                    UnitOfWork.BudgetPriorityRepo.DeleteBudgetPriorityLibrary(libraryId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{BudgetPriorityError}::DeleteBudgetPriorityLibrary - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{BudgetPriorityError}::DeleteBudgetPriorityLibrary - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetScenarioBudgetPriorities/{simulationId}")]
        [Authorize(Policy = Policy.ViewBudgetPriorityFromScenario)]
        public async Task<IActionResult> GetScenarioBudgetPriorities(Guid simulationId)
        {
            try
            {
                var result = new List<BudgetPriorityDTO>();
                await Task.Factory.StartNew(() =>
                {
                    _claimHelper.CheckUserSimulationReadAuthorization(simulationId, UserId);
                    result = UnitOfWork.BudgetPriorityRepo.GetScenarioBudgetPriorities(simulationId);
                });

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{BudgetPriorityError}::GetScenarioBudgetPriorities - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{BudgetPriorityError}::GetScenarioBudgetPriorities - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertScenarioBudgetPriorities/{simulationId}")]
        [Authorize(Policy = Policy.ModifyBudgetPriorityFromScenario)]
        public async Task<IActionResult> UpsertScenarioBudgetPriorities(Guid simulationId, PagingSyncModel<BudgetPriorityDTO> pagingSync)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    var dtos = _budgetPriortyService.GetSyncedScenarioDataSet(simulationId, pagingSync);
                    _claimHelper.CheckUserSimulationModifyAuthorization(simulationId, UserId);
                    UnitOfWork.BudgetPriorityRepo.UpsertOrDeleteScenarioBudgetPriorities(dtos, simulationId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{BudgetPriorityError}::UpsertScenarioBudgetPriorities - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{BudgetPriorityError}::UpsertScenarioBudgetPriorities - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetHasPermittedAccess")]
        [Authorize(Policy = Policy.ModifyBudgetPriorityFromLibrary)]
        public async Task<IActionResult> GetHasPermittedAccess()
        {
            return Ok(true);
        }

        private List<BudgetPriorityLibraryDTO> GetAllBudgetPriorityLibraries()
        {
            return UnitOfWork.BudgetPriorityRepo.GetBudgetPriorityLibraries();
        } 
    }
}
