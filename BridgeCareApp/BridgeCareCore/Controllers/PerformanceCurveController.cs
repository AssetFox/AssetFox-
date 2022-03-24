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
    public class PerformanceCurveController : BridgeCareCoreBaseController
    {
        private readonly IReadOnlyDictionary<string, CRUDMethods<PerformanceCurveDTO, PerformanceCurveLibraryDTO>> _performanceCRUDMethods;

        private Guid UserId => UnitOfWork.UserEntity?.Id ?? Guid.Empty;

        public PerformanceCurveController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork,
            IHubService hubService,
            IHttpContextAccessor httpContextAccessor) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor) =>
            _performanceCRUDMethods = CreateCRUDMethods();

        private Dictionary<string, CRUDMethods<PerformanceCurveDTO, PerformanceCurveLibraryDTO>> CreateCRUDMethods()
        {
            void UpsertAnyForScenario(Guid simulationId, List<PerformanceCurveDTO> dtos)
            {
                UnitOfWork.PerformanceCurveRepo.UpsertOrDeleteScenarioPerformanceCurves(dtos, simulationId);
            }

            void UpsertPermittedForScenario(Guid simulationId, List<PerformanceCurveDTO> dtos)
            {
                CheckUserSimulationModifyAuthorization(simulationId);
                UpsertAnyForScenario(simulationId, dtos);
            }

            List<PerformanceCurveDTO> RetrieveAnyForScenario(Guid simulationId) =>
                UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);

            void DeleteAnyFromScenario(Guid simulationId, List<PerformanceCurveDTO> dtos)
            {
                // Do Nothing
            }

            List<PerformanceCurveLibraryDTO> RetrieveAnyForLibraries() =>
                UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibraries();

            List<PerformanceCurveLibraryDTO> RetrievePermittedForLibraries()
            {
                var result = UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibraries();
                return result.Where(_ => _.Owner == UserId || _.IsShared == true).ToList();
            }

            void UpsertAnyForLibrary(PerformanceCurveLibraryDTO dto)
            {
                UnitOfWork.PerformanceCurveRepo.UpsertPerformanceCurveLibrary(dto);
                UnitOfWork.PerformanceCurveRepo.UpsertOrDeletePerformanceCurves(dto.PerformanceCurves, dto.Id);
            }

            void UpsertPermittedForLibrary(PerformanceCurveLibraryDTO dto)
            {
                if (dto.Owner == UserId || !UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibraries().Any(_ => _.Id == dto.Id))
                {
                    UpsertAnyForLibrary(dto);
                }
                else
                {
                    throw new UnauthorizedAccessException("You are not authorized to modify this library's data.");
                }
            }

            void DeleteAnyForLibrary(Guid libraryId) =>
                UnitOfWork.PerformanceCurveRepo.DeletePerformanceCurveLibrary(libraryId);

            void DeletePermittedForLibrary(Guid libraryId)
            {
                var dto = UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibraries().FirstOrDefault(_ => _.Id == libraryId);

                if (dto == null) return; // Mimic existing code that does not inform the user the library ID does not exist

                if (dto.Owner == UserId)
                {
                    DeleteAnyForLibrary(libraryId);
                }
                else
                {
                    throw new UnauthorizedAccessException("You are not authorized to modify this library's data.");
                }
            }

            var AdminCRUDMethods = new CRUDMethods<PerformanceCurveDTO, PerformanceCurveLibraryDTO>()
            {
                UpsertScenario = UpsertAnyForScenario,
                RetrieveScenario = RetrieveAnyForScenario,
                DeleteScenario = DeleteAnyFromScenario,
                UpsertLibrary = UpsertAnyForLibrary,
                RetrieveLibrary = RetrieveAnyForLibraries,
                DeleteLibrary = DeleteAnyForLibrary
            };

            var PermittedCRUDMethods = new CRUDMethods<PerformanceCurveDTO, PerformanceCurveLibraryDTO>()
            {
                UpsertScenario = UpsertPermittedForScenario,
                RetrieveScenario = RetrieveAnyForScenario,
                DeleteScenario = DeleteAnyFromScenario,
                UpsertLibrary = UpsertPermittedForLibrary,
                RetrieveLibrary = RetrievePermittedForLibraries,
                DeleteLibrary = DeletePermittedForLibrary
            };

            return new Dictionary<string, CRUDMethods<PerformanceCurveDTO, PerformanceCurveLibraryDTO>>
            {
                [Role.Administrator] = AdminCRUDMethods,
                [Role.DistrictEngineer] = PermittedCRUDMethods,
                [Role.Cwopa] = PermittedCRUDMethods,
                [Role.PlanningPartner] = PermittedCRUDMethods
            };
        }

        [HttpGet]
        [Route("GetPerformanceCurveLibraries")]
        [Authorize]
        public async Task<IActionResult> GetPerformanceCurveLibraries()
        {
            try
            {
                var result = await Task.Factory.StartNew(() => _performanceCRUDMethods[UserInfo.Role].RetrieveLibrary());
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Performance Curve error::{e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetScenarioPerformanceCurves/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> GetScenarioPerformanceCurves(Guid simulationId)
        {
            try
            {
                var result = await Task.Factory.StartNew(() => _performanceCRUDMethods[UserInfo.Role].RetrieveScenario(simulationId));
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Performance Curve error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertPerformanceCurveLibrary")]
        [Authorize(Policy = SecurityConstants.Policy.AdminOrDistrictEngineer)]
        public async Task<IActionResult> UpsertPerformanceCurveLibrary(PerformanceCurveLibraryDTO dto)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    _performanceCRUDMethods[UserInfo.Role].UpsertLibrary(dto);
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Performance Curve error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertScenarioPerformanceCurves/{simulationId}")]
        [Authorize(Policy = SecurityConstants.Policy.AdminOrDistrictEngineer)]
        public async Task<IActionResult> UpsertScenarioPerformanceCurves(Guid simulationId, List<PerformanceCurveDTO> dtos)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    _performanceCRUDMethods[UserInfo.Role].UpsertScenario(simulationId, dtos);
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Performance Curve error::{e.Message}");
                throw;
            }
        }

        [HttpDelete]
        [Route("DeletePerformanceCurveLibrary/{libraryId}")]
        [Authorize(Policy = SecurityConstants.Policy.AdminOrDistrictEngineer)]
        public async Task<IActionResult> DeletePerformanceCurveLibrary(Guid libraryId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    _performanceCRUDMethods[UserInfo.Role].DeleteLibrary(libraryId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Performance Curve error::{e.Message}");
                throw;
            }
        }
    }
}
