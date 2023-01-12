using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Controllers.BaseController;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;
using BridgeCareCore.Security.Interfaces;
using BridgeCareCore.Services;
using BridgeCareCore.Utils.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Policy = BridgeCareCore.Security.SecurityConstants.Policy;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerformanceCurveController : BridgeCareCoreBaseController
    {
        public const string DeteriorationModelError = "Deterioration Model Error";

        private Guid UserId => UnitOfWork.CurrentUser?.Id ?? Guid.Empty;
        private readonly IPerformanceCurvesService _performanceCurvesService;
        private readonly IPerformanceCurvesPagingService _performanceCurvePagingService;
        private readonly IClaimHelper _claimHelper;

        public PerformanceCurveController(IEsecSecurity esecSecurity, IUnitOfWork unitOfWork,
            IHubService hubService,
            IHttpContextAccessor httpContextAccessor,
            IPerformanceCurvesService performanceCurvesService, IPerformanceCurvesPagingService performanceCurvesPagingService,
            IClaimHelper claimHelper) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            _performanceCurvesService = performanceCurvesService ?? throw new ArgumentNullException(nameof(performanceCurvesService));
            _performanceCurvePagingService = performanceCurvesPagingService ?? throw new ArgumentNullException(nameof(performanceCurvesPagingService));
            _claimHelper = claimHelper ?? throw new ArgumentNullException(nameof(claimHelper));
        }

        [HttpPost]
        [Route("GetScenarioPerformanceCurvePage/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> GetScenarioPerformanceCurvePage(Guid simulationId, PagingRequestModel<PerformanceCurveDTO> pageRequest)
        {
            try
            {
                var result = await Task.Factory.StartNew(() => _performanceCurvePagingService.GetScenarioPage(simulationId, pageRequest));
                return Ok(result);
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::GetScenarioPerformanceCurvePage for {simulationName} - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("GetLibraryPerformanceCurvePage/{libraryId}")]
        [Authorize]
        public async Task<IActionResult> GetLibraryPerformanceCurvePage(Guid libraryId, PagingRequestModel<PerformanceCurveDTO> pageRequest)
        {
            try
            {
                var result = await Task.Factory.StartNew(() => _performanceCurvePagingService.GetLibraryPage(libraryId, pageRequest));
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::GetLibraryPerformanceCurvePage - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetPerformanceCurveLibraries")]
        [Authorize(Policy = Policy.ViewPerformanceCurveFromLibrary)]
        public async Task<IActionResult> GetPerformanceCurveLibraries()
        {
            try
            {
                var result = new List<PerformanceCurveLibraryDTO>();
                await Task.Factory.StartNew(() =>
                {
                    result = UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrariesNoPerformanceCurves();
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        result = result.Where(_ => _.Owner == UserId || _.IsShared == true).ToList();
                    }
                });

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::GetPerformanceCurveLibraries - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetScenarioPerformanceCurves/{simulationId}")]
        [Authorize(Policy = Policy.ViewPerformanceCurveFromScenario)]
        public async Task<IActionResult> GetScenarioPerformanceCurves(Guid simulationId)
        {
            try
            {
                var result = new List<PerformanceCurveDTO>();
                await Task.Factory.StartNew(() =>
                {
                    _claimHelper.CheckUserSimulationReadAuthorization(simulationId, UserId);
                    result = UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
                });

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::GetScenarioPerformanceCurves for {simulationName} - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::GetScenarioPerformanceCurves for {simulationName} - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertPerformanceCurveLibrary")]
        [Authorize(Policy = Policy.ModifyPerformanceCurveFromLibrary)]
        public async Task<IActionResult> UpsertPerformanceCurveLibrary(LibraryUpsertPagingRequestModel<PerformanceCurveLibraryDTO, PerformanceCurveDTO> upsertRequest)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    var curves = _performanceCurvePagingService.GetSyncedLibraryDataset(upsertRequest);
                    var dto = upsertRequest.Library;
                    if (dto != null)
                    {
                        _claimHelper.CheckUserLibraryModifyAuthorization(dto.Owner, UserId);
                        dto.PerformanceCurves = curves;
                    }
                    UnitOfWork.PerformanceCurveRepo.UpsertPerformanceCurveLibrary(dto);
                    UnitOfWork.PerformanceCurveRepo.UpsertOrDeletePerformanceCurves(dto.PerformanceCurves, dto.Id);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::UpsertPerformanceCurveLibrary - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::UpsertPerformanceCurveLibrary - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertScenarioPerformanceCurves/{simulationId}")]
        [Authorize(Policy = Policy.ModifyPerformanceCurveFromScenario)]
        public async Task<IActionResult> UpsertScenarioPerformanceCurves(Guid simulationId, PagingSyncModel<PerformanceCurveDTO> pagingSync)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    var dtos = _performanceCurvePagingService.GetSyncedScenarioDataSet(simulationId, pagingSync);                   
                    _claimHelper.CheckUserSimulationModifyAuthorization(simulationId, UserId);
                    UnitOfWork.PerformanceCurveRepo.UpsertOrDeleteScenarioPerformanceCurves(dtos, simulationId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::UpsertScenarioPerformanceCurves - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::UpsertScenarioPerformanceCurves - {e.Message}");
                throw;
            }
        }

        [HttpDelete]
        [Route("DeletePerformanceCurveLibrary/{libraryId}")]
        [Authorize(Policy = Policy.DeletePerformanceCurveFromLibrary)]
        public async Task<IActionResult> DeletePerformanceCurveLibrary(Guid libraryId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        var dto = GetAllPerformanceCurveLibraries().FirstOrDefault(_ => _.Id == libraryId);
                        if (dto == null) return;
                        _claimHelper.CheckUserLibraryModifyAuthorization(dto.Owner, UserId);
                    }
                    UnitOfWork.PerformanceCurveRepo.DeletePerformanceCurveLibrary(libraryId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::DeletePerformanceCurveLibrary - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::DeletePerformanceCurveLibrary - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("ImportLibraryPerformanceCurvesExcelFile")]
        [Authorize(Policy = Policy.ImportPerformanceCurveFromLibrary)]
        public async Task<IActionResult> ImportLibraryPerformanceCurvesExcelFile()
        {
            try
            {
                if (!ContextAccessor.HttpContext.Request.HasFormContentType)
                {
                    throw new ConstraintException("Request MIME type is invalid.");
                }

                if (ContextAccessor.HttpContext.Request.Form.Files.Count < 1)
                {
                    throw new ConstraintException("PerformanceCurves file not found.");
                }

                if (!ContextAccessor.HttpContext.Request.Form.TryGetValue("libraryId", out var libraryId))
                {
                    throw new ConstraintException("Request contained no performance curve library id.");
                }

                var performanceCurveLibraryId = Guid.Parse(libraryId.ToString());
                var excelPackage = new ExcelPackage(ContextAccessor.HttpContext.Request.Form.Files[0].OpenReadStream());

                var currentUserCriteriaFilter = new UserCriteriaDTO
                {
                    HasCriteria = false
                };
                if (ContextAccessor.HttpContext.Request.Form.ContainsKey("currentUserCriteriaFilter"))
                {
                    currentUserCriteriaFilter =
                        Newtonsoft.Json.JsonConvert.DeserializeObject<UserCriteriaDTO>(
                            ContextAccessor.HttpContext.Request.Form["currentUserCriteriaFilter"]);
                }
                                
                var result= new PerformanceCurvesImportResultDTO();
                await Task.Factory.StartNew(() =>
                {
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        var existingPerformanceCurveLibrary = UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(performanceCurveLibraryId);
                        if (existingPerformanceCurveLibrary != null)
                        {
                            _claimHelper.CheckUserLibraryModifyAuthorization(existingPerformanceCurveLibrary.Owner, UserId);
                        }
                    }
                    result = _performanceCurvesService.ImportLibraryPerformanceCurvesFile(performanceCurveLibraryId, excelPackage, currentUserCriteriaFilter);
                });

                if (result.WarningMessage != null)
                {
                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastWarning, result.WarningMessage);
                }
                return Ok(result.PerformanceCurveLibraryDTO);
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::ImportLibraryPerformanceCurvesExcelFile - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::ImportLibraryPerformanceCurvesExcelFile - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("ImportScenarioPerformanceCurvesExcelFile")]
        [Authorize(Policy = Policy.ImportPerformanceCurveFromScenario)]
        public async Task<IActionResult> ImportScenarioPerformanceCurvesExcelFile()
        {
            try
            {
                if (!ContextAccessor.HttpContext.Request.HasFormContentType)
                {
                    throw new ConstraintException("Request MIME type is invalid.");
                }

                if (ContextAccessor.HttpContext.Request.Form.Files.Count < 1)
                {
                    throw new ConstraintException("PerformanceCurves file not found.");
                }

                if (!ContextAccessor.HttpContext.Request.Form.TryGetValue("simulationId", out var id))
                {
                    throw new ConstraintException("Request contained no simulation id.");
                }

                var simulationId = Guid.Parse(id.ToString());
                var excelPackage = new ExcelPackage(ContextAccessor.HttpContext.Request.Form.Files[0].OpenReadStream());
                var currentUserCriteriaFilter = new UserCriteriaDTO
                {
                    HasCriteria = false
                };
                if (ContextAccessor.HttpContext.Request.Form.ContainsKey("currentUserCriteriaFilter"))
                {
                    currentUserCriteriaFilter =
                        Newtonsoft.Json.JsonConvert.DeserializeObject<UserCriteriaDTO>(
                            ContextAccessor.HttpContext.Request.Form["currentUserCriteriaFilter"]);
                }

                var result = new ScenarioPerformanceCurvesImportResultDTO();
                await Task.Factory.StartNew(() =>
                {
                    _claimHelper.CheckUserSimulationModifyAuthorization(simulationId, UserId);
                    result = _performanceCurvesService.ImportScenarioPerformanceCurvesFile(simulationId, excelPackage, currentUserCriteriaFilter);
                });

                if (result.WarningMessage != null)
                {
                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastWarning, result.WarningMessage);
                }
                return Ok(result.PerformanceCurves);
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::ImportScenarioPerformanceCurvesExcelFile - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::ImportScenarioPerformanceCurvesExcelFile - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("ExportScenarioPerformanceCurvesExcelFile/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> ExportScenarioPerformanceCurvesExcelFile(Guid simulationId)
        {
            try
            {
                var result =
                    await Task.Factory.StartNew(() => _performanceCurvesService.ExportScenarioPerformanceCurvesFile(simulationId));

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::ExportScenarioPerformanceCurvesExcelFile for {simulationName} - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::ExportScenarioPerformanceCurvesExcelFile for {simulationName} - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("ExportLibraryPerformanceCurvesExcelFile/{performanceCurveLibraryId}")]
        [Authorize]
        public async Task<IActionResult> ExportLibraryPerformanceCurvesExcelFile(Guid performanceCurveLibraryId)
        {
            try
            {
                var result =
                    await Task.Factory.StartNew(() => _performanceCurvesService.ExportLibraryPerformanceCurvesFile(performanceCurveLibraryId));

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::ExportLibraryPerformanceCurvesExcelFile - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::ExportLibraryPerformanceCurvesExcelFile - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("DownloadPerformanceCurvesTemplate")]
        [Authorize]
        public async Task<IActionResult> DownloadPerformanceCurvesTemplate()
        {
            try
            {
                var filePath = AppDomain.CurrentDomain.BaseDirectory + "DownloadTemplates\\DeteriorationModels_template.xlsx";
                var fileData = System.IO.File.ReadAllBytes(filePath);
                var result = await Task.Factory.StartNew(() => new FileInfoDTO
                {
                    FileName = "DeteriorationModels_template",
                    FileData = Convert.ToBase64String(fileData),
                    MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                });

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::DownloadPerformanceCurvesTemplate - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::DownloadPerformanceCurvesTemplate - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetHasPermittedAccess")]
        [Authorize]
        [Authorize(Policy = Policy.ModifyOrDeletePerformanceCurveFromLibrary)]
        public async Task<IActionResult> GetHasPermittedAccess()
        {
            return Ok(true);
        }

        private List<PerformanceCurveLibraryDTO> GetAllPerformanceCurveLibraries()
        {
            return UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibraries();
        }
    }
}
