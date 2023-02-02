using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using OfficeOpenXml;
using BridgeCareCore.Interfaces.DefaultData;

namespace BridgeCareCore.Services
{
    public class InvestmentBudgetsService : IInvestmentBudgetsService
    {
        private static UnitOfDataPersistenceWork _unitOfWork;
        private static IExpressionValidationService _expressionValidationService;
        public readonly IInvestmentDefaultDataService _investmentDefaultDataService;
        protected readonly IHubService HubService;

        public InvestmentBudgetsService(UnitOfDataPersistenceWork unitOfWork,
            IExpressionValidationService expressionValidationService, IHubService hubService,
            IInvestmentDefaultDataService investmentDefaultDataService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _expressionValidationService = expressionValidationService ??
                                           throw new ArgumentNullException(nameof(expressionValidationService));
            HubService = hubService ?? throw new ArgumentNullException(nameof(hubService));
            _investmentDefaultDataService = investmentDefaultDataService ?? throw new ArgumentNullException(nameof(investmentDefaultDataService));
        }

        private void AddHeaderCells(ExcelWorksheet worksheet, List<string> headers)
        {
            var startRow = worksheet.Cells.Start.Row;
            var startCol = worksheet.Cells.Start.Column;

            worksheet.Cells[startRow, startCol].Value = "Year";

            headers.ForEach(budgetName =>
            {
                worksheet.Cells[startRow, ++startCol].Value = budgetName;
            });
        }

        private void AddDataCells(ExcelWorksheet worksheet, Dictionary<int, List<decimal>> budgetAmountsPerYear)
        {
            var startRow = worksheet.Cells.Start.Row;
            var startCol = worksheet.Cells.Start.Column;

            budgetAmountsPerYear.Keys.ForEach(year =>
            {
                worksheet.Cells[++startRow, startCol].Value = year;

                var budgetAmounts = budgetAmountsPerYear[year];

                var budgetCol = startCol + 1;
                budgetAmounts.ForEach(amount =>
                {
                    worksheet.Cells[startRow, budgetCol].Value = amount;
                    budgetCol++;
                });
            });

            ExcelHelper.SetCustomFormat(
                worksheet.Cells[2, 2, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column],
                ExcelHelperCellFormat.Accounting);
        }

        private FileInfoDTO CreateInvestmentBudgetsSampleFile()
        {
            var fileName = "sample_investment_budgets_import_export_file.xlsx";

            using var excelPackage = new ExcelPackage(new FileInfo(fileName));

            var budgetWorksheet = excelPackage.Workbook.Worksheets.Add("Budget");
            var criteriaWorksheet = excelPackage.Workbook.Worksheets.Add("Criteria");

            var budgetNames = new List<string>
            {
                "Sample Budget 1",
                "Sample Budget 2",
                "Sample Budget 3",
                "Sample Budget 4"
            };

            AddHeaderCells(budgetWorksheet, budgetNames);

            var sampleBudgetAmountsPerYear = new Dictionary<int, List<decimal>>();

            var sampleBudgetAmounts = Enumerable.Repeat(decimal.Parse("5000000"), 4).ToList();
            var currentYear = DateTime.Now.Year;

            for (var i = 0; i < 4; i++)
            {
                sampleBudgetAmountsPerYear.Add(currentYear, sampleBudgetAmounts);
                currentYear++;
            }

            AddDataCells(budgetWorksheet, sampleBudgetAmountsPerYear);

            criteriaWorksheet.Cells[1, 1].Value = "BUDGET_NAME";
            criteriaWorksheet.Cells[1, 2].Value = "CRITERIA";
            var currentRow = 2;
            var sampleCriteria = "[INTERSTATE]='Y'";
            budgetNames.ForEach(budgetName =>
            {
                criteriaWorksheet.Cells[currentRow, 1].Value = budgetName;
                criteriaWorksheet.Cells[currentRow, 2].Value = sampleCriteria;
                currentRow++;
            });

            return new FileInfoDTO
            {
                FileName = fileName,
                FileData = Convert.ToBase64String(excelPackage.GetAsByteArray()),
                MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            };
        }

