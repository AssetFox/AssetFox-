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
    // using BudgetPriorityUpsertMethod = Action<Guid, List<BudgetPriorityDTO>>;

    [Route("api/[controller]")]
    [ApiController]
    public class BudgetPriorityController : BridgeCareCoreBaseController
    {
        //private readonly IReadOnlyDictionary<string, BudgetPriorityUpsertMethod> _budgetPriorityUpsertMethods;
        private readonly IReadOnlyDictionary<string, CRUDMethods<BudgetPriorityDTO, BudgetPriorityLibraryDTO>> _budgetPriorityMethods;

        private Guid UserId => UnitOfWork.UserEntity?.Id ?? Guid.Empty;

        public BudgetPriorityController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor) =>
            _budgetPriorityMethods = CreateMethods();

        private Dictionary<string, CRUDMethods<BudgetPriorityDTO, BudgetPriorityLibraryDTO>> CreateMethods()
        {
            void UpsertAnyFromScenario(Guid simulationId, List<BudgetPriorityDTO> dtos)
            {
                UnitOfWork.BudgetPriorityRepo.UpsertOrDeleteScenarioBudgetPriorities(dtos, simulationId);
            }

            void UpsertPermittedFromScenario(Guid simulationId, List<BudgetPriorityDTO> dtos)
            {
                CheckUserSimulationModifyAuthorization(simulationId);
                UpsertAnyFromScenario(simulationId, dtos);
            }

            List<BudgetPriorityDTO> RetrieveAnyFromScenario(Guid simulationId) => UnitOfWork.BudgetPriorityRepo
                    .GetScenarioBudgetPriorities(simulationId);

            void DeleteAnyFromScenario(Guid simulationId, List<BudgetPriorityDTO> dtos)
            {
                // Do nothing
            }

            List<BudgetPriorityLibraryDTO> RetrieveAnyFromLibrary() => UnitOfWork.BudgetPriorityRepo
                    .GetBudgetPriorityLibraries();

            List<BudgetPriorityLibraryDTO> RetrievePermittedFromLibrary()
            {
                var result = UnitOfWork.BudgetPriorityRepo.GetBudgetPriorityLibraries();
                return result.Where(_ => _.Owner == UserId || _.IsShared == true).ToList();
            }

            void UpsertAnyFromLibrary(BudgetPriorityLibraryDTO dto)
            {
                UnitOfWork.BudgetPriorityRepo.UpsertBudgetPriorityLibrary(dto);
                UnitOfWork.BudgetPriorityRepo.UpsertOrDeleteBudgetPriorities(dto.BudgetPriorities, dto.Id);
            }

            void UpsertPermittedFromLibrary(BudgetPriorityLibraryDTO dto)
            {
                if (dto.Owner == UserId)
                {
                    UpsertAnyFromLibrary(dto);
                }
                else
                {
                    throw new UnauthorizedAccessException("You are not authorized to modify this library's data.");
                }
            }

            void DeleteAnyFromLibrary(Guid libraryId) => UnitOfWork.BudgetPriorityRepo.DeleteBudgetPriorityLibrary(libraryId);

            void DeletePermittedFromLibrary(Guid libraryId)
            {
                var dto = UnitOfWork.BudgetPriorityRepo.GetBudgetPriorityLibraries().FirstOrDefault(_ => _.Id == libraryId);

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

            var AdminCRUDMethods = new CRUDMethods<BudgetPriorityDTO, BudgetPriorityLibraryDTO>()
            {
                UpsertScenario = UpsertAnyFromScenario,
                RetrieveScenario = RetrieveAnyFromScenario,
                DeleteScenario = DeleteAnyFromScenario,
                UpsertLibrary = UpsertAnyFromLibrary,
                RetrieveLibrary = RetrieveAnyFromLibrary,
                DeleteLibrary = DeleteAnyFromLibrary
            };

            var PermittedCRUDMethods = new CRUDMethods<BudgetPriorityDTO, BudgetPriorityLibraryDTO>()
            {
                UpsertScenario = UpsertPermittedFromScenario,
                RetrieveScenario = RetrieveAnyFromScenario,
                DeleteScenario = DeleteAnyFromScenario,
                UpsertLibrary = UpsertPermittedFromLibrary,
                RetrieveLibrary = RetrievePermittedFromLibrary,
                DeleteLibrary = DeletePermittedFromLibrary
            };

            return new Dictionary<string, CRUDMethods<BudgetPriorityDTO, BudgetPriorityLibraryDTO>>
            {
                [Role.Administrator] = AdminCRUDMethods,
                [Role.DistrictEngineer] = PermittedCRUDMethods,
                [Role.Cwopa] = PermittedCRUDMethods,
                [Role.PlanningPartner] = PermittedCRUDMethods
            };

            //return new Dictionary<string, BudgetPriorityUpsertMethod>
            //{
            //    [Role.Administrator] = UpsertAnyFromScenario,
            //    [Role.DistrictEngineer] = UpsertPermittedFromScenario,
            //    [Role.Cwopa] = UpsertPermittedFromScenario,
            //    [Role.PlanningPartner] = UpsertPermittedFromScenario
            //};
        }

        [HttpGet]
        [Route("GetBudgetPriorityLibraries")]
        [Authorize]
        public async Task<IActionResult> GetBudgetPriorityLibraries()
        {
            try
            {
                //var result = await Task.Factory.StartNew(() => UnitOfWork.BudgetPriorityRepo
                //    .GetBudgetPriorityLibraries());
                var result = await Task.Factory.StartNew(() => _budgetPriorityMethods[UserInfo.Role].RetrieveLibrary());

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Budget Priority error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertBudgetPriorityLibrary")]
        [Authorize(Policy = SecurityConstants.Policy.AdminOrDistrictEngineer)]
        public async Task<IActionResult> UpsertBudgetPriorityLibrary(BudgetPriorityLibraryDTO dto)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    //UnitOfWork.BudgetPriorityRepo.UpsertBudgetPriorityLibrary(dto);
                    //UnitOfWork.BudgetPriorityRepo.UpsertOrDeleteBudgetPriorities(dto.BudgetPriorities, dto.Id);
                    _budgetPriorityMethods[UserInfo.Role].UpsertLibrary(dto);
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Budget Priority error::{e.Message}");
                throw;
            }
        }

        [HttpDelete]
        [Route("DeleteBudgetPriorityLibrary/{libraryId}")]
        [Authorize(Policy = SecurityConstants.Policy.AdminOrDistrictEngineer)]
        public async Task<IActionResult> DeleteBudgetPriorityLibrary(Guid libraryId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    //UnitOfWork.BudgetPriorityRepo.DeleteBudgetPriorityLibrary(libraryId);
                    _budgetPriorityMethods[UserInfo.Role].DeleteLibrary(libraryId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Budget Priority error::{e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetScenarioBudgetPriorities/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> GetScenarioBudgetPriorities(Guid simulationId)
        {
            try
            {
                //var result = await Task.Factory.StartNew(() => UnitOfWork.BudgetPriorityRepo
                //    .GetScenarioBudgetPriorities(simulationId));
                var result = await Task.Factory.StartNew(() => _budgetPriorityMethods[UserInfo.Role].RetrieveScenario(simulationId));

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Budget Priority error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertScenarioBudgetPriorities/{simulationId}")]
        [Authorize(Policy = SecurityConstants.Policy.AdminOrDistrictEngineer)]
        public async Task<IActionResult> UpsertScenarioBudgetPriorities(Guid simulationId, List<BudgetPriorityDTO> dtos)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    //_budgetPriorityMethods[UserInfo.Role](simulationId, dtos);
                    _budgetPriorityMethods[UserInfo.Role].UpsertScenario(simulationId, dtos);
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Budget Priority error::{e.Message}");
                throw;
            }
        }
    }
}
