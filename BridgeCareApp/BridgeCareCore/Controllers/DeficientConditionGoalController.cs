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
    using DeficientConditionGoalUpsertMethod = Action<Guid, DeficientConditionGoalLibraryDTO>;

    [Route("api/[controller]")]
    [ApiController]
    public class DeficientConditionGoalController : BridgeCareCoreBaseController
    {
        private readonly IReadOnlyDictionary<string, DeficientConditionGoalUpsertMethod>
            _deficientConditionGoalUpsertMethods;

        public DeficientConditionGoalController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor) =>
            _deficientConditionGoalUpsertMethods = CreateUpsertMethods();

        private Dictionary<string, DeficientConditionGoalUpsertMethod> CreateUpsertMethods()
        {
            void UpsertAny(Guid simulationId, DeficientConditionGoalLibraryDTO dto)
            {
                UnitOfWork.DeficientConditionGoalRepo.UpsertDeficientConditionGoalLibrary(dto, simulationId);
                UnitOfWork.DeficientConditionGoalRepo.UpsertOrDeleteDeficientConditionGoals(
                    dto.DeficientConditionGoals, dto.Id);
            }

            void UpsertPermitted(Guid simulationId, DeficientConditionGoalLibraryDTO dto)
            {
                CheckUserSimulationModifyAuthorization(simulationId);
                UpsertAny(simulationId, dto);
            }

            return new Dictionary<string, DeficientConditionGoalUpsertMethod>
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
                    .DeficientConditionGoalLibrariesWithDeficientConditionGoals());
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Deficient Condition Goal error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertDeficientConditionGoalLibrary/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> UpsertDeficientConditionGoalLibrary(Guid simulationId, DeficientConditionGoalLibraryDTO dto)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    _deficientConditionGoalUpsertMethods[UserInfo.Role](simulationId, dto);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                UnitOfWork.Rollback();
                return Unauthorized();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Deficient Condition Goal error::{e.Message}");
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
                HubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Deficient Condition Goal error::{e.Message}");
                throw;
            }
        }
    }
}