        public FileInfoDTO ExportScenarioInvestmentBudgetsFile(Guid simulationId)
        {
            // hit by InvestmentTests.ShouldExportScenarioBudgetsFile and by InvestmentTests.ShouldExportSampleScenarioBudgetsFile
            var budgetAmounts = _unitOfWork.BudgetAmountRepo.GetScenarioBudgetAmounts(simulationId);
            var criteriaPerBudgetName = _unitOfWork.BudgetRepo.GetCriteriaPerBudgetNameForSimulation(simulationId);
            if (budgetAmounts.Any())
            {
                var simulationName = _unitOfWork.Context.Simulation.Where(_ => _.Id == simulationId)
                    .Select(_ => new SimulationEntity { Name = _.Name }).AsNoTracking().Single().Name;

                var fileName = $"{simulationName.Trim().Replace(" ", "_")}_investment_budgets.xlsx";

                using var excelPackage = new ExcelPackage(new FileInfo(fileName));

                var budgetWorksheet = excelPackage.Workbook.Worksheets.Add("Budget");

                var budgetNames = budgetAmounts.Select(_ => _.BudgetName).Distinct().OrderBy(budgetName => budgetName)
                    .ToList();

                AddHeaderCells(budgetWorksheet, budgetNames);

                var budgetAmountsPerYear = budgetAmounts
                    .OrderBy(budgetAmount => budgetAmount.Year)
                    .GroupBy(budgetAmount => budgetAmount.Year, entities => entities)
                    .ToDictionary(group => group.Key, entities => entities
                        .OrderBy(budgetAmount => budgetAmount.BudgetName)
                        .Select(budgetAmount => budgetAmount.Value).ToList());

                AddDataCells(budgetWorksheet, budgetAmountsPerYear);

                if (criteriaPerBudgetName.Any())
                {
                    var criteriaWorksheet = excelPackage.Workbook.Worksheets.Add("Criteria");

                    criteriaWorksheet.Cells[1, 1].Value = "BUDGET_NAME";
                    criteriaWorksheet.Cells[1, 2].Value = "CRITERIA";

                    var currentRow = 2;
                    criteriaPerBudgetName.ForEach(criteriaPerBudgetName =>
                    {
                        criteriaWorksheet.Cells[currentRow, 1].Value = criteriaPerBudgetName.Key;
                        criteriaWorksheet.Cells[currentRow, 2].Value = criteriaPerBudgetName.Value;
                        currentRow++;
                    });
                }

                return new FileInfoDTO
                {
                    FileName = fileName,
                    FileData = Convert.ToBase64String(excelPackage.GetAsByteArray()),
                    MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                };
            }

            return CreateInvestmentBudgetsSampleFile();
        }

        public FileInfoDTO ExportLibraryInvestmentBudgetsFile(Guid budgetLibraryId)
        {
            // InvestmentTests.ShouldExportSampleLibraryBudgetsFile
            var budgetAmounts = _unitOfWork.BudgetAmountRepo.GetLibraryBudgetAmounts(budgetLibraryId);
            var criteriaPerBudgetName = _unitOfWork.BudgetRepo.GetCriteriaPerBudgetNameForBudgetLibrary(budgetLibraryId);

            if (budgetAmounts.Any())
            {
                var budgetLibraryName = _unitOfWork.BudgetRepo.GetBudgetLibraryName(budgetLibraryId);

                var fileName = $"{budgetLibraryName.Trim().Replace(" ", "_")}_investment_budgets.xlsx";

                using var excelPackage = new ExcelPackage(new FileInfo(fileName));

                var budgetWorksheet = excelPackage.Workbook.Worksheets.Add("Budget");

                var budgetNames = budgetAmounts.Select(_ => _.Budget.Name).Distinct().OrderBy(budgetName => budgetName)
                    .ToList();

                AddHeaderCells(budgetWorksheet, budgetNames);

                var budgetAmountsPerYear = budgetAmounts
                    .OrderBy(budgetAmount => budgetAmount.Year)
                    .GroupBy(budgetAmount => budgetAmount.Year, entities => entities)
                    .ToDictionary(group => group.Key, entities => entities
                        .OrderBy(budgetAmount => budgetAmount.Budget.Name)
                        .Select(budgetAmount => budgetAmount.Value).ToList());

                AddDataCells(budgetWorksheet, budgetAmountsPerYear);

                if (criteriaPerBudgetName.Any())
                {
                    var criteriaWorksheet = excelPackage.Workbook.Worksheets.Add("Criteria");

                    criteriaWorksheet.Cells[1, 1].Value = "BUDGET_NAME";
                    criteriaWorksheet.Cells[1, 2].Value = "CRITERIA";

                    var currentRow = 2;
                    criteriaPerBudgetName.ForEach(criteriaPerBudgetName =>
                    {
                        criteriaWorksheet.Cells[currentRow, 1].Value = criteriaPerBudgetName.Key;
                        criteriaWorksheet.Cells[currentRow, 2].Value = criteriaPerBudgetName.Value;
                        currentRow++;
                    });
                }

                return new FileInfoDTO
                {
                    FileName = fileName,
                    FileData = Convert.ToBase64String(excelPackage.GetAsByteArray()),
                    MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                };
            }

            return CreateInvestmentBudgetsSampleFile();
        }

