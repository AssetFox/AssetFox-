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
    public class RemainingLifeLimitController : BridgeCareCoreBaseController
    {
        private readonly IReadOnlyDictionary<string, CRUDMethods<RemainingLifeLimitDTO, RemainingLifeLimitLibraryDTO>> _remainingLifeLimitMethods;

        private Guid UserId => UnitOfWork.CurrentUser?.Id ?? Guid.Empty;

        public RemainingLifeLimitController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor) =>
            _remainingLifeLimitMethods = CreateCRUDMethods();

        private Dictionary<string, CRUDMethods<RemainingLifeLimitDTO, RemainingLifeLimitLibraryDTO>> CreateCRUDMethods()
        {
            List<RemainingLifeLimitDTO> RetrieveAnyForScenario(Guid scenarioId)
                => UnitOfWork.RemainingLifeLimitRepo.GetScenarioRemainingLifeLimits(scenarioId);

            void UpsertAnyForScenario(Guid scenarioId, List<RemainingLifeLimitDTO> dtos)
            {
                UnitOfWork.RemainingLifeLimitRepo.UpsertOrDeleteScenarioRemainingLifeLimits(dtos, scenarioId);
            }

            void UpsertPermittedForScenario(Guid scenarioId, List<RemainingLifeLimitDTO> dtos)
            {
                CheckUserSimulationModifyAuthorization(scenarioId);
                UpsertAnyForScenario(scenarioId, dtos);
            }

            void DeleteAnyFromScenario(Guid scenarioId, List<RemainingLifeLimitDTO> dtos)
            {
                // Do Nothing
            }

            List<RemainingLifeLimitLibraryDTO> RetrieveAnyForLibraries() =>
                UnitOfWork.RemainingLifeLimitRepo.RemainingLifeLimitLibrariesWithRemainingLifeLimits();

            List<RemainingLifeLimitLibraryDTO> RetrievePermittedForLibraries()
            {
                var result = UnitOfWork.RemainingLifeLimitRepo.RemainingLifeLimitLibrariesWithRemainingLifeLimits();
                return result.Where(_ => _.Owner == UserId || _.IsShared == true).ToList();
            }

            void UpsertAnyForLibrary(RemainingLifeLimitLibraryDTO dto)
            {
                UnitOfWork.RemainingLifeLimitRepo.UpsertRemainingLifeLimitLibrary(dto);
                UnitOfWork.RemainingLifeLimitRepo.UpsertOrDeleteRemainingLifeLimits(dto.RemainingLifeLimits, dto.Id);
            }

            void UpsertPermittedForLibrary(RemainingLifeLimitLibraryDTO dto)
            {
                var currentRecord = UnitOfWork.RemainingLifeLimitRepo
                    .RemainingLifeLimitLibrariesWithRemainingLifeLimits()
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
                UnitOfWork.RemainingLifeLimitRepo.DeleteRemainingLifeLimitLibrary(libraryId);

            void DeletePermittedFromLibrary(Guid libraryId)
            {
                var dto = UnitOfWork.RemainingLifeLimitRepo
                    .RemainingLifeLimitLibrariesWithRemainingLifeLimits()
                    .FirstOrDefault(_ => _.Id == libraryId);

                if (dto == null) return;  // Mimic existing code that does not inform the user the library ID does not exist

                if (dto.Owner == UserId)
                {
                    DeleteAnyFromLibrary(dto.Id);
                }
                else
                {
                    throw new UnauthorizedAccessException("You are not authorized to modify this library's data.");
                }
            }

            var AdminCRUDMethods = new CRUDMethods<RemainingLifeLimitDTO, RemainingLifeLimitLibraryDTO>()
            {
                UpsertScenario = UpsertAnyForScenario,
                RetrieveScenario = RetrieveAnyForScenario,
                DeleteScenario = DeleteAnyFromScenario,
                UpsertLibrary = UpsertAnyForLibrary,
                RetrieveLibrary = RetrieveAnyForLibraries,
                DeleteLibrary = DeleteAnyFromLibrary
            };

            var PermittedCRUDMethods = new CRUDMethods<RemainingLifeLimitDTO, RemainingLifeLimitLibraryDTO>()
            {
                UpsertScenario = UpsertPermittedForScenario,
                RetrieveScenario = RetrieveAnyForScenario,
                DeleteScenario = DeleteAnyFromScenario,
                UpsertLibrary = UpsertPermittedForLibrary,
                RetrieveLibrary = RetrievePermittedForLibraries,
                DeleteLibrary = DeletePermittedFromLibrary
            };

            return new Dictionary<string, CRUDMethods<RemainingLifeLimitDTO, RemainingLifeLimitLibraryDTO>>
            {
                [Role.Administrator] = AdminCRUDMethods,
                [Role.DistrictEngineer] = PermittedCRUDMethods,
                [Role.Cwopa] = PermittedCRUDMethods,
                [Role.PlanningPartner] = PermittedCRUDMethods
            };
        }

        [HttpGet]
        [Route("GetRemainingLifeLimitLibraries")]
        [Authorize(Policy = SecurityConstants.Policy.Admin)]
        public async Task<IActionResult> RemainingLifeLimitLibraries()
        {
            try
            {
                var result = await Task.Factory.StartNew(() => _remainingLifeLimitMethods[UserInfo.Role].RetrieveLibrary());
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Remaining Life Limit error::{e.Message}");
                throw;
            }
        }
        [HttpGet]
        [Route("GetScenarioRemainingLifeLimits/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> GetScenarioRemainingLifeLimits(Guid simulationId)
        {
            try
            {
                var result = await Task.Factory.StartNew(() => _remainingLifeLimitMethods[UserInfo.Role].RetrieveScenario(simulationId));
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Remaining life limit error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertRemainingLifeLimitLibrary/")]
        [Authorize(Policy = SecurityConstants.Policy.Admin)]
        public async Task<IActionResult> UpsertRemainingLifeLimitLibrary(RemainingLifeLimitLibraryDTO dto)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    _remainingLifeLimitMethods[UserInfo.Role].UpsertLibrary(dto);
                    UnitOfWork.Commit();
                });


                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Remaining Life Limit error::{e.Message}");
                throw;
            }
        }
        [HttpPost]
        [Route("UpsertScenarioRemainingLifeLimits/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> UpsertScenarioRemainingLifeLimits(Guid simulationId, List<RemainingLifeLimitDTO> dtos)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    _remainingLifeLimitMethods[UserInfo.Role].UpsertScenario(simulationId, dtos);
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
        [Route("DeleteRemainingLifeLimitLibrary/{libraryId}")]
        [Authorize(Policy = SecurityConstants.Policy.Admin)]
        public async Task<IActionResult> DeleteRemainingLifeLimitLibrary(Guid libraryId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    _remainingLifeLimitMethods[UserInfo.Role].DeleteLibrary(libraryId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Remaining Life Limit error::{e.Message}");
                throw;
            }
        }
    }
}
