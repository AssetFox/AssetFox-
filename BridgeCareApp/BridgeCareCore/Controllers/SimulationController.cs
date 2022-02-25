﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Static;
using BridgeCareCore.Controllers.BaseController;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    using SimulationDeleteMethod = Action<Guid>;
    using SimulationRunMethod = Func<Guid, Guid, Task>;
    using SimulationUpdateMethod = Action<SimulationDTO>;

    [Route("api/[controller]")]
    [ApiController]
    public class SimulationController : BridgeCareCoreBaseController
    {
        private readonly ISimulationAnalysis _simulationAnalysis;

        private readonly IReadOnlyDictionary<string, SimulationUpdateMethod> _simulationUpdateMethods;
        private readonly IReadOnlyDictionary<string, SimulationDeleteMethod> _simulationDeleteMethods;
        private readonly IReadOnlyDictionary<string, SimulationRunMethod> _simulationRunMethods;

        public SimulationController(ISimulationAnalysis simulationAnalysis, IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork,
            IHubService hubService, IHttpContextAccessor httpContextAccessor) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            _simulationAnalysis = simulationAnalysis ?? throw new ArgumentNullException(nameof(simulationAnalysis));
            _simulationUpdateMethods = CreateUpdateMethods();
            _simulationDeleteMethods = CreateDeleteMethods();
            _simulationRunMethods = CreateRunMethods();
        }

        private Dictionary<string, SimulationUpdateMethod> CreateUpdateMethods()
        {
            void UpdateAnySimulation(SimulationDTO dto) => UnitOfWork.SimulationRepo.UpdateSimulation(dto);

            void UpdatePermittedSimulation(SimulationDTO dto)
            {
                CheckUserSimulationModifyAuthorization(dto.Id);
                UpdateAnySimulation(dto);
            }

            return new Dictionary<string, SimulationUpdateMethod>
            {
                [Role.Administrator] = UpdateAnySimulation,
                [Role.Cwopa] = UpdatePermittedSimulation,
                [Role.DistrictEngineer] = UpdatePermittedSimulation,
                [Role.PlanningPartner] = UpdatePermittedSimulation
            };
        }

        private Dictionary<string, SimulationDeleteMethod> CreateDeleteMethods()
        {
            void DeleteAnySimulation(Guid simulationId) => UnitOfWork.SimulationRepo.DeleteSimulation(simulationId);

            void DeletePermittedSimulation(Guid simulationId)
            {
                CheckUserSimulationModifyAuthorization(simulationId);
                DeleteAnySimulation(simulationId);
            }

            return new Dictionary<string, SimulationDeleteMethod>
            {
                [Role.Administrator] = DeleteAnySimulation,
                [Role.Cwopa] = DeletePermittedSimulation,
                [Role.DistrictEngineer] = DeletePermittedSimulation,
                [Role.PlanningPartner] = DeletePermittedSimulation
            };
        }

        private Dictionary<string, SimulationRunMethod> CreateRunMethods()
        {
            Task RunAnySimulation(Guid networkId, Guid simulationId) =>
                _simulationAnalysis.CreateAndRun(networkId, simulationId);

            Task RunPermittedSimulation(Guid networkId, Guid simulationId)
            {
                CheckUserSimulationModifyAuthorization(simulationId);
                return RunAnySimulation(networkId, simulationId);
            }

            return new Dictionary<string, SimulationRunMethod>
            {
                [Role.Administrator] = RunAnySimulation,
                [Role.Cwopa] = RunPermittedSimulation,
                [Role.DistrictEngineer] = RunPermittedSimulation,
                [Role.PlanningPartner] = RunPermittedSimulation
            };
        }

        [HttpGet]
        [Route("GetScenarios/")]
        [Authorize]
        public async Task<IActionResult> GetSimulations()
        {
            try
            {
                var result = await Task.Factory.StartNew(() => UnitOfWork.SimulationRepo.GetAllScenario());
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Scenario error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("CreateScenario/{networkId}")]
        [Authorize]
        public async Task<IActionResult> CreateSimulation(Guid networkId, [FromBody] SimulationDTO dto)
        {
            try
            {
                var result = await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    UnitOfWork.SimulationRepo.CreateSimulation(networkId, dto);
                    UnitOfWork.Commit();
                    return UnitOfWork.SimulationRepo.GetSimulation(dto.Id);
                });

                return Ok(result);
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Scenario error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("CloneScenario/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> CloneSimulation(Guid simulationId)
        {
            try
            {
                var result = await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    var cloneResult = UnitOfWork.SimulationRepo.CloneSimulation(simulationId);
                    UnitOfWork.Commit();
                    return cloneResult;
                });

                if (result.WarningMessage != null)
                {
                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastWarning, result.WarningMessage);
                }
                return Ok(result.Simulation);
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Scenario error::{e.Message}");
                throw;
            }
        }

        [HttpPut]
        [Route("UpdateScenario")]
        [Authorize]
        public async Task<IActionResult> UpdateSimulation([FromBody] SimulationDTO dto)
        {
            try
            {
                var result = await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    _simulationUpdateMethods[UserInfo.Role](dto);
                    UnitOfWork.Commit();
                    return UnitOfWork.SimulationRepo.GetSimulation(dto.Id);
                });

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                UnitOfWork.Rollback();
                return Unauthorized();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Scenario error::{e.Message}");
                throw;
            }
        }

        [HttpDelete]
        [Route("DeleteScenario/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> DeleteSimulation(Guid simulationId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    _simulationDeleteMethods[UserInfo.Role](simulationId);
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Scenario error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("RunSimulation/{networkId}/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> RunSimulation(Guid networkId, Guid simulationId)
        {
            try
            {
                var runTask = _simulationRunMethods[UserInfo.Role](networkId, simulationId);

                await Task.Delay(500); // Allow a brief moment for an empty queue to start running the submission.
                if (runTask.Status is TaskStatus.Created)
                {
                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastScenarioStatusUpdate, "Queued to run.", simulationId);
                }

                await runTask;
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Scenario error::{e.Message}");
                if (e is not SimulationException)
                {
                    var logDto = SimulationLogDtos.GenericException(simulationId, e);
                    UnitOfWork.SimulationLogRepo.CreateLog(logDto);
                }
                throw;
            }
        }
    }
}