        public ScenarioBudgetImportResultDTO ImportScenarioInvestmentBudgetsFile(Guid simulationId, ExcelPackage excelPackage,
            UserCriteriaDTO currentUserCriteriaFilter, bool overwriteBudgets)
        {
            // InvestmentTests.ImportScenarioInvestmentBudgetsExcelFile
            var budgetWorksheet = excelPackage.Workbook.Worksheets[0];
            var budgetWorksheetEnd = budgetWorksheet.Dimension.End;

            var worksheetBudgetNames = budgetWorksheet.Cells[1, 2, 1, budgetWorksheetEnd.Column]
                .Select(cell => cell.GetValue<string>()).ToList();

            var criteriaPerBudgetName = new Dictionary<string, string>();
            if (excelPackage.Workbook.Worksheets.Count > 1)
            {
                var criteriaWorksheet = excelPackage.Workbook.Worksheets[1];
                var budgetCol = 1;
                var criteriaCol = 2;
                for (var row = 2; row <= criteriaWorksheet.Dimension.End.Row; row++)
                {
                    var budgetName = criteriaWorksheet.GetValue<string>(row, budgetCol);
                    var criteria = criteriaWorksheet.GetValue<string>(row, criteriaCol);
                    if (!criteriaPerBudgetName.ContainsKey(budgetName))
                    {
                        criteriaPerBudgetName.Add(budgetName, string.Empty);
                    }

                    criteriaPerBudgetName[budgetName] = criteria;
                }
            }

            if (overwriteBudgets)
            {
                if (criteriaPerBudgetName.Values.Any())
                {
                    var budgetNames = criteriaPerBudgetName.Keys.ToList();
                    _unitOfWork.CriterionLibraryRepo.DeleteAllSingleUseCriterionLibrariesWithBudgetNamesForSimulation(simulationId, budgetNames);
                }
                var projects = _unitOfWork.Context.CommittedProject.Where(_ => _.SimulationId == simulationId && _.ScenarioBudgetId != null).ToList();
                if (projects.Count > 0)
                {
                    projects.ForEach(_ => _.ScenarioBudgetId = null);
                    _unitOfWork.Context.UpdateAll(projects);
                }

                _unitOfWork.BudgetRepo.DeleteAllScenarioBudgetsForSimulation(simulationId);
            }
            var existingBudgetEntities = _unitOfWork.BudgetRepo.GetScenarioBudgets(simulationId)
                .Select(_ => _.ToScenarioEntityWithBudgetAmount(simulationId)).ToList();

            var existingBudgetNames = existingBudgetEntities.Select(existingBudget => existingBudget.Name).ToList();
            var newBudgetEntities = worksheetBudgetNames.Where(budgetName => !existingBudgetNames.Contains(budgetName))
                .Select(budgetName => new ScenarioBudgetEntity
                {
                    Id = Guid.NewGuid(),
                    SimulationId = simulationId,
                    Name = budgetName
                }).ToList();

            var budgetAmountsPerBudgetYearTuple = new Dictionary<(string, int), ScenarioBudgetAmountEntity>();
            existingBudgetEntities.ForEach(budget =>
            {
                budget.ScenarioBudgetAmounts.ForEach(budgetAmount =>
                {
                    // The if condition is to avoid any duplicate records coming from the DB.
                    // Ideally the database must have only 1 record per Budget name per year.
                    if (!budgetAmountsPerBudgetYearTuple.ContainsKey((budget.Name, budgetAmount.Year)))
                    {
                        budgetAmountsPerBudgetYearTuple.Add((budget.Name, budgetAmount.Year), budgetAmount);
                    }
                });
            });

            var newBudgetAmountEntities = new List<ScenarioBudgetAmountEntity>();
            for (var row = 2; row <= budgetWorksheetEnd.Row; row++)
            {
                for (var col = 2; col <= budgetWorksheetEnd.Column; col++)
                {
                    var budgetName = budgetWorksheet.GetValue<string>(1, col);
                    var year = budgetWorksheet.GetValue<int>(row, 1);
                    var budgetYearTuple = (budgetName, year);
                    if (!budgetAmountsPerBudgetYearTuple.ContainsKey(budgetYearTuple))
                    {
                        var budgetId = existingBudgetEntities.Any(_ => _.Name == budgetYearTuple.budgetName)
                            ? existingBudgetEntities
                                .Single(_ => _.Name == budgetYearTuple.budgetName).Id
                            : newBudgetEntities
                                .Single(_ => _.Name == budgetYearTuple.budgetName).Id;

                        newBudgetAmountEntities.Add(new ScenarioBudgetAmountEntity
                        {
                            Id = Guid.NewGuid(),
                            ScenarioBudgetId = budgetId,
                            Year = budgetYearTuple.year,
                            Value = budgetWorksheet.GetValue<decimal>(row, col)
                        });
                    }
                    else
                    {
                        budgetAmountsPerBudgetYearTuple[budgetYearTuple].Value = budgetWorksheet.GetValue<decimal>(row, col);
                    }
                }
            }

            _unitOfWork.Context.AddAll(newBudgetEntities, _unitOfWork.CurrentUser?.Id);
            _unitOfWork.Context.AddAll(newBudgetAmountEntities, _unitOfWork.CurrentUser?.Id);
            _unitOfWork.Context.UpdateAll(budgetAmountsPerBudgetYearTuple.Values.ToList(), _unitOfWork.CurrentUser?.Id);

            var budgetsWithInvalidCriteria = new List<string>();
            var criteriaBudgetNamesNotPresentInBudgetTab = new List<string>();
            if (criteriaPerBudgetName.Values.Any())
            {
                var allBudgetEntities = new List<ScenarioBudgetEntity>();
                allBudgetEntities.AddRange(existingBudgetEntities);
                allBudgetEntities.AddRange(newBudgetEntities);

                var budgetNames = criteriaPerBudgetName.Keys.ToList();

                _unitOfWork.Context.DeleteAll<CriterionLibraryEntity>(_ =>
                    _.IsSingleUse && _.CriterionLibraryScenarioBudgetJoins.Any(join =>
                        join.ScenarioBudget.SimulationId == simulationId &&
                        budgetNames.Contains(join.ScenarioBudget.Name)));

                var criteria = new List<CriterionLibraryEntity>();
                var criteriaJoins = new List<CriterionLibraryScenarioBudgetEntity>();
                var invalidOperationEx = false;
                var exceptionMessage = "";
                criteriaPerBudgetName.Where(_ => !string.IsNullOrEmpty(_.Value)).ToList().ForEach(criterionPerBudgetName =>
                {
                    var budgetName = criterionPerBudgetName.Key;
                    var criterion = criterionPerBudgetName.Value;
                    if (!string.IsNullOrEmpty(criterion))
                    {
                        var validationResult = _expressionValidationService.ValidateCriterionWithoutResults(criterion, currentUserCriteriaFilter);
                        if (validationResult.IsValid)
                        {
                            var criterionId = Guid.NewGuid();
                            criteria.Add(new CriterionLibraryEntity
                            {
                                Id = criterionId,
                                IsSingleUse = true,
                                Name = $"{budgetName} Criterion Library",
                                MergedCriteriaExpression = criterion
                            });
                            try
                            {
                                criteriaJoins.Add(new CriterionLibraryScenarioBudgetEntity
                                {
                                    CriterionLibraryId = criterionId,
                                    ScenarioBudgetId = allBudgetEntities.Single(_ => _.Name.ToUpperInvariant() == budgetName.ToUpperInvariant()).Id
                                });
                            }
                            catch (ArgumentNullException ex)
                            {
                                throw new ArgumentNullException(ex.Message);
                            }
                            catch (InvalidOperationException ex)
                            {
                                invalidOperationEx = true;
                                exceptionMessage = ex.Message;
                                criteriaBudgetNamesNotPresentInBudgetTab.Add(budgetName);
                            }
                        }
                        else
                        {
                            budgetsWithInvalidCriteria.Add(budgetName);
                        }
                    }
                });
                if (invalidOperationEx)
                {
                    var sb = new StringBuilder();
                    if (criteriaBudgetNamesNotPresentInBudgetTab.Count > 0)
                    {
                        sb.Append($" The following budget names are in criteria TAB but not in Budget TAB: {string.Join(",", criteriaBudgetNamesNotPresentInBudgetTab)}");
                    }
                    HubService.SendRealTimeMessage("", HubConstant.BroadcastError, sb.ToString());
                    throw new InvalidOperationException(exceptionMessage);
                }
                _unitOfWork.Context.AddAll(criteria, _unitOfWork.CurrentUser?.Id);
                _unitOfWork.Context.AddAll(criteriaJoins, _unitOfWork.CurrentUser?.Id);
            }

            var warningSb = new StringBuilder();
            
            if (budgetsWithInvalidCriteria.Any())
            {
                warningSb.Append($"The following budgets had invalid criteria: {string.Join(", ", budgetsWithInvalidCriteria)}. ");
            }

            var budgets = _unitOfWork.BudgetRepo.GetScenarioBudgets(simulationId);
            if (budgets != null && budgets.Count > 0 && budgets.First().BudgetAmounts.Count != 0)
            {
                var firstYear = budgets.First().BudgetAmounts.Min(_ => _.Year);
                var numberOfYears = budgets.First().BudgetAmounts.Count;

                var investmentPlan = _unitOfWork.InvestmentPlanRepo.GetInvestmentPlan(simulationId);
                if (investmentPlan.FirstYearOfAnalysisPeriod != firstYear || investmentPlan.NumberOfYearsInAnalysisPeriod != numberOfYears)
                {                
                    if(investmentPlan.Id == Guid.Empty)
                    {
                        var planData = _investmentDefaultDataService.GetInvestmentDefaultData().Result;
                        investmentPlan.InflationRatePercentage = planData.InflationRatePercentage;
                        investmentPlan.MinimumProjectCostLimit = planData.MinimumProjectCostLimit;
                        investmentPlan.Id = Guid.NewGuid();
                    }
                    investmentPlan.FirstYearOfAnalysisPeriod = firstYear;
                    investmentPlan.NumberOfYearsInAnalysisPeriod = numberOfYears;
                    _unitOfWork.InvestmentPlanRepo.UpsertInvestmentPlan(investmentPlan, simulationId);
                }


            }
            return new ScenarioBudgetImportResultDTO
            {
                Budgets = _unitOfWork.BudgetRepo.GetScenarioBudgets(simulationId),
                WarningMessage = !string.IsNullOrEmpty(warningSb.ToString())
                    ? warningSb.ToString()
                    : null
            };
        }

