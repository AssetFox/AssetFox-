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
using BridgeCareCore.Services.General_Work_Queue.WorkItems;
using BridgeCareCore.Services;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TreatmentController : BridgeCareCoreBaseController
    {
        public const string TreatmentError = "Treatment Error";
        public const string RequestedToModifyNonexistentLibraryErrorMessage = "The request says to modify a library, but the library does not exist.";
        public const string RequestedToCreateExistingLibraryErrorMessage = "The request says to create a new library, but the library already exists.";
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
        [Route("GetTreatmentLibraryModifiedDate/{libraryId}")]
        [Authorize(Policy = Policy.ModifyInvestmentFromLibrary)]
        public async Task<IActionResult> GetTreatmentLibraryDate(Guid libraryId)
        {
            try
            {
                var users = new DateTime();
                await Task.Factory.StartNew(() =>
                {
                    users = UnitOfWork.SelectableTreatmentRepo.GetLibraryModifiedDate(libraryId);
                });
                return Ok(users);
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"Investment error::{e.Message}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"Investment error::{e.Message}", e);
            }
            return Ok();
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
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::GetTreatmentLibraries - {e.Message}", e);
            }
            return Ok();
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
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::GetSimpleTreatmentsByLibraryId - {e.Message}", e);
            }
            return Ok();
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
                        var library = UnitOfWork.SelectableTreatmentRepo.GetSingleTreatmentLibaryNoChildren(selectableTreatment.LibraryId);
                        if (library != null) library.IsModified = selectableTreatment.IsModified;
                        result = library;
                    }                                         
                });
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::GetSelectedTreatmentById - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError} ::GetSelectedTreatmentById - {e.Message}", e);
            }
            return Ok();
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
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::GetSelectedTreatmentById - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError} ::GetSelectedTreatmentById - {e.Message}", e);
            }
            return Ok();
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
            catch (UnauthorizedAccessException e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::GetScenarioSelectedTreatments for {simulationName} - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::GetScenarioSelectedTreatments for {simulationName} - {e.Message}", e);
            }
            return Ok();
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
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError} ::GetScenarioSelectedTreatmentById - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError} ::GetScenarioSelectedTreatmentById - {e.Message}", e);
            }
            return Ok();
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
            catch (UnauthorizedAccessException e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError} ::GetSimpleTreatmentsByScenarioId for {simulationName} - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError} ::GetSimpleTreatmentsByScenarioId for {simulationName} - {e.Message}", e);
            }
            return Ok();
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
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::GetTreatmentLibraryUsers - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"Investment error::{e.Message}", e);
            }
            return Ok();
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
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"Investment error::{e.Message}", e);
            }
            catch (InvalidOperationException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"Investment error::{e.Message}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"Investment error::{e.Message}", e);
            }
            return Ok();
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
                    var libraryAccess = UnitOfWork.TreatmentLibraryUserRepo.GetLibraryAccess(upsertRequest.Library.Id, UserId);
                    if (libraryAccess.LibraryExists == upsertRequest.IsNewLibrary)
                    {
                        var errorMessage = libraryAccess.LibraryExists ? RequestedToCreateExistingLibraryErrorMessage : RequestedToModifyNonexistentLibraryErrorMessage;
                        throw new InvalidOperationException(errorMessage);
                    }
                    var items = _treatmentPagingService.GetSyncedLibraryDataset(upsertRequest);
                    var dto = upsertRequest.Library;
                    if (dto != null)
                    {
                        _claimHelper.CheckUserLibraryModifyAuthorization(libraryAccess, UserId);
                        dto.Treatments = items;
                    }
                    UnitOfWork.SelectableTreatmentRepo.UpsertOrDeleteTreatmentLibraryTreatmentsAndPossiblyUsers(dto, upsertRequest.IsNewLibrary, UserId);
                });

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::UpsertTreatmentLibrary - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::UpsertTreatmentLibrary - {e.Message}", e);
            }
            return Ok();
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
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::ExportLibraryTreatmentsExcelFile - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::ExportLibraryTreatmentsExcelFile - {e.Message}", e);
            }
            return Ok();
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
                    TreatmentDtoListService.AddLibraryIdToScenarioSelectableTreatments(dtos, pagingSync.LibraryId);
                    TreatmentDtoListService.AddModifiedToScenarioSelectableTreatments(dtos, pagingSync.IsModified);
                    UnitOfWork.SelectableTreatmentRepo.UpsertOrDeleteScenarioSelectableTreatment(dtos, simulationId);
                });

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::UpsertScenarioSelectedTreatments for {simulationName} - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::UpsertScenarioSelectedTreatments for {simulationName} - {e.Message}", e);
            }
            return Ok();
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
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::DeleteTreatmentLibrary - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::DeleteTreatmentLibrary - {e.Message}", e);
            }
            return Ok();
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
                var analysisHandle = _generalWorkQueueService.CreateAndRunInFastQueue(workItem);

                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastWorkQueueUpdate, libraryId.ToString());

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::ImportLibraryTreatmentsFile - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::ImportLibraryTreatmentsFile - {e.Message}", e);
            }
            return Ok();
        }

        [HttpPost]
        [Route("ImportLibraryTreatmentsFileSingle")]
        [Authorize(Policy = Policy.ImportTreatmentFromLibrary)]
        public async Task<IActionResult> ImportLibraryTreatmentsFileSingle()
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

                ImportLibraryTreatmentWorkitemSingle workItem = new ImportLibraryTreatmentWorkitemSingle(treatmentLibraryId, excelPackage, UserInfo.Name, libraryName);
                var analysisHandle = _generalWorkQueueService.CreateAndRun(workItem);

                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastFastWorkQueueUpdate, libraryId.ToString());

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::ImportLibraryTreatmentsFile - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::ImportLibraryTreatmentsFile - {e.Message}", e);
            }
            return Ok();
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
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::DeleteTreatment {treatmentName} - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::DeleteTreatment {treatmentName}- {e.Message}", e);
            }
            return Ok();
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
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::DeleteScenarioSelectableTreatment {treatmentName} - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::DeleteScenarioSelectableTreatment {treatmentName} - {e.Message}", e);
            }
            return Ok();
        }

        [HttpPost]
        [Route("ImportScenarioTreatmentsFileSingle")]
        [Authorize(Policy = Policy.ImportTreatmentFromScenario)]
        public async Task<IActionResult> ImportScenarioTreatmentsFileSingle()
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

                ImportScenarioSingleTreatmentWorkitem workItem = new ImportScenarioSingleTreatmentWorkitem(simulationId, excelPackage, UserInfo.Name, simulationName);
                var analysisHandle = _generalWorkQueueService.CreateAndRun(workItem);

                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastWorkQueueUpdate, simulationId.ToString());

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::ImportScenarioTreatmentsFile - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::ImportScenarioTreatmentsFile - {e.Message}", e);
            }
            return Ok();
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
                var analysisHandle = _generalWorkQueueService.CreateAndRunInFastQueue(workItem);

                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastFastWorkQueueUpdate, simulationId.ToString());

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::ImportScenarioTreatmentsFile - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::ImportScenarioTreatmentsFile - {e.Message}", e);
            }
            return Ok();
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
            catch (UnauthorizedAccessException e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::ExportScenarioTreatmentsExcelFile for {simulationName} - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::ExportScenarioTreatmentsExcelFile for {simulationName} - {e.Message}", e);
            }
            return Ok();
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
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::DownloadScenarioTreatmentsTemplate - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::DownloadScenarioTreatmentsTemplate - {e.Message}", e);
            }
            return Ok();
        }

        [HttpPost]
        [Route("ImportScenarioTreatmentSupersedeRulesFile")]
        [Authorize(Policy = Policy.ImportTreatmentSupersedeRuleFromScenario)]
        public async Task<IActionResult> ImportScenarioTreatmentSupersedeRulesFile()
        {
            try
            {
                if (!ContextAccessor.HttpContext.Request.HasFormContentType)
                {
                    throw new ConstraintException("Request MIME type is invalid.");
                }

                if (ContextAccessor.HttpContext.Request.Form.Files.Count < 1)
                {
                    throw new ConstraintException("Treatment Supersede Rule file not found.");
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

                var workItem = new ImportScenarioTreatmentSupersedeRuleWorkitem(simulationId, excelPackage, UserInfo.Name, simulationName);
                var analysisHandle = _generalWorkQueueService.CreateAndRunInFastQueue(workItem);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastFastWorkQueueUpdate, simulationId.ToString());

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::ImportScenarioTreatmentSupersedeRulesFile - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::ImportScenarioTreatmentSupersedeRulesFile - {e.Message}", e);
            }
            return Ok();
        }

        [HttpGet]
        [Route("ExportScenarioTreatmentSupersedeRuleExcelFile/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> ExportScenarioTreatmentSupersedeRuleExcelFile(Guid simulationId)
        {
            try
            {
                var result =
                    await Task.Factory.StartNew(() => _treatmentService.ExportScenarioTreatmentSupersedeRuleExcelFile(simulationId));

                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::ExportScenarioTreatmentSupersedeRuleExcelFile for {simulationName} - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::ExportScenarioTreatmentSupersedeRuleExcelFile for {simulationName} - {e.Message}", e);
            }
            return Ok();
        }

        [HttpGet]
        [Route("DownloadTreatmentSupersedeRuleTemplate")]
        [Authorize]
        // Note: Scenario settings and Libraries will use same API
        public async Task<IActionResult> DownloadTreatmentSupersedeRuleTemplate()
        {
            try
            {
                var filePath = AppDomain.CurrentDomain.BaseDirectory + "DownloadTemplates\\TreatmentSupersedeRules_template.xlsx";
                var fileData = System.IO.File.ReadAllBytes(filePath);
                var result = await Task.Factory.StartNew(() => new FileInfoDTO
                {
                    FileName = "TreatmentSupersedeRules_template",
                    FileData = Convert.ToBase64String(fileData),
                    MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                });

                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::DownloadTreatmentSupersedeRuleTemplate - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::DownloadTreatmentSupersedeRuleTemplate - {e.Message}", e);
            }
            return Ok();
        }

        [HttpGet]
        [Route("ExportTreatmentSupersedeRuleExcelFile/{libraryId}")]
        [Authorize]
        public async Task<IActionResult> ExportTreatmentSupersedeRuleExcelFile(Guid libraryId)
        {
            try
            {
                var result =
                    await Task.Factory.StartNew(() => _treatmentService.ExportLibraryTreatmentSupersedeRuleExcelFile(libraryId));

                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::ExportTreatmentSupersedeRuleExcelFile - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::ExportTreatmentSupersedeRuleExcelFile - {e.Message}", e);
            }
            return Ok();
        }

        [HttpPost]
        [Route("ImportLibraryTreatmentSupersedeRulesFile")]
        [Authorize(Policy = Policy.ImportTreatmentSupersedeRuleFromLibrary)]
        public async Task<IActionResult> ImportLibraryTreatmentSupersedeRulesFile()
        {
            try
            {
                if (!ContextAccessor.HttpContext.Request.HasFormContentType)
                {
                    throw new ConstraintException("Request MIME type is invalid.");
                }

                if (ContextAccessor.HttpContext.Request.Form.Files.Count < 1)
                {
                    throw new ConstraintException("Treatment Supersede Rule file not found.");
                }

                if (!ContextAccessor.HttpContext.Request.Form.TryGetValue("libraryId", out var id) || id == Guid.Empty.ToString())
                {
                    throw new ConstraintException("Request contained no library id.");
                }

                var excelPackage = new ExcelPackage(ContextAccessor.HttpContext.Request.Form.Files[0].OpenReadStream());
                var libraryId = Guid.Parse(id.ToString());
                var libraryName = string.Empty;
                await Task.Factory.StartNew(() =>
                {
                    var existingTreatmentLibrary = UnitOfWork.SelectableTreatmentRepo.GetSingleTreatmentLibary(libraryId);
                    if (existingTreatmentLibrary != null)
                    {
                        libraryName = existingTreatmentLibrary.Name;
                        if (_claimHelper.RequirePermittedCheck())
                        {
                            var accessModel = UnitOfWork.TreatmentLibraryUserRepo.GetLibraryAccess(libraryId, UserId);
                            _claimHelper.CheckUserLibraryRecreateAuthorization(accessModel, UserId);
                        }
                    }
                });

                var workItem = new ImportLibraryTreatmentSupersedeRuleWorkitem(libraryId, excelPackage, UserInfo.Name, libraryName);
                var analysisHandle = _generalWorkQueueService.CreateAndRunInFastQueue(workItem);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastFastWorkQueueUpdate, libraryId.ToString());

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::ImportLibraryTreatmentSupersedeRulesFile - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::ImportLibraryTreatmentSupersedeRulesFile - {e.Message}", e);
            }
            return Ok();
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
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::DownloadLibraryTreatmentsTemplate - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::DownloadLibraryTreatmentsTemplate - {e.Message}", e);
            }
            return Ok();
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
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{TreatmentError}::GetIsSharedLibrary - {HubService.errorList["Exception"]}", e);
            }
            return Ok();
        }

        private List<TreatmentLibraryDTO> GetAllTreatmentLibraries()
        {
            return UnitOfWork.SelectableTreatmentRepo.GetAllTreatmentLibraries();
        }
    }
}
