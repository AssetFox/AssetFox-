using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MoreLinq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Services.SummaryReport;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace BridgeCareCore.Services
{
    public class InvestmentBudgetsService : IInvestmentBudgetsService
    {
        private static UnitOfDataPersistenceWork _unitOfWork;
        private static IExpressionValidationService _expressionValidationService;

        public InvestmentBudgetsService(UnitOfDataPersistenceWork unitOfWork,
            IExpressionValidationService expressionValidationService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _expressionValidationService = expressionValidationService ??
                                           throw new ArgumentNullException(nameof(expressionValidationService));
        }

        private void AddHeaderCells(ExcelWorksheet worksheet, List<string> budgetNames)
        {
            var startRow = worksheet.Cells.Start.Row;
            var startCol = worksheet.Cells.Start.Column;

            worksheet.Cells[startRow, startCol].Value = "Year";

            budgetNames.ForEach(budgetName =>
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

            var headers = new List<string>
            {
                "Sample Budget 1",
                "Sample Budget 2",
                "Sample Budget 3",
                "Sample Budget 4"
            };

            AddHeaderCells(budgetWorksheet, headers);
            AddHeaderCells(criteriaWorksheet, new List<string>
            {
                "BUDGET_NAME", "CRITERIA"
            });

            var sampleBudgetAmountsPerYear = new Dictionary<int, List<decimal>>();

            var sampleBudgetAmounts = Enumerable.Repeat(decimal.Parse("5000000"), 4).ToList();
            var currentYear = DateTime.Now.Year;

            for (var i = 0; i < 4; i++)
            {
                sampleBudgetAmountsPerYear.Add(currentYear, sampleBudgetAmounts);
                currentYear++;
            }

            AddDataCells(budgetWorksheet, sampleBudgetAmountsPerYear);

            var currentRow = 2;
            var sampleCriteria = "[INTERSTATE]='Y'";
            headers.ForEach(budgetName =>
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

            if (budgetAmounts.Any())
            {
                var simulationName = _unitOfWork.Context.Simulation.Where(_ => _.Id == simulationId)
                    .Select(_ => new SimulationEntity {Name = _.Name}).AsNoTracking().Single().Name;

                var fileName = $"{simulationName.Trim().Replace(" ", "_")}_investment_budgets.xlsx";

                using var excelPackage = new ExcelPackage(new FileInfo(fileName));

                var worksheet = excelPackage.Workbook.Worksheets.Add("Sheet1");

                var budgetNames = budgetAmounts.Select(_ => _.ScenarioBudget.Name).Distinct().OrderBy(budgetName => budgetName)
                    .ToList();

                AddHeaderCells(worksheet, budgetNames);

                var budgetAmountsPerYear = budgetAmounts
                    .OrderBy(budgetAmount => budgetAmount.Year)
                    .GroupBy(budgetAmount => budgetAmount.Year, entities => entities)
                    .ToDictionary(group => group.Key, entities => entities
                        .OrderBy(budgetAmount => budgetAmount.ScenarioBudget.Name)
                        .Select(budgetAmount => budgetAmount.Value).ToList());

                AddDataCells(worksheet, budgetAmountsPerYear);

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

            if (budgetAmounts.Any())
            {
                var budgetLibraryName = _unitOfWork.Context.BudgetLibrary.Where(_ => _.Id == budgetLibraryId)
                    .Select(budgetLibrary => new BudgetLibraryEntity {Name = budgetLibrary.Name}).AsNoTracking().Single()
                    .Name;

                var fileName = $"{budgetLibraryName.Trim().Replace(" ", "_")}_investment_budgets.xlsx";

                using var excelPackage = new ExcelPackage(new FileInfo(fileName));

                var worksheet = excelPackage.Workbook.Worksheets.Add("Sheet1");

                var budgetNames = budgetAmounts.Select(_ => _.Budget.Name).Distinct().OrderBy(budgetName => budgetName)
                    .ToList();

                AddHeaderCells(worksheet, budgetNames);

                var budgetAmountsPerYear = budgetAmounts
                    .OrderBy(budgetAmount => budgetAmount.Year)
                    .GroupBy(budgetAmount => budgetAmount.Year, entities => entities)
                    .ToDictionary(group => group.Key, entities => entities
                        .OrderBy(budgetAmount => budgetAmount.Budget.Name)
                        .Select(budgetAmount => budgetAmount.Value).ToList());

                AddDataCells(worksheet, budgetAmountsPerYear);

                return new FileInfoDTO
                {
                    FileName = fileName,
                    FileData = Convert.ToBase64String(excelPackage.GetAsByteArray()),
                    MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                };
            }

            return CreateInvestmentBudgetsSampleFile();
        }


        public ScenarioBudgetImportResultDTO ImportScenarioInvestmentBudgetsFile(Guid simulationId, ExcelPackage excelPackage, UserCriteriaDTO currentUserCriteriaFilter)
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

            var existingBudgetEntities = _unitOfWork.BudgetRepo.GetScenarioBudgets(simulationId)
                .Select(_ => _.ToScenarioEntity(simulationId)).ToList();

            var existingBudgetNames = existingBudgetEntities.Select(existingBudget => existingBudget.Name).ToList();
            var newBudgetEntities = worksheetBudgetNames.Where(budgetName => !existingBudgetNames.Contains(budgetName))
                .Select(budgetName => new ScenarioBudgetEntity
                {
                    Id = Guid.NewGuid(), SimulationId = simulationId, Name = budgetName
                }).ToList();

            var budgetAmountsPerBudgetYearTuple = new Dictionary<(string, int), ScenarioBudgetAmountEntity>();
            existingBudgetEntities.ForEach(budget =>
            {
                budget.ScenarioBudgetAmounts.ForEach(budgetAmount =>
                    budgetAmountsPerBudgetYearTuple.Add((budget.Name, budgetAmount.Year), budgetAmount));
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

            _unitOfWork.Context.AddAll(newBudgetEntities, _unitOfWork.UserEntity?.Id);
            _unitOfWork.Context.AddAll(newBudgetAmountEntities, _unitOfWork.UserEntity?.Id);
            _unitOfWork.Context.UpdateAll(budgetAmountsPerBudgetYearTuple.Values.ToList(), _unitOfWork.UserEntity?.Id);

            var budgetsWithInvalidCriteria = new List<string>();
            if (criteriaPerBudgetName.Values.Any())
            {
                var allBudgetEntities = new List<ScenarioBudgetEntity>();
                allBudgetEntities.AddRange(existingBudgetEntities);
                allBudgetEntities.AddRange(newBudgetEntities);

                var budgetNames = criteriaPerBudgetName.Keys.ToList();
                _unitOfWork.Context.DeleteAll<CriterionLibraryEntity>(_ =>
                    _.IsSingleUse && _.CriterionLibraryScenarioBudgetJoins.All(join =>
                        join.ScenarioBudget.SimulationId == simulationId &&
                        budgetNames.Contains(join.ScenarioBudget.Name)));

                var criteria = new List<CriterionLibraryEntity>();
                var criteriaJoins = new List<CriterionLibraryScenarioBudgetEntity>();
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
                            criteriaJoins.Add(new CriterionLibraryScenarioBudgetEntity
                            {
                                CriterionLibraryId = criterionId,
                                ScenarioBudgetId = allBudgetEntities.Single(_ => _.Name == budgetName).Id
                            });
                        }
                        else
                        {
                            budgetsWithInvalidCriteria.Add(budgetName);
                        }
                    }
                });
                _unitOfWork.Context.AddAll(criteria, _unitOfWork.UserEntity?.Id);
                _unitOfWork.Context.AddAll(criteriaJoins, _unitOfWork.UserEntity?.Id);
            }


            return new ScenarioBudgetImportResultDTO
            {
                Budgets = _unitOfWork.BudgetRepo.GetScenarioBudgets(simulationId),
                WarningMessage = budgetsWithInvalidCriteria.Any()
                    ? $"The following budgets had invalid criteria: {string.Join(", ", budgetsWithInvalidCriteria)}"
                    : null
            };
        }

        public BudgetImportResultDTO ImportLibraryInvestmentBudgetsFile(Guid budgetLibraryId, ExcelPackage excelPackage, UserCriteriaDTO currentUserCriteriaFilter)
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

            var existingBudgetEntities = _unitOfWork.BudgetRepo.GetLibraryBudgets(budgetLibraryId);

            var existingBudgetNames = existingBudgetEntities.Select(existingBudget => existingBudget.Name).ToList();
            var newBudgetEntities = worksheetBudgetNames.Where(budgetName => !existingBudgetNames.Contains(budgetName))
                .Select(budgetName => new BudgetEntity
                {
                    Id = Guid.NewGuid(), BudgetLibraryId = budgetLibraryId, Name = budgetName
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

            _unitOfWork.Context.AddAll(newBudgetEntities, _unitOfWork.UserEntity?.Id);
            _unitOfWork.Context.AddAll(newBudgetAmountEntities, _unitOfWork.UserEntity?.Id);
            _unitOfWork.Context.UpdateAll(budgetAmountsPerBudgetYearTuple.Values.ToList(), _unitOfWork.UserEntity?.Id);

            var budgetsWithInvalidCriteria = new List<string>();
            if (criteriaPerBudgetName.Values.Any())
            {
                var allBudgetEntities = new List<BudgetEntity>();
                allBudgetEntities.AddRange(existingBudgetEntities);
                allBudgetEntities.AddRange(newBudgetEntities);

                var budgetNames = criteriaPerBudgetName.Keys.ToList();
                _unitOfWork.Context.DeleteAll<CriterionLibraryEntity>(_ =>
                    _.IsSingleUse && _.CriterionLibraryBudgetJoins.All(join =>
                        join.Budget.BudgetLibraryId == budgetLibraryId &&
                        budgetNames.Contains(join.Budget.Name)));

                var criteria = new List<CriterionLibraryEntity>();
                var criteriaJoins = new List<CriterionLibraryBudgetEntity>();
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
                            criteriaJoins.Add(new CriterionLibraryBudgetEntity
                            {
                                CriterionLibraryId = criterionId,
                                BudgetId = allBudgetEntities.Single(_ => _.Name == budgetName).Id
                            });
                        }
                        else
                        {
                            budgetsWithInvalidCriteria.Add(budgetName);
                        }
                    }

                });
                _unitOfWork.Context.AddAll(criteria, _unitOfWork.UserEntity?.Id);
                _unitOfWork.Context.AddAll(criteriaJoins, _unitOfWork.UserEntity?.Id);
            }

            return new BudgetImportResultDTO
            {
                BudgetLibrary = _unitOfWork.BudgetRepo.GetBudgetLibrary(budgetLibraryId),
                WarningMessage = budgetsWithInvalidCriteria.Any()
                    ? $"The following budgets had invalid criteria: {string.Join(", ", budgetsWithInvalidCriteria)}"
                    : null
            };
        }
    }
}
