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
using BridgeCareCore.Security;
using BridgeCareCore.Services;
using BridgeCareCore.Services.General_Work_Queue.WorkItems;
using AppliedResearchAssociates.iAM.Hubs.Services;
using Microsoft.CodeAnalysis;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SimulationController : BridgeCareCoreBaseController
    {
        public const string SimulationError = "Scenario Error";

        private readonly ISimulationPagingService _simulationService;
        private readonly IWorkQueueService _workQueueService;
        private readonly IGeneralWorkQueueService _generalWorkQueueService;
        private readonly IClaimHelper _claimHelper;
        private Guid UserId => UnitOfWork.CurrentUser?.Id ?? Guid.Empty;

        public SimulationController(ISimulationPagingService simulationService, IWorkQueueService workQueueService, IEsecSecurity esecSecurity, IUnitOfWork unitOfWork,
            IHubService hubService, IHttpContextAccessor httpContextAccessor, IClaimHelper claimHelper, IGeneralWorkQueueService generalWorkQueueService) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            _simulationService = simulationService ?? throw new ArgumentNullException(nameof(simulationService));
            _workQueueService = workQueueService ?? throw new ArgumentNullException(nameof(workQueueService));
            _claimHelper = claimHelper ?? throw new ArgumentNullException(nameof(claimHelper));
            this._generalWorkQueueService = generalWorkQueueService ?? throw new ArgumentNullException(nameof(generalWorkQueueService));
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
        [Route("GetWorkQueuePage")]
        [Authorize]
        public async Task<IActionResult> GetWorkQueuePage([FromBody] PagingRequestModel<QueuedWorkDTO> request)
        {
            try
            {                
                var result = await Task.Factory.StartNew(() => _workQueueService.GetWorkQueuePage(request));
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SimulationError}::GetWorkQueuePage - {e.Message}");
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
                    UnitOfWork.SimulationRepo.CreateSimulation(networkId, dto);
                    return UnitOfWork.SimulationRepo.GetSimulation(dto.Id);
                });
                
                return Ok(result);
            }
            catch (Exception e)
            {
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

                    _claimHelper.CheckUserSimulationModifyAuthorization(dto.scenarioId, UserId);
                    var cloneResult = UnitOfWork.SimulationRepo.CloneSimulation(dto.scenarioId, dto.networkId, dto.scenarioName);
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
        public async Task<IActionResult> DeleteScenario(Guid simulationId)
        {
            var simulationName = "";
            try
            {
                
                await Task.Factory.StartNew(() =>
                {
                    _claimHelper.CheckUserSimulationModifyAuthorization(simulationId, UserId);
                    simulationName = UnitOfWork.SimulationRepo.GetSimulationName(simulationId);
                });
                DeleteSimulationWorkitem workItem = new DeleteSimulationWorkitem(simulationId, UserInfo.Name, simulationName);
                var analysisHandle = _generalWorkQueueService.CreateAndRun(workItem);

                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastWorkQueueUpdate, simulationId.ToString());

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SimulationError}::DeleteSimulation {simulationName} - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SimulationError}::DeleteSimulation {simulationName} - {e.Message}");
                throw;
            }
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
                AnalysisWorkItem workItem = new(networkId, simulationId, UserInfo, scenarioName);
                var analysisHandle = _generalWorkQueueService.CreateAndRun(workItem);
                // Before sending a "queued" message that may overwrite early messages from the run,
                // allow a brief moment for an empty queue to start running the submission.
                await Task.Delay(500);
                if (!analysisHandle.WorkHasStarted)
                {
                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastSimulationAnalysisDetail, "Queued to run.");
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
        [Route("CancelSimulation/{workId}")]
        [Authorize]
        public async Task<IActionResult> CancelSimulation(Guid workId)
        {
            try
            {
                var work = _workQueueService.GetQueuedWorkByWorkId(workId);
                if(work == null)
                    return Ok();
                if(work.WorkType == WorkType.SimulationAnalysis)
                {
                    _claimHelper.CheckUserSimulationCancelAnalysisAuthorization(workId, UserInfo.Name, false);
                    var hasBeenRemovedFromQueue = _generalWorkQueueService.Cancel(workId);
                    await Task.Delay(125);

                    if (hasBeenRemovedFromQueue)
                    {
                        HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastSimulationAnalysisDetail, new SimulationAnalysisDetailDTO
                        {
                            SimulationId = workId,
                            Status = "Canceled"
                        });
                    }
                    else
                    {
                        HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastSimulationAnalysisDetail, new SimulationAnalysisDetailDTO
                        {
                            SimulationId = workId,
                            Status = "Canceling analysis..."
                        });
                    }
                }
                else
                {
                    var hasBeenRemovedFromQueue = _generalWorkQueueService.Cancel(workId);
                    if(hasBeenRemovedFromQueue)
                        HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastWorkQueueUpdate, workId);
                    else
                        HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastWorkQueueStatusUpdate, new QueuedWorkStatusUpdateModel() { Id = workId, Status = "Canceling" });
                }                           

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(workId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Current user role does not have permission to cancel analysis for {simulationName} ::{e.Message}");
                throw;
            }
            catch (Exception e)
            {
                
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(workId);
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
                    UnitOfWork.SimulationRepo.SetNoTreatmentBeforeCommitted(simulationId);
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
                    UnitOfWork.SimulationRepo.RemoveNoTreatmentBeforeCommitted(simulationId);
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
                    var noTreatmentBeforeCommitted = UnitOfWork.SimulationRepo.GetNoTreatmentBeforeCommitted(simulationId);
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
                _claimHelper.CheckUserSimulationModifyAuthorization(simulationId, UserId);
                var scenarioName = "";
                await Task.Factory.StartNew(() =>
                {
                    scenarioName = UnitOfWork.SimulationRepo.GetSimulationName(simulationId);
                });
                SimulationOutputConversionWorkitem workItem = new SimulationOutputConversionWorkitem(simulationId, UserInfo.Name, scenarioName);
                var analysisHandle = _generalWorkQueueService.CreateAndRun(workItem);

                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastWorkQueueUpdate, simulationId.ToString());

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
