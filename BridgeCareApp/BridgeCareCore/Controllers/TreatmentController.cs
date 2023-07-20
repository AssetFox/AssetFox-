using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Controllers.BaseController;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using BridgeCareCore.Utils.Interfaces;
using Policy = BridgeCareCore.Security.SecurityConstants.Policy;
using BridgeCareCore.Models;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.Generics;
using BridgeCareCore.Services.General_Work_Queue.WorkItems;
using AppliedResearchAssociates.iAM.Analysis;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TreatmentController : BridgeCareCoreBaseController
    {
        public const string TreatmentError = "Treatment Error";

        private readonly ITreatmentService _treatmentService;
        private readonly ITreatmentPagingService _treatmentPagingService;
        private readonly IClaimHelper _claimHelper;
        private readonly IGeneralWorkQueueService _generalWorkQueueService;

        private Guid UserId => UnitOfWork.CurrentUser?.Id ?? Guid.Empty;
        public TreatmentController(ITreatmentService treatmentService, ITreatmentPagingService treatmentPagingService,
            IEsecSecurity esecSecurity,
            IUnitOfWork unitOfWork,
            IHubService hubService,
            IHttpContextAccessor httpContextAccessor,
            IClaimHelper claimHelper, IGeneralWorkQueueService generalWorkQueueService) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            _treatmentService = treatmentService ?? throw new ArgumentNullException(nameof(treatmentService));
            _treatmentPagingService = treatmentPagingService ?? throw new ArgumentNullException(nameof(treatmentPagingService));
            _claimHelper = claimHelper ?? throw new ArgumentNullException(nameof(claimHelper));
            _generalWorkQueueService = generalWorkQueueService ?? throw new ArgumentNullException(nameof(generalWorkQueueService));
        }

        [HttpGet]
        [Route("GetTreatmentLibraries")]
        [Authorize(Policy = Policy.ViewTreatmentFromLibrary)]
        public async Task<IActionResult> GetTreatmentLibraries()
        {
            try
            {
                var result = new List<TreatmentLibraryDTO>();
                await Task.Factory.StartNew(() =>
                {
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        result = UnitOfWork.SelectableTreatmentRepo.GetTreatmentLibrariesNoChildrenAccessibleToUser(UserId);
                    }
                    else
                    {
                        result = UnitOfWork.SelectableTreatmentRepo.GetAllTreatmentLibrariesNoChildren();
                    }
                });

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TreatmentError}::GetTreatmentLibraries - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetSimpleTreatmentsByLibraryId/{libraryId}")]
        [Authorize(Policy = Policy.ViewTreatmentFromLibrary)]
        public async Task<IActionResult> GetSimpleTreatmentsByLibraryId(Guid libraryId)
        {
            try
            {
                var result = new List<SimpleTreatmentDTO>();
                await Task.Factory.StartNew(() =>
                {
                    var library = UnitOfWork.SelectableTreatmentRepo.GetSingleTreatmentLibaryNoChildren(libraryId);
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        result = UnitOfWork.SelectableTreatmentRepo.GetSimpleTreatmentsByLibraryId(libraryId);
                    }
                    else
                        result = UnitOfWork.SelectableTreatmentRepo.GetSimpleTreatmentsByLibraryId(libraryId);
                });

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TreatmentError}::GetSimpleTreatmentsByLibraryId - {e.Message}");
                throw;
            }
        }
        [HttpGet]
        [Route("GetTreatmentLibraryFromSimulationId/{simulationId}")]
        [Authorize(Policy = Policy.ViewTreatmentFromScenario)]
        public async Task<IActionResult> GetTreatmentLibraryFromSimulationId(Guid simulationId)
        {
            try
            {
                var result = new TreatmentLibraryDTO();
                await Task.Factory.StartNew(() =>
                {
                    var selectableTreatment = UnitOfWork.SelectableTreatmentRepo.GetDefaultTreatment(simulationId);
                    if (selectableTreatment == null)
                    {
                        result = null;                     
                    }
                    else
                    {
                        var library = UnitOfWork.SelectableTreatmentRepo.GetAllTreatmentLibraries().FirstOrDefault(_ => _.Id == selectableTreatment.LibraryId);
                        if (library != null) library.IsModified = selectableTreatment.IsModified;
                        result = library;
                    }                                         
                });
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TreatmentError}::GetSelectedTreatmentById - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TreatmentError} ::GetSelectedTreatmentById - {e.Message}");
                throw;
            }
        }
        [HttpGet]
        [Route("GetSelectedTreatmentById/{id}")]
        [Authorize(Policy = Policy.ViewTreatmentFromScenario)]
        public async Task<IActionResult> GetSelectedTreatmentById(Guid id)
        {
            try
            {
                var result = new TreatmentDTO();
                await Task.Factory.StartNew(() =>
                {
                    var library = UnitOfWork.SelectableTreatmentRepo.GetTreatmentLibraryWithSingleTreatmentByTreatmentId(id);
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        var accessModel = UnitOfWork.TreatmentLibraryUserRepo.GetLibraryAccess(library.Id, UserId);
                        _claimHelper.CheckGetLibraryUsersValidity(accessModel, UserId);
                        result = library.Treatments[0];
                    }
                    else
                        result = library.Treatments[0];
                });

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TreatmentError}::GetSelectedTreatmentById - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TreatmentError} ::GetSelectedTreatmentById - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetScenarioSelectedTreatments/{simulationId}")]
        [Authorize(Policy = Policy.ViewTreatmentFromScenario)]
        public async Task<IActionResult> GetScenarioSelectedTreatments(Guid simulationId)
        {
            try
            {
                var result = new List<TreatmentDTO>();
                await Task.Factory.StartNew(() =>
                {
                    _claimHelper.CheckUserSimulationReadAuthorization(simulationId, UserId);
                    result = UnitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatments(simulationId);
                });

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TreatmentError}::GetScenarioSelectedTreatments for {simulationName} - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TreatmentError}::GetScenarioSelectedTreatments for {simulationName} - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetScenarioSelectedTreatmentById/{id}")]
        [Authorize(Policy = Policy.ViewTreatmentFromScenario)]
        public async Task<IActionResult> GetScenarioSelectedTreatmentById(Guid id)
        {
            try
            {
                var result = new TreatmentDTO();
                await Task.Factory.StartNew(() =>
                {
                    var scenarioTreatment = UnitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatmentById(id);
                    _claimHelper.CheckUserSimulationReadAuthorization(scenarioTreatment.SimulationId, UserId);
                    result = scenarioTreatment.Treatment;
                });

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TreatmentError} ::GetScenarioSelectedTreatmentById - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TreatmentError} ::GetScenarioSelectedTreatmentById - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetSimpleTreatmentsByScenarioId/{simulationId}")]
        [Authorize(Policy = Policy.ViewTreatmentFromScenario)]
        public async Task<IActionResult> GetSimpleTreatmentsByScenarioId(Guid simulationId)
        {
            try
            {
                var result = new List<SimpleTreatmentDTO>();
                await Task.Factory.StartNew(() =>
                {
                    _claimHelper.CheckUserSimulationReadAuthorization(simulationId, UserId);
                    result = UnitOfWork.SelectableTreatmentRepo.GetSimpleTreatmentsBySimulationId(simulationId);
                });

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TreatmentError} ::GetSimpleTreatmentsByScenarioId for {simulationName} - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TreatmentError} ::GetSimpleTreatmentsByScenarioId for {simulationName} - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetTreatmentLibraryUsers/{libraryId}")]
        [Authorize(Policy = Policy.ViewTreatmentFromLibrary)]
        public async Task<IActionResult> GetTreatmentLibraryUsers(Guid libraryId)
        {
            try
            {
                List<LibraryUserDTO> users = new List<LibraryUserDTO>();
                await Task.Factory.StartNew(() =>
                {
                    var accessModel = UnitOfWork.TreatmentLibraryUserRepo.GetLibraryAccess(libraryId, UserId);
                    _claimHelper.CheckGetLibraryUsersValidity(accessModel, UserId);
                    users = UnitOfWork.TreatmentLibraryUserRepo.GetLibraryUsers(libraryId);
                });
                return Ok(users);
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TreatmentError}::GetTreatmentLibraryUsers - {HubService.errorList["Unauthorized"]}");
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertOrDeleteTreatmentLibraryUsers/{libraryId}")]
        [Authorize(Policy = Policy.ModifyInvestmentFromLibrary)]
        public async Task<IActionResult> UpsertOrDeleteTreatmentLibraryUsers(Guid libraryId, List<LibraryUserDTO> proposedUsers)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    var libraryUsers = UnitOfWork.TreatmentLibraryUserRepo.GetLibraryUsers(libraryId);
                    _claimHelper.CheckAccessModifyValidity(libraryUsers, proposedUsers, UserId);
                    UnitOfWork.TreatmentLibraryUserRepo.UpsertOrDeleteUsers(libraryId, proposedUsers);
                });
                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
                return Ok();
            }
            catch (InvalidOperationException e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
                return BadRequest();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertTreatmentLibrary")]
        [Authorize(Policy = Policy.ModifyTreatmentFromLibrary)]
        public async Task<IActionResult> UpsertTreatmentLibrary(LibraryUpsertPagingRequestModel<TreatmentLibraryDTO, TreatmentDTO> upsertRequest)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    var treatments = _treatmentPagingService.GetSyncedLibraryDataset(upsertRequest);
                    var dto = upsertRequest.Library;
                    dto.Treatments = treatments;
                    if (dto != null)
                    {
                        var accessModel = UnitOfWork.TreatmentLibraryUserRepo.GetLibraryAccess(dto.Id, UserId);
                        _claimHelper.CheckGetLibraryUsersValidity(accessModel, UserId);
                    }
                    UnitOfWork.SelectableTreatmentRepo.UpsertOrDeleteTreatmentLibraryTreatmentsAndPossiblyUsers(dto, upsertRequest.IsNewLibrary, UserId);
                });

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TreatmentError}::UpsertTreatmentLibrary - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TreatmentError}::UpsertTreatmentLibrary - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("ExportLibraryTreatmentsExcelFile/{libraryId}")]
        public async Task<IActionResult> ExportLibraryTreatmentsExcelFile(Guid libraryId)
        {
            try
            {
                var result =
                    await Task.Factory.StartNew(() => _treatmentService.ExportLibraryTreatmentsExcelFile(libraryId));

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TreatmentError}::ExportLibraryTreatmentsExcelFile - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TreatmentError}::ExportLibraryTreatmentsExcelFile - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertScenarioSelectedTreatments/{simulationId}")]
        [Authorize(Policy = Policy.ModifyTreatmentFromScenario)]
        public async Task<IActionResult> UpsertScenarioSelectedTreatments(Guid simulationId, PagingSyncModel<TreatmentDTO> pagingSync)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    _claimHelper.CheckUserSimulationModifyAuthorization(simulationId, UserId);
                    var dtos = _treatmentPagingService.GetSyncedScenarioDataSet(simulationId, pagingSync);
                    UnitOfWork.SelectableTreatmentRepo.AddLibraryIdToScenarioSelectableTreatments(dtos, pagingSync.LibraryId);
                    UnitOfWork.SelectableTreatmentRepo.AddModifiedToScenarioSelectableTreatments(dtos, pagingSync.IsModified);
                    UnitOfWork.SelectableTreatmentRepo.UpsertOrDeleteScenarioSelectableTreatment(dtos, simulationId);
                });

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TreatmentError}::UpsertScenarioSelectedTreatments for {simulationName} - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TreatmentError}::UpsertScenarioSelectedTreatments for {simulationName} - {e.Message}");
                throw;
            }
        }

        [HttpDelete]
        [Route("DeleteTreatmentLibrary/{libraryId}")]
        [Authorize(Policy = Policy.DeleteTreatmentFromLibrary)]
        public async Task<IActionResult> DeleteTreatmentLibrary(Guid libraryId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        var dto = GetAllTreatmentLibraries().FirstOrDefault(_ => _.Id == libraryId);
                        if (dto == null) return;
                        var access = UnitOfWork.TreatmentLibraryUserRepo.GetLibraryAccess(libraryId, UserId);
                        _claimHelper.CheckUserLibraryDeleteAuthorization(access, UserId);
                    }
                    UnitOfWork.SelectableTreatmentRepo.DeleteTreatmentLibrary(libraryId);
                });

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TreatmentError}::DeleteTreatmentLibrary - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TreatmentError}::DeleteTreatmentLibrary - {e.Message}");
                throw;
            }
        }


        [HttpPost]
        [Route("ImportLibraryTreatmentsFile")]
        [Authorize(Policy = Policy.ImportTreatmentFromLibrary)]
        public async Task<IActionResult> ImportLibraryTreatmentsFile()
        {
            try
            {
                if (!ContextAccessor.HttpContext.Request.HasFormContentType)
                {
                    throw new ConstraintException("Request MIME type is invalid.");
                }

                if (ContextAccessor.HttpContext.Request.Form.Files.Count < 1)
                {
                    throw new ConstraintException("Treatments file not found.");
                }

                if (!ContextAccessor.HttpContext.Request.Form.TryGetValue("libraryId", out var libraryId))
                {
                    throw new ConstraintException("Request contained no treatment library id.");
                }

                var treatmentLibraryId = Guid.Parse(libraryId.ToString());
                var excelPackage = new ExcelPackage(ContextAccessor.HttpContext.Request.Form.Files[0].OpenReadStream());
  
                var libraryName = "";
                await Task.Factory.StartNew(() =>
                {
                    var existingTreatmentLibrary = UnitOfWork.SelectableTreatmentRepo.GetSingleTreatmentLibary(treatmentLibraryId);
                    if (existingTreatmentLibrary != null)
                        libraryName = existingTreatmentLibrary.Name;
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        
                        if (existingTreatmentLibrary != null)
                        {
                            var accessModel = UnitOfWork.TreatmentLibraryUserRepo.GetLibraryAccess(treatmentLibraryId, UserId);
                            _claimHelper.CheckUserLibraryRecreateAuthorization(accessModel, UserId);
                        }
                    }
                });

                ImportLibraryTreatmentWorkitem workItem = new ImportLibraryTreatmentWorkitem(treatmentLibraryId, excelPackage, UserInfo.Name, libraryName);
                var analysisHandle = _generalWorkQueueService.CreateAndRun(workItem);

                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastWorkQueueUpdate, libraryId.ToString());

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TreatmentError}::ImportLibraryTreatmentsFile - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TreatmentError}::ImportLibraryTreatmentsFile - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("DeleteTreatment/{libraryId}")]
        [Authorize(Policy = Policy.DeleteTreatmentFromLibrary)]
        public async Task<IActionResult> DeleteTreatment(TreatmentDTO treatment, Guid libraryId)
        {
            var treatmentName = treatment?.Name ?? "null";
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        var dto = GetAllTreatmentLibraries().FirstOrDefault(_ => _.Id == libraryId);
                        if (dto == null || treatment == null) return;

                        var access = UnitOfWork.TreatmentLibraryUserRepo.GetLibraryAccess(libraryId, UserId);
                        _claimHelper.CheckUserLibraryDeleteAuthorization(access, UserId);
                    }
                    UnitOfWork.SelectableTreatmentRepo.DeleteTreatment(treatment, libraryId);
                });

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TreatmentError}::DeleteTreatment {treatmentName} - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TreatmentError}::DeleteTreatment {treatmentName}- {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("DeleteScenarioSelectableTreatment/{simulationId}")]
        [Authorize(Policy = Policy.ModifyTreatmentFromScenario)]
        public async Task<IActionResult> DeleteScenarioSelectableTreatment(TreatmentDTO scenarioSelectableTreatment, Guid simulationId)
        {
            var treatmentName = scenarioSelectableTreatment?.Name ?? "null";
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    _claimHelper.CheckUserSimulationModifyAuthorization(simulationId, UserId);
                    UnitOfWork.SelectableTreatmentRepo.DeleteScenarioSelectableTreatment(scenarioSelectableTreatment, simulationId);
                });

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TreatmentError}::DeleteScenarioSelectableTreatment {treatmentName} - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TreatmentError}::DeleteScenarioSelectableTreatment {treatmentName} - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("ImportScenarioTreatmentsFile")]
        [Authorize(Policy = Policy.ImportTreatmentFromScenario)]
        public async Task<IActionResult> ImportScenarioTreatmentsFile()
        {
            try
            {
                if (!ContextAccessor.HttpContext.Request.HasFormContentType)
                {
                    throw new ConstraintException("Request MIME type is invalid.");
                }

                if (ContextAccessor.HttpContext.Request.Form.Files.Count < 1)
                {
                    throw new ConstraintException("Treatments file not found.");
                }

                if (!ContextAccessor.HttpContext.Request.Form.TryGetValue("simulationId", out var id))
                {
                    throw new ConstraintException("Request contained no simulation id.");
                }

                var excelPackage = new ExcelPackage(ContextAccessor.HttpContext.Request.Form.Files[0].OpenReadStream());
                var simulationId = Guid.Parse(id.ToString());

                var simulationName = "";
                await Task.Factory.StartNew(() =>
                {
                    _claimHelper.CheckUserSimulationModifyAuthorization(simulationId, UserId);
                    simulationName = UnitOfWork.SimulationRepo.GetSimulationName(simulationId);
                });

                ImportScenarioTreatmentWorkitem workItem = new ImportScenarioTreatmentWorkitem(simulationId, excelPackage, UserInfo.Name, simulationName);
                var analysisHandle = _generalWorkQueueService.CreateAndRun(workItem);

                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastWorkQueueUpdate, simulationId.ToString());

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TreatmentError}::ImportScenarioTreatmentsFile - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TreatmentError}::ImportScenarioTreatmentsFile - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("ExportScenarioTreatmentsExcelFile/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> ExportScenarioTreatmentsExcelFile(Guid simulationId)
        {
            try
            {
                // Rename
                var result =
                    await Task.Factory.StartNew(() => _treatmentService.ExportScenarioTreatmentsExcelFile(simulationId));

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TreatmentError}::ExportScenarioTreatmentsExcelFile for {simulationName} - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TreatmentError}::ExportScenarioTreatmentsExcelFile for {simulationName} - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("DownloadScenarioTreatmentsTemplate")]
        [Authorize]
        public async Task<IActionResult> DownloadScenarioTreatmentsTemplate()
        {
            try
            {
                var filePath = AppDomain.CurrentDomain.BaseDirectory + "DownloadTemplates\\Scenario_treatments_template.xlsx";
                var fileData = System.IO.File.ReadAllBytes(filePath);
                var result = await Task.Factory.StartNew(() => new FileInfoDTO
                {
                    FileName = "Scenario_treatments_template",
                    FileData = Convert.ToBase64String(fileData),
                    MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                });

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TreatmentError}::DownloadScenarioTreatmentsTemplate - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TreatmentError}::DownloadScenarioTreatmentsTemplate - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("DownloadLibraryTreatmentsTemplate")]
        [Authorize]
        public async Task<IActionResult> DownloadLibraryTreatmentsTemplate()
        {
            try
            {
                var filePath = AppDomain.CurrentDomain.BaseDirectory + "DownloadTemplates\\Library_treatments_template.xlsx";
                var fileData = System.IO.File.ReadAllBytes(filePath);
                var result = await Task.Factory.StartNew(() => new FileInfoDTO
                {
                    FileName = "Library_treatments_template",
                    FileData = Convert.ToBase64String(fileData),
                    MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                });

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TreatmentError}::DownloadLibraryTreatmentsTemplate - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TreatmentError}::DownloadLibraryTreatmentsTemplate - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetHasPermittedAccess")]
        [Authorize(Policy = Policy.ModifyOrDeleteTreatmentFromLibrary)]
        public async Task<IActionResult> GetHasPermittedAccess()
        {
            return Ok(true);
        }
        [HttpGet]
        [Route("GetIsSharedLibrary/{treatmentLibraryId}")]
        [Authorize]
        public async Task<IActionResult> GetIsSharedLibrary(Guid treatmentLibraryId)
        {
            bool result = true;
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    var users = UnitOfWork.TreatmentLibraryUserRepo.GetLibraryUsers(treatmentLibraryId);
                    if (users.Count > 0)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                });
                return Ok(result);
            }
            catch (Exception)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TreatmentError}::GetIsSharedLibrary - {HubService.errorList["Exception"]}");
                throw;
            }
        }
        private List<TreatmentLibraryDTO> GetAllTreatmentLibraries()
        {
            return UnitOfWork.SelectableTreatmentRepo.GetAllTreatmentLibraries();
        }
    }
}
