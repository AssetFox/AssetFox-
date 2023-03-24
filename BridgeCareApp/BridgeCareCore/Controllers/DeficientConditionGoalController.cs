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
    public class DeficientConditionGoalController : BridgeCareCoreBaseController
    {
        public const string DeficientConditionGoalError = "Deficient Condition Goal Error";
        private Guid UserId => UnitOfWork.CurrentUser?.Id ?? Guid.Empty;
        private readonly IClaimHelper _claimHelper;
        private readonly IDeficientConditionGoalPagingService _deficientConditionGoalService;

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
                        result = result.Where(_ => _.Owner == UserId || _.IsShared == true).ToList();
                    }
                });

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeficientConditionGoalError}::DeficientConditionGoalLibraries - {e.Message}");
                throw;
            }
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
            catch (UnauthorizedAccessException)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeficientConditionGoalError}::GetScenarioDeficientConditionGoals for {simulationName} - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeficientConditionGoalError}::GetScenarioDeficientConditionGoals for {simulationName} - {e.Message}");
                throw;
            }
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeficientConditionGoalError}::GetScenarioDeficientConditionGoalPage for {simulationName} - {e.Message}");
                throw;
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeficientConditionGoalError}::GetScenarioDeficientConditionGoalPage for {simulationName} - {e.Message}");
                throw;
            }
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeficientConditionGoalError} ::GetLibraryDeficientConditionGoalPage - {e.Message}");
                throw;
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeficientConditionGoalError} ::GetLibraryDeficientConditionGoalPage - {e.Message}");
                throw;
            }
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
                    var items = _deficientConditionGoalService.GetSyncedLibraryDataset(upsertRequest);
                    var dto = upsertRequest.Library;
                    if (dto != null)
                    {
                        _claimHelper.CheckIfAdminOrOwner(dto.Owner, UserId);
                        dto.DeficientConditionGoals = items;
                    }
                    UnitOfWork.DeficientConditionGoalRepo.UpsertDeficientConditionGoalLibraryAndGoals(dto);
                });

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeficientConditionGoalError}::UpsertDeficientConditionGoalLibrary - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeficientConditionGoalError}::UpsertDeficientConditionGoalLibrary - {e.Message}");
                throw;
            }
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
                    UnitOfWork.DeficientConditionGoalRepo.AddLibraryIdToScenarioDeficientConditionGoal(dtos, pagingSync.LibraryId);
                    UnitOfWork.DeficientConditionGoalRepo.AddModifiedToScenarioDeficientConditionGoal(dtos, pagingSync.IsModified);
                    UnitOfWork.DeficientConditionGoalRepo.UpsertOrDeleteScenarioDeficientConditionGoals(dtos, simulationId);
                });


                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeficientConditionGoalError}::UpsertDeficientConditionGoalLibrary for {simulationName} - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeficientConditionGoalError}::UpsertDeficientConditionGoalLibrary for {simulationName} - {e.Message}");
                throw;
            }
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
                        _claimHelper.CheckIfAdminOrOwner(dto.Owner, UserId);
                    }
                    UnitOfWork.DeficientConditionGoalRepo.DeleteDeficientConditionGoalLibrary(libraryId);
                });

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeficientConditionGoalError}::DeleteDeficientConditionGoalLibrary - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeficientConditionGoalError}::DeleteDeficientConditionGoalLibrary - {e.Message}");
                throw;
            }
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeficientConditionGoalError}::GetIsSharedLibrary - {e.Message}");
                throw;
            }
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
                    _claimHelper.RequirePermittedCheck();
                    users = UnitOfWork.DeficientConditionGoalRepo.GetLibraryUsers(libraryId);
                });
                return Ok(users);
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeficientConditionGoalError}::GetDeficientConditionGoalLibraryUsers - {e.Message}");
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeficientConditionGoalError}::GetDeficientConditionGoalLibraryUsers - {e.Message}");
                throw;
            }
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeficientConditionGoalError}::UpsertOrDeleteDeleteConditionGoalLibraryUsers - {e.Message}");
                return Ok();
            }
            catch (InvalidOperationException e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeficientConditionGoalError}::UpsertOrDeleteDeleteConditionGoalLibraryUsers - {e.Message}");
                return BadRequest();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeficientConditionGoalError}::UpsertOrDeleteDeleteConditionGoalLibraryUsers - {e.Message}");
                throw;
            }
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
