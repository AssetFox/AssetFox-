using System;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.DTOs.Static;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Controllers.BaseController;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;
using BridgeCareCore.Models.General_Work_Queue;
using BridgeCareCore.Security.Interfaces;
using BridgeCareCore.Services;
using BridgeCareCore.Services.General_Work_Queue.WorkItems;
using BridgeCareCore.Utils.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Dac.Model;
using MoreLinq;
using Policy = BridgeCareCore.Security.SecurityConstants.Policy;
using AppliedResearchAssociates.Validation;
using System.Collections.Generic;
using BridgeCareCore.Models.Validation;
using ValidationResult = AppliedResearchAssociates.Validation.ValidationResult;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SimulationController : BridgeCareCoreBaseController
    {
        public const string SimulationError = "Scenario Error";
        public const string workQueueError = "Work Queue Error";

        private readonly ISimulationPagingService _simulationService;
        private readonly IWorkQueueService _workQueueService;
        private readonly IGeneralWorkQueueService _generalWorkQueueService;
        private readonly IClaimHelper _claimHelper;
        private readonly ICompleteSimulationCloningService _completeSimulationCloningService;

        private Guid UserId => UnitOfWork.CurrentUser?.Id ?? Guid.Empty;

        public SimulationController(
            ISimulationPagingService simulationService,
            IWorkQueueService workQueueService,
            IEsecSecurity esecSecurity,
            IUnitOfWork unitOfWork,
            IHubService hubService,
            IHttpContextAccessor httpContextAccessor,
            IClaimHelper claimHelper,
            ICompleteSimulationCloningService completeSimulationCloningService,
            IGeneralWorkQueueService generalWorkQueueService) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            _simulationService = simulationService ?? throw new ArgumentNullException(nameof(simulationService));
            _workQueueService = workQueueService ?? throw new ArgumentNullException(nameof(workQueueService));
            _claimHelper = claimHelper ?? throw new ArgumentNullException(nameof(claimHelper));
            _generalWorkQueueService = generalWorkQueueService ?? throw new ArgumentNullException(nameof(generalWorkQueueService));
            _completeSimulationCloningService = completeSimulationCloningService ?? throw new ArgumentNullException(nameof(completeSimulationCloningService));
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
                var result = await Task.Factory.StartNew(() =>
                {
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
        [Route("GetFastWorkQueuePage")]
        [Authorize]
        public async Task<IActionResult> GetFastWorkQueuePage([FromBody] PagingRequestModel<QueuedWorkDTO> request)
        {
            try
            {
                var result = await Task.Factory.StartNew(() => _workQueueService.GetFastWorkQueuePage(request));
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SimulationError}::GetFastWorkQueuePage - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("GetQueuedWorkByDomainIdAndWorkType")]
        [Authorize]
        public async Task<IActionResult> GetQueuedWorkByDomainIdAndWorkType([FromBody] WorkQueueRequestModel request)
        {
            try
            {
                var result = await Task.Factory.StartNew(() => _workQueueService.GetFastQueuedWorkByDomainIdAndWorkType(request.DomainId, request.WorkType));
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SimulationError}::GetQueuedWorkByDomainIdAndWorkType - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("GetFastQueuedWorkByDomainIdAndWorkType")]
        [Authorize]
        public async Task<IActionResult> GetFastQueuedWorkByDomainIdAndWorkType([FromBody] WorkQueueRequestModel request)
        {
            try
            {
                var result = await Task.Factory.StartNew(() => _workQueueService.GetFastQueuedWorkByDomainIdAndWorkType(request.DomainId, request.WorkType));
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SimulationError}::GetFastQueuedWorkByDomainIdAndWorkType - {e.Message}");
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
                    _claimHelper.CheckUserSimulationModifyAuthorization(dto.ScenarioId, UserId);
                    var isCompatible = _completeSimulationCloningService.CheckCompatibleNetworkAttributes(dto);
                    if (!isCompatible)
                    {
                        //Provide error message when networks are not compatible
                        HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SimulationError}::CloneSimulation - destination network is not compatible.");
                        return null;
                    }
                    var cloneResult = _completeSimulationCloningService.Clone(dto);
                    return cloneResult;
                });

                if (result == null)
                {
                    return Ok();
                }
                else
                {
                    return Ok(result.Simulation);
                }


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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastWorkQueueUpdate, simulationId);
                await Task.Delay(500);
                if (!analysisHandle.WorkHasStarted)
                {
                    var message = new SimulationAnalysisDetailDTO()
                    {
                        SimulationId = simulationId,
                        Status = $"Queued to run.",
                    };
                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastSimulationAnalysisDetail, message);
                }

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
        [Route("CancelWorkQueueItem/{workId}")]
        [Authorize]
        public async Task<IActionResult> CancelInWorkQueue(string workId)
        {
            try
            {
                var work = _workQueueService.GetQueuedWorkByWorkId(workId);
                if (work == null)
                    return Ok();
                if (work.WorkType == WorkType.SimulationAnalysis)
                {
                    _claimHelper.CheckUserSimulationCancelAnalysisAuthorization(work.DomainId, UserInfo.Name, false);
                    var hasBeenRemovedFromQueue = _generalWorkQueueService.Cancel(workId);
                    await Task.Delay(125);

                    if (hasBeenRemovedFromQueue)
                    {
                        HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastSimulationAnalysisDetail, new SimulationAnalysisDetailDTO
                        {
                            SimulationId = work.DomainId,
                            Status = "Canceled"
                        });
                        HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastWorkQueueUpdate, workId);
                    }
                    else
                    {
                        HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastSimulationAnalysisDetail, new SimulationAnalysisDetailDTO
                        {
                            SimulationId = work.DomainId,
                            Status = "Canceling analysis..."
                        });
                    }
                }
                else
                {
                    var hasBeenRemovedFromQueue = _generalWorkQueueService.Cancel(workId);
                    if (hasBeenRemovedFromQueue)
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

        [HttpDelete]
        [Route("CancelFastQueueItem/{workId}")]
        [Authorize]
        public async Task<IActionResult> CancelFastQueueItem(string workId)
        {
            try
            {
                var work = _workQueueService.GetFastQueuedWorkByWorkId(workId);
                if (work == null)
                    return Ok();


                var hasBeenRemovedFromQueue = _generalWorkQueueService.CancelInFastQueue(workId);
                if (hasBeenRemovedFromQueue)
                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastFastWorkQueueUpdate, workId);
                else
                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastFastWorkQueueStatusUpdate, new QueuedWorkStatusUpdateModel() { Id = workId, Status = "Canceling" });


                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(workId);

                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Current user role does not have permission to cancel work queue processes ::{e.Message}");
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

        [HttpPost]
        [Route("ValidateSimulation/{networkId}/{simulationId}")]
        [Authorize(Policy = Policy.ValidateSimulation)]
        public async Task<IActionResult> ValidateSimulation(Guid networkId, Guid simulationId)
        {
            try
            {
                _claimHelper.CheckUserSimulationModifyAuthorization(simulationId, UserId);
                var validationResultBag = new ValidationResultBag();
                var validationResultList = new List<ValidationResult>();
                var preChecksValidationResults = new List<PreChecksValidationResult>();

                await Task.Factory.StartNew(() =>
                {
                    var simulation = AnalysisInputLoading.GetSimulationWithoutAssets(UnitOfWork, networkId, simulationId, validationResultBag);
                    if (validationResultBag.Count > 0)
                    {
                        validationResultList = validationResultBag.AsEnumerable().ToList();
                        GetPreChecksValidationResults(preChecksValidationResults, validationResultList);
                    }
                    else
                    {
                        validationResultBag = simulation.GetAllValidationResults(Enumerable.Empty<string>());
                        validationResultList.AddRange(validationResultBag.AsEnumerable().ToList());
                        GetPreChecksValidationResults(preChecksValidationResults, validationResultList);
                    }
                });
                
                return Ok(preChecksValidationResults);
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SimulationError}::ValidateSimulation - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationName(simulationId);
                if (e is not SimulationException)
                {
                    var logDto = SimulationLogDtos.GenericException(simulationId, e);
                    UnitOfWork.SimulationLogRepo.CreateLog(logDto);
                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SimulationError}::ValidateSimulation {simulationName} - {e.Message}");
                }
                else
                {
                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SimulationError}::ValidateSimulation {simulationName} - {e.Message}");
                }
                throw;
            }
        }

        private static void GetPreChecksValidationResults(List<PreChecksValidationResult> validationResults, List<ValidationResult> validationResultList) =>
            validationResults.AddRange(from validationResult in validationResultList
                                       let toAdd = new PreChecksValidationResult(validationResult.Status, validationResult.Message)
                                       select toAdd);
    }
}
