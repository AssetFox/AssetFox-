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
    public class DeficientConditionGoalController : BridgeCareCoreBaseController
    {
        public const string DeficientConditionGoalError = "Deficient Condition Goal Error";
        private Guid UserId => UnitOfWork.CurrentUser?.Id ?? Guid.Empty;
        private readonly IClaimHelper _claimHelper;
        private readonly IDeficientConditionGoalPagingService _deficientConditionGoalService;

        public const string RequestedToModifyNonexistentLibraryErrorMessage = "The request says to modify a library, but the library does not exist.";
        public const string RequestedToCreateExistingLibraryErrorMessage = "The request says to create a new library, but the library already exists.";

        public DeficientConditionGoalController(IEsecSecurity esecSecurity, IUnitOfWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor, IClaimHelper claimHelper,
            IDeficientConditionGoalPagingService deficientConditionGoalService) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            _claimHelper = claimHelper ?? throw new ArgumentNullException(nameof(claimHelper));
            _deficientConditionGoalService = deficientConditionGoalService ?? throw new ArgumentNullException(nameof(deficientConditionGoalService));
        }

        [HttpGet]
        [Route("GetDeficientConditionGoalLibraries")]
        [Authorize(Policy = Policy.ViewDeficientConditionGoalFromlLibrary)]
        public async Task<IActionResult> DeficientConditionGoalLibraries()
        {
            try
            {
                var result = new List<DeficientConditionGoalLibraryDTO>();
                await Task.Factory.StartNew(() =>
                {
                    result = UnitOfWork.DeficientConditionGoalRepo.GetDeficientConditionGoalLibrariesNoChildren();
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        result = UnitOfWork.DeficientConditionGoalRepo.GetDeficientConditionGoalLibrariesNoChildrenAccessibleToUser(UserId);
                    }
                    else
                    {
                        result = UnitOfWork.DeficientConditionGoalRepo.GetDeficientConditionGoalLibrariesNoChildren();
                    }
                });

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeficientConditionGoalError}::DeficientConditionGoalLibraries - {e.Message}", e);
            }
            return Ok();
        }

        [HttpGet]
        [Route("GetDeficientModifiedDate/{libraryId}")]
        [Authorize(Policy = Policy.ModifyInvestmentFromLibrary)]
        public async Task<IActionResult> GetDeficientLibraryDate(Guid libraryId)
        {
            try
            {
                var users = new DateTime();
                await Task.Factory.StartNew(() =>
                {
                    users = UnitOfWork.DeficientConditionGoalRepo.GetLibraryModifiedDate(libraryId);
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

        [HttpGet]
        [Route("GetScenarioDeficientConditionGoals/{simulationId}")]
        [Authorize(Policy = Policy.ViewDeficientConditionGoalFromScenario)]
        public async Task<IActionResult> GetScenarioDeficientConditionGoals(Guid simulationId)
        {
            try
            {
                var result = new List<DeficientConditionGoalDTO>();
                await Task.Factory.StartNew(() =>
                {
                    _claimHelper.CheckUserSimulationReadAuthorization(simulationId, UserId);
                    result = UnitOfWork.DeficientConditionGoalRepo.GetScenarioDeficientConditionGoals(simulationId);
                });

                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeficientConditionGoalError}::GetScenarioDeficientConditionGoals for {simulationName} - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeficientConditionGoalError}::GetScenarioDeficientConditionGoals for {simulationName} - {e.Message}", e);
            }
            return Ok();
        }

        [HttpPost]
        [Route("GetScenarioDeficientConditionGoalPage/{simulationId}")]
        [Authorize(Policy = Policy.ViewDeficientConditionGoalFromScenario)]
        public async Task<IActionResult> GetScenarioDeficientConditionGoalPage(Guid simulationId, PagingRequestModel<DeficientConditionGoalDTO> pageRequest)
        {
            try
            {
                var result = new PagingPageModel<DeficientConditionGoalDTO>();
                await Task.Factory.StartNew(() =>
                {
                    _claimHelper.CheckUserSimulationReadAuthorization(simulationId, UserId);
                    result = _deficientConditionGoalService.GetScenarioPage(simulationId, pageRequest);
                });

                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeficientConditionGoalError}::GetScenarioDeficientConditionGoalPage for {simulationName} - {e.Message}", e);
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeficientConditionGoalError}::GetScenarioDeficientConditionGoalPage for {simulationName} - {e.Message}", e);
            }
            return Ok();
        }

        [HttpPost]
        [Route("GetLibraryDeficientConditionGoalPage/{libraryId}")]
        [Authorize(Policy = Policy.ViewDeficientConditionGoalFromlLibrary)]
        public async Task<IActionResult> GetLibraryDeficientConditionGoalPage(Guid libraryId, PagingRequestModel<DeficientConditionGoalDTO> pageRequest)
        {
            try
            {
                var result = new PagingPageModel<DeficientConditionGoalDTO>();
                await Task.Factory.StartNew(() =>
                {
                    result = _deficientConditionGoalService.GetLibraryPage(libraryId, pageRequest);
                });

                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeficientConditionGoalError} ::GetLibraryDeficientConditionGoalPage - {e.Message}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeficientConditionGoalError} ::GetLibraryDeficientConditionGoalPage - {e.Message}", e);
            }
            return Ok();
        }

        [HttpPost]
        [Route("UpsertDeficientConditionGoalLibrary/")]
        [Authorize(Policy = Policy.ModifyDeficientConditionGoalFromLibrary)]
        public async Task<IActionResult> UpsertDeficientConditionGoalLibrary(LibraryUpsertPagingRequestModel<DeficientConditionGoalLibraryDTO, DeficientConditionGoalDTO> upsertRequest)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    var libraryAccess = UnitOfWork.DeficientConditionGoalRepo.GetLibraryAccess(upsertRequest.Library.Id, UserId);
                    if (libraryAccess.LibraryExists == upsertRequest.IsNewLibrary)
                    {
                        var errorMessage = libraryAccess.LibraryExists ? RequestedToCreateExistingLibraryErrorMessage : RequestedToModifyNonexistentLibraryErrorMessage;
                        throw new InvalidOperationException(errorMessage);
                    }
                    var items = _deficientConditionGoalService.GetSyncedLibraryDataset(upsertRequest);
                    var dto = upsertRequest.Library;
                    if (dto != null)
                    {
                        _claimHelper.CheckUserLibraryModifyAuthorization(libraryAccess, UserId);
                        dto.DeficientConditionGoals = items;
                    }
                    UnitOfWork.DeficientConditionGoalRepo.UpsertDeficientConditionGoalLibraryAndGoals(dto);
                });

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeficientConditionGoalError}::UpsertDeficientConditionGoalLibrary - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeficientConditionGoalError}::UpsertDeficientConditionGoalLibrary - {e.Message}", e);
            }
            return Ok();
        }

        [HttpPost]
        [Route("UpsertScenarioDeficientConditionGoals/{simulationId}")]
        [Authorize(Policy = Policy.ModifyDeficientConditionGoalFromScenario)]
        public async Task<IActionResult> UpsertScenarioDeficientConditionGoals(Guid simulationId, PagingSyncModel<DeficientConditionGoalDTO> pagingSync)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    _claimHelper.CheckUserSimulationModifyAuthorization(simulationId, UserId);
                    var dtos = _deficientConditionGoalService.GetSyncedScenarioDataSet(simulationId, pagingSync);
                    DeficientConditionGoalDtoListService.AddLibraryIdToScenarioDeficientConditionGoal(dtos, pagingSync.LibraryId);
                    DeficientConditionGoalDtoListService.AddModifiedToScenarioDeficientConditionGoal(dtos, pagingSync.IsModified);
                    UnitOfWork.DeficientConditionGoalRepo.UpsertOrDeleteScenarioDeficientConditionGoals(dtos, simulationId);
                });


                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeficientConditionGoalError}::UpsertDeficientConditionGoalLibrary for {simulationName} - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeficientConditionGoalError}::UpsertDeficientConditionGoalLibrary for {simulationName} - {e.Message}", e);
            }
            return Ok();
        }

        [HttpDelete]
        [Route("DeleteDeficientConditionGoalLibrary/{libraryId}")]
        [Authorize(Policy = Policy.ModifyDeficientConditionGoalFromLibrary)]
        public async Task<IActionResult> DeleteDeficientConditionGoalLibrary(Guid libraryId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        var dto = UnitOfWork.DeficientConditionGoalRepo.GetDeficientConditionGoalLibrariesWithDeficientConditionGoals()
                        .FirstOrDefault(_ => _.Id == libraryId);
                        if (dto == null) return;

                        var access = UnitOfWork.DeficientConditionGoalRepo.GetLibraryAccess(libraryId, UserId);
                        _claimHelper.CheckUserLibraryModifyAuthorization(access, UserId);
                    }
                    UnitOfWork.DeficientConditionGoalRepo.DeleteDeficientConditionGoalLibrary(libraryId);
                });

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeficientConditionGoalError}::DeleteDeficientConditionGoalLibrary - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeficientConditionGoalError}::DeleteDeficientConditionGoalLibrary - {e.Message}", e);
            }
            return Ok();
        }

        [HttpGet]
        [Route("GetIsSharedLibrary/{deficientConditionGoalLibraryId}")]
        [Authorize(Policy = Policy.ViewDeficientConditionGoalFromlLibrary)]
        public async Task<IActionResult> GetIsSharedLibrary(Guid deficientConditionGoalLibraryId)
        {
            try
            {
                bool result = false;
                await Task.Factory.StartNew(() =>
                {
                    var users = UnitOfWork.DeficientConditionGoalRepo.GetLibraryUsers(deficientConditionGoalLibraryId);
                    if (users.Count <= 0)
                    {
                        result = false;
                    }
                    else
                    {
                        result = true;
                    }
                });
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeficientConditionGoalError}::GetIsSharedLibrary - {e.Message}", e);
            }
            return Ok();
        }
        [HttpGet]
        [Route("GetDeficientConditionGoalLibraryUsers/{libraryId}")]
        [Authorize(Policy = Policy.ViewDeficientConditionGoalFromlLibrary)]
        public async Task<IActionResult> GetDeficientConditionGoalLibraryUsers(Guid libraryId)
        {
            try
            {
                List<LibraryUserDTO> users = new List<LibraryUserDTO>();
                await Task.Factory.StartNew(() =>
                {
                    var accessModel = UnitOfWork.DeficientConditionGoalRepo.GetLibraryAccess(libraryId, UserId);
                    _claimHelper.CheckGetLibraryUsersValidity(accessModel, UserId);
                    users = UnitOfWork.DeficientConditionGoalRepo.GetLibraryUsers(libraryId);
                });
                return Ok(users);
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeficientConditionGoalError}::GetDeficientConditionGoalLibraryUsers - {e.Message}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeficientConditionGoalError}::GetDeficientConditionGoalLibraryUsers - {e.Message}", e);
            }
            return Ok();
        }
        [HttpPost]
        [Route("UpsertOrDeleteDeficientConditionGoalLibraryUsers/{libraryId}")]
        [Authorize(Policy = Policy.ModifyDeficientConditionGoalFromLibrary)]
        public async Task<IActionResult> UpsertOrDeleteDeleteConditionGoalLibraryUsers(Guid libraryId, List<LibraryUserDTO> proposedUsers)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    var libraryUsers = UnitOfWork.DeficientConditionGoalRepo.GetLibraryUsers(libraryId);
                    _claimHelper.RequirePermittedCheck();
                    UnitOfWork.DeficientConditionGoalRepo.UpsertOrDeleteUsers(libraryId, proposedUsers);
                });
                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeficientConditionGoalError}::UpsertOrDeleteDeleteConditionGoalLibraryUsers - {e.Message}", e);
            }
            catch (InvalidOperationException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeficientConditionGoalError}::UpsertOrDeleteDeleteConditionGoalLibraryUsers - {e.Message}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeficientConditionGoalError}::UpsertOrDeleteDeleteConditionGoalLibraryUsers - {e.Message}", e);
            }
            return Ok();
        }

        [HttpGet]
        [Route("GetHasPermittedAccess")]
        [Authorize]
        [Authorize(Policy = Policy.ModifyDeficientConditionGoalFromLibrary)]
        public async Task<IActionResult> GetHasPermittedAccess()
        {
            await Task.Factory.StartNew(() =>
            {
            });
            return Ok(true);
        }
    }
}
