using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Controllers.BaseController;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    using TargetConditionGoalUpsertMethod = Action<Guid, TargetConditionGoalLibraryDTO>;

    [Route("api/[controller]")]
    [ApiController]
    public class TargetConditionGoalController : BridgeCareCoreBaseController
    {
        private readonly IReadOnlyDictionary<string, TargetConditionGoalUpsertMethod>
            _targetConditionGoalUpsertMethods;

        public TargetConditionGoalController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor) =>
            _targetConditionGoalUpsertMethods = CreateUpsertMethods();

        private Dictionary<string, TargetConditionGoalUpsertMethod> CreateUpsertMethods()
        {
            void UpsertAny(Guid simulationId, TargetConditionGoalLibraryDTO dto)
            {
                UnitOfWork.TargetConditionGoalRepo.UpsertTargetConditionGoalLibrary(dto, simulationId);
                UnitOfWork.TargetConditionGoalRepo.UpsertOrDeleteTargetConditionGoals(dto.TargetConditionGoals, dto.Id);
            }

            void UpsertPermitted(Guid simulationId, TargetConditionGoalLibraryDTO dto)
            {
                CheckUserSimulationModifyAuthorization(simulationId);
                UpsertAny(simulationId, dto);
            }

            return new Dictionary<string, TargetConditionGoalUpsertMethod>
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
                    .TargetConditionGoalLibrariesWithTargetConditionGoals());
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Target Condition Goal error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertTargetConditionGoalLibrary/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> UpsertTargetConditionGoalLibrary(Guid simulationId, TargetConditionGoalLibraryDTO dto)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    _targetConditionGoalUpsertMethods[UserInfo.Role](simulationId, dto);
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