        public BudgetImportResultDTO ImportLibraryInvestmentBudgetsFile(Guid budgetLibraryId, ExcelPackage excelPackage, UserCriteriaDTO currentUserCriteriaFilter,
            bool overwriteBudgets)
        {
            // InvestmentTests.ShouldImportLibraryBudgetsFromFile
            var budgetWorksheet = excelPackage.Workbook.Worksheets[0];
            var budgetWorksheetEnd = budgetWorksheet.Dimension.End;

            var worksheetBudgetNames = budgetWorksheet.Cells[1, 2, 1, budgetWorksheetEnd.Column]
                .Select(cell => cell.GetValue<string>()).ToList();

            var criteriaPerBudgetName = new Dictionary<string, string>();
            if (excelPackage.Workbook.Worksheets.Count > 1)
            {
                var criteriaWorksheet = excelPackage.Workbook.Worksheets[1];
                var budgetCol = 1;
                var criteriaCol = 2;
                for (var row = 2; row <= criteriaWorksheet.Dimension.End.Row; row++)
                {
                    var budgetName = criteriaWorksheet.GetValue<string>(row, budgetCol);
                    var criteria = criteriaWorksheet.GetValue<string>(row, criteriaCol);
                    if (!criteriaPerBudgetName.ContainsKey(budgetName))
                    {
                        criteriaPerBudgetName.Add(budgetName, string.Empty);
                    }

                    criteriaPerBudgetName[budgetName] = criteria;
                }
            }

            if (overwriteBudgets)
            {
                if (criteriaPerBudgetName.Values.Any())
                {
                    var budgetNames = criteriaPerBudgetName.Keys.ToList();

                    _unitOfWork.Context.DeleteAll<CriterionLibraryEntity>(_ =>
                    _.IsSingleUse && _.CriterionLibraryBudgetJoins.Any(join =>
                        join.Budget.BudgetLibraryId == budgetLibraryId &&
                        budgetNames.Contains(join.Budget.Name)));
                }

                _unitOfWork.Context.DeleteAll<BudgetEntity>(_ => _.BudgetLibraryId == budgetLibraryId);
            }

            var existingBudgetEntities = _unitOfWork.BudgetRepo.GetLibraryBudgets(budgetLibraryId);

            var existingBudgetNames = existingBudgetEntities.Select(existingBudget => existingBudget.Name).ToList();
            var newBudgetEntities = worksheetBudgetNames.Where(budgetName => !existingBudgetNames.Contains(budgetName))
                .Select(budgetName => new BudgetEntity
                {
                    Id = Guid.NewGuid(),
                    BudgetLibraryId = budgetLibraryId,
                    Name = budgetName
                }).ToList();

            var budgetAmountsPerBudgetYearTuple = new Dictionary<(string, int), BudgetAmountEntity>();
            existingBudgetEntities.ForEach(budget =>
            {
                budget.BudgetAmounts.ForEach(budgetAmount =>
                    budgetAmountsPerBudgetYearTuple.Add((budget.Name, budgetAmount.Year), budgetAmount));
            });

            var newBudgetAmountEntities = new List<BudgetAmountEntity>();
            for (var row = 2; row <= budgetWorksheetEnd.Row; row++)
            {
                for (var col = 2; col <= budgetWorksheetEnd.Column; col++)
                {
                    var budgetName = budgetWorksheet.GetValue<string>(1, col);
                    var year = budgetWorksheet.GetValue<int>(row, 1);
                    var budgetYearTuple = (budgetName, year);
                    if (!budgetAmountsPerBudgetYearTuple.ContainsKey(budgetYearTuple))
                    {
                        var budgetId = existingBudgetEntities.Any(_ => _.Name == budgetYearTuple.budgetName)
                            ? existingBudgetEntities
                                .Single(_ => _.Name == budgetYearTuple.budgetName).Id
                            : newBudgetEntities
                                .Single(_ => _.Name == budgetYearTuple.budgetName).Id;

                        newBudgetAmountEntities.Add(new BudgetAmountEntity
                        {
                            Id = Guid.NewGuid(),
                            BudgetId = budgetId,
                            Year = budgetYearTuple.year,
                            Value = budgetWorksheet.GetValue<decimal>(row, col)
                        });
                    }
                    else
                    {
                        budgetAmountsPerBudgetYearTuple[budgetYearTuple].Value = budgetWorksheet.GetValue<decimal>(row, col);
                    }
                }
            }

            _unitOfWork.Context.AddAll(newBudgetEntities, _unitOfWork.CurrentUser?.Id);
            _unitOfWork.Context.AddAll(newBudgetAmountEntities, _unitOfWork.CurrentUser?.Id);
            _unitOfWork.Context.UpdateAll(budgetAmountsPerBudgetYearTuple.Values.ToList(), _unitOfWork.CurrentUser?.Id);

            var budgetsWithInvalidCriteria = new List<string>();
            var criteriaBudgetNamesNotPresentInBudgetTab = new List<string>();
            if (criteriaPerBudgetName.Values.Any())
            {
                var allBudgetEntities = new List<BudgetEntity>();
                allBudgetEntities.AddRange(existingBudgetEntities);
                allBudgetEntities.AddRange(newBudgetEntities);

                var budgetNames = criteriaPerBudgetName.Keys.ToList();
                _unitOfWork.Context.DeleteAll<CriterionLibraryEntity>(_ =>
                    _.IsSingleUse && _.CriterionLibraryBudgetJoins.Any(join =>
                        join.Budget.BudgetLibraryId == budgetLibraryId &&
                        budgetNames.Contains(join.Budget.Name)));

                var criteria = new List<CriterionLibraryEntity>();
                var criteriaJoins = new List<CriterionLibraryBudgetEntity>();
                var invalidOperationEx = false;
                var exceptionMessage = "";
                criteriaPerBudgetName.Where(_ => !string.IsNullOrEmpty(_.Value)).ToList().ForEach(criterionPerBudgetName =>
                {
                    var budgetName = criterionPerBudgetName.Key;
                    var criterion = criterionPerBudgetName.Value;
                    if (!string.IsNullOrEmpty(criterion))
                    {
                        var validationResult = _expressionValidationService.ValidateCriterionWithoutResults(criterion, currentUserCriteriaFilter);
                        if (validationResult.IsValid)
                        {
                            var criterionId = Guid.NewGuid();
                            criteria.Add(new CriterionLibraryEntity
                            {
                                Id = criterionId,
                                IsSingleUse = true,
                                Name = $"{budgetName} Criterion Library",
                                MergedCriteriaExpression = criterion
                            });
                            
                            try
                            {
                                criteriaJoins.Add(new CriterionLibraryBudgetEntity
                                {
                                    CriterionLibraryId = criterionId,
                                    BudgetId = allBudgetEntities.Single(_ => _.Name == budgetName).Id
                                });
                            }
                            catch (ArgumentNullException ex)
                            {
                                throw new ArgumentNullException(ex.Message);
                            }
                            catch (InvalidOperationException ex)
                            {
                                invalidOperationEx = true;
                                exceptionMessage = ex.Message;
                                criteriaBudgetNamesNotPresentInBudgetTab.Add(budgetName);
                            }
                        }
                        else
                        {
                            budgetsWithInvalidCriteria.Add(budgetName);
                        }
                    }
                });
                if (invalidOperationEx)
                {
                    var sb = new StringBuilder();
                    if (criteriaBudgetNamesNotPresentInBudgetTab.Count > 0)
                    {
                        sb.Append($" The following budget names are in criteria TAB but not in Budget TAB: {string.Join(",", criteriaBudgetNamesNotPresentInBudgetTab)}");
                    }
                    HubService.SendRealTimeMessage("", HubConstant.BroadcastError, sb.ToString());
                    throw new InvalidOperationException(exceptionMessage);
                }
                _unitOfWork.Context.AddAll(criteria, _unitOfWork.CurrentUser?.Id);
                _unitOfWork.Context.AddAll(criteriaJoins, _unitOfWork.CurrentUser?.Id);
            }

            var warningSb = new StringBuilder();
            if (budgetsWithInvalidCriteria.Any())
            {
                warningSb.Append($"The following budgets had invalid criteria: {string.Join(", ", budgetsWithInvalidCriteria)}");
            }
            return new BudgetImportResultDTO
            {
                BudgetLibrary = _unitOfWork.BudgetRepo.GetBudgetLibrary(budgetLibraryId),
                WarningMessage = !string.IsNullOrEmpty(warningSb.ToString())
                    ? warningSb.ToString()
                    : null
            };
        }

        
    }
}
