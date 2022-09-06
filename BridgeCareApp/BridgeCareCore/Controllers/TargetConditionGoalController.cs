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
    public class TargetConditionGoalController : BridgeCareCoreBaseController
    {
        private Guid UserId => UnitOfWork.CurrentUser?.Id ?? Guid.Empty;
        private readonly IClaimHelper _claimHelper;

        public TargetConditionGoalController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor, IClaimHelper claimHelper) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {         
            _claimHelper = claimHelper ?? throw new ArgumentNullException(nameof(claimHelper));
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
                    result = GetAllTargetConditionGoalLibrariesWithTargetConditionGoals();
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        result = result.Where(_ => _.Owner == UserId || _.IsShared == true).ToList();
                    }
                });

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Target Condition Goal error::{e.Message}");
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
                    _claimHelper.CheckUserSimulationReadAuthorization(simulationId);
                    result = UnitOfWork.TargetConditionGoalRepo.GetScenarioTargetConditionGoals(simulationId);
                });

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Target Condition Goal error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertTargetConditionGoalLibrary")]
        [Authorize(Policy = Policy.ModifyTargetConditionGoalFromLibrary)]
        public async Task<IActionResult> UpsertTargetConditionGoalLibrary(TargetConditionGoalLibraryDTO dto)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    var currentRecord = UnitOfWork.TargetConditionGoalRepo.GetTargetConditionGoalLibrariesWithTargetConditionGoals()
                    .FirstOrDefault(_ => _.Id == dto.Id);
                    // by pass owner check if no record
                    if (currentRecord != null)
                    {
                        _claimHelper.CheckUserLibraryModifyAuthorization(currentRecord.Owner);
                    }
                    UnitOfWork.TargetConditionGoalRepo.UpsertTargetConditionGoalLibrary(dto);
                    UnitOfWork.TargetConditionGoalRepo.UpsertOrDeleteTargetConditionGoals(dto.TargetConditionGoals, dto.Id);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                UnitOfWork.Rollback();
                return Unauthorized();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Target Condition Goal error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertScenarioTargetConditionGoals/{simulationId}")]
        [Authorize(Policy = Policy.ModifyTargetConditionGoalFromScenario)]
        public async Task<IActionResult> UpsertScenarioTargetConditionGoals(Guid simulationId, List<TargetConditionGoalDTO> dtos)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    _claimHelper.CheckUserSimulationModifyAuthorization(simulationId);
                    UnitOfWork.TargetConditionGoalRepo.UpsertOrDeleteScenarioTargetConditionGoals(dtos, simulationId);
                    UnitOfWork.Commit();
                });


                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                UnitOfWork.Rollback();
                return Unauthorized();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Target condition goal error::{e.Message}");
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
                        _claimHelper.CheckUserLibraryModifyAuthorization(dto.Owner);
                    }
                    UnitOfWork.TargetConditionGoalRepo.DeleteTargetConditionGoalLibrary(libraryId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Target Condition Goal error::{e.Message}");
                throw;
            }
        }

        private List<TargetConditionGoalLibraryDTO> GetAllTargetConditionGoalLibrariesWithTargetConditionGoals()
        {
            return UnitOfWork.TargetConditionGoalRepo.GetTargetConditionGoalLibrariesWithTargetConditionGoals();
        }
    }
}
