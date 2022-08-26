using System;
using System.Collections.Generic;
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
using System.Data;
using OfficeOpenXml;
using BridgeCareCore.Utils.Interfaces;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerformanceCurveController : BridgeCareCoreBaseController
    {
        private Guid UserId => UnitOfWork.CurrentUser?.Id ?? Guid.Empty;
        private readonly IPerformanceCurvesService _performanceCurvesService;
        private readonly IClaimHelper _claimHelper;

        public PerformanceCurveController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork,
            IHubService hubService, IHttpContextAccessor httpContextAccessor, IPerformanceCurvesService performanceCurvesService, IClaimHelper claimHelper) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            _performanceCurvesService = performanceCurvesService ?? throw new ArgumentNullException(nameof(performanceCurvesService));
            _claimHelper = claimHelper ?? throw new ArgumentNullException(nameof(claimHelper));
        }      

        [HttpGet]
        [Route("GetPerformanceCurveLibraries")]
        [Authorize]
        public async Task<IActionResult> GetPerformanceCurveLibraries()
        {
            try
            {
                var result = new List<PerformanceCurveLibraryDTO>();
                await Task.Factory.StartNew(() =>
                {
                    result = GetAllPerformanceCurveLibraries();
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        result = result.Where(_ => _.Owner == UserId || _.IsShared == true).ToList();
                    }
                });

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Deterioration model error::{e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetScenarioPerformanceCurves/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> GetScenarioPerformanceCurves(Guid simulationId)
        {
            try
            {
                var result = new List<PerformanceCurveDTO>();
                await Task.Factory.StartNew(() =>
                {
                    _claimHelper.CheckUserSimulationReadAuthorization(simulationId);
                    result = UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
                });
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Deterioration model error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertPerformanceCurveLibrary")]
        [Authorize]
        public async Task<IActionResult> UpsertPerformanceCurveLibrary(PerformanceCurveLibraryDTO dto)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {

                    UnitOfWork.BeginTransaction();
                    var currentRecord = GetAllPerformanceCurveLibraries().FirstOrDefault(_ => _.Id == dto.Id);
                    // by pass owner check if no record
                    if (currentRecord != null)
                    {
                        _claimHelper.CheckUserLibraryModifyAuthorization(currentRecord.Owner);
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
                return Unauthorized();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Deterioration model error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertScenarioPerformanceCurves/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> UpsertScenarioPerformanceCurves(Guid simulationId, List<PerformanceCurveDTO> dtos)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    _claimHelper.CheckUserSimulationModifyAuthorization(simulationId);
                    UnitOfWork.PerformanceCurveRepo.UpsertOrDeleteScenarioPerformanceCurves(dtos, simulationId);
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Deterioration model error::{e.Message}");
                throw;
            }
        }

        [HttpDelete]
        [Route("DeletePerformanceCurveLibrary/{libraryId}")]
        [Authorize]
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
                        _claimHelper.CheckUserLibraryModifyAuthorization(dto.Owner);
                    }
                    UnitOfWork.PerformanceCurveRepo.DeletePerformanceCurveLibrary(libraryId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Deterioration model error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("ImportLibraryPerformanceCurvesExcelFile")]
        [Authorize]
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
                            _claimHelper.CheckUserLibraryModifyAuthorization(existingPerformanceCurveLibrary.Owner);
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
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Deterioration model error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("ImportScenarioPerformanceCurvesExcelFile")]
        [Authorize]
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
                    _claimHelper.CheckUserSimulationModifyAuthorization(simulationId);
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
                return Unauthorized();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Deterioration model error::{e.Message}");
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
                return Unauthorized();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Deterioration model error::{e.Message}");
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
                return Unauthorized();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Deterioration model error::{e.Message}");
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
                return Unauthorized();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Deterioration model error::{e.Message}");
                throw;
            }
        }

        private List<PerformanceCurveLibraryDTO> GetAllPerformanceCurveLibraries()
        {
            return UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibraries();
        }
    }
}
