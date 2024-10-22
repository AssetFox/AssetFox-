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
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TargetConditionGoalError}::TargetConditionGoalLibraries - {e.Message}", e);
            }
            return Ok();
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
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"Investment error::{e.Message}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"Investment error::{e.Message}", e);
            }
            return Ok();
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
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TargetConditionGoalError}::GetLibraryTargetConditionGoalPage - {e.Message}", e);
            }
            return Ok();
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
            catch (UnauthorizedAccessException e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TargetConditionGoalError}::GetScenarioTargetConditionGoalPage for {simulationName} - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TargetConditionGoalError}::GetScenarioTargetConditionGoalPage for {simulationName} - {e.Message}", e);
            }
            return Ok();
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
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TargetConditionGoalError}::GetScenarioTargetConditionGoals for {simulationName} - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TargetConditionGoalError}::GetScenarioTargetConditionGoals for {simulationName} - {e.Message}", e);
            }
            return Ok();
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
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TargetConditionGoalError}::UpsertTargetConditionGoalLibrary - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TargetConditionGoalError}::UpsertTargetConditionGoalLibrary - {e.Message}", e);
            }
            return Ok();
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
                    TargetConditionGoalDtoListService.AddLibraryIdToScenarioTargetConditionGoal(dtos, pagingSync.LibraryId);
                    TargetConditionGoalDtoListService.AddModifiedToScenarioTargetConditionGoal(dtos, pagingSync.IsModified);
                    UnitOfWork.TargetConditionGoalRepo.UpsertOrDeleteScenarioTargetConditionGoals(dtos, simulationId);
                });


                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TargetConditionGoalError}::UpsertScenarioTargetConditionGoals for {simulationName} - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TargetConditionGoalError}::UpsertScenarioTargetConditionGoals for {simulationName} - {e.Message}", e);
            }
            return Ok();
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
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TargetConditionGoalError}::DeleteTargetConditionGoalLibrary - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TargetConditionGoalError}::DeleteTargetConditionGoalLibrary - {e.Message}", e);
            }
            return Ok();
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
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TargetConditionGoalError}::GetTargetConditionGoalLibraryUsers - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TargetConditionGoalError}::GetTargetConditionGoalLibraryUsers - {HubService.errorList["Exception"]}", e);
            }
            return Ok();
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
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TargetConditionGoalError}::UpsertOrDeleteTargetConditionGoalLibraryUsers - {e.Message}", e);
            }
            catch (InvalidOperationException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TargetConditionGoalError}::UpsertOrDeleteTargetConditionGoalLibraryUsers - {e.Message}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TargetConditionGoalError}::UpsertOrDeleteTargetConditionGoalLibraryUsers - {e.Message}", e);
            }
            return Ok();
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
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TargetConditionGoalError}::GetIsSharedLibrary - {HubService.errorList["Exception"]}", e);
            }
            return Ok();
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
