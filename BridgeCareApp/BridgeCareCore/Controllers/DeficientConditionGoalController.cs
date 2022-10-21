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

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeficientConditionGoalController : BridgeCareCoreBaseController
    {
        private Guid UserId => UnitOfWork.CurrentUser?.Id ?? Guid.Empty;
        private readonly IClaimHelper _claimHelper;

        public DeficientConditionGoalController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor, IClaimHelper claimHelper) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            _claimHelper = claimHelper ?? throw new ArgumentNullException(nameof(claimHelper));
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
                    result = UnitOfWork.DeficientConditionGoalRepo.GetDeficientConditionGoalLibrariesWithDeficientConditionGoals();
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        result = result.Where(_ => _.Owner == UserId || _.IsShared == true).ToList();
                    }
                });

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Deficient Condition Goal error::{e.Message}");
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
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Deficient Condition Goal error::{e.Message}");
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Deficient Condition Goal error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertDeficientConditionGoalLibrary/")]
        [Authorize(Policy = Policy.ModifyDeficientConditionGoalFromLibrary)]
        public async Task<IActionResult> UpsertDeficientConditionGoalLibrary(DeficientConditionGoalLibraryDTO dto)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    _claimHelper.OldWayCheckUserLibraryModifyAuthorization(dto.Owner, UserId);
                    UnitOfWork.DeficientConditionGoalRepo.UpsertDeficientConditionGoalLibrary(dto);
                    UnitOfWork.DeficientConditionGoalRepo.UpsertOrDeleteDeficientConditionGoals(dto.DeficientConditionGoals, dto.Id);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Deficient Condition Goal error::{e.Message}");
                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Deficient Condition Goal error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertScenarioDeficientConditionGoals/{simulationId}")]
        [Authorize(Policy = Policy.ModifyDeficientConditionGoalFromScenario)]
        public async Task<IActionResult> UpsertScenarioDeficientConditionGoals(Guid simulationId, List<DeficientConditionGoalDTO> dtos)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    _claimHelper.CheckUserSimulationModifyAuthorization(simulationId, UserId);
                    UnitOfWork.DeficientConditionGoalRepo.UpsertOrDeleteScenarioDeficientConditionGoals(dtos, simulationId);
                    UnitOfWork.Commit();
                });


                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Deficient Condition Goal error::{e.Message}");
                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Deficient condition goal error::{e.Message}");
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
                    UnitOfWork.BeginTransaction();
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        var dto = UnitOfWork.DeficientConditionGoalRepo.GetDeficientConditionGoalLibrariesWithDeficientConditionGoals()
                        .FirstOrDefault(_ => _.Id == libraryId);
                        if (dto == null) return;
                        _claimHelper.OldWayCheckUserLibraryModifyAuthorization(dto.Owner, UserId);
                    }
                    UnitOfWork.DeficientConditionGoalRepo.DeleteDeficientConditionGoalLibrary(libraryId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Deficient Condition Goal error::{e.Message}");
                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Deficient Condition Goal error::{e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetHasPermittedAccess")]
        [Authorize]
        [Authorize(Policy = Policy.ModifyDeficientConditionGoalFromLibrary)]
        public async Task<IActionResult> GetHasPermittedAccess()
        {
            return Ok(true);
        }
    }
}
