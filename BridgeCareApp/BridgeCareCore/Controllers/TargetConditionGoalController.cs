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
using BridgeCareCore.Services;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TargetConditionGoalController : BridgeCareCoreBaseController
    {
        public const string TargetConditionGoalError = "Target Condition Goal Error";
        private Guid UserId => UnitOfWork.CurrentUser?.Id ?? Guid.Empty;
        private readonly IClaimHelper _claimHelper;
        private readonly ITargetConditionGoalPagingService _targetConditionGoalService;

        public TargetConditionGoalController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor, IClaimHelper claimHelper, ITargetConditionGoalPagingService targetConditionGoalService) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {         
            _claimHelper = claimHelper ?? throw new ArgumentNullException(nameof(claimHelper));
            _targetConditionGoalService = targetConditionGoalService ?? throw new ArgumentNullException(nameof(targetConditionGoalService));
        }     

        [HttpGet]
        [Route("GetTargetConditionGoalLibraries")]
        [Authorize(Policy = Policy.ViewTargetConditionGoalFromLibrary)]
        public async Task<IActionResult> TargetConditionGoalLibraries()
        {
            try
            {
                var result = new List<TargetConditionGoalLibraryDTO>();
                await Task.Factory.StartNew(() =>
                {
                    result = UnitOfWork.TargetConditionGoalRepo.GetTargetConditionGoalLibrariesNoChildren();
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        result = result.Where(_ => _.Owner == UserId || _.IsShared == true).ToList();
                    }
                });

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TargetConditionGoalError}::TargetConditionGoalLibraries - {HubService.errorList["Exception"]}");
                throw;
            }
        }

        [HttpPost]
        [Route("GetLibraryTargetConditionGoalPage/{libraryId}")]
        [Authorize(Policy = Policy.ViewTargetConditionGoalFromLibrary)]
        public async Task<IActionResult> GetLibraryTargetConditionGoalPage(Guid libraryId, PagingRequestModel<TargetConditionGoalDTO> pageRequest)
        {
            try
            {
                var result = new PagingPageModel<TargetConditionGoalDTO>();
                await Task.Factory.StartNew(() =>
                {
                    result = _targetConditionGoalService.GetLibraryTargetConditionGoalPage(libraryId, pageRequest);
                });

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TargetConditionGoalError}::GetLibraryTargetConditionGoalPage - {HubService.errorList["Exception"]}");
                throw;
            }
        }

        [HttpPost]
        [Route("GetScenarioTargetConditionGoalPage/{simulationId}")]
        [Authorize(Policy = Policy.ViewTargetConditionGoalFromScenario)]
        public async Task<IActionResult> GetScenarioTargetConditionGoalPage(Guid simulationId, PagingRequestModel<TargetConditionGoalDTO> pageRequest)
        {
            try
            {
                var result = new PagingPageModel<TargetConditionGoalDTO>();
                await Task.Factory.StartNew(() =>
                {
                    _claimHelper.CheckUserSimulationReadAuthorization(simulationId, UserId);
                    result = _targetConditionGoalService.GetTargetConditionGoalPage(simulationId, pageRequest);
                });

                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TargetConditionGoalError}::GetScenarioTargetConditionGoalPage - {HubService.errorList["Unauthorized"]}");
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TargetConditionGoalError}::GetScenarioTargetConditionGoalPage - {HubService.errorList["Exception"]}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetScenarioTargetConditionGoals/{simulationId}")]
        [Authorize(Policy = Policy.ViewTargetConditionGoalFromScenario)]
        public async Task<IActionResult> GetScenarioTargetConditionGoals(Guid simulationId)
        {
            try
            {
                var result = new List<TargetConditionGoalDTO>();
                await Task.Factory.StartNew(() =>
                {
                    _claimHelper.CheckUserSimulationReadAuthorization(simulationId, UserId);
                    result = UnitOfWork.TargetConditionGoalRepo.GetScenarioTargetConditionGoals(simulationId);
                });

                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TargetConditionGoalError}::GetScenarioTargetConditionGoals - {HubService.errorList["Unauthorized"]}");
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TargetConditionGoalError}::GetScenarioTargetConditionGoals - {HubService.errorList["Exception"]}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertTargetConditionGoalLibrary")]
        [Authorize(Policy = Policy.ModifyTargetConditionGoalFromLibrary)]
        public async Task<IActionResult> UpsertTargetConditionGoalLibrary(LibraryUpsertPagingRequestModel<TargetConditionGoalLibraryDTO, TargetConditionGoalDTO> upsertRequest)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    var items = new List<TargetConditionGoalDTO>();
                    if (upsertRequest.PagingSync.LibraryId != null)
                        items = _targetConditionGoalService.GetSyncedLibraryDataset(upsertRequest.PagingSync.LibraryId.Value, upsertRequest.PagingSync);
                    else if (!upsertRequest.IsNewLibrary)
                        items = _targetConditionGoalService.GetSyncedLibraryDataset(upsertRequest.Library.Id, upsertRequest.PagingSync);
                    if (upsertRequest.PagingSync.LibraryId != null && upsertRequest.PagingSync.LibraryId != upsertRequest.Library.Id)
                    {
                        items.ForEach(item =>
                        {
                            item.Id = Guid.NewGuid();
                            item.CriterionLibrary.Id = Guid.NewGuid();
                        });
                    }                       
                    var dto = upsertRequest.Library;
                    if (dto != null)
                    {
                        _claimHelper.CheckUserLibraryModifyAuthorization(dto.Owner, UserId);
                        dto.TargetConditionGoals = items;
                    }
                    UnitOfWork.TargetConditionGoalRepo.UpsertTargetConditionGoalLibrary(dto);
                    UnitOfWork.TargetConditionGoalRepo.UpsertOrDeleteTargetConditionGoals(dto.TargetConditionGoals, dto.Id);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TargetConditionGoalError}::UpsertTargetConditionGoalLibrary - {HubService.errorList["Unauthorized"]}");
                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TargetConditionGoalError}::UpsertTargetConditionGoalLibrary - {HubService.errorList["Exception"]}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertScenarioTargetConditionGoals/{simulationId}")]
        [Authorize(Policy = Policy.ModifyTargetConditionGoalFromScenario)]
        public async Task<IActionResult> UpsertScenarioTargetConditionGoals(Guid simulationId, PagingSyncModel<TargetConditionGoalDTO> pagingSync)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    var dtos = _targetConditionGoalService.GetSyncedScenarioDataset(simulationId, pagingSync);
                    _claimHelper.CheckUserSimulationModifyAuthorization(simulationId, UserId);
                    UnitOfWork.TargetConditionGoalRepo.UpsertOrDeleteScenarioTargetConditionGoals(dtos, simulationId);
                    UnitOfWork.Commit();
                });


                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TargetConditionGoalError}::UpsertScenarioTargetConditionGoals - {HubService.errorList["Unauthorized"]}");
                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TargetConditionGoalError}::UpsertScenarioTargetConditionGoals - {HubService.errorList["Exception"]}");
                throw;
            }
        }

        [HttpDelete]
        [Route("DeleteTargetConditionGoalLibrary/{libraryId}")]
        [Authorize(Policy = Policy.DeleteTargetConditionGoalFromLibrary)]
        public async Task<IActionResult> DeleteTargetConditionGoalLibrary(Guid libraryId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        var dto = GetAllTargetConditionGoalLibrariesWithTargetConditionGoals().FirstOrDefault(_ => _.Id == libraryId);
                        if (dto == null) return;
                        _claimHelper.CheckUserLibraryModifyAuthorization(dto.Owner, UserId);
                    }
                    UnitOfWork.TargetConditionGoalRepo.DeleteTargetConditionGoalLibrary(libraryId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TargetConditionGoalError}::DeleteTargetConditionGoalLibrary - {HubService.errorList["Unauthorized"]}");
                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TargetConditionGoalError}::DeleteTargetConditionGoalLibrary - {HubService.errorList["Exception"]}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetHasPermittedAccess")]
        [Authorize]
        [Authorize(Policy = Policy.ModifyOrDeleteTargetConditionGoalFromLibrary)]
        public async Task<IActionResult> GetHasPermittedAccess()
        {
            return Ok(true);
        }

        private List<TargetConditionGoalLibraryDTO> GetAllTargetConditionGoalLibrariesWithTargetConditionGoals()
        {
            return UnitOfWork.TargetConditionGoalRepo.GetTargetConditionGoalLibrariesWithTargetConditionGoals();
        }
    }
}
