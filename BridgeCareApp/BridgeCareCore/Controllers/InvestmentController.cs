using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Controllers.BaseController;
using BridgeCareCore.Hubs;
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
    using InvestmentUpsertMethod = Action<Guid, InvestmentDTO>;
    using InvestmentImportMethod = Func<bool, ExcelPackage, Guid, List<BudgetDTO>>;


    [Route("api/[controller]")]
    [ApiController]
    public class InvestmentController : BridgeCareCoreBaseController
    {
        private static IInvestmentBudgetsService _investmentBudgetsService;
        private readonly IReadOnlyDictionary<string, InvestmentUpsertMethod> _investmentUpsertMethods;
        private readonly IReadOnlyDictionary<string, InvestmentImportMethod> _investmentImportMethods;
        public readonly IInvestmentDefaultDataService _investmentDefaultDataService;

        public InvestmentController(IInvestmentBudgetsService investmentBudgetsService, IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService, IHttpContextAccessor httpContextAccessor, IInvestmentDefaultDataService investmentDefaultDataService) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            _investmentBudgetsService = investmentBudgetsService ??
                                        throw new ArgumentNullException(nameof(investmentBudgetsService));
            _investmentUpsertMethods = CreateUpsertMethods();
            _investmentImportMethods = CreateImportMethods();
            _investmentDefaultDataService = investmentDefaultDataService ?? throw new ArgumentNullException(nameof(investmentDefaultDataService));
        }

        private Dictionary<string, InvestmentUpsertMethod> CreateUpsertMethods()
        {
            void UpsertAny(Guid simulationId, InvestmentDTO data)
            {
                UnitOfWork.BudgetRepo.UpsertOrDeleteScenarioBudgets(data.ScenarioBudgets, simulationId);
                UnitOfWork.InvestmentPlanRepo.UpsertInvestmentPlan(data.InvestmentPlan, simulationId);
            }

            void UpsertPermitted(Guid simulationId, InvestmentDTO data)
            {
                CheckUserSimulationModifyAuthorization(simulationId);
                UpsertAny(simulationId, data);
            }

            return new Dictionary<string, InvestmentUpsertMethod>
            {
                [Role.Administrator] = UpsertAny,
                [Role.DistrictEngineer] = UpsertPermitted,
                [Role.Cwopa] = UpsertPermitted,
                [Role.PlanningPartner] = UpsertPermitted
            };
        }

        private Dictionary<string, InvestmentImportMethod> CreateImportMethods()
        {
            List<BudgetDTO> UpsertAny(bool overwriteBudgets, ExcelPackage excelPackage, Guid simulationId)
            {
                if (overwriteBudgets)
                {
                    UnitOfWork.Context.DeleteAll<ScenarioBudgetEntity>(_ => _.SimulationId == simulationId);
                }
                return _investmentBudgetsService.ImportScenarioInvestmentBudgetsFile(simulationId, excelPackage);
            }

            List<BudgetDTO> UpsertPermitted(bool overwriteBudgets, ExcelPackage excelPackage, Guid simulationId)
            {
                CheckUserSimulationModifyAuthorization(simulationId);
                return UpsertAny(overwriteBudgets, excelPackage, simulationId);
            }

            return new Dictionary<string, InvestmentImportMethod>
            {
                [Role.Administrator] = UpsertAny,
                [Role.DistrictEngineer] = UpsertPermitted,
                [Role.Cwopa] = UpsertPermitted,
                [Role.PlanningPartner] = UpsertPermitted
            };
        }

        [HttpGet]
        [Route("GetInvestment/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> GetInvestment(Guid simulationId)
        {
            try
            {
                var result = await Task.Factory.StartNew(() =>
                {
                    var budgets = UnitOfWork.BudgetRepo
                        .GetScenarioBudgets(simulationId);

                    var scenarioInvestmentPlan = UnitOfWork.InvestmentPlanRepo
                        .GetInvestmentPlan(simulationId);
                    SetDefaultDataForNewScenario(scenarioInvestmentPlan);
                    return new InvestmentDTO
                    {
                        ScenarioBudgets = budgets, InvestmentPlan = scenarioInvestmentPlan
                    };
                });


                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
                throw;
            }
        }

        private void SetDefaultDataForNewScenario(InvestmentPlanDTO scenarioInvestmentPlan)
        {
            if (scenarioInvestmentPlan.Id == Guid.Empty)
            {
                var investmentDefaultData = _investmentDefaultDataService.GetInvestmentDefaultData().Result;
                scenarioInvestmentPlan.MinimumProjectCostLimit = investmentDefaultData.MinimumProjectCostLimit;
                scenarioInvestmentPlan.InflationRatePercentage = investmentDefaultData.InflationRatePercentage;
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
                    _investmentUpsertMethods[UserInfo.Role](simulationId, data);
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
                var result = await Task.Factory.StartNew(() => UnitOfWork.BudgetRepo.GetBudgetLibraries());

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
                    UnitOfWork.BudgetRepo.UpsertBudgetLibrary(data);
                    UnitOfWork.BudgetRepo.UpsertOrDeleteBudgets(data.Budgets, data.Id);
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
                    UnitOfWork.BudgetRepo.DeleteBudgetLibrary(libraryId);
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

                var result = await Task.Factory.StartNew(() =>
                {
                    if (overwriteBudgets)
                    {
                        UnitOfWork.Context.DeleteAll<BudgetEntity>(_ => _.BudgetLibraryId == budgetLibraryId);
                    }
                    return _investmentBudgetsService.ImportLibraryInvestmentBudgetsFile(budgetLibraryId, excelPackage);
                });

                return Ok(result);
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

                var result = await Task.Factory.StartNew(() =>
                    _investmentImportMethods[UserInfo.Role](overwriteBudgets, excelPackage, simulationId));

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
    }
}
