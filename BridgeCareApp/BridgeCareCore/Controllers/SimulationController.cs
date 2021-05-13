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
    using SimulationDeleteMethod = Action<Guid>;
    using SimulationRunMethod = Action<Guid, Guid>;
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
            void RunAnySimulation(Guid networkId, Guid simulationId) =>
                _simulationAnalysis.CreateAndRun(networkId, simulationId);

            void RunPermittedSimulation(Guid networkId, Guid simulationId)
            {
                CheckUserSimulationModifyAuthorization(simulationId);
                RunAnySimulation(networkId, simulationId);
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
        [Route("GetScenarios/{networkId}")]
        [Authorize]
        public async Task<IActionResult> GetSimulations(Guid networkId)
        {
            try
            {
                var result = await Task.Factory.StartNew(() => UnitOfWork.SimulationRepo.GetAllInNetwork(networkId));
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

                return Ok(result);
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
                await Task.Factory.StartNew(() =>
                    _simulationRunMethods[UserInfo.Role](networkId, simulationId));

                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Scenario error::{e.Message}");
                throw;
            }
        }
    }
}
