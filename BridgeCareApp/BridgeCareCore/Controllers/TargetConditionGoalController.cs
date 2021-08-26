using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Controllers.BaseController;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    using ScenarioTargetConditionGoalUpsertMethod = Action<Guid, List<TargetConditionGoalDTO>>;

    [Route("api/[controller]")]
    [ApiController]
    public class TargetConditionGoalController : BridgeCareCoreBaseController
    {
        private readonly IReadOnlyDictionary<string, ScenarioTargetConditionGoalUpsertMethod>
            _scenarioTargetConditionGoalUpsertMethods;

        public TargetConditionGoalController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor) =>
            _scenarioTargetConditionGoalUpsertMethods = CreateTargetConditionUpsertMethods();

        private Dictionary<string, ScenarioTargetConditionGoalUpsertMethod> CreateTargetConditionUpsertMethods()
        {
            void UpsertAny(Guid simulationId, List<TargetConditionGoalDTO> dtos)
            {
                UnitOfWork.TargetConditionGoalRepo.UpsertOrDeleteScenarioTargetConditionGoals(dtos, simulationId);
            }

            void UpsertPermitted(Guid simulationId, List<TargetConditionGoalDTO> dtos)
            {
                CheckUserSimulationModifyAuthorization(simulationId);
                UpsertAny(simulationId, dtos);
            }

            return new Dictionary<string, ScenarioTargetConditionGoalUpsertMethod>
            {
                [Role.Administrator] = UpsertAny,
                [Role.DistrictEngineer] = UpsertPermitted,
                [Role.Cwopa] = UpsertPermitted,
                [Role.PlanningPartner] = UpsertPermitted
            };
        }

        [HttpGet]
        [Route("GetTargetConditionGoalLibraries")]
        [Authorize]
        public async Task<IActionResult> TargetConditionGoalLibraries()
        {
            try
            {
                var result = await Task.Factory.StartNew(() => UnitOfWork.TargetConditionGoalRepo
                    .GetTargetConditionGoalLibrariesWithTargetConditionGoals());
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
        [Authorize]
        public async Task<IActionResult> GetScenarioTargetConditionGoals(Guid simulationId)
        {
            try
            {
                var result = await Task.Factory.StartNew(() => UnitOfWork.TargetConditionGoalRepo
                    .GetScenarioTargetConditionGoals(simulationId));
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
        [Authorize]
        public async Task<IActionResult> UpsertTargetConditionGoalLibrary(TargetConditionGoalLibraryDTO dto)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
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
        [Authorize(Policy = SecurityConstants.Policy.AdminOrDistrictEngineer)]
        public async Task<IActionResult> UpsertScenarioTargetConditionGoals(Guid simulationId, List<TargetConditionGoalDTO> dtos)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    _scenarioTargetConditionGoalUpsertMethods[UserInfo.Role](simulationId, dtos);
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
        [Authorize]
        public async Task<IActionResult> DeleteTargetConditionGoalLibrary(Guid libraryId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
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
    }
}
