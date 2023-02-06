using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Interfaces.DefaultData;
using MoreLinq;
using OfficeOpenXml;

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
                var simulationName = _unitOfWork.SimulationRepo.GetSimulationName(simulationId);
                if (simulationName == null)
                {
                    throw new InvalidOperationException($"No simulation with id {simulationId}");
                }

                var fileName = $"{simulationName.Trim().Replace(" ", "_")}_investment_budgets.xlsx";

                using var excelPackage = new ExcelPackage(new FileInfo(fileName));

                var budgetWorksheet = excelPackage.Workbook.Worksheets.Add("Budget");

                var budgetNames = budgetAmounts.Select(_ => _.BudgetName).Distinct().OrderBy(budgetName => budgetName)
                    .ToList();

                AddHeaderCells(budgetWorksheet, budgetNames);

                var budgetAmountsPerYear = budgetAmounts
                    .OrderBy(budgetAmount => budgetAmount.Year)
                    .GroupBy(budgetAmount => budgetAmount.Year, dto => dto)
                    .ToDictionary(group => group.Key, dtos => dtos
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

                var budgetNames = budgetAmounts.Select(_ => _.BudgetName).Distinct().OrderBy(budgetName => budgetName)
                    .ToList();

                AddHeaderCells(budgetWorksheet, budgetNames);

                var budgetAmountsPerYear = budgetAmounts
                    .OrderBy(budgetAmount => budgetAmount.Year)
                    .GroupBy(budgetAmount => budgetAmount.Year, dto => dto)
                    .ToDictionary(group => group.Key, dtos => dtos
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

                _unitOfWork.BudgetRepo.DeleteAllScenarioBudgetsForSimulation(simulationId);
            }
            var existingBudgets = _unitOfWork.BudgetRepo.GetScenarioBudgets(simulationId);

            var existingBudgetNames = existingBudgets.Select(existingBudget => existingBudget.Name).ToList();
            var newBudgets = worksheetBudgetNames.Where(budgetName => !existingBudgetNames.Contains(budgetName))
                .Select(budgetName => new BudgetDTO
                {
                    Id = Guid.NewGuid(),
                    Name = budgetName
                }).ToList();

            var budgetAmountsPerBudgetYearTuple = new Dictionary<(string, int), BudgetAmountDTOWithBudgetId>();
            existingBudgets.ForEach(budget =>
            {
                budget.BudgetAmounts.ForEach(budgetAmount =>
                {
                    // The if condition is to avoid any duplicate records coming from the DB.
                    // Ideally the database must have only 1 record per Budget name per year.
                    if (!budgetAmountsPerBudgetYearTuple.ContainsKey((budget.Name, budgetAmount.Year)))
                    {
                        var budgetAmountDtoWithBudgetId = new BudgetAmountDTOWithBudgetId
                        {
                            BudgetAmount = budgetAmount,
                            BudgetId = budget.Id,
                        };
                        budgetAmountsPerBudgetYearTuple.Add((budget.Name, budgetAmount.Year), budgetAmountDtoWithBudgetId);
                    }
                });
            });

            var newBudgetAmounts = new List<BudgetAmountDTOWithBudgetId>();
            for (var row = 2; row <= budgetWorksheetEnd.Row; row++)
            {
                for (var col = 2; col <= budgetWorksheetEnd.Column; col++)
                {
                    var budgetName = budgetWorksheet.GetValue<string>(1, col);
                    var year = budgetWorksheet.GetValue<int>(row, 1);
                    var budgetYearTuple = (budgetName, year);
                    if (!budgetAmountsPerBudgetYearTuple.ContainsKey(budgetYearTuple))
                    {
                        var budgetId = existingBudgets.Any(_ => _.Name == budgetYearTuple.budgetName)
                            ? existingBudgets
                                .Single(_ => _.Name == budgetYearTuple.budgetName).Id
                            : newBudgets
                                .Single(_ => _.Name == budgetYearTuple.budgetName).Id;

                        newBudgetAmounts.Add(new BudgetAmountDTOWithBudgetId
                        {
                            BudgetId = budgetId,
                            BudgetAmount = new BudgetAmountDTO
                            {
                                Id = Guid.NewGuid(),
                                Year = budgetYearTuple.year,
                                Value = budgetWorksheet.GetValue<decimal>(row, col)
                            }
                        });
                    }
                    else
                    {
                        budgetAmountsPerBudgetYearTuple[budgetYearTuple].BudgetAmount.Value = budgetWorksheet.GetValue<decimal>(row, col);
                    }
                }
            }

            _unitOfWork.BudgetRepo.AddScenarioBudgets(simulationId, newBudgets);
            _unitOfWork.BudgetRepo.AddScenarioBudgetAmounts(newBudgetAmounts);
            var values = budgetAmountsPerBudgetYearTuple.Values.ToList();
            _unitOfWork.BudgetRepo.UpdateScenarioBudgetAmounts(simulationId, values);

            var budgetsWithInvalidCriteria = new List<string>();
            var criteriaBudgetNamesNotPresentInBudgetTab = new List<string>();
            if (criteriaPerBudgetName.Values.Any())
            {
                var allBudgets = new List<BudgetDTO>();
                allBudgets.AddRange(existingBudgets);
                allBudgets.AddRange(newBudgets);

                var budgetNames = criteriaPerBudgetName.Keys.ToList();

                _unitOfWork.CriterionLibraryRepo.DeleteAllSingleUseCriterionLibrariesWithBudgetNamesForSimulation(simulationId, budgetNames);

                var criteria = new List<CriterionLibraryDTO>();
                var criteriaJoins = new List<CriterionLibraryScenarioBudgetDTO>();
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
                            criteria.Add(new CriterionLibraryDTO
                            {
                                Id = criterionId,
                                IsSingleUse = true,
                                Name = $"{budgetName} Criterion Library",
                                MergedCriteriaExpression = criterion
                            });
                            try
                            {
                                criteriaJoins.Add(new CriterionLibraryScenarioBudgetDTO
                                {
                                    CriterionLibraryId = criterionId,
                                    ScenarioBudgetId = allBudgets.Single(_ => _.Name.ToUpperInvariant() == budgetName.ToUpperInvariant()).Id
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
                _unitOfWork.CriterionLibraryRepo.AddLibraries(criteria);
                _unitOfWork.CriterionLibraryRepo.AddLibraryScenarioBudgetJoins(criteriaJoins);
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
                    _unitOfWork.CriterionLibraryRepo.DeleteAllSingleUseCriterionLibrariesWithBudgetNamesForBudgetLibrary(budgetLibraryId, budgetNames);
                }

                _unitOfWork.BudgetRepo.DeleteAllBudgetsForLibrary(budgetLibraryId);
            }

            var existingBudgets = _unitOfWork.BudgetRepo.GetLibraryBudgets(budgetLibraryId);

            var existingBudgetNames = existingBudgets.Select(existingBudget => existingBudget.Name).ToList();
            var newBudgets = worksheetBudgetNames.Where(budgetName => !existingBudgetNames.Contains(budgetName))
                .Select(budgetName => new BudgetDTOWithLibraryId
                {
                    BudgetLibraryId = budgetLibraryId,
                    Budget = new BudgetDTO
                    {
                        Id = Guid.NewGuid(),
                        Name = budgetName
                    },
                }).ToList();

            var budgetAmountsPerBudgetYearTuple = new Dictionary<(string, int), BudgetAmountDTOWithBudgetId>();
            existingBudgets.ForEach(budget =>
            {
                budget.BudgetAmounts.ForEach(budgetAmount =>
                    budgetAmountsPerBudgetYearTuple.Add((budget.Name, budgetAmount.Year), new BudgetAmountDTOWithBudgetId
                    {
                        BudgetAmount = budgetAmount,
                        BudgetId = budget.Id,
                    }));
            });

            var newBudgetAmounts = new List<BudgetAmountDTOWithBudgetId>();
            for (var row = 2; row <= budgetWorksheetEnd.Row; row++)
            {
                for (var col = 2; col <= budgetWorksheetEnd.Column; col++)
                {
                    var budgetName = budgetWorksheet.GetValue<string>(1, col);
                    var year = budgetWorksheet.GetValue<int>(row, 1);
                    var budgetYearTuple = (budgetName, year);
                    if (!budgetAmountsPerBudgetYearTuple.ContainsKey(budgetYearTuple))
                    {
                        var budgetId = existingBudgets.Any(_ => _.Name == budgetYearTuple.budgetName)
                            ? existingBudgets
                                .Single(_ => _.Name == budgetYearTuple.budgetName).Id
                            : newBudgets
                                .Single(_ => _.Budget.Name == budgetYearTuple.budgetName).Budget.Id;

                        newBudgetAmounts.Add(new BudgetAmountDTOWithBudgetId
                        {
                            BudgetId = budgetId,
                            BudgetAmount = new BudgetAmountDTO
                            {
                                Id = Guid.NewGuid(),
                                Year = budgetYearTuple.year,
                                Value = budgetWorksheet.GetValue<decimal>(row, col),
                            }
                        });
                    }
                    else
                    {
                        budgetAmountsPerBudgetYearTuple[budgetYearTuple].BudgetAmount.Value = budgetWorksheet.GetValue<decimal>(row, col);
                    }
                }
            }

            _unitOfWork.BudgetRepo.AddBudgets(newBudgets);
            _unitOfWork.BudgetRepo.AddLibraryBudgetAmounts(newBudgetAmounts);
            var values = budgetAmountsPerBudgetYearTuple.Values.ToList();
            _unitOfWork.BudgetRepo.UpdateLibraryBudgetAmounts(values);

            var budgetsWithInvalidCriteria = new List<string>();
            var criteriaBudgetNamesNotPresentInBudgetTab = new List<string>();
            if (criteriaPerBudgetName.Values.Any())
            {
                var allBudgets = new List<BudgetDTO>();
                allBudgets.AddRange(existingBudgets);
                allBudgets.AddRange(newBudgets.Select(b => b.Budget));

                var budgetNames = criteriaPerBudgetName.Keys.ToList();
                _unitOfWork.CriterionLibraryRepo.DeleteAllSingleUseCriterionLibrariesWithBudgetNamesForBudgetLibrary(budgetLibraryId, budgetNames);

                var criteria = new List<CriterionLibraryDTO>();
                var criteriaJoins = new List<CriterionLibraryBudgetDTO>();
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
                            criteria.Add(new CriterionLibraryDTO
                            {
                                Id = criterionId,
                                IsSingleUse = true,
                                Name = $"{budgetName} Criterion Library",
                                MergedCriteriaExpression = criterion
                            });
                            
                            try
                            {
                                criteriaJoins.Add(new CriterionLibraryBudgetDTO
                                {
                                    CriterionLibraryId = criterionId,
                                    BudgetId = allBudgets.Single(_ => _.Name == budgetName).Id
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
                _unitOfWork.CriterionLibraryRepo.AddLibraries(criteria);
                _unitOfWork.CriterionLibraryRepo.AddLibraryBudgetJoins(criteriaJoins);
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
