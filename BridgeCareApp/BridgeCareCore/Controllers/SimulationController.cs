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
using Policy = BridgeCareCore.Security.SecurityConstants.Policy;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SimulationController : BridgeCareCoreBaseController
    {
        public const string SimulationError = "Scenario Error";

        private readonly ISimulationAnalysis _simulationAnalysis;
        private readonly ISimulationService _simulationService;
        private readonly ISimulationQueueService _simulationQueueService;
        private readonly IClaimHelper _claimHelper;
        private Guid UserId => UnitOfWork.CurrentUser?.Id ?? Guid.Empty;

        public SimulationController(ISimulationAnalysis simulationAnalysis, ISimulationService simulationService, ISimulationQueueService simulationQueueService, IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork,
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
                // copied comment for // TODO:  Replace with query to find all shared simulations
                var result = await Task.Factory.StartNew(() => UnitOfWork.SimulationRepo.GetAllScenario());
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
        public async Task<IActionResult> GetSimulationQueuePage([FromBody] PagingRequestModel<QueuedSimulationDTO> request)
        {
            try
            {                
                var result = await Task.Factory.StartNew(() => _simulationQueueService.GetSimulationQueuePage(request));
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
        [Authorize]
        public async Task<IActionResult> CloneSimulation([FromBody] CloneSimulationDTO dto)
        {
            try
            {
                var result = await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    // Copied comment TODO (the user should only be able to clone scenarios that they have access to)
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
            try
            {
                var result = await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    _claimHelper.CheckUserSimulationModifyAuthorization(dto.Id, UserId);
                    UnitOfWork.SimulationRepo.UpdateSimulation(dto);
                    UnitOfWork.Commit();
                    return UnitOfWork.SimulationRepo.GetSimulation(dto.Id);
                });

                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SimulationError}::UpdateSimulation - {HubService.errorList["Unauthorized"]}");
                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SimulationError}::UpdateSimulation - {e.Message}");
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
            catch (UnauthorizedAccessException e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SimulationError}::DeleteSimulation - {HubService.errorList["Unauthorized"]}");
                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SimulationError}::DeleteSimulation - {e.Message}");
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SimulationError}::DeleteSimulation - {e.Message}");
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
                var analysisHandle = _simulationAnalysis.CreateAndRun(networkId, simulationId, UserInfo);
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
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SimulationError}::RunSimulation - {HubService.errorList["Unauthorized"]}");
                return Ok();
            }
            catch (Exception e)
            {
                if (e is not SimulationException)
                {
                    var logDto = SimulationLogDtos.GenericException(simulationId, e);
                    UnitOfWork.SimulationLogRepo.CreateLog(logDto);
                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SimulationError}::RunSimulation - {e.Message}");
                }
                else
                {
                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SimulationError}::RunSimulation - {e.Message}");
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Current user role does not have permission to cancel analysis::{e.Message}");
                // Note: Return Ok result is required(we might investigate more on this later), sending "UnauthorizedAccess" here to handle this case correctly in front end.
                return Ok("UnauthorizedAccess");
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Error canceling simulation analysis::{e.Message}");
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
                return Ok();
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
                return Ok();
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Error Getting NoTreatmentBeforeCommitted::{e.Message}");
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Error Getting NoTreatmentBeforeCommitted::{e.Message}");
                throw;
            }
        }
    }
}
