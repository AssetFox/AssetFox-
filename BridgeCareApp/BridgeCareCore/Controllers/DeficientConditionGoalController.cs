using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Controllers.BaseController;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeficientConditionGoalController : BridgeCareCoreBaseController
    {
        private readonly IReadOnlyDictionary<string, CRUDMethods<DeficientConditionGoalDTO, DeficientConditionGoalLibraryDTO>>
            _deficientConditionGoalsCRUDOperations;
        private Guid UserId => UnitOfWork.CurrentUser?.Id ?? Guid.Empty;

        public DeficientConditionGoalController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor) =>
            _deficientConditionGoalsCRUDOperations = CreateCRUDOperations();

        public Dictionary<string, CRUDMethods<DeficientConditionGoalDTO, DeficientConditionGoalLibraryDTO>> CreateCRUDOperations()
        {
            List<DeficientConditionGoalDTO> RetrieveAnyForScenario(Guid simulationId) => UnitOfWork.DeficientConditionGoalRepo
                    .GetScenarioDeficientConditionGoals(simulationId);

            void UpsertAnyForScenario(Guid simulationId, List<DeficientConditionGoalDTO> dtos)
            {
                UnitOfWork.DeficientConditionGoalRepo.UpsertOrDeleteScenarioDeficientConditionGoals(dtos, simulationId);
            }

            void UpsertPermittedForScenario(Guid simulationId, List<DeficientConditionGoalDTO> dtos)
            {
                CheckUserSimulationModifyAuthorization(simulationId);
                UpsertAnyForScenario(simulationId, dtos);
            }

            void DeleteAnyForScenario(Guid simulationId, List<DeficientConditionGoalDTO> dtos)
            {
                // Do nothing
            }

            List<DeficientConditionGoalLibraryDTO> RetrieveAnyForLibraries() => UnitOfWork.DeficientConditionGoalRepo
                    .GetDeficientConditionGoalLibrariesWithDeficientConditionGoals();

            List<DeficientConditionGoalLibraryDTO> RetrievePermittedForLibraries()
            {
                var result = UnitOfWork.DeficientConditionGoalRepo
                    .GetDeficientConditionGoalLibrariesWithDeficientConditionGoals();
                return result.Where(_ => _.Owner == UserId || _.IsShared == true).ToList();
            }

            void UpsertAnyForLibrary(DeficientConditionGoalLibraryDTO dto)
            {
                UnitOfWork.DeficientConditionGoalRepo.UpsertDeficientConditionGoalLibrary(dto);
                UnitOfWork.DeficientConditionGoalRepo.UpsertOrDeleteDeficientConditionGoals(dto.DeficientConditionGoals, dto.Id);
            }

            void UpsertPermittedForLibrary(DeficientConditionGoalLibraryDTO dto)
            {
                if (dto.Owner == UserId)
                {
                    UpsertAnyForLibrary(dto);
                }
                else
                {
                    throw new UnauthorizedAccessException("You are not authorized to modify this simulation's data.");
                }
            }

            void DeleteAnyForLibrary(Guid libraryId) => UnitOfWork.DeficientConditionGoalRepo.DeleteDeficientConditionGoalLibrary(libraryId);

            void DeletePermittedForLibrary(Guid libraryId)
            {
                var dto = UnitOfWork.DeficientConditionGoalRepo.GetDeficientConditionGoalLibrariesWithDeficientConditionGoals()
                    .FirstOrDefault(_ => _.Id == libraryId);
                if (dto == null) return; // Mimic existing code that does not inform the user the library ID does not exist

                if (dto.Owner == UserId)
                {
                    DeleteAnyForLibrary(libraryId);
                }
                else
                {
                    throw new UnauthorizedAccessException("You are not authorized to modify this simulation's data.");
                }
            }

            var AllCRUDAccess = new CRUDMethods<DeficientConditionGoalDTO, DeficientConditionGoalLibraryDTO>()
            {
                UpsertScenario = UpsertAnyForScenario,
                RetrieveScenario = RetrieveAnyForScenario,
                DeleteScenario = DeleteAnyForScenario,
                UpsertLibrary = UpsertAnyForLibrary,
                RetrieveLibrary = RetrieveAnyForLibraries,
                DeleteLibrary = DeleteAnyForLibrary
            };

            var PermittedCRUDAccess = new CRUDMethods<DeficientConditionGoalDTO, DeficientConditionGoalLibraryDTO>()
            {
                UpsertScenario = UpsertPermittedForScenario,
                RetrieveScenario = RetrieveAnyForScenario,
                DeleteScenario = DeleteAnyForScenario,
                UpsertLibrary = UpsertPermittedForLibrary,
                RetrieveLibrary = RetrievePermittedForLibraries,
                DeleteLibrary = DeletePermittedForLibrary
            };

            return new Dictionary<string, CRUDMethods<DeficientConditionGoalDTO, DeficientConditionGoalLibraryDTO>>()
            {
                [Role.Administrator] = AllCRUDAccess,
                [Role.DistrictEngineer] = PermittedCRUDAccess,
                [Role.Cwopa] = PermittedCRUDAccess,
                [Role.PlanningPartner] = PermittedCRUDAccess
            };
        }

        [HttpGet]
        [Route("GetDeficientConditionGoalLibraries")]
        [Authorize]
        public async Task<IActionResult> DeficientConditionGoalLibraries()
        {
            try
            {
                var result = await Task.Factory.StartNew(() => _deficientConditionGoalsCRUDOperations[UserInfo.Role].RetrieveLibrary());
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
                var result = await Task.Factory.StartNew(() => _deficientConditionGoalsCRUDOperations[UserInfo.Role].RetrieveScenario(simulationId));
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
                    _deficientConditionGoalsCRUDOperations[UserInfo.Role].UpsertLibrary(dto);
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
        [Authorize]
        public async Task<IActionResult> UpsertScenarioDeficientConditionGoals(Guid simulationId, List<DeficientConditionGoalDTO> dtos)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    _deficientConditionGoalsCRUDOperations[UserInfo.Role].UpsertScenario(simulationId, dtos);
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
                    _deficientConditionGoalsCRUDOperations[UserInfo.Role].DeleteLibrary(libraryId);
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
