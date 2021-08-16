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
    using ScenarioDeficientConditionGoalUpsertMethod = Action<Guid, List<DeficientConditionGoalDTO>>;

    [Route("api/[controller]")]
    [ApiController]
    public class DeficientConditionGoalController : BridgeCareCoreBaseController
    {
        private readonly IReadOnlyDictionary<string, ScenarioDeficientConditionGoalUpsertMethod>
            _scenarioDeficientConditionGoalUpsertMethods;

        public DeficientConditionGoalController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor) =>
            _scenarioDeficientConditionGoalUpsertMethods = CreateUpsertMethods();

        private Dictionary<string, ScenarioDeficientConditionGoalUpsertMethod> CreateUpsertMethods()
        {
            void UpsertAny(Guid simulationId, List<DeficientConditionGoalDTO> dtos)
            {
                UnitOfWork.DeficientConditionGoalRepo.UpsertOrDeleteScenarioDeficientConditionGoals(dtos, simulationId);
            }

            void UpsertPermitted(Guid simulationId, List<DeficientConditionGoalDTO> dtos)
            {
                CheckUserSimulationModifyAuthorization(simulationId);
                UpsertAny(simulationId, dtos);
            }

            return new Dictionary<string, ScenarioDeficientConditionGoalUpsertMethod>
            {
                [Role.Administrator] = UpsertAny,
                [Role.DistrictEngineer] = UpsertPermitted,
                [Role.Cwopa] = UpsertPermitted,
                [Role.PlanningPartner] = UpsertPermitted
            };
        }

        [HttpGet]
        [Route("GetDeficientConditionGoalLibraries")]
        [Authorize]
        public async Task<IActionResult> DeficientConditionGoalLibraries()
        {
            try
            {
                var result = await Task.Factory.StartNew(() => UnitOfWork.DeficientConditionGoalRepo
                    .GetDeficientConditionGoalLibrariesWithDeficientConditionGoals());
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
        [Authorize]
        public async Task<IActionResult> GetScenarioDeficientConditionGoals(Guid simulationId)
        {
            try
            {
                var result = await Task.Factory.StartNew(() => UnitOfWork.DeficientConditionGoalRepo
                    .GetScenarioDeficientConditionGoals(simulationId));
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Deficient Condition Goal error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertDeficientConditionGoalLibrary/")]
        [Authorize]
        public async Task<IActionResult> UpsertDeficientConditionGoalLibrary(DeficientConditionGoalLibraryDTO dto)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    UnitOfWork.DeficientConditionGoalRepo.UpsertDeficientConditionGoalLibrary(dto);
                    UnitOfWork.DeficientConditionGoalRepo.UpsertOrDeleteDeficientConditionGoals(dto.DeficientConditionGoals, dto.Id);
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Deficient Condition Goal error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertScenarioDeficientConditionGoals/{simulationId}")]
        [Authorize(Policy = SecurityConstants.Policy.AdminOrDistrictEngineer)]
        public async Task<IActionResult> UpsertScenarioDeficientConditionGoals(Guid simulationId, List<DeficientConditionGoalDTO> dtos)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    _scenarioDeficientConditionGoalUpsertMethods[UserInfo.Role](simulationId, dtos);
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Deficient condition goal error::{e.Message}");
                throw;
            }
        }

        [HttpDelete]
        [Route("DeleteDeficientConditionGoalLibrary/{libraryId}")]
        [Authorize]
        public async Task<IActionResult> DeleteDeficientConditionGoalLibrary(Guid libraryId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    UnitOfWork.DeficientConditionGoalRepo.DeleteDeficientConditionGoalLibrary(libraryId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Deficient Condition Goal error::{e.Message}");
                throw;
            }
        }
    }
}
