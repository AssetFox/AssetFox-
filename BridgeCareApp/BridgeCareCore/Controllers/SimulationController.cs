using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Interfaces.Simulation;
using BridgeCareCore.Logging;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    using SimulationDeleteMethod = Action<UserInfoDTO, Guid>;
    using SimulationRunMethod = Action<UserInfoDTO, Guid, Guid>;
    using SimulationUpdateMethod = Action<UserInfoDTO, SimulationDTO>;

    [Route("api/[controller]")]
    [ApiController]
    public class SimulationController : Controller
    {
        private readonly ISimulationAnalysis _simulationAnalysis;
        private readonly UnitOfDataPersistenceWork _unitOfDataPersistenceWork;
        private readonly ILog _logger;
        private readonly IEsecSecurity _esecSecurity;

        private readonly IReadOnlyDictionary<string, SimulationUpdateMethod> SimulationUpdateMethods;
        private readonly IReadOnlyDictionary<string, SimulationDeleteMethod> SimulationDeleteMethods;
        private readonly IReadOnlyDictionary<string, SimulationRunMethod> SimulationRunMethods;

        public SimulationController(ISimulationAnalysis simulationAnalysis, UnitOfDataPersistenceWork unitOfDataPersistenceWork, ILog logger, IEsecSecurity esecSecurity)
        {
            _simulationAnalysis = simulationAnalysis ?? throw new ArgumentNullException(nameof(simulationAnalysis));
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
            _logger = logger;
            _esecSecurity = esecSecurity ?? throw new ArgumentNullException(nameof(esecSecurity));
            SimulationUpdateMethods = CreateUpdateMethods();
            SimulationDeleteMethods = CreateDeleteMethods();
            SimulationRunMethods = CreateRunMethods();
        }

        private Dictionary<string, SimulationUpdateMethod> CreateUpdateMethods()
        {
            void UpdateAnySimulation(UserInfoDTO userInfo, SimulationDTO dto) =>
                _unitOfDataPersistenceWork.SimulationRepo.UpdateSimulation(dto, userInfo);

            void UpdatePermittedSimulation(UserInfoDTO userInfo, SimulationDTO dto) =>
                _unitOfDataPersistenceWork.SimulationRepo.UpdatePermittedSimulation(userInfo, dto);

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
            void DeleteAnySimulation(UserInfoDTO userInfo, Guid simulationId) =>
                _unitOfDataPersistenceWork.SimulationRepo.DeleteSimulation(simulationId);

            void DeletePermittedSimulation(UserInfoDTO userInfo, Guid simulationId) =>
                _unitOfDataPersistenceWork.SimulationRepo.DeletePermittedSimulation(userInfo, simulationId);

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
            void RunAnySimulation(UserInfoDTO userInfo, Guid networkId, Guid simulationId) =>
                _simulationAnalysis.CreateAndRun(networkId, simulationId);

            void RunPermittedSimulation(UserInfoDTO userInfo, Guid networkId, Guid simulationId) =>
                _simulationAnalysis.CreateAndRunPermitted(userInfo, networkId, simulationId);

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
                var result = await _unitOfDataPersistenceWork.SimulationRepo.GetAllInNetwork(networkId);
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.Error($"{e.Message}::{e.StackTrace}");
                return StatusCode(500, e);
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
                _unitOfDataPersistenceWork.BeginTransaction();
                var result = await Task.Factory.StartNew(() =>
                {
                    _unitOfDataPersistenceWork.SimulationRepo.CreateSimulation(networkId, dto, userInfo.ToDto());
                    _unitOfDataPersistenceWork.Commit();
                    return _unitOfDataPersistenceWork.SimulationRepo.GetSimulation(dto.Id);
                });

                return Ok(result.Result);
            }
            catch (Exception e)
            {
                _unitOfDataPersistenceWork.Rollback();
                Console.WriteLine(e);
                return BadRequest(e);
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
                _unitOfDataPersistenceWork.BeginTransaction();
                var result = await Task.Factory.StartNew(() =>
                {
                    var cloneResult = _unitOfDataPersistenceWork.SimulationRepo.CloneSimulation(simulationId, userInfo.ToDto());
                    _unitOfDataPersistenceWork.Commit();
                    return cloneResult;
                });
                return Ok(result.Result);
            }
            catch (Exception e)
            {
                _unitOfDataPersistenceWork.Rollback();
                Console.WriteLine(e);
                return BadRequest(e);
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
                _unitOfDataPersistenceWork.BeginTransaction();
                var result = await Task.Factory.StartNew(() =>
                {
                    SimulationUpdateMethods[userInfo.Role](userInfo.ToDto(), dto);
                    _unitOfDataPersistenceWork.Commit();
                    return _unitOfDataPersistenceWork.SimulationRepo.GetSimulation(dto.Id);
                });
                return Ok(result.Result);
            }
            catch (UnauthorizedAccessException e)
            {
                _unitOfDataPersistenceWork.Rollback();
                Console.WriteLine(e);
                return Unauthorized(e);
            }
            catch (Exception e)
            {
                _unitOfDataPersistenceWork.Rollback();
                Console.WriteLine(e);
                return BadRequest(e);
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
                _unitOfDataPersistenceWork.BeginTransaction();
                await Task.Factory.StartNew(() => SimulationDeleteMethods[userInfo.Role](userInfo.ToDto(), simulationId));
                _unitOfDataPersistenceWork.Commit();
                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                _unitOfDataPersistenceWork.Rollback();
                Console.WriteLine(e);
                return Unauthorized(e);
            }
            catch (Exception e)
            {
                _unitOfDataPersistenceWork.Rollback();
                Console.WriteLine(e);
                return BadRequest(e);
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
                await Task.Factory.StartNew(() =>
                    SimulationRunMethods[userInfo.Role](userInfo.ToDto(), networkId, simulationId));
                return Ok();
            }
            catch (Exception e)
            {
                _logger.Error($"{e.Message}::{e.StackTrace}");
                return StatusCode(500, e);
            }
        }
    }
}
