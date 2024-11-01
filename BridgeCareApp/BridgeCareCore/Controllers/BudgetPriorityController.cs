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
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.Generics;
using BridgeCareCore.Services;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BudgetPriorityController : BridgeCareCoreBaseController
    {
        public const string BudgetPriorityError = "Budget Priority Error";
        private readonly IClaimHelper _claimHelper;

        public const string RequestedToModifyNonexistentLibraryErrorMessage = "The request says to modify a library, but the library does not exist.";
        public const string RequestedToCreateExistingLibraryErrorMessage = "The request says to create a new library, but the library already exists.";

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
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{BudgetPriorityError}::GetScenarioBudgetPriorityPage for {simulationName} - {e.Message}", e);
                return Ok();
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
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{BudgetPriorityError}::GetLibraryBudgetPriorityPage - {e.Message}", e);
                return Ok();
            }
        }

     
        [HttpGet]
        [Route("GetBudgetPriorityLibraryModifiedDate/{libraryId}")]
        [Authorize(Policy = Policy.ModifyInvestmentFromLibrary)]
        public async Task<IActionResult> GetBudgetPriorityLibraryDate(Guid libraryId)
        {
            try
            {
                var users = new DateTime();
                await Task.Factory.StartNew(() =>
                {
                    users = UnitOfWork.BudgetPriorityRepo.GetLibraryModifiedDate(libraryId);
                });
                return Ok(users);
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"Investment error::{e.Message}", e);
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"Investment error::{e.Message}", e);
                return Ok();
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
                        result = UnitOfWork.BudgetPriorityRepo.GetBudgetPriorityLibrariesNoChildrenAccessibleToUser(UserId);
                    } 
                });

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{BudgetPriorityError}::GetBudgetPriorityLibraries - {e.Message}", e);
                return Ok();
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
                    var libraryAccess = UnitOfWork.BudgetPriorityRepo.GetLibraryAccess(upsertRequest.Library.Id, UserId);
                    if (libraryAccess.LibraryExists == upsertRequest.IsNewLibrary)
                    {
                        var errorMessage = libraryAccess.LibraryExists ? RequestedToCreateExistingLibraryErrorMessage : RequestedToModifyNonexistentLibraryErrorMessage;
                        throw new InvalidOperationException(errorMessage);
                    }
                    var items = _budgetPriortyService.GetSyncedLibraryDataset(upsertRequest);
                    var dto = upsertRequest.Library;
                    if (dto != null)
                    {
                        _claimHelper.CheckUserLibraryModifyAuthorization(libraryAccess, UserId);
                        dto.BudgetPriorities = items;
                    }
                    UnitOfWork.BudgetPriorityRepo.UpsertOrDeleteBudgetPriorityLibraryAndPriorities(dto, upsertRequest.IsNewLibrary, UserId);
                });

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{BudgetPriorityError}::UpsertBudgetPriorityLibrary - {HubService.errorList["Unauthorized"]}", e);
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{BudgetPriorityError}::UpsertBudgetPriorityLibrary - {e.Message}", e);
                return Ok();
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
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        var dto = GetAllBudgetPriorityLibraries().FirstOrDefault(_ => _.Id == libraryId);
                        if (dto == null) return;

                        var access = UnitOfWork.BudgetPriorityRepo.GetLibraryAccess(libraryId, UserId);
                        _claimHelper.CheckUserLibraryDeleteAuthorization(access, UserId);
                    }
                    UnitOfWork.BudgetPriorityRepo.DeleteBudgetPriorityLibrary(libraryId);
                });

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{BudgetPriorityError}::DeleteBudgetPriorityLibrary - {HubService.errorList["Unauthorized"]}", e);
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{BudgetPriorityError}::DeleteBudgetPriorityLibrary - {e.Message}", e);
                return Ok();
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
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{BudgetPriorityError}::GetScenarioBudgetPriorities - {HubService.errorList["Unauthorized"]}", e);
                throw;
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{BudgetPriorityError}::GetScenarioBudgetPriorities - {e.Message}", e);
                return Ok();
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
                    var dtos = _budgetPriortyService.GetSyncedScenarioDataSet(simulationId, pagingSync);
                    _claimHelper.CheckUserSimulationModifyAuthorization(simulationId, UserId);
                    BudgetPriorityDtoListService.AddLibraryIdToScenarioBudgetPriority(dtos, pagingSync.LibraryId);
                    BudgetPriorityDtoListService.AddModifiedToScenarioBudgetPriority(dtos, pagingSync.IsModified);
                    UnitOfWork.BudgetPriorityRepo.UpsertOrDeleteScenarioBudgetPriorities(dtos, simulationId);
                });

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{BudgetPriorityError}::UpsertScenarioBudgetPriorities - {HubService.errorList["Unauthorized"]}", e);
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{BudgetPriorityError}::UpsertScenarioBudgetPriorities - {e.Message}", e);
                return Ok();
            }
        }
        [HttpGet]
        [Route("GetBudgetPriorityLibraryUsers/{libraryId}")]
        [Authorize(Policy = Policy.ViewBudgetPriorityFromLibrary)]
        public async Task<IActionResult> GetBudgetPriorityLibraryUsers(Guid libraryId)
        {
            try
            {
                List<LibraryUserDTO> users = new List<LibraryUserDTO>();
                await Task.Factory.StartNew(() =>
                {
                    var accessModel = UnitOfWork.BudgetPriorityRepo.GetLibraryAccess(libraryId, UserId);
                    _claimHelper.CheckGetLibraryUsersValidity(accessModel, UserId);
                    users = UnitOfWork.BudgetPriorityRepo.GetLibraryUsers(libraryId);
                });
                return Ok(users);
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{BudgetPriorityError}::GetBudgetPriorityLibraryUsers - {HubService.errorList["Unauthorized"]}", e);
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{BudgetPriorityError}::GetBudgetPriorityLibraryUsers - {e.Message}", e);
                return Ok();
            }
        }

        [HttpPost]
        [Route("UpsertOrDeleteBudgetPriorityLibraryUsers/{libraryId}")]
        [Authorize(Policy = Policy.ModifyBudgetPriorityFromLibrary)]
        public async Task<IActionResult> UpsertOrDeleteBudgetPriorityLibraryUsers(Guid libraryId, List<LibraryUserDTO> proposedUsers)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    var libraryUsers = UnitOfWork.BudgetPriorityRepo.GetLibraryUsers(libraryId);
                    _claimHelper.CheckAccessModifyValidity(libraryUsers, proposedUsers, UserId);
                    UnitOfWork.BudgetPriorityRepo.UpsertOrDeleteUsers(libraryId, proposedUsers);
                });
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{BudgetPriorityError}::UpsertOrDeleteBudgetPriorityLibraryUsers - {e.Message}", e);
                return Ok();
            }
        }
        [HttpGet]
        [Route("GetIsSharedLibrary/{budgetPriorityLibraryId}")]
        [Authorize(Policy = Policy.ViewBudgetPriorityFromLibrary)]
        public async Task<IActionResult> GetIsSharedLibrary(Guid budgetPriorityLibraryId)
        {
            try
            {
                bool result = false;
                await Task.Factory.StartNew(() =>
                {
                    var users = UnitOfWork.BudgetPriorityRepo.GetLibraryUsers(budgetPriorityLibraryId);
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
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{BudgetPriorityError}::GetIsSharedLibrary - {e.Message}", e);
                return Ok();
            }
        }
        [HttpGet]
        [Route("GetHasPermittedAccess")]
        [Authorize(Policy = Policy.ModifyBudgetPriorityFromLibrary)]
        public async Task<IActionResult> GetHasPermittedAccess()
        {
            await Task.Factory.StartNew(() =>
            {
            });
            return Ok(true);
        }

        private List<BudgetPriorityLibraryDTO> GetAllBudgetPriorityLibraries()
        {
            return UnitOfWork.BudgetPriorityRepo.GetBudgetPriorityLibraries();
        } 
    }
}
