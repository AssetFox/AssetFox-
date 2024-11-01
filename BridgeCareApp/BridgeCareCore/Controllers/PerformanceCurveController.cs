using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
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
using BridgeCareCore.Services.General_Work_Queue.WorkItems;
using BridgeCareCore.Utils.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph.Models;
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
        private readonly IGeneralWorkQueueService _generalWorkQueueService;

        public const string RequestedToModifyNonexistentLibraryErrorMessage = "The request says to modify a library, but the library does not exist.";
        public const string RequestedToCreateExistingLibraryErrorMessage = "The request says to create a new library, but the library already exists.";

        public PerformanceCurveController(IEsecSecurity esecSecurity, IUnitOfWork unitOfWork,
            IHubService hubService,
            IHttpContextAccessor httpContextAccessor,
            IPerformanceCurvesService performanceCurvesService, IPerformanceCurvesPagingService performanceCurvesPagingService,
            IClaimHelper claimHelper, IGeneralWorkQueueService generalWorkQueueService) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            _performanceCurvesService = performanceCurvesService ?? throw new ArgumentNullException(nameof(performanceCurvesService));
            _performanceCurvePagingService = performanceCurvesPagingService ?? throw new ArgumentNullException(nameof(performanceCurvesPagingService));
            _claimHelper = claimHelper ?? throw new ArgumentNullException(nameof(claimHelper));
            _generalWorkQueueService = generalWorkQueueService ?? throw new ArgumentNullException(nameof(generalWorkQueueService));
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
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeteriorationModelError}::GetScenarioPerformanceCurvePage for {simulationName} - {e.Message}", e);
            }
            return Ok();
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
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeteriorationModelError}::GetLibraryPerformanceCurvePage - {e.Message}", e);
            }
            return Ok();
        }

        [HttpGet]
        [Route("GetPerformanceLibraryModifiedDate/{libraryId}")]
        [Authorize(Policy = Policy.ModifyInvestmentFromLibrary)]
        public async Task<IActionResult> GetBudgetLibraryDate(Guid libraryId)
        {
            try
            {
                var users = new DateTime();
                await Task.Factory.StartNew(() =>
                {
                    users = UnitOfWork.PerformanceCurveRepo.GetLibraryModifiedDate(libraryId);
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
                        result = UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrariesNoChildrenAccessibleToUser(UserId);
                    }
                });

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeteriorationModelError}::GetPerformanceCurveLibraries - {e.Message}", e);
            }
            return Ok();
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
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeteriorationModelError}::GetScenarioPerformanceCurves for {simulationName} - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeteriorationModelError}::GetScenarioPerformanceCurves for {simulationName} - {e.Message}", e);
            }
            return Ok();
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
                    var libraryAccess = UnitOfWork.PerformanceCurveRepo.GetLibraryAccess(upsertRequest.Library.Id, UserId);
                    if (libraryAccess.LibraryExists == upsertRequest.IsNewLibrary)
                    {
                        var errorMessage = libraryAccess.LibraryExists ? RequestedToCreateExistingLibraryErrorMessage : RequestedToModifyNonexistentLibraryErrorMessage;
                        throw new InvalidOperationException(errorMessage);
                    }
                    var curves = _performanceCurvePagingService.GetSyncedLibraryDataset(upsertRequest);
                    var dto = upsertRequest.Library;
                    if (dto != null)
                    {
                        _claimHelper.CheckUserLibraryModifyAuthorization(libraryAccess, UserId);
                        dto.PerformanceCurves = curves;
                    }
                    UnitOfWork.PerformanceCurveRepo.UpsertOrDeletePerformanceCurveLibraryAndCurves(dto, upsertRequest.IsNewLibrary, UserId);
                });

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeteriorationModelError}::UpsertPerformanceCurveLibrary - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeteriorationModelError}::UpsertPerformanceCurveLibrary - {e.Message}", e);
            }
            return Ok();
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
                    var dtos = _performanceCurvePagingService.GetSyncedScenarioDataSet(simulationId, pagingSync);                   
                    _claimHelper.CheckUserSimulationModifyAuthorization(simulationId, UserId);
                    PerformanceCurveDtoListService.AddLibraryIdToScenarioPerformanceCurves(dtos, pagingSync.LibraryId);
                    PerformanceCurveDtoListService.AddModifiedToScenarioPerformanceCurve(dtos, pagingSync.IsModified);
                    UnitOfWork.PerformanceCurveRepo.UpsertOrDeleteScenarioPerformanceCurves(dtos, simulationId);
                });

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeteriorationModelError}::UpsertScenarioPerformanceCurves - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeteriorationModelError}::UpsertScenarioPerformanceCurves - {e.Message}", e);
            }
            return Ok();
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
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        var dto = GetAllPerformanceCurveLibraries().FirstOrDefault(_ => _.Id == libraryId);
                        if (dto == null) return;

                        var access = UnitOfWork.PerformanceCurveRepo.GetLibraryAccess(libraryId, UserId);
                        _claimHelper.CheckUserLibraryDeleteAuthorization(access, UserId);
                    }
                    UnitOfWork.PerformanceCurveRepo.DeletePerformanceCurveLibrary(libraryId);
                });

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeteriorationModelError}::DeletePerformanceCurveLibrary - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeteriorationModelError}::DeletePerformanceCurveLibrary - {e.Message}", e);
            }
            return Ok();
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

                var libraryName = "";
                await Task.Factory.StartNew(() =>
                {
                    var existingPerformanceCurveLibrary = UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(performanceCurveLibraryId);
                    if(existingPerformanceCurveLibrary != null)
                        libraryName = existingPerformanceCurveLibrary.Name;
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        
                        if (existingPerformanceCurveLibrary != null)
                        {
                            var accessModel = UnitOfWork.PerformanceCurveRepo.GetLibraryAccess(performanceCurveLibraryId, UserId);
                            _claimHelper.CheckUserLibraryRecreateAuthorization(accessModel, UserId);
                        }
                    }
                });
                ImportLibraryPerformanceCurveWorkitem workItem = new ImportLibraryPerformanceCurveWorkitem(performanceCurveLibraryId, excelPackage, currentUserCriteriaFilter, UserInfo.Name, libraryName);
                var analysisHandle = _generalWorkQueueService.CreateAndRunInFastQueue(workItem);

                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastFastWorkQueueUpdate, libraryId.ToString());

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeteriorationModelError}::ImportLibraryPerformanceCurvesExcelFile - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeteriorationModelError}::ImportLibraryPerformanceCurvesExcelFile - {e.Message}", e);
            }
            return Ok();
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

                var simulationName = "";
                await Task.Factory.StartNew(() =>
                {
                    _claimHelper.CheckUserSimulationModifyAuthorization(simulationId, UserId);
                    simulationName = UnitOfWork.SimulationRepo.GetSimulationName(simulationId);
                });

                ImportScenarioPerformanceCurveWorkitem workItem = new ImportScenarioPerformanceCurveWorkitem(simulationId, excelPackage, currentUserCriteriaFilter, UserInfo.Name, simulationName);
                var analysisHandle = _generalWorkQueueService.CreateAndRunInFastQueue(workItem);

                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastFastWorkQueueUpdate, simulationId.ToString());

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeteriorationModelError}::ImportScenarioPerformanceCurvesExcelFile - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeteriorationModelError}::ImportScenarioPerformanceCurvesExcelFile - {e.Message}", e);
            }
            return Ok();
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
            catch (UnauthorizedAccessException e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeteriorationModelError}::ExportScenarioPerformanceCurvesExcelFile for {simulationName} - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeteriorationModelError}::ExportScenarioPerformanceCurvesExcelFile for {simulationName} - {e.Message}", e);
            }
            return Ok();
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
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeteriorationModelError}::ExportLibraryPerformanceCurvesExcelFile - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeteriorationModelError}::ExportLibraryPerformanceCurvesExcelFile - {e.Message}", e);
            }
            return Ok();
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
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeteriorationModelError}::DownloadPerformanceCurvesTemplate - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeteriorationModelError}::DownloadPerformanceCurvesTemplate - {e.Message}", e);
            }
            return Ok();
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
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeteriorationModelError}::GetPerformanceCurveLibraryUsers - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeteriorationModelError}::GetPerformanceCurveLibraryUsers - {HubService.errorList["Exception"]}", e);
            }
            return Ok();
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
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeteriorationModelError}::UpsertOrDeletePerformanceCurveLibraryUsers - {HubService.errorList["Unauthorized"]}", e);
            }
            catch (InvalidOperationException e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeteriorationModelError}::UpsertOrDeletePerformanceCurveLibraryUsers - {HubService.errorList["InvalidOperationException"]}", e);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeteriorationModelError}::UpsertOrDeletePerformanceCurveLibraryUsers - {HubService.errorList["Exception"]}", e);
            }
            return Ok();
        }
        [HttpGet]
        [Route("GetIsSharedLibrary/{performanceCurveLibraryId}")]
        [Authorize(Policy = Policy.ViewPerformanceCurveFromLibrary)]
        public async Task<IActionResult> GetIsSharedLibrary(Guid performanceCurveLibraryId)
        {
            bool result = false;
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    var users = UnitOfWork.PerformanceCurveRepo.GetLibraryUsers(performanceCurveLibraryId);
                    if (users.Count<=0)
                    {
                        result = false;
                    } else
                    {
                        result = true;
                    }
                });
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeteriorationModelError}::GetIsSharedLibrary - {HubService.errorList["Exception"]}", e);
            }
            return Ok();
        }

        [HttpGet]
        [Route("GetDistinctScenarioPerformanceFactorAttributeNames")]
        [Authorize(Policy = Policy.ViewPerformanceCurveFromLibrary)]
        public async Task<IActionResult> GetDistinctPerformanceFactorAttributes()
        {
            var result = new List<string>();
            try
            {
                await Task.Factory.StartNew(() => result = UnitOfWork.PerformanceCurveRepo.GetDistinctScenarioPerformanceFactorAttributeNames());
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{DeteriorationModelError}::GetDistinctScenarioPerformanceFactorAttributeNames - {HubService.errorList["Exception"]}", e);
            }
            return Ok();
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
