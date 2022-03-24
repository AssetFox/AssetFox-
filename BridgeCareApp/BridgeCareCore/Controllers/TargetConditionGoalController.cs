using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
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
    [Route("api/[controller]")]
    [ApiController]
    public class TargetConditionGoalController : BridgeCareCoreBaseController
    {
        private readonly IReadOnlyDictionary<string, CRUDMethods<TargetConditionGoalDTO, TargetConditionGoalLibraryDTO>>
            _targetConditionCRUDMethods;

        private Guid UserId => UnitOfWork.UserEntity?.Id ?? Guid.Empty;

        public TargetConditionGoalController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor) =>
            _targetConditionCRUDMethods = CreateCRUDMethods();

        private Dictionary<string, CRUDMethods<TargetConditionGoalDTO, TargetConditionGoalLibraryDTO>> CreateCRUDMethods()
        {
            void UpsertAnyForScenario(Guid simulationId, List<TargetConditionGoalDTO> dtos)
            {
                UnitOfWork.TargetConditionGoalRepo.UpsertOrDeleteScenarioTargetConditionGoals(dtos, simulationId);
            }

            void UpsertPermittedForScenario(Guid simulationId, List<TargetConditionGoalDTO> dtos)
            {
                CheckUserSimulationModifyAuthorization(simulationId);
                UpsertAnyForScenario(simulationId, dtos);
            }

            List<TargetConditionGoalDTO> RetrieveAnyForScenario(Guid simulationId) =>
                UnitOfWork.TargetConditionGoalRepo.GetScenarioTargetConditionGoals(simulationId);

            void DeleteAnyFromScenario(Guid simulationId, List<TargetConditionGoalDTO> dtos)
            {
                // Do Nothing
            }

            List<TargetConditionGoalLibraryDTO> RetrieveAnyForLibrary() =>
                UnitOfWork.TargetConditionGoalRepo.GetTargetConditionGoalLibrariesWithTargetConditionGoals();

            List<TargetConditionGoalLibraryDTO> RetrievePermittedForLibrary()
            {
                var result = UnitOfWork.TargetConditionGoalRepo.GetTargetConditionGoalLibrariesWithTargetConditionGoals();
                return result.Where(_ => _.Owner == UserId || _.IsShared == true).ToList();
            }

            void UpsertAnyForLibrary(TargetConditionGoalLibraryDTO dto)
            {
                UnitOfWork.TargetConditionGoalRepo.UpsertTargetConditionGoalLibrary(dto);
                UnitOfWork.TargetConditionGoalRepo.UpsertOrDeleteTargetConditionGoals(dto.TargetConditionGoals, dto.Id);
            }

            void UpsertPermittedForLibrary(TargetConditionGoalLibraryDTO dto)
            {
                var currentRecord = UnitOfWork.TargetConditionGoalRepo
                    .GetTargetConditionGoalLibrariesWithTargetConditionGoals()
                    .FirstOrDefault(_ => _.Id == dto.Id);
                if (currentRecord?.Owner == UserId || currentRecord == null)
                {
                    UpsertAnyForLibrary(dto);
                }
                else
                {
                    throw new UnauthorizedAccessException("You are not authorized to modify this library's data.");
                }
            }

            void DeleteAnyFromLibrary(Guid libraryId) =>
                UnitOfWork.TargetConditionGoalRepo.DeleteTargetConditionGoalLibrary(libraryId);

            void DeletePermittedFromLibrary(Guid libraryId)
            {
                var dto = UnitOfWork.TargetConditionGoalRepo
                    .GetTargetConditionGoalLibrariesWithTargetConditionGoals()
                    .FirstOrDefault(_ => _.Id == libraryId);

                if (dto == null) return; // Mimic existing code that does not inform the user the library ID does not exist

                if (dto.Owner == UserId)
                {
                    DeleteAnyFromLibrary(libraryId);
                }
                else
                {
                    throw new UnauthorizedAccessException("You are not authorized to modify this library's data.");
                }
            }

            var AdminCRUDMethods = new CRUDMethods<TargetConditionGoalDTO, TargetConditionGoalLibraryDTO>()
            {
                UpsertScenario = UpsertAnyForScenario,
                RetrieveScenario = RetrieveAnyForScenario,
                DeleteScenario = DeleteAnyFromScenario,
                UpsertLibrary = UpsertAnyForLibrary,
                RetrieveLibrary = RetrieveAnyForLibrary,
                DeleteLibrary = DeleteAnyFromLibrary
            };

            var PermittedCRUDMethods = new CRUDMethods<TargetConditionGoalDTO, TargetConditionGoalLibraryDTO>()
            {
                UpsertScenario = UpsertPermittedForScenario,
                RetrieveScenario = RetrieveAnyForScenario,
                DeleteScenario = DeleteAnyFromScenario,
                UpsertLibrary = UpsertPermittedForLibrary,
                RetrieveLibrary = RetrievePermittedForLibrary,
                DeleteLibrary = DeletePermittedFromLibrary
            };

            return new Dictionary<string, CRUDMethods<TargetConditionGoalDTO, TargetConditionGoalLibraryDTO>>
            {
                [Role.Administrator] = AdminCRUDMethods,
                [Role.DistrictEngineer] = PermittedCRUDMethods,
                [Role.Cwopa] = PermittedCRUDMethods,
                [Role.PlanningPartner] = PermittedCRUDMethods
            };
        }     

        [HttpGet]
        [Route("GetTargetConditionGoalLibraries")]
        [Authorize]
        public async Task<IActionResult> TargetConditionGoalLibraries()
        {
            try
            {
                var result = await Task.Factory.StartNew(() => _targetConditionCRUDMethods[UserInfo.Role].RetrieveLibrary());
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
                var result = await Task.Factory.StartNew(() => _targetConditionCRUDMethods[UserInfo.Role].RetrieveScenario(simulationId));
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
                    _targetConditionCRUDMethods[UserInfo.Role].UpsertLibrary(dto);
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
                    _targetConditionCRUDMethods[UserInfo.Role].UpsertScenario(simulationId, dtos);
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
                    _targetConditionCRUDMethods[UserInfo.Role].DeleteLibrary(libraryId);
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
