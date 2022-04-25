﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Controllers.BaseController;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using OfficeOpenXml;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerformanceCurveController : BridgeCareCoreBaseController
    {
        private readonly IReadOnlyDictionary<string, PerformanceCurvesCRUDMethods> _performanceCRUDMethods;
        private Guid UserId => UnitOfWork.UserEntity?.Id ?? Guid.Empty;
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
                UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibraries();

            List<PerformanceCurveLibraryDTO> RetrievePermittedForLibraries()
            {
                var result = UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibraries();
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
                var result = await Task.Factory.StartNew(() => _performanceCRUDMethods[UserInfo.Role].RetrieveLibrary());
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Performance Curve error::{e.Message}");
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
                var result = await Task.Factory.StartNew(() => _performanceCRUDMethods[UserInfo.Role].RetrieveScenario(simulationId));
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Performance Curve error::{e.Message}");
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Performance Curve error::{e.Message}");
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Performance Curve error::{e.Message}");
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Performance Curve error::{e.Message}");
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Performance curves error::{e.Message}");
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Performance curves error::{e.Message}");
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
