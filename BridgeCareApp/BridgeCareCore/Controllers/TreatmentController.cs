﻿using System;
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
using Microsoft.EntityFrameworkCore;
using Policy = BridgeCareCore.Security.SecurityConstants.Policy;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using BridgeCareCore.Models;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TreatmentController : BridgeCareCoreBaseController
    {
        public const string TreatmentError = "Treatment Error";

        private readonly ITreatmentService _treatmentService;
        private readonly IClaimHelper _claimHelper;

        private Guid UserId => UnitOfWork.CurrentUser?.Id ?? Guid.Empty;
        public TreatmentController(ITreatmentService treatmentService, IEsecSecurity esecSecurity, IUnitOfWork unitOfWork, IHubService hubService, IHttpContextAccessor httpContextAccessor, IClaimHelper claimHelper) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            _treatmentService = treatmentService;
            _claimHelper = claimHelper ?? throw new ArgumentNullException(nameof(claimHelper));
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
                    result = UnitOfWork.SelectableTreatmentRepo.GetAllTreatmentLibrariesNoChildren();
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        result = result.Where(_ => _.Owner == UserId || _.IsShared == true).ToList();
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
                        if (library.Owner == UserId || library.IsShared == true)
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
        [Route("GetSelectedTreatmentById/{id}")]
        [Authorize(Policy = Policy.ViewTreatmentFromScenario)]
        public async Task<IActionResult> GetSelectedTreatmentById(Guid id)
        {
            try
            {

                var result = new TreatmentDTO();
                await Task.Factory.StartNew(() =>
                {
                    var entity = UnitOfWork.Context.SelectableTreatment.AsNoTracking()
                    .Include(_ => _.TreatmentCosts)
                    .ThenInclude(_ => _.TreatmentCostEquationJoin)
                    .ThenInclude(_ => _.Equation)
                    .Include(_ => _.TreatmentCosts)
                    .ThenInclude(_ => _.CriterionLibraryTreatmentCostJoin)
                    .ThenInclude(_ => _.CriterionLibrary)
                    .Include(_ => _.TreatmentConsequences.OrderBy(__ => __.Attribute.Name))
                    .ThenInclude(_ => _.Attribute)
                    .Include(_ => _.TreatmentConsequences)
                    .ThenInclude(_ => _.ConditionalTreatmentConsequenceEquationJoin)
                    .ThenInclude(_ => _.Equation)
                    .Include(_ => _.TreatmentConsequences)
                    .ThenInclude(_ => _.CriterionLibraryConditionalTreatmentConsequenceJoin)
                    .ThenInclude(_ => _.CriterionLibrary)
                    .Include(_ => _.CriterionLibrarySelectableTreatmentJoin)
                    .ThenInclude(_ => _.CriterionLibrary)
                    .Include(_ => _.TreatmentLibrary)
                    .Single(_ => _.Id == id);
                    var library = entity.TreatmentLibrary.ToDto();
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        if (library.Owner == UserId || library.IsShared == true)
                            result = entity.ToDto();
                    }
                    else
                        result = entity.ToDto();
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
                    var entity = UnitOfWork.Context.ScenarioSelectableTreatment.AsNoTracking()
                    .Include(_ => _.ScenarioTreatmentCosts)
                    .ThenInclude(_ => _.ScenarioTreatmentCostEquationJoin)
                    .ThenInclude(_ => _.Equation)
                    .Include(_ => _.ScenarioTreatmentCosts)
                    .ThenInclude(_ => _.CriterionLibraryScenarioTreatmentCostJoin)
                    .ThenInclude(_ => _.CriterionLibrary)
                    .Include(_ => _.ScenarioTreatmentConsequences.OrderBy(__ => __.Attribute.Name))
                    .ThenInclude(_ => _.Attribute)
                    .Include(_ => _.ScenarioTreatmentConsequences)
                    .ThenInclude(_ => _.ScenarioConditionalTreatmentConsequenceEquationJoin)
                    .ThenInclude(_ => _.Equation)
                    .Include(_ => _.ScenarioTreatmentConsequences)
                    .ThenInclude(_ => _.CriterionLibraryScenarioConditionalTreatmentConsequenceJoin)
                    .ThenInclude(_ => _.CriterionLibrary)
                    .Include(_ => _.ScenarioSelectableTreatmentScenarioBudgetJoins)
                    .ThenInclude(_ => _.ScenarioBudget)
                    .Include(_ => _.CriterionLibraryScenarioSelectableTreatmentJoin)
                    .ThenInclude(_ => _.CriterionLibrary)
                    .Single(_ => _.Id == id);
                    _claimHelper.CheckUserSimulationReadAuthorization(entity.SimulationId, UserId);
                    result = entity.ToDto();
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

        [HttpPost]
        [Route("UpsertTreatmentLibrary")]
        [Authorize(Policy = Policy.ModifyTreatmentFromLibrary)]
        public async Task<IActionResult> UpsertTreatmentLibrary(LibraryUpsertPagingRequestModel<TreatmentLibraryDTO, TreatmentDTO> upsertRequest)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    var dto = _treatmentService.GetSyncedLibraryDataset(upsertRequest);
                    if (dto != null)
                    {
                        _claimHelper.OldWayCheckUserLibraryModifyAuthorization(dto.Owner, UserId);
                    }
                    UnitOfWork.SelectableTreatmentRepo.UpsertTreatmentLibrary(dto);
                    UnitOfWork.SelectableTreatmentRepo.UpsertOrDeleteTreatments(dto.Treatments, dto.Id);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TreatmentError}::UpsertTreatmentLibrary - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
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
                    UnitOfWork.BeginTransaction();
                    _claimHelper.CheckUserSimulationModifyAuthorization(simulationId, UserId);
                    var dtos = _treatmentService.GetSyncedScenarioDataset(simulationId, pagingSync);
                    UnitOfWork.SelectableTreatmentRepo.UpsertOrDeleteScenarioSelectableTreatment(dtos, simulationId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                UnitOfWork.Rollback();
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TreatmentError}::UpsertScenarioSelectedTreatments for {simulationName} - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
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
                    UnitOfWork.BeginTransaction();
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        var dto = GetAllTreatmentLibraries().FirstOrDefault(_ => _.Id == libraryId);
                        if (dto == null) return;
                        _claimHelper.OldWayCheckUserLibraryModifyAuthorization(dto.Owner, UserId);
                    }
                    UnitOfWork.SelectableTreatmentRepo.DeleteTreatmentLibrary(libraryId);
                    UnitOfWork.Commit();
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
                UnitOfWork.Rollback();
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
                var result = new TreatmentImportResultDTO();
                await Task.Factory.StartNew(() =>
                {
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        var existingTreatmentLibrary = UnitOfWork.SelectableTreatmentRepo.GetSingleTreatmentLibary(treatmentLibraryId);
                        if (existingTreatmentLibrary != null)
                        {
                            _claimHelper.OldWayCheckUserLibraryModifyAuthorization(existingTreatmentLibrary.Owner, UserId);
                        }
                    }
                    result = _treatmentService.ImportLibraryTreatmentsFile(treatmentLibraryId, excelPackage);
                });

                if (!string.IsNullOrEmpty(result.WarningMessage))
                {
                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastWarning, result.WarningMessage);
                    return Ok(null);
                }

                return Ok(result.TreatmentLibrary);
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
                    UnitOfWork.BeginTransaction();
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        var dto = GetAllTreatmentLibraries().FirstOrDefault(_ => _.Id == libraryId);
                        if (dto == null || treatment == null) return;
                        _claimHelper.OldWayCheckUserLibraryModifyAuthorization(dto.Owner, UserId);
                    }
                    UnitOfWork.SelectableTreatmentRepo.DeleteTreatment(treatment, libraryId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TreatmentError}::DeleteTreatment {treatmentName} - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
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
                    UnitOfWork.BeginTransaction();
                    _claimHelper.CheckUserSimulationModifyAuthorization(simulationId, UserId);
                    UnitOfWork.SelectableTreatmentRepo.DeleteScenarioSelectableTreatment(scenarioSelectableTreatment, simulationId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TreatmentError}::DeleteScenarioSelectableTreatment {treatmentName} - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
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
                var result = new ScenarioTreatmentImportResultDTO();
                await Task.Factory.StartNew(() =>
                {
                    _claimHelper.CheckUserSimulationModifyAuthorization(simulationId, UserId);
                    result = _treatmentService.ImportScenarioTreatmentsFile(simulationId, excelPackage);
                });

                if (!string.IsNullOrEmpty(result.WarningMessage))
                {
                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastWarning, result.WarningMessage);
                    return Ok(null);
                }

                return Ok(result.Treatments);
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
        [Authorize]
        [Authorize(Policy = Policy.ModifyOrDeleteTreatmentFromLibrary)]
        public async Task<IActionResult> GetHasPermittedAccess()
        {
            return Ok(true);
        }

        private List<TreatmentLibraryDTO> GetAllTreatmentLibraries()
        {
            return UnitOfWork.SelectableTreatmentRepo.GetAllTreatmentLibraries();
        }
    }
}
