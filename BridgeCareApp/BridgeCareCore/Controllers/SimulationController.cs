using System;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Static;
using BridgeCareCore.Controllers.BaseController;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BridgeCareCore.Models;
using BridgeCareCore.Utils.Interfaces;
using Policy = BridgeCareCore.Security.SecurityConstants.Policy;using MoreLinq;
using System.Linq;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SimulationController : BridgeCareCoreBaseController
    {
        public const string SimulationError = "Scenario Error";

        private readonly ISimulationAnalysis _simulationAnalysis;
        private readonly ISimulationPagingService _simulationService;
        private readonly IWorkQueueService _simulationQueueService;
        private readonly IClaimHelper _claimHelper;
        private Guid UserId => UnitOfWork.CurrentUser?.Id ?? Guid.Empty;

        public SimulationController(ISimulationAnalysis simulationAnalysis, ISimulationPagingService simulationService, IWorkQueueService simulationQueueService, IEsecSecurity esecSecurity, IUnitOfWork unitOfWork,
            IHubService hubService, IHttpContextAccessor httpContextAccessor, IClaimHelper claimHelper) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            _simulationAnalysis = simulationAnalysis ?? throw new ArgumentNullException(nameof(simulationAnalysis));
            _simulationService = simulationService ?? throw new ArgumentNullException(nameof(simulationService));
            _simulationQueueService = simulationQueueService ?? throw new ArgumentNullException(nameof(simulationQueueService));
            _claimHelper = claimHelper ?? throw new ArgumentNullException(nameof(claimHelper));
        }

        [HttpPost]
        [Route("GetUserScenariosPage")]
        [Authorize]
        public async Task<IActionResult> GetUserScenariosPage([FromBody] PagingRequestModel<SimulationDTO> request)
        {
            try
            {
                var result = await Task.Factory.StartNew(() => _simulationService.GetUserScenarioPage(request));
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SimulationError}::GetUserScenariosPage - {e.Message}");
                throw;
            }

        }

        [HttpPost]
        [Route("GetSharedScenariosPage")]
        [Authorize]
        public async Task<IActionResult> GetSharedScenariosPage([FromBody] PagingRequestModel<SimulationDTO> request)
        {
            try
            {
                var result = await Task.Factory.StartNew(() => _simulationService.GetSharedScenarioPage(request, UserInfo.HasAdminAccess, UserInfo.HasSimulationAccess));
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SimulationError}::GetSharedScenariosPage - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("GetCurrentUserOrSharedScenario/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUserOrSharedScenario(Guid simulationId)
        {
            try
            {
                var result = await Task.Factory.StartNew(() => UnitOfWork.SimulationRepo.GetCurrentUserOrSharedScenario(simulationId, UserInfo.HasAdminAccess, UserInfo.HasSimulationAccess));
                return Ok(result);
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SimulationError}::GetCurrentUserOrSharedScenario for simulation {simulationName} - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetScenarios/")]
        [Authorize(Policy = Policy.ViewSimulation)]
        public async Task<IActionResult> GetSimulations()
        {
            try
            {
                var result = await Task.Factory.StartNew(() => {
                    var scenariosToReturn = UnitOfWork.SimulationRepo.GetSharedScenarios(UserInfo.HasAdminAccess, UserInfo.HasSimulationAccess);
                    scenariosToReturn = scenariosToReturn.Concat(UnitOfWork.SimulationRepo.GetUserScenarios()).ToList();
                    return scenariosToReturn;
                });
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SimulationError}::GetSimulations - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("GetSimulationQueuePage")]
        [Authorize]
        public async Task<IActionResult> GetSimulationQueuePage([FromBody] PagingRequestModel<QueuedWorkDTO> request)
        {
            try
            {                
                var result = await Task.Factory.StartNew(() => _simulationQueueService.GetWorkQueuePage(request));
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SimulationError}::GetSimulationQueuePage - {e.Message}");
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SimulationError}::CreateSimulation {dto.Name} - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("CloneScenario/")]
        [Authorize(Policy = Policy.CloneSimulation)]
        public async Task<IActionResult> CloneSimulation([FromBody] CloneSimulationDTO dto)
        {
            try
            {
                var result = await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    _claimHelper.CheckUserSimulationModifyAuthorization(dto.scenarioId, UserId);
                    var cloneResult = UnitOfWork.SimulationRepo.CloneSimulation(dto.scenarioId, dto.networkId, dto.scenarioName);
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SimulationError}::CloneSimulation - {e.Message}");
                throw;
            }
        }

        [HttpPut]
        [Route("UpdateScenario")]
        [Authorize(Policy = Policy.UpdateSimulation)]
        public async Task<IActionResult> UpdateSimulation([FromBody] SimulationDTO dto)
        {
            var simulationName = dto?.Name ?? "null";
            try
            {
                var result = await Task.Factory.StartNew(() =>
                {
                    _claimHelper.CheckUserSimulationModifyAuthorization(dto.Id, UserId);
                    UnitOfWork.SimulationRepo.UpdateSimulationAndPossiblyUsers(dto);
                    return UnitOfWork.SimulationRepo.GetSimulation(dto.Id);
                });

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SimulationError}::UpdateSimulation {simulationName} - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SimulationError}::UpdateSimulation {simulationName} - {e.Message}");
                throw;
            }
        }

        [HttpDelete]
        [Route("DeleteScenario/{simulationId}")]
        [Authorize(Policy = Policy.DeleteSimulation)]
        public async Task<IActionResult> DeleteSimulation(Guid simulationId)
        {
            try
            {
                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                UnitOfWork.Rollback();
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationName(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SimulationError}::DeleteSimulation {simulationName} - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationName(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SimulationError}::DeleteSimulation {simulationName} - {e.Message}");
                throw;
            }
            finally
            {
                Response.OnCompleted(async () => {
                    await DeleteSimulationOperation(simulationId);
                });
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastTaskCompleted, $"{UnitOfWork.SimulationRepo.GetSimulationName(simulationId)} deleted");
            }
        }

        public async Task<IActionResult> DeleteSimulationOperation(Guid simulationId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    _claimHelper.CheckUserSimulationModifyAuthorization(simulationId, UserId);
                    UnitOfWork.SimulationRepo.DeleteSimulation(simulationId);
                    UnitOfWork.Commit();
                });
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationName(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SimulationError}::DeleteSimulation {simulationName} - {e.Message}");
            }
            return Ok();
        }

        [HttpPost]
        [Route("RunSimulation/{networkId}/{simulationId}")]
        [Authorize(Policy = Policy.RunSimulation)]
        public async Task<IActionResult> RunSimulation(Guid networkId, Guid simulationId)
        {
            try
            {
                _claimHelper.CheckUserSimulationModifyAuthorization(simulationId, UserId);
                var scenarioName = "";
                await Task.Factory.StartNew(() =>
                {
                    scenarioName = UnitOfWork.SimulationRepo.GetSimulationName(simulationId);
                });
                var analysisHandle = _simulationAnalysis.CreateAndRun(networkId, simulationId, UserInfo, scenarioName);
                // Before sending a "queued" message that may overwrite early messages from the run,
                // allow a brief moment for an empty queue to start running the submission.
                await Task.Delay(500);
                if (!analysisHandle.WorkHasStarted)
                {
                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastSimulationAnalysisDetail, new SimulationAnalysisDetailDTO
                    {
                        SimulationId = simulationId,
                        Status = "Queued to run."
                    });
                }

                //await analysisHandle.WorkCompletion;
                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SimulationError}::RunSimulation - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationName(simulationId);
                if (e is not SimulationException)
                {
                    var logDto = SimulationLogDtos.GenericException(simulationId, e);
                    UnitOfWork.SimulationLogRepo.CreateLog(logDto);
                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SimulationError}::RunSimulation {simulationName} - {e.Message}");
                }
                else
                {
                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SimulationError}::RunSimulation {simulationName} - {e.Message}");
                }
                throw;
            }
        }

        [HttpDelete]
        [Route("CancelSimulation/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> CancelSimulation(Guid simulationId)
        {
            try
            {
                _claimHelper.CheckUserSimulationCancelAnalysisAuthorization(simulationId, UserInfo.Name, false);
                var hasBeenRemovedFromQueue = _simulationAnalysis.Cancel(simulationId);
                await Task.Delay(125);

                if (hasBeenRemovedFromQueue)
                {
                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastSimulationAnalysisDetail, new SimulationAnalysisDetailDTO
                    {
                        SimulationId = simulationId,
                        Status = "Canceled"
                    });
                }
                else
                {
                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastSimulationAnalysisDetail, new SimulationAnalysisDetailDTO
                    {
                        SimulationId = simulationId,
                        Status = "Canceling analysis..."
                    });

                }
                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Current user role does not have permission to cancel analysis for {simulationName} ::{e.Message}");
                throw;
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Error canceling simulation analysis for {simulationName}::{e.Message}");
                throw;
            }
        }
                
        [HttpPost]
        [Route("SetNoTreatmentBeforeCommitted/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> SetNoTreatmentBeforeCommitted(Guid simulationId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    UnitOfWork.SimulationRepo.SetNoTreatmentBeforeCommitted(simulationId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Error Setting NoTreatmentBeforeCommitted::{e.Message}");
                throw;
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Error Setting NoTreatmentBeforeCommitted::{e.Message}");
                throw;
            }
        }
                
        [HttpPost]
        [Route("RemoveNoTreatmentBeforeCommitted/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> RemoveNoTreatmentBeforeCommitted(Guid simulationId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    UnitOfWork.SimulationRepo.RemoveNoTreatmentBeforeCommitted(simulationId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Error Removing NoTreatmentBeforeCommitted::{e.Message}");
                throw;
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Error Removing NoTreatmentBeforeCommitted::{e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetNoTreatmentBeforeCommitted/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> GetNoTreatmentBeforeCommitted(Guid simulationId)
        {
            try
            {
                var result = await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    var noTreatmentBeforeCommitted = UnitOfWork.SimulationRepo.GetNoTreatmentBeforeCommitted(simulationId);
                    UnitOfWork.Commit();
                    return noTreatmentBeforeCommitted;
                });

                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Error Getting NoTreatmentBeforeCommitted for {simulationName} ::{e.Message}");
                throw;
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Error Getting NoTreatmentBeforeCommitted for {simulationName} ::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("ConvertSimulationOutputToRelational/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> ConvertSimulationOutputToRelational(Guid simulationId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.SimulationOutputRepo.ConvertSimulationOutpuFromJsonTorelational(simulationId);
                });

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Error Converting Simulation Output from Json to Relational::{e.Message}");
                throw;
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Error Converting Simulation Output from Json to Relationa::{e.Message}");
                throw;
            }
        }
    }
}
