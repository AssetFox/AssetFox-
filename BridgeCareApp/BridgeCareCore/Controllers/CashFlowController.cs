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
    public class CashFlowController : BridgeCareCoreBaseController
    {
        private readonly IReadOnlyDictionary<string, CRUDMethods<CashFlowRuleDTO, CashFlowRuleLibraryDTO>> _cashFlowCRUDMethods;
        private Guid UserId => UnitOfWork.UserEntity?.Id ?? Guid.Empty;

        public CashFlowController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor) =>
            _cashFlowCRUDMethods = CreateCRUDOperations();

        private Dictionary<string, CRUDMethods<CashFlowRuleDTO, CashFlowRuleLibraryDTO>> CreateCRUDOperations()
        {
            List<CashFlowRuleDTO> GetAnyFromScenario(Guid simulationId) => UnitOfWork.CashFlowRuleRepo
                    .GetScenarioCashFlowRules(simulationId);

            void UpsertAnyFromScenario(Guid simulationId, List<CashFlowRuleDTO> dtos)
            {
                UnitOfWork.CashFlowRuleRepo.UpsertOrDeleteScenarioCashFlowRules(dtos, simulationId);
            }

            void UpsertPermittedFromScenario(Guid simulationId, List<CashFlowRuleDTO> dtos)
            {
                CheckUserSimulationModifyAuthorization(simulationId);
                UpsertAnyFromScenario(simulationId, dtos);
            }

            void DeleteAnyFromScenario(Guid simulationId, List<CashFlowRuleDTO> dtos)
            {
                // Do Nothing
            }

            List<CashFlowRuleLibraryDTO> GetAnyFromLibrary() => UnitOfWork.CashFlowRuleRepo
                    .GetCashFlowRuleLibraries();

            List<CashFlowRuleLibraryDTO> GetPermittedFromLibrary()
            {
                var result = UnitOfWork.CashFlowRuleRepo.GetCashFlowRuleLibraries();
                return result.Where(_ => _.Owner == UserId || _.IsShared == true).ToList();
            }

            void UpsertAnyFromLibrary(CashFlowRuleLibraryDTO dto)
            {
                UnitOfWork.CashFlowRuleRepo.UpsertCashFlowRuleLibrary(dto);
                UnitOfWork.CashFlowRuleRepo.UpsertOrDeleteCashFlowRules(dto.CashFlowRules, dto.Id);
            }

            void UpsertPermittedFromLibrary(CashFlowRuleLibraryDTO dto)
            {
                if (dto.Owner == UserId)
                {
                    UpsertAnyFromLibrary(dto);
                }
                else
                {
                    throw new UnauthorizedAccessException("You are not authorized to modify this simulation's data.");
                }
            }

            void DeleteAnyFromLibrary(Guid libraryId) => UnitOfWork.CashFlowRuleRepo.DeleteCashFlowRuleLibrary(libraryId);

            void DeletePermittedFromLibrary(Guid libraryId)
            {
                var dto = UnitOfWork.CashFlowRuleRepo.GetCashFlowRuleLibraries().FirstOrDefault(_ => _.Id == libraryId);

                if (dto == null) return; // Mimic existing code that does not inform the user the library ID does not exist

                if (dto.Owner == UserId)
                {
                    DeleteAnyFromLibrary(libraryId);
                }
                else
                {
                    throw new UnauthorizedAccessException("You are not authorized to modify this simulation's data.");
                }
            }

            var AllCRUDAccess = new CRUDMethods<CashFlowRuleDTO, CashFlowRuleLibraryDTO>()
            {
                UpsertScenario = UpsertAnyFromScenario,
                RetrieveScenario = GetAnyFromScenario,
                DeleteScenario = DeleteAnyFromScenario,
                UpsertLibrary = UpsertAnyFromLibrary,
                RetrieveLibrary = GetAnyFromLibrary,
                DeleteLibrary = DeletePermittedFromLibrary
            };

            var PermittedCRUDAccess = new CRUDMethods<CashFlowRuleDTO, CashFlowRuleLibraryDTO>()
            {
                UpsertScenario = UpsertPermittedFromScenario,
                RetrieveScenario = GetAnyFromScenario,
                DeleteScenario = DeleteAnyFromScenario,
                UpsertLibrary = UpsertPermittedFromLibrary,
                RetrieveLibrary = GetPermittedFromLibrary,
                DeleteLibrary = DeleteAnyFromLibrary
            };

            return new Dictionary<string, CRUDMethods<CashFlowRuleDTO, CashFlowRuleLibraryDTO>>()
            {
                [Role.Administrator] = AllCRUDAccess,
                [Role.DistrictEngineer] = PermittedCRUDAccess,
                [Role.Cwopa] = PermittedCRUDAccess,
                [Role.PlanningPartner] = PermittedCRUDAccess
            };
        }

        [HttpGet]
        [Route("GetCashFlowRuleLibraries")]
        [Authorize]
        public async Task<IActionResult> GetCashFlowRuleLibraries()
        {
            try
            {
                var result = await Task.Factory.StartNew(() => _cashFlowCRUDMethods[UserInfo.Role].RetrieveLibrary());

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Cash Flow error::{e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetScenarioCashFlowRules/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> GetScenarioCashFlowRules(Guid simulationId)
        {
            try
            {
                var result = await Task.Factory.StartNew(() => _cashFlowCRUDMethods[UserInfo.Role].RetrieveScenario(simulationId));

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Cash Flow error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertCashFlowRuleLibrary")]
        [Authorize(Policy = SecurityConstants.Policy.AdminOrDistrictEngineer)]
        public async Task<IActionResult> UpsertCashFlowRuleLibrary(CashFlowRuleLibraryDTO dto)
        {
            try
            {
                 await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    _cashFlowCRUDMethods[UserInfo.Role].UpsertLibrary(dto);
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Cash Flow error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertScenarioCashFlowRules/{simulationId}")]
        [Authorize(Policy = SecurityConstants.Policy.AdminOrDistrictEngineer)]
        public async Task<IActionResult> UpsertScenarioCashFlowRules(Guid simulationId, List<CashFlowRuleDTO> dtos)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    _cashFlowCRUDMethods[UserInfo.Role].UpsertScenario(simulationId, dtos);
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Cash Flow error::{e.Message}");
                throw;
            }
        }

        [HttpDelete]
        [Route("DeleteCashFlowRuleLibrary/{libraryId}")]
        [Authorize(Policy = SecurityConstants.Policy.AdminOrDistrictEngineer)]
        public async Task<IActionResult> DeleteCashFlowRuleLibrary(Guid libraryId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    _cashFlowCRUDMethods[UserInfo.Role].DeleteLibrary(libraryId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Cash Flow error::{e.Message}");
                throw;
            }
        }
    }
}
