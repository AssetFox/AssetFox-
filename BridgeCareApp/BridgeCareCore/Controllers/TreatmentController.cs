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
    using ScenarioTreatmentUpsertMethod = Action<Guid, List<TreatmentDTO>>;

    [Route("api/[controller]")]
    [ApiController]
    public class TreatmentController : BridgeCareCoreBaseController
    {
        private readonly IReadOnlyDictionary<string, ScenarioTreatmentUpsertMethod> _scenarioTreatmentUpsertMethods;

        public TreatmentController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor) =>
            _scenarioTreatmentUpsertMethods = CreateScenarioUpsertMethods();

        private Dictionary<string, ScenarioTreatmentUpsertMethod> CreateScenarioUpsertMethods()
        {
            void UpsertAny(Guid simulationId, List<TreatmentDTO> dtos)
            {
                UnitOfWork.SelectableTreatmentRepo.UpsertOrDeleteScenarioSelectableTreatment(dtos, simulationId);
            }

            void UpsertPermitted(Guid simulationId, List<TreatmentDTO> dtos)
            {
                CheckUserSimulationModifyAuthorization(simulationId);
                UpsertAny(simulationId, dtos);
            }

            return new Dictionary<string, ScenarioTreatmentUpsertMethod>
            {
                [Role.Administrator] = UpsertAny,
                [Role.DistrictEngineer] = UpsertPermitted
            };
        }

        [HttpGet]
        [Route("GetTreatmentLibraries")]
        [Authorize]
        public async Task<IActionResult> TreatmentLibraries()
        {
            try
            {
                var result = await Task.Factory.StartNew(() => UnitOfWork.SelectableTreatmentRepo
                    .GetTreatmentLibrariesWithTreatments());
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Treatment error::{e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetScenarioSelectedTreatments/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> GetScenarioSelectedTreatments(Guid simulationId)
        {
            try
            {
                var result = await Task.Factory.StartNew(() => UnitOfWork.SelectableTreatmentRepo
                    .GetScenarioSelectableTreatments(simulationId));
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Treatment error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertTreatmentLibrary")]
        [Authorize(Policy = SecurityConstants.Policy.AdminOrDistrictEngineer)]
        public async Task<IActionResult> UpsertTreatmentLibrary(TreatmentLibraryDTO dto)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    UnitOfWork.SelectableTreatmentRepo.UpsertTreatmentLibrary(dto);
                    UnitOfWork.SelectableTreatmentRepo.UpsertOrDeleteTreatments(dto.Treatments, dto.Id);
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Treatment error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertScenarioSelectableTreatments/{simulationId}")]
        [Authorize(Policy = SecurityConstants.Policy.AdminOrDistrictEngineer)]
        public async Task<IActionResult> UpsertScenarioSelectableTreatments(Guid SimulationId, List<TreatmentDTO> dtos)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    _scenarioTreatmentUpsertMethods[UserInfo.Role](SimulationId, dtos);
                    UnitOfWork.Commit();
                });
                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                UnitOfWork.Rollback();
                return Unauthorized();
            }
            catch(Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Treatment error::{e.Message}");
                throw;
            }
        }

        [HttpDelete]
        [Route("DeleteTreatmentLibrary/{libraryId}")]
        [Authorize(Policy = SecurityConstants.Policy.AdminOrDistrictEngineer)]
        public async Task<IActionResult> DeleteTreatmentLibrary(Guid libraryId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    UnitOfWork.SelectableTreatmentRepo.DeleteTreatmentLibrary(libraryId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Treatment error::{e.Message}");
                throw;
            }
        }
    }
}
