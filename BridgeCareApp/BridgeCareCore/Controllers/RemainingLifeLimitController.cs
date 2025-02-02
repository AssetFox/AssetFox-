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
    public class RemainingLifeLimitController : BridgeCareCoreBaseController
    {
        public const string RemainingLifeLimitError = "Remaining Life Limit Error";
        private Guid UserId => UnitOfWork.CurrentUser?.Id ?? Guid.Empty;
        private readonly IClaimHelper _claimHelper;
        private readonly IRemainingLifeLimitPagingService _remainingLifeLimitService;

        public const string RequestedToModifyNonexistentLibraryErrorMessage = "The request says to modify a library, but the library does not exist.";
        public const string RequestedToCreateExistingLibraryErrorMessage = "The request says to create a new library, but the library already exists.";

        public RemainingLifeLimitController(IEsecSecurity esecSecurity, IUnitOfWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor, IClaimHelper claimHelper,
            IRemainingLifeLimitPagingService remainingLifeService) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            _claimHelper = claimHelper ?? throw new ArgumentNullException(nameof(claimHelper));
            _remainingLifeLimitService = remainingLifeService ?? throw new ArgumentNullException(nameof(remainingLifeService));
        }

        [HttpGet]
        [Route("GetRemainingLibraryModifiedDate/{libraryId}")]
        [Authorize(Policy = Policy.ModifyInvestmentFromLibrary)]
        public async Task<IActionResult> GetRemainingLibraryDate(Guid libraryId)
        {
            try
            {
                var users = new DateTime();
                await Task.Factory.StartNew(() =>
                {
                    users = UnitOfWork.RemainingLifeLimitRepo.GetLibraryModifiedDate(libraryId);
                });
                return Ok(users);
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"Investment error::{e.Message}",e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"Investment error::{e.Message}", e);
            }
            return Ok();
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
                    result = _remainingLifeLimitService.GetScenarioPage(simulationId, pageRequest);
                });

                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{RemainingLifeLimitError}::GetScenarioRemainingLifeLimitPage for {simulationName} - {e.Message}", e);
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{RemainingLifeLimitError}::GetScenarioRemainingLifeLimitPage for {simulationName} - {e.Message}", e);
            }
            return Ok();
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
                    result = _remainingLifeLimitService.GetLibraryPage(libraryId, pageRequest);
                });

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{RemainingLifeLimitError} ::GetLibraryRemainingLifeLimitPage - {e.Message}", e);
            }
            return Ok();
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
                        result = UnitOfWork.RemainingLifeLimitRepo.GetRemainingLifeLimitLibrariesNoChildrenAccessibleToUser(UserId);
                    }
                    else
                    {
                        result = UnitOfWork.RemainingLifeLimitRepo.GetAllRemainingLifeLimitLibrariesNoChildren();
                    }
                });

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{RemainingLifeLimitError}::RemainingLifeLimitLibraries - {e.Message}", e);
            }
            return Ok();
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
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{RemainingLifeLimitError}::GetScenarioRemainingLifeLimits for {simulationName} - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{RemainingLifeLimitError}::GetScenarioRemainingLifeLimits for {simulationName} - {e.Message}", e);
            }
            return Ok();
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
                    var libraryAccess = UnitOfWork.RemainingLifeLimitRepo.GetLibraryAccess(upsertRequest.Library.Id, UserId);
                    if (libraryAccess.LibraryExists == upsertRequest.IsNewLibrary)
                    {
                        var errorMessage = libraryAccess.LibraryExists ? RequestedToCreateExistingLibraryErrorMessage : RequestedToModifyNonexistentLibraryErrorMessage;
                        throw new InvalidOperationException(errorMessage);
                    }
                    var items = _remainingLifeLimitService.GetSyncedLibraryDataset(upsertRequest);                   
                    var dto = upsertRequest.Library;
                    if (dto != null)
                    {
                        _claimHelper.CheckUserLibraryModifyAuthorization(libraryAccess, UserId);
                        dto.RemainingLifeLimits = items;
                    }
                    UnitOfWork.RemainingLifeLimitRepo.UpsertRemainingLifeLimitLibraryAndLimits(dto);
                });

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{RemainingLifeLimitError}::UpsertRemainingLifeLimitLibrary - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{RemainingLifeLimitError}::UpsertRemainingLifeLimitLibrary - {e.Message}", e);
            }
            return Ok();
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
                    var dtos = _remainingLifeLimitService.GetSyncedScenarioDataSet(simulationId, pagingSync);
                    _claimHelper.CheckUserSimulationModifyAuthorization(simulationId, UserId);
                    RemainingLifeLimitDtoListService.AddLibraryIdToScenarioRemainingLifeLimit(dtos, pagingSync.LibraryId);
                    RemainingLifeLimitDtoListService.AddModifiedToScenarioRemainingLifeLimit(dtos, pagingSync.IsModified);
                    UnitOfWork.RemainingLifeLimitRepo.UpsertOrDeleteScenarioRemainingLifeLimits(dtos, simulationId);
                });

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{RemainingLifeLimitError}::UpsertScenarioRemainingLifeLimits for {simulationName} - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{RemainingLifeLimitError}::UpsertScenarioRemainingLifeLimits for {simulationName} - {e.Message}", e);
            }
            return Ok();
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
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        var dto = GetAllRemainingLifeLimitLibrariesWithRemainingLifeLimits()
                        .FirstOrDefault(_ => _.Id == libraryId);
                        if (dto == null) return;

                        var access = UnitOfWork.RemainingLifeLimitRepo.GetLibraryAccess(libraryId, UserId);
                        _claimHelper.CheckUserLibraryDeleteAuthorization(access, UserId);
                    }
                    UnitOfWork.RemainingLifeLimitRepo.DeleteRemainingLifeLimitLibrary(libraryId);
                });

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{RemainingLifeLimitError}::DeleteRemainingLifeLimitLibrary - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{RemainingLifeLimitError}::DeleteRemainingLifeLimitLibrary - {e.Message}", e);
            }
            return Ok();
        }

        private List<RemainingLifeLimitLibraryDTO> GetAllRemainingLifeLimitLibrariesWithRemainingLifeLimits()
        {
            return UnitOfWork.RemainingLifeLimitRepo.GetAllRemainingLifeLimitLibrariesWithRemainingLifeLimits();
        }

        [HttpGet]
        [Route("GetIsSharedLibrary/{remainingLifeLimitLibraryId}")]
        [Authorize(Policy = Policy.ViewRemainingLifeLimitFromLibrary)]
        public async Task<IActionResult> GetIsSharedLibrary(Guid remainingLifeLimitLibraryId)
        {
            try
            {
                bool result = false;
                await Task.Factory.StartNew(() =>
                {
                    var users = UnitOfWork.RemainingLifeLimitRepo.GetLibraryUsers(remainingLifeLimitLibraryId);
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
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{RemainingLifeLimitError}::GetIsSharedLibrary - {e.Message}", e);
            }
            return Ok();
        }
        [HttpGet]
        [Route("GetRemainingLifeLimitLibraryUsers/{libraryId}")]
        [Authorize(Policy = Policy.ViewRemainingLifeLimitFromLibrary)]
        public async Task<IActionResult> GetRemainingLifeLimitLibraryUsers(Guid libraryId)
        {
            try
            {
                List<LibraryUserDTO> users = new List<LibraryUserDTO>();
                await Task.Factory.StartNew(() =>
                {
                    var accessModel = UnitOfWork.RemainingLifeLimitRepo.GetLibraryAccess(libraryId, UserId);
                    _claimHelper.RequirePermittedCheck();
                    users = UnitOfWork.RemainingLifeLimitRepo.GetLibraryUsers(libraryId);
                });
                return Ok(users);
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{RemainingLifeLimitError}::GetRemainingLifeLimitLibraryUsers - {e.Message}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{RemainingLifeLimitError}::GetRemainingLifeLimitLibraryUsers - {HubService.errorList["Exception"]}", e);
            }
            return Ok();
        }
        [HttpPost]
        [Route("UpsertOrDeleteRemainingLifeLimitLibraryUsers/{libraryId}")]
        [Authorize(Policy = Policy.ModifyRemainingLifeLimitFromLibrary)]
        public async Task<IActionResult> UpsertOrDeleteRemainingLifeLimitLibraryUsers(Guid libraryId, List<LibraryUserDTO> proposedUsers)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    var libraryUsers = UnitOfWork.RemainingLifeLimitRepo.GetLibraryUsers(libraryId);
                    _claimHelper.RequirePermittedCheck();
                    UnitOfWork.RemainingLifeLimitRepo.UpsertOrDeleteUsers(libraryId, proposedUsers);
                });
                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{RemainingLifeLimitError}::UpsertOrDeleteRemainingLifeLimitLibraryUsers - {e.Message}", e);
            }
            catch (InvalidOperationException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{RemainingLifeLimitError}::UpsertOrDeleteRemainingLifeLimitLibraryUsers - {e.Message}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{RemainingLifeLimitError}::UpsertOrDeleteRemainingLifeLimitLibraryUsers - {e.Message}", e);
            }
            return Ok();
        }
    }
}
