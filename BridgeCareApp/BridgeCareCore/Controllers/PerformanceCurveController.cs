using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.Generics;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Controllers.BaseController;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;
using BridgeCareCore.Security.Interfaces;
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
        private readonly IClaimHelper _claimHelper;

        public PerformanceCurveController(IEsecSecurity esecSecurity, IUnitOfWork unitOfWork,
            IHubService hubService, IHttpContextAccessor httpContextAccessor, IPerformanceCurvesService performanceCurvesService, IClaimHelper claimHelper) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            _performanceCurvesService = performanceCurvesService ?? throw new ArgumentNullException(nameof(performanceCurvesService));
            _claimHelper = claimHelper ?? throw new ArgumentNullException(nameof(claimHelper));
        }

        [HttpPost]
        [Route("GetScenarioPerformanceCurvePage/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> GetScenarioPerformanceCurvePage(Guid simulationId, PagingRequestModel<PerformanceCurveDTO> pageRequest)
        {
            try
            {
                var result = await Task.Factory.StartNew(() => _performanceCurvesService.GetScenarioPerformanceCurvePage(simulationId, pageRequest));
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::GetScenarioPerformanceCurvePage - {HubService.errorList["Exception"]}");
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
                var result = await Task.Factory.StartNew(() => _performanceCurvesService.GetLibraryPerformanceCurvePage(libraryId, pageRequest));
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::GetPerformanceCurveLibraries - {HubService.errorList["Exception"]}");
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
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::GetScenarioPerformanceCurves - {HubService.errorList["Unauthorized"]}");
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::GetScenarioPerformanceCurves - {HubService.errorList["Exception"]}");
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

                    var libraryAccess = UnitOfWork.PerformanceCurveRepo.GetLibraryAccess(upsertRequest.Library.Id, UserId);
                    if (libraryAccess.LibraryExists == upsertRequest.IsNewLibrary)
                    {
                        throw new Exception("Invalid operation occurred.");
                    }
                    var curves = new List<PerformanceCurveDTO>();
                    if (upsertRequest.PagingSync.LibraryId != null)
                        curves = _performanceCurvesService.GetSyncedLibraryDataset(upsertRequest.PagingSync.LibraryId.Value, upsertRequest.PagingSync);
                    else if (!upsertRequest.IsNewLibrary)
                        curves = _performanceCurvesService.GetSyncedLibraryDataset(upsertRequest.Library.Id, upsertRequest.PagingSync);
                    if (upsertRequest.PagingSync.LibraryId != null && upsertRequest.PagingSync.LibraryId != upsertRequest.Library.Id)
                        curves.ForEach(curve => curve.Id = Guid.NewGuid());
                    var dto = upsertRequest.Library;
                    if (dto != null)
                    {
                        _claimHelper.CheckUserLibraryModifyAuthorization(libraryAccess, UserId);
                        dto.PerformanceCurves = curves;
                    }
                    UnitOfWork.PerformanceCurveRepo.UpsertPerformanceCurveLibrary(dto);
                    UnitOfWork.PerformanceCurveRepo.UpsertOrDeletePerformanceCurves(dto.PerformanceCurves, dto.Id);
                    if (upsertRequest.IsNewLibrary)
                    {
                        var users = LibraryUserDtolists.OwnerAccess(UserId);
                        UnitOfWork.PerformanceCurveRepo.UpsertOrDeleteUsers(dto.Id, users);
                    }
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::UpsertPerformanceCurveLibrary - {HubService.errorList["Unauthorized"]}");
                return Ok();
            }
            catch (Exception)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::UpsertPerformanceCurveLibrary - {HubService.errorList["Exception"]}");
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
                    var dtos = _performanceCurvesService.GetSyncedScenarioDataset(simulationId, pagingSync);                   
                    _claimHelper.CheckUserSimulationModifyAuthorization(simulationId, UserId);
                    UnitOfWork.PerformanceCurveRepo.UpsertOrDeleteScenarioPerformanceCurves(dtos, simulationId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::UpsertScenarioPerformanceCurves - {HubService.errorList["Unauthorized"]}");
                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::UpsertScenarioPerformanceCurves - {HubService.errorList["Exception"]}");
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

                    var access = UnitOfWork.PerformanceCurveRepo.GetLibraryAccess(libraryId, UserId);
                    _claimHelper.CheckUserLibraryDeleteAuthorization(access, UserId);

                    UnitOfWork.PerformanceCurveRepo.DeletePerformanceCurveLibrary(libraryId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::DeletePerformanceCurveLibrary - {HubService.errorList["Unauthorized"]}");
                return Ok();
            }
            catch (Exception)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::DeletePerformanceCurveLibrary - {HubService.errorList["Exception"]}");
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
                            var accessModel = UnitOfWork.PerformanceCurveRepo.GetLibraryAccess(performanceCurveLibraryId, UserId);
                            _claimHelper.CheckUserLibraryRecreateAuthorization(accessModel, UserId);
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
                return Ok();
            }
            catch (Exception)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::ImportLibraryPerformanceCurvesExcelFile - {HubService.errorList["Exception"]}");
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
                return Ok();
            }
            catch (Exception)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::ImportScenarioPerformanceCurvesExcelFile - {HubService.errorList["Exception"]}");
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::ExportScenarioPerformanceCurvesExcelFile - {HubService.errorList["Unauthorized"]}");
                return Unauthorized();
            }
            catch (Exception)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::ExportScenarioPerformanceCurvesExcelFile - {HubService.errorList["Exception"]}");
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
                return Unauthorized();
            }
            catch (Exception)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::ExportLibraryPerformanceCurvesExcelFile - {HubService.errorList["Exception"]}");
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
                return Unauthorized();
            }
            catch (Exception)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::DownloadPerformanceCurvesTemplate - {HubService.errorList["Exception"]}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetPerformanceCurveLibraryUsers/{libraryId}")]
        [Authorize(Policy=Policy.ViewPerformanceCurveFromLibrary)]
        public async Task<IActionResult> GetPerformanceCurveLibraryUsers(Guid libraryId)
        {
            try
            {
                List<LibraryUserDTO> users = new List<LibraryUserDTO>();
                await Task.Factory.StartNew(() =>
                {
                    var accessModel = UnitOfWork.PerformanceCurveRepo.GetLibraryAccess(libraryId, UserId);
                    _claimHelper.CheckGetLibraryUsersValidity(accessModel, UserId);
                    users = UnitOfWork.PerformanceCurveRepo.GetLibraryUsers(libraryId);
                });
                return Ok(users);
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::GetPerformanceCurveLibraryUsers - {HubService.errorList["Unauthorized"]}");
                return Ok();
            }
            catch (Exception)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::GetPerformanceCurveLibraryUsers - {HubService.errorList["Exception"]}");
                throw;
            }
        }
        [HttpPost]
        [Route("UpsertOrDeletePerformanceCurveLibraryUsers/{libraryId}")]
        [Authorize(Policy=Policy.ModifyOrDeletePerformanceCurveFromLibrary)]
        public async Task<IActionResult> UpsertOrDeletePerformanceCurveLibraryUsers(Guid libraryId, List<LibraryUserDTO> proposedUsers)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    var libraryUsers = UnitOfWork.PerformanceCurveRepo.GetLibraryUsers(libraryId);
                    _claimHelper.CheckAccessModifyValidity(libraryUsers, proposedUsers, UserId);
                    UnitOfWork.PerformanceCurveRepo.UpsertOrDeleteUsers(libraryId, proposedUsers);
                });
                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::UpsertOrDeletePerformanceCurveLibraryUsers - {HubService.errorList["Unauthorized"]}");
                return Ok();
            }
            catch (InvalidOperationException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::UpsertOrDeletePerformanceCurveLibraryUsers - {HubService.errorList["InvalidOperationException"]}");
                return BadRequest();
            }
            catch (Exception)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::UpsertOrDeletePerformanceCurveLibraryUsers - {HubService.errorList["Exception"]}");
                throw;
            }
        }
        [HttpGet]
        [Route("GetIsSharedLibrary/{performanceCurveLibraryId}")]
        [Authorize]
        public async Task<IActionResult> GetIsSharedLibrary(Guid performanceCurveLibraryId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    var users = UnitOfWork.PerformanceCurveRepo.GetLibraryUsers(performanceCurveLibraryId);
                    if (users.Count > 0)
                    {
                        return new JsonResult(true);
                    }
                    else
                    {
                        return new JsonResult(false);
                    }
                });
                return Ok();
            }
            catch (Exception)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{TreatmentError}::GetIsSharedLibrary - {HubService.errorList["Exception"]}");
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
