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

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TreatmentController : BridgeCareCoreBaseController
    {
        private readonly ITreatmentService _treatmentService;
        private readonly IClaimHelper _claimHelper;

        private Guid UserId => UnitOfWork.CurrentUser?.Id ?? Guid.Empty;
        public TreatmentController(ITreatmentService treatmentService, IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService, IHttpContextAccessor httpContextAccessor, IClaimHelper claimHelper) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            _treatmentService = treatmentService;
            _claimHelper = claimHelper ?? throw new ArgumentNullException(nameof(claimHelper));
        }       

        [HttpGet]
        [Route("GetTreatmentLibraries")]
        [Authorize]
        public async Task<IActionResult> GetTreatmentLibraries()
        {
            try
            {
                var result = new List<TreatmentLibraryDTO>();
                await Task.Factory.StartNew(() =>
                {
                    result = GetAllTreatmentLibraries();
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        result = result.Where(_ => _.Owner == UserId || _.IsShared == true).ToList();
                    }
                });

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Treatment error::{e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetScenarioSelectedTreatments/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> GetScenarioSelectedTreatments(Guid simulationId)
        {
            try
            {
                var result = new List<TreatmentDTO>();
                await Task.Factory.StartNew(() =>
                {
                    _claimHelper.CheckUserSimulationReadAuthorization(simulationId);
                    result = UnitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatments(simulationId);
                });

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Treatment error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertTreatmentLibrary")]
        [Authorize]
        public async Task<IActionResult> UpsertTreatmentLibrary(TreatmentLibraryDTO dto)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    var currentRecord = GetAllTreatmentLibraries().FirstOrDefault(_ => _.Id == dto.Id);
                    // by pass owner check if no record
                    if (currentRecord != null)
                    {
                        _claimHelper.CheckUserLibraryModifyAuthorization(currentRecord.Owner);
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
                return Unauthorized();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Treatment error::{e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("ExportLibraryTreatmentsExcelFile/{libraryId}")]
        [Authorize]
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
                return Unauthorized();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Treatment error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertScenarioSelectedTreatments/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> UpsertScenarioSelectedTreatments(Guid simulationId, List<TreatmentDTO> dtos)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    _claimHelper.CheckUserSimulationModifyAuthorization(simulationId);
                    UnitOfWork.SelectableTreatmentRepo.UpsertOrDeleteScenarioSelectableTreatment(dtos, simulationId);
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Treatment error::{e.Message}");
                throw;
            }
        }

        [HttpDelete]
        [Route("DeleteTreatmentLibrary/{libraryId}")]
        [Authorize]
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
                        _claimHelper.CheckUserLibraryModifyAuthorization(dto.Owner);
                    }
                    UnitOfWork.SelectableTreatmentRepo.DeleteTreatmentLibrary(libraryId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Treatment error::{e.Message}");
                throw;
            }
        }


        [HttpPost]
        [Route("ImportLibraryTreatmentsFile")]
        [Authorize]
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
                            _claimHelper.CheckUserLibraryModifyAuthorization(existingTreatmentLibrary.Owner);
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
                return Unauthorized();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Treatment error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("DeleteTreatment/{libraryId}")]
        [Authorize]
        public async Task<IActionResult> DeleteTreatment(TreatmentDTO treatment, Guid libraryId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        var dto = GetAllTreatmentLibraries().FirstOrDefault(_ => _.Id == libraryId);
                        if (dto == null || treatment == null) return;
                        _claimHelper.CheckUserLibraryModifyAuthorization(dto.Owner);
                    }
                    UnitOfWork.SelectableTreatmentRepo.DeleteTreatment(treatment, libraryId);
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Treatment error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("DeleteScenarioSelectableTreatment/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> DeleteScenarioSelectableTreatment(TreatmentDTO scenarioSelectableTreatment, Guid simulationId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    _claimHelper.CheckUserSimulationModifyAuthorization(simulationId);
                    UnitOfWork.SelectableTreatmentRepo.DeleteScenarioSelectableTreatment(scenarioSelectableTreatment, simulationId);
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Treatment error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("ImportScenarioTreatmentsFile")]
        [Authorize]
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
                    _claimHelper.CheckUserSimulationModifyAuthorization(simulationId);
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
                return Unauthorized();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Treatment error::{e.Message}");
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
                return Unauthorized();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Treatment error::{e.Message}");
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
                return Unauthorized();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Treatment error::{e.Message}");
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
                return Unauthorized();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Treatment error::{e.Message}");
                throw;
            }
        }

        private List<TreatmentLibraryDTO> GetAllTreatmentLibraries()
        {
            return UnitOfWork.SelectableTreatmentRepo.GetAllTreatmentLibraries();
        }
    }
}
