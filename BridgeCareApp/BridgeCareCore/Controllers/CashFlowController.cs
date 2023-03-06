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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CashFlowError} ::GetLibraryCashFlowRulePage - {e.Message}");
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
            catch (UnauthorizedAccessException)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CashFlowError}::GetScenarioCashFlowRulePage for {simulationName} - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CashFlowError}::GetScenarioCashFlowRulePage for {simulationName} - {e.Message}");
                throw;
            }
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
                        result = result.Where(_ => _.Owner == UserId || _.IsShared == true).ToList();
                    }
                });

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CashFlowError}::GetCashFlowRuleLibraries - {e.Message}");
                throw;
            }
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
            catch (UnauthorizedAccessException)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CashFlowError}::GetScenarioCashFlowRules for {simulationName} - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CashFlowError}::GetScenarioCashFlowRules for {simulationName} - {e.Message}");
                throw;
            }
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
                    UnitOfWork.BeginTransaction();
                    var items = _cashFlowService.GetSyncedLibraryDataset(upsertRequest);                  
                    var dto = upsertRequest.Library;
                    if (dto != null)
                    {
                        _claimHelper.CheckIfAdminOrOwner(dto.Owner, UserId);
                        dto.CashFlowRules = items;
                    }
                    UnitOfWork.CashFlowRuleRepo.UpsertCashFlowRuleLibrary(dto);
                    UnitOfWork.CashFlowRuleRepo.UpsertOrDeleteCashFlowRules(dto.CashFlowRules, dto.Id);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CashFlowError}::UpsertCashFlowRuleLibrary - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CashFlowError}::UpsertCashFlowRuleLibrary - {e.Message}");
                throw;
            }
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
                    UnitOfWork.BeginTransaction();
                    var dtos = _cashFlowService.GetSyncedScenarioDataSet(simulationId, pagingSync);
                    _claimHelper.CheckUserSimulationModifyAuthorization(simulationId, UserId);
                    UnitOfWork.CashFlowRuleRepo.UpsertOrDeleteScenarioCashFlowRules(dtos, simulationId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                UnitOfWork.Rollback();
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CashFlowError}::UpsertScenarioCashFlowRules for {simulationName} - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CashFlowError}::UpsertScenarioCashFlowRules for {simulationName} - {e.Message}");
                throw;
            }
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
                    UnitOfWork.BeginTransaction();
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        var dto = GetAllCashFlowRuleLibraries().FirstOrDefault(_ => _.Id == libraryId);
                        if (dto == null) return;
                        _claimHelper.CheckIfAdminOrOwner(dto.Owner, UserId);
                    }
                    UnitOfWork.CashFlowRuleRepo.DeleteCashFlowRuleLibrary(libraryId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CashFlowError}::DeleteCashFlowRuleLibrary - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CashFlowError}::DeleteCashFlowRuleLibrary - {e.Message}");
                throw;
            }
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CashFlowError}::GetIsSharedLibrary - {e.Message}");
                throw;
            }
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CashFlowError}::GetCashFlowRuleLibraryUsers - {e.Message}");
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CashFlowError}::GetCashFlowRuleLibraryUsers - {e.Message}");
                throw;
            }
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CashFlowError}::UpsertOrDeleteCashFlowRuleLibraryUsers - {e.Message}");
                return Ok();
            }
            catch (InvalidOperationException e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CashFlowError}::UpsertOrDeleteCashFlowRuleLibraryUsers - {e.Message}");
                return BadRequest();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CashFlowError}::UpsertOrDeleteCashFlowRuleLibraryUsers - {e.Message}");
                throw;
            }
        }
        private List<CashFlowRuleLibraryDTO> GetAllCashFlowRuleLibraries()
        {
            return UnitOfWork.CashFlowRuleRepo.GetCashFlowRuleLibraries();
        }
    }
}
