using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using BridgeCareCore.Controllers.BaseController;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Interfaces.DefaultData;
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
    public class InvestmentController : BridgeCareCoreBaseController
    {
        private static IInvestmentBudgetsService _investmentBudgetsService;
        private readonly IReadOnlyDictionary<string, InvestmentCRUDMethods> _investmentCRUDMethods;
        public readonly IInvestmentDefaultDataService _investmentDefaultDataService;

        private Guid UserId => UnitOfWork.CurrentUser?.Id ?? Guid.Empty;

        public InvestmentController(IInvestmentBudgetsService investmentBudgetsService, IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService, IHttpContextAccessor httpContextAccessor, IInvestmentDefaultDataService investmentDefaultDataService) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            _investmentBudgetsService = investmentBudgetsService ??
                                        throw new ArgumentNullException(nameof(investmentBudgetsService));
            _investmentCRUDMethods = CreateCRUDMethods();
            _investmentDefaultDataService = investmentDefaultDataService ?? throw new ArgumentNullException(nameof(investmentDefaultDataService));
        }

        private Dictionary<string, InvestmentCRUDMethods> CreateCRUDMethods()
        {
            void UpsertAnyForScenario(Guid simulationId, InvestmentDTO data)
            {
                UnitOfWork.BudgetRepo.UpsertOrDeleteScenarioBudgets(data.ScenarioBudgets, simulationId);
                UnitOfWork.InvestmentPlanRepo.UpsertInvestmentPlan(data.InvestmentPlan, simulationId);
            }

            void UpsertPermittedForScenario(Guid simulationId, InvestmentDTO data)
            {
                CheckUserSimulationModifyAuthorization(simulationId);
                UpsertAnyForScenario(simulationId, data);
            }

            void UpsertAnyForLibrary(BudgetLibraryDTO dto)
            {
                UnitOfWork.BudgetRepo.UpsertBudgetLibrary(dto);
                UnitOfWork.BudgetRepo.UpsertOrDeleteBudgets(dto.Budgets, dto.Id);
            }

            void UpsertPermittedForLibrary(BudgetLibraryDTO dto)
            {
                if (dto.Owner == UserId)
                {
                    UpsertAnyForLibrary(dto);
                }
                else
                {
                    throw new UnauthorizedAccessException("You are not authorized to modify this simulation's data.");
                }
            }

            ScenarioBudgetImportResultDTO ImportAnyForScenario(bool overwriteBudgets, ExcelPackage excelPackage, Guid simulationId, UserCriteriaDTO currentUserCriteriaFilter)
            {
                return _investmentBudgetsService.ImportScenarioInvestmentBudgetsFile(simulationId, excelPackage, currentUserCriteriaFilter, overwriteBudgets);
            }

            ScenarioBudgetImportResultDTO ImportPermittedForScenario(bool overwriteBudgets, ExcelPackage excelPackage, Guid simulationId, UserCriteriaDTO currentUserCriteriaFilter)
            {
                CheckUserSimulationModifyAuthorization(simulationId);
                return ImportAnyForScenario(overwriteBudgets, excelPackage, simulationId, currentUserCriteriaFilter);
            }

            BudgetImportResultDTO ImportAnyForLibrary(bool overwriteBudgets, ExcelPackage excelPackage, Guid libraryId, UserCriteriaDTO currentUserCriteriaFilter) =>
                _investmentBudgetsService.ImportLibraryInvestmentBudgetsFile(libraryId, excelPackage, currentUserCriteriaFilter, overwriteBudgets);

            BudgetImportResultDTO ImportPermittedForLibrary(bool overwriteBudgets, ExcelPackage excelPackage, Guid libraryId, UserCriteriaDTO currentUserCriteriaFilter)
            {
                // If the budget already exists, do not allow the user to overwrite it if they are not the owneer
                var existingBudgetLibrary = UnitOfWork.BudgetRepo.GetBudgetLibraries().FirstOrDefault(_ => _.Id == libraryId);
                if (existingBudgetLibrary != null)
                {
                    if (existingBudgetLibrary.Owner != UserId)
                    {
                        // The budget exists AND its owner is not the user.  This is the only time importing is not valid
                        throw new UnauthorizedAccessException("You are not authorized to modify this library's data.");
                    }
                }

                // This can be imported
                return ImportAnyForLibrary(overwriteBudgets, excelPackage, libraryId, currentUserCriteriaFilter);
            }

            void DeleteAnyFromScenario(Guid scenarioId, InvestmentDTO dto)
            {
                // Do Nothing
            }

            void DeleteAnyFromLibrary(Guid libraryId) => UnitOfWork.BudgetRepo.DeleteBudgetLibrary(libraryId);

            void DeletePermittedFromLibrary(Guid libraryId)
            {
                var budgetLibrary = UnitOfWork.BudgetRepo.GetBudgetLibraries().FirstOrDefault(_ => _.Id == libraryId);

                if (budgetLibrary == null) return; // Mimic existing code that does not inform the user the library ID does not exist

                if (budgetLibrary.Owner == UserId)
                {
                    DeleteAnyFromLibrary(libraryId);
                }
                else
                {
                    throw new UnauthorizedAccessException("You are not authorized to modify this library's data.");
                }
            }

            InvestmentDTO GetAnyForScenario(Guid scenarioId)
            {
                var budgets = UnitOfWork.BudgetRepo.GetScenarioBudgets(scenarioId);

                var scenarioInvestmentPlan = UnitOfWork.InvestmentPlanRepo.GetInvestmentPlan(scenarioId);
                if (scenarioInvestmentPlan.Id == Guid.Empty)
                {
                    var investmentDefaultData = _investmentDefaultDataService.GetInvestmentDefaultData().Result;
                    scenarioInvestmentPlan.MinimumProjectCostLimit = investmentDefaultData.MinimumProjectCostLimit;
                    scenarioInvestmentPlan.InflationRatePercentage = investmentDefaultData.InflationRatePercentage;
                }

                return new InvestmentDTO
                {
                    ScenarioBudgets = budgets,
                    InvestmentPlan = scenarioInvestmentPlan
                };
            }

            List<BudgetLibraryDTO> GetAnyForLibrary() => UnitOfWork.BudgetRepo.GetBudgetLibraries();

            List<BudgetLibraryDTO> GetPermittedForLibrary()
            {
                var result = UnitOfWork.BudgetRepo.GetBudgetLibraries();
                return result.Where(_ => _.Owner == UserId || _.IsShared == true).ToList();
            }

            var AdminCRUDMethods = new InvestmentCRUDMethods()
            {
                UpsertScenario = UpsertAnyForScenario,
                RetrieveScenario = GetAnyForScenario,
                DeleteScenario = DeleteAnyFromScenario,
                UpsertLibrary = UpsertAnyForLibrary,
                RetrieveLibrary = GetAnyForLibrary,
                DeleteLibrary = DeleteAnyFromLibrary,
                ImportScenarioInvestment = ImportAnyForScenario,
                ImportLibraryInvestment = ImportAnyForLibrary
            };

            var PermittedCRUDMethods = new InvestmentCRUDMethods()
            {
                UpsertScenario = UpsertPermittedForScenario,
                RetrieveScenario = GetAnyForScenario,
                DeleteScenario = DeleteAnyFromScenario,
                UpsertLibrary = UpsertPermittedForLibrary,
                RetrieveLibrary = GetAnyForLibrary,
                DeleteLibrary = DeletePermittedFromLibrary,
                ImportScenarioInvestment = ImportPermittedForScenario,
                ImportLibraryInvestment = ImportPermittedForLibrary
            };

            return new Dictionary<string, InvestmentCRUDMethods>()
            {
                [Role.Administrator] = AdminCRUDMethods,
                [Role.DistrictEngineer] = PermittedCRUDMethods,
                [Role.Cwopa] = PermittedCRUDMethods,
                [Role.PlanningPartner] = PermittedCRUDMethods
            };
        }

        [HttpGet]
        [Route("GetInvestment/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> GetInvestment(Guid simulationId)
        {
            try
            {
                var result = await Task.Factory.StartNew(() => _investmentCRUDMethods[UserInfo.Role].RetrieveScenario(simulationId));

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertInvestment/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> UpsertInvestment(Guid simulationId, [FromBody] InvestmentDTO data)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    _investmentCRUDMethods[UserInfo.Role].UpsertScenario(simulationId, data);
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetBudgetLibraries")]
        [Authorize]
        public async Task<IActionResult> GetBudgetLibraries()
        {
            try
            {
                var result = await Task.Factory.StartNew(() => _investmentCRUDMethods[UserInfo.Role].RetrieveLibrary());

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertBudgetLibrary")]
        [Authorize]
        public async Task<IActionResult> UpsertBudgetLibrary([FromBody] BudgetLibraryDTO data)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    _investmentCRUDMethods[UserInfo.Role].UpsertLibrary(data);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
                throw;
            }
        }

        [HttpDelete]
        [Route("DeleteBudgetLibrary/{libraryId}")]
        [Authorize]
        public async Task<IActionResult> DeleteBudgetLibrary(Guid libraryId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    _investmentCRUDMethods[UserInfo.Role].DeleteLibrary(libraryId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetScenarioSimpleBudgetDetails/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> GetScenarioSimpleBudgetDetails(Guid simulationId)
        {
            try
            {
                var result = await Task.Factory.StartNew(() => UnitOfWork.BudgetRepo
                    .GetScenarioSimpleBudgetDetails(simulationId));
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("ImportLibraryInvestmentBudgetsExcelFile")]
        [Authorize]
        public async Task<IActionResult> ImportLibraryInvestmentBudgetsExcelFile()
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
                    throw new ConstraintException("Request contained no budget library id.");
                }

                var budgetLibraryId = Guid.Parse(libraryId.ToString());

                var excelPackage = new ExcelPackage(ContextAccessor.HttpContext.Request.Form.Files[0].OpenReadStream());

                var overwriteBudgets = false;
                if (ContextAccessor.HttpContext.Request.Form.ContainsKey("overwriteBudgets"))
                {
                    overwriteBudgets = ContextAccessor.HttpContext.Request.Form["overwriteBudgets"].ToString() == "1";
                }

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
                    return _investmentCRUDMethods[UserInfo.Role].ImportLibraryInvestment(overwriteBudgets, excelPackage, budgetLibraryId, currentUserCriteriaFilter);
                });

                if (result.WarningMessage != null)
                {
                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastWarning, result.WarningMessage);
                }
                return Ok(result.BudgetLibrary);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("ImportScenarioInvestmentBudgetsExcelFile")]
        [Authorize]
        public async Task<IActionResult> ImportScenarioInvestmentBudgetsExcelFile()
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

                if (!ContextAccessor.HttpContext.Request.Form.TryGetValue("simulationId", out var id))
                {
                    throw new ConstraintException("Request contained no simulation id.");
                }

                var simulationId = Guid.Parse(id.ToString());

                var excelPackage = new ExcelPackage(ContextAccessor.HttpContext.Request.Form.Files[0].OpenReadStream());

                var overwriteBudgets = false;
                if (ContextAccessor.HttpContext.Request.Form.ContainsKey("overwriteBudgets"))
                {
                    overwriteBudgets = ContextAccessor.HttpContext.Request.Form["overwriteBudgets"].ToString() == "1";
                }

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
                    _investmentCRUDMethods[UserInfo.Role].ImportScenarioInvestment(overwriteBudgets, excelPackage, simulationId, currentUserCriteriaFilter));

                if (result.WarningMessage != null)
                {
                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastWarning, result.WarningMessage);
                }
                return Ok(result.Budgets);
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

        [HttpGet]
        [Route("ExportScenarioInvestmentBudgetsExcelFile/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> ExportScenarioInvestmentBudgetsExcelFile(Guid simulationId)
        {
            try
            {
                var result =
                    await Task.Factory.StartNew(() => _investmentBudgetsService.ExportScenarioInvestmentBudgetsFile(simulationId));

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

        [HttpGet]
        [Route("ExportLibraryInvestmentBudgetsExcelFile/{budgetLibraryId}")]
        [Authorize]
        public async Task<IActionResult> ExportLibraryInvestmentBudgetsExcelFile(Guid budgetLibraryId)
        {
            try
            {
                var result =
                    await Task.Factory.StartNew(() => _investmentBudgetsService.ExportLibraryInvestmentBudgetsFile(budgetLibraryId));

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

        [HttpGet]
        [Route("DownloadInvestmentBudgetsTemplate")]
        [Authorize]
        public async Task<IActionResult> DownloadInvestmentBudgetsTemplate()
        {
            try
            {
                var filePath = AppDomain.CurrentDomain.BaseDirectory + "DownloadTemplates\\Investment_budgets_template.xlsx";
                var fileData = System.IO.File.ReadAllBytes(filePath);
                var result = await Task.Factory.StartNew(() => new FileInfoDTO
                {
                    FileName = "Investment_budgets_template",
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
                throw;
            }
        }
    }

    internal class InvestmentCRUDMethods
    {
        public Action<Guid, InvestmentDTO> UpsertScenario { get; set; }
        public Func<Guid, InvestmentDTO> RetrieveScenario { get; set; }
        public Action<Guid, InvestmentDTO> DeleteScenario { get; set; }
        public Action<BudgetLibraryDTO> UpsertLibrary { get; set; }
        public Func<List<BudgetLibraryDTO>> RetrieveLibrary { get; set; }
        public Action<Guid> DeleteLibrary { get; set; }
        public Func<bool, ExcelPackage, Guid, UserCriteriaDTO, ScenarioBudgetImportResultDTO> ImportScenarioInvestment { get; set; }
        public Func<bool, ExcelPackage, Guid, UserCriteriaDTO, BudgetImportResultDTO> ImportLibraryInvestment { get; set; }
    }
}
