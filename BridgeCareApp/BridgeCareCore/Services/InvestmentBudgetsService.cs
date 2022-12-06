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
            var budgetAmounts = _unitOfWork.BudgetAmountRepo.GetScenarioBudgetAmounts(simulationId);
            var criteriaPerBudgetName = _unitOfWork.Context.ScenarioBudget.AsNoTracking().AsSplitQuery()
                .Include(_ => _.CriterionLibraryScenarioBudgetJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Where(_ => _.SimulationId == simulationId)
                .ToDictionary(_ => _.Name,
                    _ => _.CriterionLibraryScenarioBudgetJoin?.CriterionLibrary.MergedCriteriaExpression ?? "");

            if (budgetAmounts.Any())
            {
                var simulationName = _unitOfWork.Context.Simulation.Where(_ => _.Id == simulationId)
                    .Select(_ => new SimulationEntity { Name = _.Name }).AsNoTracking().Single().Name;

                var fileName = $"{simulationName.Trim().Replace(" ", "_")}_investment_budgets.xlsx";

                using var excelPackage = new ExcelPackage(new FileInfo(fileName));

                var budgetWorksheet = excelPackage.Workbook.Worksheets.Add("Budget");

                var budgetNames = budgetAmounts.Select(_ => _.ScenarioBudget.Name).Distinct().OrderBy(budgetName => budgetName)
                    .ToList();

                AddHeaderCells(budgetWorksheet, budgetNames);

                var budgetAmountsPerYear = budgetAmounts
                    .OrderBy(budgetAmount => budgetAmount.Year)
                    .GroupBy(budgetAmount => budgetAmount.Year, entities => entities)
                    .ToDictionary(group => group.Key, entities => entities
                        .OrderBy(budgetAmount => budgetAmount.ScenarioBudget.Name)
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
            var budgetAmounts = _unitOfWork.BudgetAmountRepo.GetLibraryBudgetAmounts(budgetLibraryId);
            var criteriaPerBudgetName = _unitOfWork.Context.Budget.AsNoTracking().AsSplitQuery()
                .Include(_ => _.CriterionLibraryBudgetJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Where(_ => _.BudgetLibraryId == budgetLibraryId)
                .ToDictionary(_ => _.Name,
                    _ => _.CriterionLibraryBudgetJoin?.CriterionLibrary.MergedCriteriaExpression ?? "");

            if (budgetAmounts.Any())
            {
                var budgetLibraryName = _unitOfWork.Context.BudgetLibrary.Where(_ => _.Id == budgetLibraryId)
                    .Select(budgetLibrary => new BudgetLibraryEntity { Name = budgetLibrary.Name }).AsNoTracking().Single()
                    .Name;

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
                        _.IsSingleUse && _.CriterionLibraryScenarioBudgetJoins.Any(join =>
                            join.ScenarioBudget.SimulationId == simulationId &&
                            budgetNames.Contains(join.ScenarioBudget.Name)));
                }

                _unitOfWork.Context.DeleteAll<ScenarioBudgetEntity>(_ => _.SimulationId == simulationId);
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

        public InvestmentPagingPageModel GetLibraryInvestmentPage(Guid libraryId, InvestmentPagingRequestModel request)
        {
            var skip = 0;
            var take = 0;
            var total = 0;
            var items = new List<BudgetDTO>();
            var lastYear = 0;
            var budgets = _unitOfWork.BudgetRepo.GetBudgetLibrary(libraryId).Budgets;


            budgets = SyncedDataset(budgets, request.PagingSync);



            if (request.sortColumn.Trim() != "")
                budgets = OrderByColumn(budgets, request.sortColumn, request.isDescending);
            if (budgets.Count > 0 && budgets[0].BudgetAmounts.Count > 0)
                lastYear = budgets[0].BudgetAmounts.Max(_ => _.Year);

            if (request.RowsPerPage > 0)
            {
                take = request.RowsPerPage;
                skip = request.RowsPerPage * (request.Page - 1);
                total = budgets.Count != 0 ? budgets.First().BudgetAmounts.Count : 0;
                budgets.ForEach(_ => _.BudgetAmounts = _.BudgetAmounts.Skip(skip).Take(take).ToList());
                items = budgets;
            }
            else
            {
                items = budgets;
                return new InvestmentPagingPageModel()
                {
                    Items = items,
                    TotalItems = total,
                    LastYear = lastYear
                };
            }

            return new InvestmentPagingPageModel()
            {
                Items = items,
                TotalItems = total,
                LastYear = lastYear
            };
        }

        public InvestmentPagingPageModel GetScenarioInvestmentPage(Guid simulationId, InvestmentPagingRequestModel request)
        {
            var skip = 0;
            var take = 0;
            var items = new List<BudgetDTO>();
            var total = 0;
            var lastYear = 0;
            var investmentPlan = request.PagingSync.Investment == null ? _unitOfWork.InvestmentPlanRepo.GetInvestmentPlan(simulationId) : request.PagingSync.Investment;
            if (investmentPlan.Id == Guid.Empty)
            {
                var investmentDefaultData = _investmentDefaultDataService.GetInvestmentDefaultData().Result;
                investmentPlan.MinimumProjectCostLimit = investmentDefaultData.MinimumProjectCostLimit;
                investmentPlan.InflationRatePercentage = investmentDefaultData.InflationRatePercentage;
            }

            var budgets = request.PagingSync.LibraryId == null ? _unitOfWork.BudgetRepo.GetScenarioBudgets(simulationId) :
                _unitOfWork.BudgetRepo.GetBudgetLibrary(request.PagingSync.LibraryId.Value).Budgets;

            budgets = SyncedDataset(budgets, request.PagingSync);

            if (request.sortColumn.Trim() != "")
                budgets = OrderByColumn(budgets, request.sortColumn, request.isDescending);

            if (budgets.Count > 0 && budgets[0].BudgetAmounts.Count > 0)
                lastYear = budgets[0].BudgetAmounts.Max(_ => _.Year);

            if (request.RowsPerPage > 0)
            {
                take = request.RowsPerPage;
                skip = request.RowsPerPage * (request.Page - 1);
                total = budgets.Count != 0 ? budgets.First().BudgetAmounts.Count : 0;
                budgets.ForEach(_ => _.BudgetAmounts = _.BudgetAmounts.Skip(skip).Take(take).ToList());
                items = budgets;
            }
            else
            {
                items = budgets;
                return new InvestmentPagingPageModel()
                {
                    Items = items,
                    TotalItems = total,
                    LastYear = lastYear,
                    InvestmentPlan = investmentPlan
                };
            }

            return new InvestmentPagingPageModel()
            {
                Items = items,
                TotalItems = total,
                LastYear = lastYear,
                InvestmentPlan = investmentPlan
            };
        }

        public List<BudgetDTO> GetSyncedInvestmentDataset(Guid simulationId, InvestmentPagingSyncModel request)
        {
            var budgets = request.LibraryId == null ?
                    _unitOfWork.BudgetRepo.GetScenarioBudgets(simulationId) :
                    _unitOfWork.BudgetRepo.GetBudgetLibrary(request.LibraryId.Value).Budgets;
            budgets =  SyncedDataset(budgets, request);

            if(request.LibraryId != null)
            {
                budgets.ForEach(_ =>
                {
                    _.Id = Guid.NewGuid();
                    _.BudgetAmounts.ForEach(__ => __.Id = Guid.NewGuid());
                });
            }

            budgets.ForEach(_ => _.BudgetAmounts.ForEach(__ => __.Year += request.FirstYearAnalysisBudgetShift));
            return budgets;
        }

        public List<BudgetDTO> GetSyncedLibraryDataset(Guid libraryId, InvestmentPagingSyncModel request)
        {
            var budgets = _unitOfWork.BudgetRepo.GetBudgetLibrary(libraryId).Budgets;
            return SyncedDataset(budgets, request);
        }

        private List<BudgetDTO> OrderByColumn(List<BudgetDTO> budgets, string sortColumn, bool isDescending)
        {
            sortColumn = sortColumn?.ToLower().Trim();
            switch (sortColumn)
            {
            case "year":
                if (isDescending)
                {
                    budgets.ForEach(_ => _.BudgetAmounts = _.BudgetAmounts.OrderByDescending(__ => __.Year).ToList());
                    return budgets;
                }
                else
                {
                    budgets.ForEach(_ => _.BudgetAmounts = _.BudgetAmounts.OrderBy(__ => __.Year).ToList());
                    return budgets;
                }
            default:
                var budget = budgets.FirstOrDefault(_ => _.Name.ToLower().Trim() == sortColumn);
                if (isDescending)
                {
                    budget.BudgetAmounts = budget.BudgetAmounts.OrderByDescending(_ => _.Value).ToList();
                    var dict = budget.BudgetAmounts.ToDictionary(_ => _.Year, _ => _.Value);
                    budgets.ForEach(_ => _.BudgetAmounts = _.BudgetAmounts.OrderByDescending(__ => dict[__.Year]).ToList());
                }
                else
                {
                    budget.BudgetAmounts = budget.BudgetAmounts.OrderBy(_ => _.Value).ToList();
                    var dict = budget.BudgetAmounts.ToDictionary(_ => _.Year, _ => _.Value);
                    budgets.ForEach(_ => _.BudgetAmounts = _.BudgetAmounts.OrderBy(__ => dict[__.Year]).ToList());
                }                   
                
                return budgets;
            }
        }

        private List<BudgetDTO> SyncedDataset(List<BudgetDTO> budgets, InvestmentPagingSyncModel syncModel)
        {
            budgets = budgets.Concat(syncModel.AddedBudgets).Where(_ => !syncModel.BudgetsForDeletion.Contains(_.Id)).ToList();
            for (var i = 0; i < budgets.Count; i++)
            {
                var budget = budgets[i];
                var item = syncModel.UpdatedBudgets.FirstOrDefault(row => row.Id == budget.Id);
                if(item != null)
                {
                    budget.Name = item.Name;
                    budget.BudgetOrder = item.BudgetOrder;
                    budget.CriterionLibrary = item.CriterionLibrary;
                }               
                if(syncModel.Deletionyears.Count != 0)
                    budget.BudgetAmounts = budget.BudgetAmounts.Where(_ => !syncModel.Deletionyears.Contains(_.Year)).ToList();
                if (syncModel.AddedBudgetAmounts.ContainsKey(budget.Name))
                    budget.BudgetAmounts = budget.BudgetAmounts.Concat(syncModel.AddedBudgetAmounts[budget.Name]).ToList();
                if (syncModel.UpdatedBudgetAmounts.ContainsKey(budget.Name))
                    for (var o = 0; o < budget.BudgetAmounts.Count; o++)
                    {
                        var amount = syncModel.UpdatedBudgetAmounts[budget.Name].FirstOrDefault(row => row.Id == budget.BudgetAmounts[o].Id);
                        if (amount != null)
                            budget.BudgetAmounts[o] = amount;
                    }
            }

            return budgets;
        }
    }
}
