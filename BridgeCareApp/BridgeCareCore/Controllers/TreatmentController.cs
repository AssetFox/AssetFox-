using System;
using System.Collections.Generic;
using System.Data;
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
using OfficeOpenXml;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TreatmentController : BridgeCareCoreBaseController
    {
        private readonly ITreatmentService _treatmentService;
        private readonly IReadOnlyDictionary<string, CRUDMethods<TreatmentDTO, TreatmentLibraryDTO>> _treatmentCRUDMethods;

        private Guid UserId => UnitOfWork.UserEntity?.Id ?? Guid.Empty;

        public TreatmentController(IEsecSecurity esecSecurity, ITreatmentService treatmentService, UnitOfDataPersistenceWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            _treatmentCRUDMethods = CreateCRUDMethods();
            _treatmentService = treatmentService;
        }

        private Dictionary<string, CRUDMethods<TreatmentDTO, TreatmentLibraryDTO>> CreateCRUDMethods()
        {
            void UpsertAnyForScenario(Guid simulationId, List<TreatmentDTO> dtos)
            {
                UnitOfWork.SelectableTreatmentRepo.UpsertOrDeleteScenarioSelectableTreatment(dtos, simulationId);
            }

            void UpsertPermittedForScenario(Guid simulationId, List<TreatmentDTO> dtos)
            {
                CheckUserSimulationModifyAuthorization(simulationId);
                UpsertAnyForScenario(simulationId, dtos);
            }

            List<TreatmentDTO> RetrieveAnyForScenario(Guid simulationId) =>
                UnitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatments(simulationId);

            void DeleteAnyFromScenario(Guid simulationId, List<TreatmentDTO> dtos)
            {
                // Do Nothing
            }

            List<TreatmentLibraryDTO> RetrieveAnyForLibraries() =>
                UnitOfWork.SelectableTreatmentRepo.GetTreatmentLibraries();

            List<TreatmentLibraryDTO> RetrievePermittedForLibraries()
            {
                var result = UnitOfWork.SelectableTreatmentRepo.GetTreatmentLibraries();
                return result.Where(_ => _.Owner == UserId || _.IsShared == true).ToList();
            }

            void UpsertAnyForLibrary(TreatmentLibraryDTO dto)
            {
                UnitOfWork.SelectableTreatmentRepo.UpsertTreatmentLibrary(dto);
                UnitOfWork.SelectableTreatmentRepo.UpsertOrDeleteTreatments(dto.Treatments, dto.Id);
            }

            void UpsertPermittedForLibrary(TreatmentLibraryDTO dto)
            {
                var currentRecord = UnitOfWork.SelectableTreatmentRepo.GetTreatmentLibraries().FirstOrDefault(_ => _.Id == dto.Id);
                if (currentRecord?.Owner == UserId || currentRecord == null)
                {
                    UpsertAnyForLibrary(dto);
                }
                else
                {
                    throw new UnauthorizedAccessException("You are not authorized to modify this library's data.");
                }
            }

            void DeleteAnyForLibrary(Guid libraryId) => UnitOfWork.SelectableTreatmentRepo.DeleteTreatmentLibrary(libraryId);

            void DeletePermittedForLibrary(Guid libraryId)
            {
                var dto = UnitOfWork.SelectableTreatmentRepo.GetTreatmentLibraries().FirstOrDefault(_ => _.Id == libraryId);

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

            var AdminCRUDMethods = new CRUDMethods<TreatmentDTO, TreatmentLibraryDTO>()
            {
                UpsertScenario = UpsertAnyForScenario,
                RetrieveScenario = RetrieveAnyForScenario,
                DeleteScenario = DeleteAnyFromScenario,
                UpsertLibrary = UpsertAnyForLibrary,
                RetrieveLibrary = RetrieveAnyForLibraries,
                DeleteLibrary = DeleteAnyForLibrary
            };

            var PermittedCRUDMethods = new CRUDMethods<TreatmentDTO, TreatmentLibraryDTO>()
            {
                UpsertScenario = UpsertPermittedForScenario,
                RetrieveScenario = RetrieveAnyForScenario,
                DeleteScenario = DeleteAnyFromScenario,
                UpsertLibrary = UpsertPermittedForLibrary,
                RetrieveLibrary = RetrievePermittedForLibraries,
                DeleteLibrary = DeletePermittedForLibrary
            };

            return new Dictionary<string, CRUDMethods<TreatmentDTO, TreatmentLibraryDTO>>
            {
                [Role.Administrator] = AdminCRUDMethods,
                [Role.DistrictEngineer] = PermittedCRUDMethods,
                [Role.Cwopa] = PermittedCRUDMethods,
                [Role.PlanningPartner] = PermittedCRUDMethods
            };
        }

        [HttpGet]
        [Route("GetTreatmentLibraries")]
        [Authorize]
        public async Task<IActionResult> GetTreatmentLibraries()
        {
            try
            {
                var result = await Task.Factory.StartNew(() => _treatmentCRUDMethods[UserInfo.Role].RetrieveLibrary());
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
                var result = await Task.Factory.StartNew(() => _treatmentCRUDMethods[UserInfo.Role].RetrieveScenario(simulationId));
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
                    _treatmentCRUDMethods[UserInfo.Role].UpsertLibrary(dto);
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
        [Route("ExportScenarioTreatmentsExcelFile/{libraryId}")]
        [Authorize]
        public async Task<IActionResult> ExportScenarioTreatmentsExcelFile(Guid libraryId)
        {
            try
            {
                var result =
                    await Task.Factory.StartNew(() => _treatmentService.GenerateExcelFile(libraryId));

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
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
                    _treatmentCRUDMethods[UserInfo.Role].UpsertScenario(simulationId, dtos);
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
                    _treatmentCRUDMethods[UserInfo.Role].DeleteLibrary(libraryId);
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
                    throw new ConstraintException("Investment budgets file not found.");
                }

                if (!ContextAccessor.HttpContext.Request.Form.TryGetValue("libraryId", out var libraryId))
                {
                    throw new ConstraintException("Request contained no treatment library id.");
                }
                var treatmentLibraryId = Guid.Parse(libraryId.ToString());

                var excelPackage = new ExcelPackage(ContextAccessor.HttpContext.Request.Form.Files[0].OpenReadStream());

                var result = await Task.Factory.StartNew(() =>
                {
                    return _treatmentService.ImportLibraryTreatmentsFile(treatmentLibraryId, excelPackage);
                });
                if (result.WarningMessage != null)
                {
                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastWarning, result.WarningMessage);
                }
                return Ok(result.TreatmentLibrary);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
                throw;
            }
        }
    }
}
