using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
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
using BridgeCareCore.Models;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerformanceCurveController : BridgeCareCoreBaseController
    {
        private readonly IReadOnlyDictionary<string, PerformanceCurvesCRUDMethods> _performanceCRUDMethods;
        private Guid UserId => UnitOfWork.CurrentUser?.Id ?? Guid.Empty;
        private readonly IPerformanceCurvesService _performanceCurvesService;

        public PerformanceCurveController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork,
            IHubService hubService, IHttpContextAccessor httpContextAccessor, IPerformanceCurvesService performanceCurvesService) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            _performanceCRUDMethods = CreateCRUDMethods();
            _performanceCurvesService = performanceCurvesService ??
                                       throw new ArgumentNullException(nameof(performanceCurvesService));
        }
        private Dictionary<string, PerformanceCurvesCRUDMethods> CreateCRUDMethods()
        {
            void UpsertAnyForScenario(Guid simulationId, List<PerformanceCurveDTO> dtos)
            {
                UnitOfWork.PerformanceCurveRepo.UpsertOrDeleteScenarioPerformanceCurves(dtos, simulationId);
            }

            void UpsertPermittedForScenario(Guid simulationId, List<PerformanceCurveDTO> dtos)
            {
                CheckUserSimulationModifyAuthorization(simulationId);
                UpsertAnyForScenario(simulationId, dtos);
            }

            List<PerformanceCurveDTO> RetrieveAnyForScenario(Guid simulationId) =>
                UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);

            void DeleteAnyFromScenario(Guid simulationId, List<PerformanceCurveDTO> dtos)
            {
                // Do Nothing
            }

            List<PerformanceCurveLibraryDTO> RetrieveAnyForLibraries() =>
                UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrariesNoPerformanceCurves();

            List<PerformanceCurveLibraryDTO> RetrievePermittedForLibraries()
            {
                var result = UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrariesNoPerformanceCurves();
                return result.Where(_ => _.Owner == UserId || _.IsShared == true).ToList();
            }

            void UpsertAnyForLibrary(PerformanceCurveLibraryDTO dto)
            {
                UnitOfWork.PerformanceCurveRepo.UpsertPerformanceCurveLibrary(dto);
                UnitOfWork.PerformanceCurveRepo.UpsertOrDeletePerformanceCurves(dto.PerformanceCurves, dto.Id);
            }

            void UpsertPermittedForLibrary(PerformanceCurveLibraryDTO dto)
            {
                var currentRecord = UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibraries().FirstOrDefault(_ => _.Id == dto.Id);
                if (currentRecord?.Owner == UserId || currentRecord == null)
                {
                    UpsertAnyForLibrary(dto);
                }
                else
                {
                    throw new UnauthorizedAccessException("You are not authorized to modify this library's data.");
                }
            }

            void DeleteAnyForLibrary(Guid libraryId) =>
                UnitOfWork.PerformanceCurveRepo.DeletePerformanceCurveLibrary(libraryId);

            void DeletePermittedForLibrary(Guid libraryId)
            {
                var dto = UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibraries().FirstOrDefault(_ => _.Id == libraryId);

                if (dto == null) return; // Mimic existing code that does not inform the user the library ID does not exist

                if (dto.Owner == UserId)
                {
                    DeleteAnyForLibrary(libraryId);
                }
                else
                {
                    throw new UnauthorizedAccessException("You are not authorized to modify this library's data.");
                }
            }

            ScenarioPerformanceCurvesImportResultDTO UpsertAnyForImportScenarioPerformanceCurves(ExcelPackage excelPackage, Guid simulationId, UserCriteriaDTO currentUserCriteriaFilter)
            {
                return _performanceCurvesService.ImportScenarioPerformanceCurvesFile(simulationId, excelPackage, currentUserCriteriaFilter);
            }

            ScenarioPerformanceCurvesImportResultDTO UpsertPermittedForImportScenarioPerformanceCurves(ExcelPackage excelPackage, Guid simulationId, UserCriteriaDTO currentUserCriteriaFilter)
            {
                CheckUserSimulationModifyAuthorization(simulationId);
                return UpsertAnyForImportScenarioPerformanceCurves(excelPackage, simulationId, currentUserCriteriaFilter);
            }

            PerformanceCurvesImportResultDTO UpsertAnyForImportLibraryPerformanceCurves(ExcelPackage excelPackage, Guid performanceCurveLibraryId, UserCriteriaDTO currentUserCriteriaFilter)
            {
                return _performanceCurvesService.ImportLibraryPerformanceCurvesFile(performanceCurveLibraryId, excelPackage, currentUserCriteriaFilter);
            }

            PerformanceCurvesImportResultDTO UpsertPermittedForImportLibraryPerformanceCurves(ExcelPackage excelPackage, Guid performanceCurveLibraryId, UserCriteriaDTO currentUserCriteriaFilter)
            {
                var existingPerformanceCurveLibrary = UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(performanceCurveLibraryId);
                if (existingPerformanceCurveLibrary == null && existingPerformanceCurveLibrary.Owner != UserId)
                {
                    throw new UnauthorizedAccessException("You are not authorized to modify this library's data.");
                }
                return UpsertAnyForImportLibraryPerformanceCurves(excelPackage, performanceCurveLibraryId, currentUserCriteriaFilter);
            }

            var AdminCRUDMethods = new PerformanceCurvesCRUDMethods()
            {
                UpsertScenario = UpsertAnyForScenario,
                RetrieveScenario = RetrieveAnyForScenario,
                DeleteScenario = DeleteAnyFromScenario,
                UpsertLibrary = UpsertAnyForLibrary,
                RetrieveLibrary = RetrieveAnyForLibraries,
                DeleteLibrary = DeleteAnyForLibrary,
                UpsertImportScenarioPerformanceCurves = UpsertAnyForImportScenarioPerformanceCurves,
                UpsertImportLibraryPerformanceCurves = UpsertAnyForImportLibraryPerformanceCurves
            };

            var PermittedCRUDMethods = new PerformanceCurvesCRUDMethods()
            {
                UpsertScenario = UpsertPermittedForScenario,
                RetrieveScenario = RetrieveAnyForScenario,
                DeleteScenario = DeleteAnyFromScenario,
                UpsertLibrary = UpsertPermittedForLibrary,
                RetrieveLibrary = RetrievePermittedForLibraries,
                DeleteLibrary = DeletePermittedForLibrary,
                UpsertImportScenarioPerformanceCurves = UpsertPermittedForImportScenarioPerformanceCurves,
                UpsertImportLibraryPerformanceCurves = UpsertAnyForImportLibraryPerformanceCurves
            };

            return new Dictionary<string, PerformanceCurvesCRUDMethods>
            {
                [Role.Administrator] = AdminCRUDMethods,
                [Role.DistrictEngineer] = PermittedCRUDMethods,
                [Role.Cwopa] = PermittedCRUDMethods,
                [Role.PlanningPartner] = PermittedCRUDMethods
            };
        }

        [HttpGet]
        [Route("GetPerformanceCurveLibraries")]
        [Authorize]
        public async Task<IActionResult> GetPerformanceCurveLibraries()
        {
            try
            {
                var result = await Task.Factory.StartNew(() =>
                _performanceCRUDMethods[UserInfo.Role].RetrieveLibrary()
                );
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Deterioration model error::{e.Message}");
                throw;
            }
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Deterioration model error::{e.Message}");
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Deterioration model error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertPerformanceCurveLibraryPage")]
        [Authorize]
        public async Task<IActionResult> UpsertPerformanceCurveLibraryPage(LibraryUpsertPagingRequestModel<PerformanceCurveLibraryDTO, PerformanceCurveDTO> upsertRequest)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    var curves = new List<PerformanceCurveDTO>();
                    if (upsertRequest.PagingSync.LibraryId != null)
                        curves = _performanceCurvesService.GetSyncedLibraryDataset(upsertRequest.PagingSync.LibraryId.Value, upsertRequest.PagingSync);
                    if (upsertRequest.PagingSync.LibraryId != upsertRequest.Library.Id)
                        curves.ForEach(curve => curve.Id = Guid.NewGuid());
                    var dto = upsertRequest.Library;
                    dto.PerformanceCurves = curves;
                    _performanceCRUDMethods[UserInfo.Role].UpsertLibrary(dto);
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
        [Route("UpsertScenarioPerformanceCurvesPage/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> UpsertScenarioPerformanceCurvesPage(Guid simulationId, PagingSyncModel<PerformanceCurveDTO> pagingSync)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    var dtos = _performanceCurvesService.GetSyncedScenarioDataset(simulationId, pagingSync);
                    _performanceCRUDMethods[UserInfo.Role].UpsertScenario(simulationId, dtos);
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
                    _performanceCRUDMethods[UserInfo.Role].DeleteLibrary(libraryId);
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

                var result = await Task.Factory.StartNew(() =>
                {
                    return _performanceCRUDMethods[UserInfo.Role].UpsertImportLibraryPerformanceCurves(excelPackage, performanceCurveLibraryId, currentUserCriteriaFilter);
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

                var result = await Task.Factory.StartNew(() =>
                    _performanceCRUDMethods[UserInfo.Role].UpsertImportScenarioPerformanceCurves(excelPackage, simulationId, currentUserCriteriaFilter));

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
    }

    internal class PerformanceCurvesCRUDMethods : CRUDMethods<PerformanceCurveDTO, PerformanceCurveLibraryDTO>
    {
        public Func<ExcelPackage, Guid, UserCriteriaDTO, ScenarioPerformanceCurvesImportResultDTO> UpsertImportScenarioPerformanceCurves { get; set; }

        public Func<ExcelPackage, Guid, UserCriteriaDTO, PerformanceCurvesImportResultDTO> UpsertImportLibraryPerformanceCurves { get; set; }
    }
}
