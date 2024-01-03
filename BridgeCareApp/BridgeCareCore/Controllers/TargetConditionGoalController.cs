﻿using System;
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

        public const string RequestedToModifyNonexistentLibraryErrorMessage = "The request says to modify a library, but the library does not exist.";
        public const string RequestedToCreateExistingLibraryErrorMessage = "The request says to create a new library, but the library already exists.";

        public TargetConditionGoalController(IEsecSecurity esecSecurity, IUnitOfWork unitOfWork, IHubService hubService,
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
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        result = UnitOfWork.TargetConditionGoalRepo.GetTargetConditionGoalLibrariesNoChildrenAccessibleToUser(UserId);
                    }
                    else
                    {
                        result = UnitOfWork.TargetConditionGoalRepo.GetTargetConditionGoalLibrariesNoChildren();
                    }
                });

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TargetConditionGoalError}::TargetConditionGoalLibraries - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetTargetLibraryModifiedDate/{libraryId}")]
        [Authorize(Policy = Policy.ModifyInvestmentFromLibrary)]
        public async Task<IActionResult> GetTargetLibraryDate(Guid libraryId)
        {
            try
            {
                var users = new DateTime();
                await Task.Factory.StartNew(() =>
                {
                    users = UnitOfWork.TargetConditionGoalRepo.GetLibraryModifiedDate(libraryId);
                });
                return Ok(users);
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
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
                    result = _targetConditionGoalService.GetLibraryPage(libraryId, pageRequest);
                });

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TargetConditionGoalError}::GetLibraryTargetConditionGoalPage - {e.Message}");
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
                    result = _targetConditionGoalService.GetScenarioPage(simulationId, pageRequest);
                });

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TargetConditionGoalError}::GetScenarioTargetConditionGoalPage for {simulationName} - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TargetConditionGoalError}::GetScenarioTargetConditionGoalPage for {simulationName} - {e.Message}");
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
            catch (UnauthorizedAccessException)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TargetConditionGoalError}::GetScenarioTargetConditionGoals for {simulationName} - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TargetConditionGoalError}::GetScenarioTargetConditionGoals for {simulationName} - {e.Message}");
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
                    var libraryAccess = UnitOfWork.TargetConditionGoalRepo.GetLibraryAccess(upsertRequest.Library.Id, UserId);
                    if (libraryAccess.LibraryExists == upsertRequest.IsNewLibrary)
                    {
                        var errorMessage = libraryAccess.LibraryExists ? RequestedToCreateExistingLibraryErrorMessage : RequestedToModifyNonexistentLibraryErrorMessage;
                        throw new InvalidOperationException(errorMessage);
                    }
                    var items = _targetConditionGoalService.GetSyncedLibraryDataset(upsertRequest);                 
                    var dto = upsertRequest.Library;
                    if (dto != null)
                    {
                        _claimHelper.CheckUserLibraryModifyAuthorization(libraryAccess, UserId);
                        dto.TargetConditionGoals = items;
                    }
                    UnitOfWork.TargetConditionGoalRepo.UpsertTargetConditionGoalLibraryGoalsAndPossiblyUser(dto, upsertRequest.IsNewLibrary, UserId);
                });

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TargetConditionGoalError}::UpsertTargetConditionGoalLibrary - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TargetConditionGoalError}::UpsertTargetConditionGoalLibrary - {e.Message}");
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
                    var dtos = _targetConditionGoalService.GetSyncedScenarioDataSet(simulationId, pagingSync);
                    _claimHelper.CheckUserSimulationModifyAuthorization(simulationId, UserId);
                    TargetConditionGoalDtoListHelper.AddLibraryIdToScenarioTargetConditionGoal(dtos, pagingSync.LibraryId);
                    TargetConditionGoalDtoListHelper.AddModifiedToScenarioTargetConditionGoal(dtos, pagingSync.IsModified);
                    UnitOfWork.TargetConditionGoalRepo.UpsertOrDeleteScenarioTargetConditionGoals(dtos, simulationId);
                });


                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TargetConditionGoalError}::UpsertScenarioTargetConditionGoals for {simulationName} - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TargetConditionGoalError}::UpsertScenarioTargetConditionGoals for {simulationName} - {e.Message}");
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
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        var dto = GetAllTargetConditionGoalLibrariesWithTargetConditionGoals().FirstOrDefault(_ => _.Id == libraryId);
                        if (dto == null) return;

                        var access = UnitOfWork.TargetConditionGoalRepo.GetLibraryAccess(libraryId, UserId);
                        _claimHelper.CheckUserLibraryDeleteAuthorization(access, UserId);
                    }
                    UnitOfWork.TargetConditionGoalRepo.DeleteTargetConditionGoalLibrary(libraryId);
                });

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TargetConditionGoalError}::DeleteTargetConditionGoalLibrary - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TargetConditionGoalError}::DeleteTargetConditionGoalLibrary - {e.Message}");
                throw;
            }
        }
        [HttpGet]
        [Route("GetTargetConditionGoalLibraryUsers/{libraryId}")]
        [Authorize(Policy = Policy.ViewTargetConditionGoalFromLibrary)]
        public async Task<IActionResult> GetTargetConditionGoalLibraryUsers(Guid libraryId)
        {
            try
            {
                List<LibraryUserDTO> users = new List<LibraryUserDTO>();
                await Task.Factory.StartNew(() =>
                {
                    var accessModel = UnitOfWork.TargetConditionGoalRepo.GetLibraryAccess(libraryId, UserId);
                    _claimHelper.CheckGetLibraryUsersValidity(accessModel, UserId);
                    users = UnitOfWork.TargetConditionGoalRepo.GetLibraryUsers(libraryId);
                });
                return Ok(users);
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TargetConditionGoalError}::GetTargetConditionGoalLibraryUsers - {HubService.errorList["Unauthorized"]}");
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TargetConditionGoalError}::GetTargetConditionGoalLibraryUsers - {HubService.errorList["Exception"]}");
                throw;
            }
        }
        [HttpPost]
        [Route("UpsertOrDeleteTargetConditionGoalLibraryUsers/{libraryId}")]
        [Authorize(Policy = Policy.ModifyTargetConditionGoalFromLibrary)]
        public async Task<IActionResult> UpsertOrDeleteTargetConditionGoalLibraryUsers(Guid libraryId, List<LibraryUserDTO> proposedUsers)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    var libraryUsers = UnitOfWork.TargetConditionGoalRepo.GetLibraryUsers(libraryId);
                    _claimHelper.CheckAccessModifyValidity(libraryUsers, proposedUsers, UserId);
                    UnitOfWork.TargetConditionGoalRepo.UpsertOrDeleteUsers(libraryId, proposedUsers);
                });
                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TargetConditionGoalError}::UpsertOrDeleteTargetConditionGoalLibraryUsers - {e.Message}");
                return Ok();
            }
            catch (InvalidOperationException e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TargetConditionGoalError}::UpsertOrDeleteTargetConditionGoalLibraryUsers - {e.Message}");
                return BadRequest();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TargetConditionGoalError}::UpsertOrDeleteTargetConditionGoalLibraryUsers - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetIsSharedLibrary/{targetConditionGoalLibraryId}")]
        [Authorize(Policy = Policy.ViewTargetConditionGoalFromLibrary)]
        public async Task<IActionResult> GetIsSharedLibrary(Guid targetConditionGoalLibraryId)
        {
            try
            {
                bool result = false;
                await Task.Factory.StartNew(() =>
                {
                    var users = UnitOfWork.TargetConditionGoalRepo.GetLibraryUsers(targetConditionGoalLibraryId);
                    var nonOwnerUsers = users.Any(x => x.AccessLevel != AppliedResearchAssociates.iAM.DTOs.Enums.LibraryAccessLevel.Owner);
                    if (nonOwnerUsers)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                });
                return Ok(result);
            }
            catch (Exception)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TargetConditionGoalError}::GetIsSharedLibrary - {HubService.errorList["Exception"]}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetHasPermittedAccess")]
        [Authorize]
        [Authorize(Policy = Policy.ModifyOrDeleteTargetConditionGoalFromLibrary)]
        public async Task<IActionResult> GetHasPermittedAccess()
        {
            await Task.Factory.StartNew(() =>
            {
            });
            return Ok(true);
        }

        private List<TargetConditionGoalLibraryDTO> GetAllTargetConditionGoalLibrariesWithTargetConditionGoals()
        {
            return UnitOfWork.TargetConditionGoalRepo.GetTargetConditionGoalLibrariesWithTargetConditionGoals();
        }
    }
}
