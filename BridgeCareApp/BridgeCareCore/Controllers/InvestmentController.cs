using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Controllers.BaseController;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace BridgeCareCore.Controllers
{
    using InvestmentUpsertMethod = Action<Guid, UpsertInvestmentDataDTO>;
    using InvestmentExportMethod = Func<Guid, Guid, FileInfoDTO>;
    using InvestmentImportMethod = Func<bool, Guid, ExcelPackage, Guid, BudgetLibraryDTO>;


    [Route("api/[controller]")]
    [ApiController]
    public class InvestmentController : BridgeCareCoreBaseController
    {
        private static IInvestmentBudgetsService _investmentBudgetsService;
        private readonly IReadOnlyDictionary<string, InvestmentUpsertMethod> _investmentUpsertMethods;
        private readonly IReadOnlyDictionary<string, InvestmentExportMethod> _investmentExportMethods;
        private readonly IReadOnlyDictionary<string, InvestmentImportMethod> _investmentImportMethods;

        public InvestmentController(IInvestmentBudgetsService investmentBudgetsService, IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork,
            IHubService hubService,
            IHttpContextAccessor httpContextAccessor) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            _investmentBudgetsService = investmentBudgetsService ??
                                        throw new ArgumentNullException(nameof(investmentBudgetsService));
            _investmentUpsertMethods = CreateUpsertMethods();
            _investmentExportMethods = CreateExportMethods();
            _investmentImportMethods = CreateImportMethods();
        }

        private Dictionary<string, InvestmentUpsertMethod> CreateUpsertMethods()
        {
            void UpsertAny(Guid simulationId, UpsertInvestmentDataDTO data)
            {
                UnitOfWork.BudgetRepo.UpsertBudgetLibrary(data.BudgetLibrary, simulationId);
                UnitOfWork.BudgetRepo.UpsertOrDeleteBudgets(data.BudgetLibrary.Budgets, data.BudgetLibrary.Id);
                UnitOfWork.InvestmentPlanRepo.UpsertInvestmentPlan(data.InvestmentPlan, simulationId);
            }

            void UpsertPermitted(Guid simulationId, UpsertInvestmentDataDTO data)
            {
                if (simulationId != Guid.Empty)
                {
                    CheckUserSimulationModifyAuthorization(simulationId);
                }
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

        private Dictionary<string, InvestmentExportMethod> CreateExportMethods()
        {
            FileInfoDTO GetAny(Guid budgetLibraryId, Guid simulationId)
            {
                return _investmentBudgetsService.ExportInvestmentBudgetsFile(budgetLibraryId);
            }

            FileInfoDTO GetPermitted(Guid budgetLibraryId, Guid simulationId)
            {
                if (simulationId != Guid.Empty)
                {
                    CheckUserSimulationModifyAuthorization(simulationId);
                }
                return GetAny(budgetLibraryId, simulationId);
            }

            return new Dictionary<string, InvestmentExportMethod>
            {
                [Role.Administrator] = GetAny,
                [Role.DistrictEngineer] = GetPermitted,
                [Role.Cwopa] = GetPermitted,
                [Role.PlanningPartner] = GetPermitted
            };
        }

        private Dictionary<string, InvestmentImportMethod> CreateImportMethods()
        {
            BudgetLibraryDTO UpsertAny(bool overwriteBudgets, Guid budgetLibraryId, ExcelPackage excelPackage, Guid simulationId)
            {
                if (overwriteBudgets)
                {
                    UnitOfWork.Context.DeleteAll<BudgetEntity>(_ => _.BudgetLibrary.Id == budgetLibraryId);
                }
                return _investmentBudgetsService.ImportInvestmentBudgetsFile(budgetLibraryId, excelPackage);
            }

            BudgetLibraryDTO UpsertPermitted(bool overwriteBudgets, Guid budgetLibraryId, ExcelPackage excelPackage, Guid simulationId)
            {
                if (simulationId != Guid.Empty)
                {
                    CheckUserSimulationModifyAuthorization(simulationId);
                }
                return UpsertAny(overwriteBudgets, budgetLibraryId, excelPackage, simulationId);
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
                    var budgetLibraries = UnitOfWork.BudgetRepo
                        .BudgetLibrariesWithBudgets();

                    var scenarioInvestmentPlan = UnitOfWork.InvestmentPlanRepo
                        .ScenarioInvestmentPlan(simulationId);
                    return new InvestmentDTO
                    {
                        BudgetLibraries = budgetLibraries, InvestmentPlan = scenarioInvestmentPlan
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

        [HttpPost]
        [Route("UpsertInvestment/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> UpsertInvestment(Guid simulationId, [FromBody] UpsertInvestmentDataDTO data)
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
                    .ScenarioSimpleBudgetDetails(simulationId));
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("ImportInvestmentBudgetsExcelFile")]
        [Authorize]
        public async Task<IActionResult> ImportInvestmentBudgetsExcelFile()
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

                var simulationId = Guid.Empty;
                if (ContextAccessor.HttpContext.Request.Form.ContainsKey("simulationId"))
                {
                    simulationId = Guid.Parse(ContextAccessor.HttpContext.Request.Form["simulationId"].ToString());
                }

                var result = await Task.Factory.StartNew(() =>
                    _investmentImportMethods[UserInfo.Role](overwriteBudgets, budgetLibraryId, excelPackage,
                        simulationId));

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
        [Route("ExportInvestmentBudgetsExcelFile/{budgetLibraryId}/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> ExportInvestmentBudgetsExcelFile(Guid budgetLibraryId, Guid simulationId)
        {
            try
            {
                var result =
                    await Task.Factory.StartNew(() => _investmentExportMethods[UserInfo.Role](budgetLibraryId, simulationId));

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
