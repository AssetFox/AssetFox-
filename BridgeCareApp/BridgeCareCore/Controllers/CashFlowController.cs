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
    public class CashFlowController : BridgeCareCoreBaseController
    {
        public const string CashFlowError = "Cash Flow Error";
        private Guid UserId => UnitOfWork.CurrentUser?.Id ?? Guid.Empty;
        private readonly IClaimHelper _claimHelper;
        private readonly ICashFlowPagingService _cashFlowService;

        public const string RequestedToModifyNonexistentLibraryErrorMessage = "The request says to modify a library, but the library does not exist.";
        public const string RequestedToCreateExistingLibraryErrorMessage = "The request says to create a new library, but the library already exists.";

        public CashFlowController(IEsecSecurity esecSecurity, IUnitOfWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor, IClaimHelper claimHelper,
            ICashFlowPagingService cashFlowService) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            _claimHelper = claimHelper ?? throw new ArgumentNullException(nameof(claimHelper));
            _cashFlowService = cashFlowService ?? throw new ArgumentNullException(nameof(cashFlowService));
        }

        [HttpPost]
        [Route("GetLibraryCashFlowRulePage/{libraryId}")]
        [Authorize(Policy = Policy.ViewCashFlowFromLibrary)]
        public async Task<IActionResult> GetLibraryCashFlowRulePage(Guid libraryId, PagingRequestModel<CashFlowRuleDTO> pageRequest)
        {
            try
            {
                var result = new PagingPageModel<CashFlowRuleDTO>();
                await Task.Factory.StartNew(() =>
                {
                    result = _cashFlowService.GetLibraryPage(libraryId, pageRequest);
                });

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{CashFlowError} ::GetLibraryCashFlowRulePage - {e.Message}", e);
                return Ok();
            }
        }

        [HttpGet]
        [Route("GetCashLibraryModifiedDate/{libraryId}")]
        [Authorize(Policy = Policy.ModifyInvestmentFromLibrary)]
        public async Task<IActionResult> GetCashLibraryDate(Guid libraryId)
        {
            try
            {
                var users = new DateTime();
                await Task.Factory.StartNew(() =>
                {
                    users = UnitOfWork.CashFlowRuleRepo.GetLibraryModifiedDate(libraryId);
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
                throw;
            }
        }

        [HttpPost]
        [Route("GetScenarioCashFlowRulePage/{simulationId}")]
        [Authorize(Policy = Policy.ViewCashFlowFromScenario)]
        public async Task<IActionResult> GetScenarioCashFlowRulePage(Guid simulationId, PagingRequestModel<CashFlowRuleDTO> pageRequest)
        {
            try
            {
                var result = new PagingPageModel<CashFlowRuleDTO>();
                await Task.Factory.StartNew(() =>
                {
                    _claimHelper.CheckUserSimulationReadAuthorization(simulationId, UserId);
                    result = _cashFlowService.GetScenarioPage(simulationId, pageRequest);
                });

                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{CashFlowError}::GetScenarioCashFlowRulePage for {simulationName} - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{CashFlowError}::GetScenarioCashFlowRulePage for {simulationName} - {e.Message}", e);
            }
            return Ok();
        }

        [HttpGet]
        [Route("GetCashFlowRuleLibraries")]
        [Authorize(Policy = Policy.ViewCashFlowFromLibrary)]
        public async Task<IActionResult> GetCashFlowRuleLibraries()
        {
            try
            {
                var result = new List<CashFlowRuleLibraryDTO>();
                await Task.Factory.StartNew(() =>
                {
                    result = UnitOfWork.CashFlowRuleRepo.GetCashFlowRuleLibrariesNoChildren();
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        result = UnitOfWork.CashFlowRuleRepo.GetCashFlowRuleLibrariesNoChildrenAccessibleToUser(UserId);
                    }
                });

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{CashFlowError}::GetCashFlowRuleLibraries - {e.Message}", e);
            }
            return Ok();
        }

        [HttpGet]
        [Route("GetScenarioCashFlowRules/{simulationId}")]
        [Authorize(Policy = Policy.ViewCashFlowFromScenario)]
        public async Task<IActionResult> GetScenarioCashFlowRules(Guid simulationId)
        {
            try
            {
                var result = new List<CashFlowRuleDTO>();
                await Task.Factory.StartNew(() =>
                {
                    _claimHelper.CheckUserSimulationReadAuthorization(simulationId, UserId);
                    result = UnitOfWork.CashFlowRuleRepo.GetScenarioCashFlowRules(simulationId);
                });

                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{CashFlowError}::GetScenarioCashFlowRules for {simulationName} - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{CashFlowError}::GetScenarioCashFlowRules for {simulationName} - {e.Message}", e);
            }
            return Ok();
        }

        [HttpPost]
        [Route("UpsertCashFlowRuleLibrary")]
        [Authorize(Policy = Policy.ModifyCashFlowFromLibrary)]
        public async Task<IActionResult> UpsertCashFlowRuleLibrary(LibraryUpsertPagingRequestModel<CashFlowRuleLibraryDTO, CashFlowRuleDTO> upsertRequest)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    var libraryAccess = UnitOfWork.CashFlowRuleRepo.GetLibraryAccess(upsertRequest.Library.Id, UserId);
                    if (libraryAccess.LibraryExists == upsertRequest.IsNewLibrary)
                    {
                        var errorMessage = libraryAccess.LibraryExists ? RequestedToCreateExistingLibraryErrorMessage : RequestedToModifyNonexistentLibraryErrorMessage;
                        throw new InvalidOperationException(errorMessage);
                    }
                    var items = _cashFlowService.GetSyncedLibraryDataset(upsertRequest);                  
                    var dto = upsertRequest.Library;
                    if (dto != null)
                    {
                        _claimHelper.CheckUserLibraryModifyAuthorization(libraryAccess, UserId);
                        dto.CashFlowRules = items;
                    }
                    UnitOfWork.CashFlowRuleRepo.UpsertCashFlowRuleLibraryAndRules(dto);
                });

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{CashFlowError}::UpsertCashFlowRuleLibrary - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{CashFlowError}::UpsertCashFlowRuleLibrary - {e.Message}", e);
            }
            return Ok();
        }

        [HttpPost]
        [Route("UpsertScenarioCashFlowRules/{simulationId}")]
        [Authorize(Policy = Policy.ModifyCashFlowFromScenario)]

        public async Task<IActionResult> UpsertScenarioCashFlowRules(Guid simulationId, PagingSyncModel<CashFlowRuleDTO> pagingSync)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    var dtos = _cashFlowService.GetSyncedScenarioDataSet(simulationId, pagingSync);
                    _claimHelper.CheckUserSimulationModifyAuthorization(simulationId, UserId);
                    CashFlowRuleDtoListService.AddLibraryIdToScenarioCashFlowRule(dtos, pagingSync.LibraryId);
                    CashFlowRuleDtoListService.AddModifiedToScenarioCashFlowRule(dtos, pagingSync.IsModified);
                    UnitOfWork.CashFlowRuleRepo.UpsertOrDeleteScenarioCashFlowRules(dtos, simulationId);
                });

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{CashFlowError}::UpsertScenarioCashFlowRules for {simulationName} - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{CashFlowError}::UpsertScenarioCashFlowRules for {simulationName} - {e.Message}", e);
            }
            return Ok();
        }

        [HttpDelete]
        [Route("DeleteCashFlowRuleLibrary/{libraryId}")]
        [Authorize(Policy = Policy.ModifyCashFlowFromLibrary)]
        public async Task<IActionResult> DeleteCashFlowRuleLibrary(Guid libraryId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        var dto = GetAllCashFlowRuleLibraries().FirstOrDefault(_ => _.Id == libraryId);
                        if (dto == null) return;

                        var access = UnitOfWork.CashFlowRuleRepo.GetLibraryAccess(libraryId, UserId);
                        _claimHelper.CheckUserLibraryDeleteAuthorization(access, UserId);

                    }
                    UnitOfWork.CashFlowRuleRepo.DeleteCashFlowRuleLibrary(libraryId);
                });

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{CashFlowError}::DeleteCashFlowRuleLibrary - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{CashFlowError}::DeleteCashFlowRuleLibrary - {e.Message}", e);
            }
            return Ok();
        }

        [HttpGet]
        [Route("GetHasPermittedAccess")]
        [Authorize(Policy = Policy.ModifyCashFlowFromLibrary)]
        public async Task<IActionResult> GetHasPermittedAccess()
        {
            return Ok(true);
        }
        [HttpGet]
        [Route("GetIsSharedLibrary/{cashFlowRuleLibraryId}")]
        [Authorize(Policy = Policy.ViewCashFlowFromLibrary)]
        public async Task<IActionResult> GetIsSharedLibrary(Guid cashFlowRuleLibraryId)
        {
            bool result = false;
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    var users = UnitOfWork.CashFlowRuleRepo.GetLibraryUsers(cashFlowRuleLibraryId);
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
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{CashFlowError}::GetIsSharedLibrary - {e.Message}", e);
            }
            return Ok();
        }
        [HttpGet]
        [Route("GetCashFlowRuleLibraryUsers/{libraryId}")]
        [Authorize(Policy = Policy.ViewCashFlowFromLibrary)]
        public async Task<IActionResult> GetCashFlowRuleLibraryUsers(Guid libraryId)
        {
            try
            {
                List<LibraryUserDTO> users = new List<LibraryUserDTO>();
                await Task.Factory.StartNew(() =>
                {
                    var accessModel = UnitOfWork.CashFlowRuleRepo.GetLibraryAccess(libraryId, UserId);
                    _claimHelper.RequirePermittedCheck();
                    users = UnitOfWork.CashFlowRuleRepo.GetLibraryUsers(libraryId);
                });
                return Ok(users);
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{CashFlowError}::GetCashFlowRuleLibraryUsers - {e.Message}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{CashFlowError}::GetCashFlowRuleLibraryUsers - {e.Message}", e);
            }
            return Ok();
        }
        [HttpPost]
        [Route("UpsertOrDeleteCashFlowRuleLibraryUsers/{libraryId}")]
        [Authorize(Policy = Policy.ModifyCashFlowFromLibrary)]
        public async Task<IActionResult> UpsertOrDeleteCashFlowRuleLibraryUsers(Guid libraryId, List<LibraryUserDTO> proposedUsers)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    var libraryUsers = UnitOfWork.CashFlowRuleRepo.GetLibraryUsers(libraryId);
                    _claimHelper.RequirePermittedCheck();
                    UnitOfWork.CashFlowRuleRepo.UpsertOrDeleteUsers(libraryId, proposedUsers);
                });
                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{CashFlowError}::UpsertOrDeleteCashFlowRuleLibraryUsers - {e.Message}", e);
            }
            catch (InvalidOperationException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{CashFlowError}::UpsertOrDeleteCashFlowRuleLibraryUsers - {e.Message}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{CashFlowError}::UpsertOrDeleteCashFlowRuleLibraryUsers - {e.Message}", e);
            }
            return Ok();
        }
        private List<CashFlowRuleLibraryDTO> GetAllCashFlowRuleLibraries()
        {
            return UnitOfWork.CashFlowRuleRepo.GetCashFlowRuleLibraries();
        }
    }
}
