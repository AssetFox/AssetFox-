using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    using SimulationDeleteMethod = Action<Guid>;
    using SimulationRunMethod = Action<Guid, Guid>;
    using SimulationUpdateMethod = Action<SimulationDTO>;

    [Route("api/[controller]")]
    [ApiController]
    public class SimulationController : HubControllerBase
    {
        private readonly IEsecSecurity _esecSecurity;
        private readonly UnitOfDataPersistenceWork _unitOfWork;
        private readonly ISimulationAnalysis _simulationAnalysis;

        private readonly IReadOnlyDictionary<string, SimulationUpdateMethod> _simulationUpdateMethods;
        private readonly IReadOnlyDictionary<string, SimulationDeleteMethod> _simulationDeleteMethods;
        private readonly IReadOnlyDictionary<string, SimulationRunMethod> _simulationRunMethods;

        public SimulationController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfDataPersistenceWork,
            ISimulationAnalysis simulationAnalysis, IHubService hubService) : base(hubService)
        {
            _esecSecurity = esecSecurity ?? throw new ArgumentNullException(nameof(esecSecurity));
            _unitOfWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
            _simulationAnalysis = simulationAnalysis ?? throw new ArgumentNullException(nameof(simulationAnalysis));
            _simulationUpdateMethods = CreateUpdateMethods();
            _simulationDeleteMethods = CreateDeleteMethods();
            _simulationRunMethods = CreateRunMethods();
        }

        private Dictionary<string, SimulationUpdateMethod> CreateUpdateMethods()
        {
            void UpdateAnySimulation(SimulationDTO dto) => _unitOfWork.SimulationRepo.UpdateSimulation(dto);

            void UpdatePermittedSimulation(SimulationDTO dto) => _unitOfWork.SimulationRepo.UpdatePermittedSimulation(dto);

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
            void DeleteAnySimulation(Guid simulationId) => _unitOfWork.SimulationRepo.DeleteSimulation(simulationId);

            void DeletePermittedSimulation(Guid simulationId) =>
                _unitOfWork.SimulationRepo.DeletePermittedSimulation(simulationId);

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

            void RunPermittedSimulation(Guid networkId, Guid simulationId) =>
                _simulationAnalysis.CreateAndRunPermitted(networkId, simulationId);

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
                var result = await Task.Factory.StartNew(() => _unitOfWork.SimulationRepo.GetAllInNetwork(networkId));
                return Ok(result);
            }
            catch (Exception e)
            {
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Scenario error::{e.Message}");
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
                var userInfo = _esecSecurity.GetUserInformation(Request);

                _unitOfWork.SetUser(userInfo.Name);

                var result = await Task.Factory.StartNew(() =>
                {
                    _unitOfWork.BeginTransaction();
                    _unitOfWork.SimulationRepo.CreateSimulation(networkId, dto);
                    _unitOfWork.Commit();
                    return _unitOfWork.SimulationRepo.GetSimulation(dto.Id);
                });

                return Ok(result);
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Scenario error::{e.Message}");
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
                var userInfo = _esecSecurity.GetUserInformation(Request);

                _unitOfWork.SetUser(userInfo.Name);

                var result = await Task.Factory.StartNew(() =>
                {
                    _unitOfWork.BeginTransaction();
                    var cloneResult = _unitOfWork.SimulationRepo.CloneSimulation(simulationId);
                    _unitOfWork.Commit();
                    return cloneResult;
                });

                return Ok(result);
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Scenario error::{e.Message}");
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
                var userInfo = _esecSecurity.GetUserInformation(Request);

                _unitOfWork.SetUser(userInfo.Name);

                var result = await Task.Factory.StartNew(() =>
                {
                    _unitOfWork.BeginTransaction();
                    _simulationUpdateMethods[userInfo.Role](dto);
                    _unitOfWork.Commit();
                    return _unitOfWork.SimulationRepo.GetSimulation(dto.Id);
                });

                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                _unitOfWork.Rollback();
                return Unauthorized();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Scenario error::{e.Message}");
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
                var userInfo = _esecSecurity.GetUserInformation(Request);

                _unitOfWork.SetUser(userInfo.Name);

                await Task.Factory.StartNew(() =>
                {
                    _unitOfWork.BeginTransaction();
                    _simulationDeleteMethods[userInfo.Role](simulationId);
                    _unitOfWork.Commit();
                });

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                _unitOfWork.Rollback();
                return Unauthorized();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Scenario error::{e.Message}");
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
                var userInfo = _esecSecurity.GetUserInformation(Request);

                _unitOfWork.SetUser(userInfo.Name);

                await Task.Factory.StartNew(() =>
                    _simulationRunMethods[userInfo.Role](networkId, simulationId));

                return Ok();
            }
            catch (Exception e)
            {
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Scenario error::{e.Message}");
                throw;
            }
        }
    }
}
