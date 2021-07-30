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
    using ScenarioPerformanceCurveUpsertMethod = Action<Guid, List<PerformanceCurveDTO>>;

    [Route("api/[controller]")]
    [ApiController]
    public class PerformanceCurveController : BridgeCareCoreBaseController
    {
        private readonly IReadOnlyDictionary<string, ScenarioPerformanceCurveUpsertMethod> _scenarioPerformanceCurveUpsertMethods;

        public PerformanceCurveController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork,
            IHubService hubService,
            IHttpContextAccessor httpContextAccessor) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor) =>
            _scenarioPerformanceCurveUpsertMethods = CreateScenarioPerformanceCurveUpsertMethods();

        private Dictionary<string, ScenarioPerformanceCurveUpsertMethod> CreateScenarioPerformanceCurveUpsertMethods()
        {
            void UpsertAny(Guid simulationId, List<PerformanceCurveDTO> dtos)
            {
                UnitOfWork.PerformanceCurveRepo.UpsertOrDeleteScenarioPerformanceCurves(dtos, simulationId);
            }

            void UpsertPermitted(Guid simulationId, List<PerformanceCurveDTO> dtos)
            {
                CheckUserSimulationModifyAuthorization(simulationId);
                UpsertAny(simulationId, dtos);
            }

            return new Dictionary<string, ScenarioPerformanceCurveUpsertMethod>
            {
                [Role.Administrator] = UpsertAny,
                [Role.DistrictEngineer] = UpsertPermitted
            };
        }

        [HttpGet]
        [Route("GetPerformanceCurveLibraries")]
        [Authorize]
        public async Task<IActionResult> GetPerformanceCurveLibraries()
        {
            try
            {
                var result = await Task.Factory.StartNew(() => UnitOfWork.PerformanceCurveRepo
                    .GetPerformanceCurveLibrariesWithPerformanceCurves());
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
                var result = await Task.Factory.StartNew(() => UnitOfWork.PerformanceCurveRepo
                    .GetScenarioPerformanceCurves(simulationId));
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
                    UnitOfWork.PerformanceCurveRepo.UpsertPerformanceCurveLibrary(dto);
                    UnitOfWork.PerformanceCurveRepo.UpsertOrDeletePerformanceCurves(dto.PerformanceCurves, dto.Id);
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
                    _scenarioPerformanceCurveUpsertMethods[UserInfo.Role](simulationId, dtos);
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
                    UnitOfWork.PerformanceCurveRepo.DeletePerformanceCurveLibrary(libraryId);
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
