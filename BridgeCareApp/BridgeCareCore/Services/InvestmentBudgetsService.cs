using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using MoreLinq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Interfaces.SummaryReport;
using BridgeCareCore.Services.SummaryReport;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace BridgeCareCore.Services
{
    public class InvestmentBudgetsService : IInvestmentBudgetsService
    {
        private static UnitOfDataPersistenceWork _unitOfWork;
        private static IExcelHelper _excelHelper;

        public InvestmentBudgetsService(UnitOfDataPersistenceWork unitOfWork, IExcelHelper excelHelper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _excelHelper = excelHelper ?? throw new ArgumentNullException(nameof(excelHelper));
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

            _excelHelper.SetCustomFormat(
                worksheet.Cells[2, 2, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column],
                ExcelHelperCellFormat.Accounting);
        }

        private FileInfoDTO CreateInvestmentBudgetsSampleFile()
        {
            var fileName = "sample_investment_budgets_import_export_file.xlsx";

            using var excelPackage = new ExcelPackage(new FileInfo(fileName));

            var worksheet = excelPackage.Workbook.Worksheets.Add("Sheet1");

            var headers = new List<string>
            {
                "Sample Budget 1",
                "Sample Budget 2",
                "Sample Budget 3",
                "Sample Budget 4"
            };

            AddHeaderCells(worksheet, headers);

            var sampleBudgetAmountsPerYear = new Dictionary<int, List<decimal>>();

            var sampleBudgetAmounts = Enumerable.Repeat(decimal.Parse("5000000"), 4).ToList();
            var currentYear = DateTime.Now.Year;

            for (var i = 0; i < 4; i++)
            {
                sampleBudgetAmountsPerYear.Add(currentYear, sampleBudgetAmounts);
                currentYear++;
            }

            AddDataCells(worksheet, sampleBudgetAmountsPerYear);

            return new FileInfoDTO
            {
                FileName = fileName,
                FileData = Convert.ToBase64String(excelPackage.GetAsByteArray()),
                MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            };
        }

        public FileInfoDTO ExportInvestmentBudgetsFile(Guid budgetLibraryId)
        {
            var budgetAmounts = _unitOfWork.BudgetAmountRepo.GetBudgetAmountsByBudgetLibraryId(budgetLibraryId);

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


        public BudgetLibraryDTO ImportInvestmentBudgetsFile(Guid budgetLibraryId, ExcelPackage excelPackage)
        {
            var worksheet = excelPackage.Workbook.Worksheets[0];
            var worksheetEnd = worksheet.Dimension.End;

            var worksheetBudgetNames = worksheet.Cells[1, 2, 1, worksheetEnd.Column]
                .Select(cell => cell.GetValue<string>()).ToList();

            var existingBudgetEntities = _unitOfWork.BudgetRepo.GetBudgetsWithBudgetAmounts(budgetLibraryId);

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
            for (var row = 2; row <= worksheetEnd.Row; row++)
            {
                for (var col = 2; col <= worksheetEnd.Column; col++)
                {
                    var budgetName = worksheet.GetValue<string>(1, col);
                    var year = worksheet.GetValue<int>(row, 1);
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
                            Value = worksheet.GetValue<decimal>(row, col)
                        });
                    }
                    else
                    {
                        budgetAmountsPerBudgetYearTuple[budgetYearTuple].Value = worksheet.GetValue<decimal>(row, col);
                    }
                }
            }

            _unitOfWork.Context.AddAll(newBudgetEntities);
            _unitOfWork.Context.AddAll(newBudgetAmountEntities);
            _unitOfWork.Context.UpdateAll(budgetAmountsPerBudgetYearTuple.Values.ToList());

            return _unitOfWork.BudgetRepo.GetBudgetLibraryWithBudgetsAndBudgetAmounts(budgetLibraryId);
        }
    }
}
