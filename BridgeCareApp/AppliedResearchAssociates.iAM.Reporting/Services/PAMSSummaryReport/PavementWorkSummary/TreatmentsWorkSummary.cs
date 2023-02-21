using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis;
using OfficeOpenXml;
using AppliedResearchAssociates.iAM.Reporting.Models.PAMSSummaryReport;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using System.Drawing;
using System;
using System.Linq;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.StaticContent;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.PavementWorkSummary
{
    public class TreatmentsWorkSummary
    {
        private PavementWorkSummaryCommon _pavementWorkSummaryCommon;

        public TreatmentsWorkSummary()
        {
            _pavementWorkSummaryCommon = new PavementWorkSummaryCommon();
        }

        public ChartRowsModel FillTreatmentsWorkSummarySections(
            ExcelWorksheet worksheet,
            CurrentCell currentCell,
            List<int> simulationYears,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int count)>> costAndLengthPerTreatmentPerYear,
            Dictionary<int, Dictionary<PavementTreatmentHelper.TreatmentGroup, (decimal treatmentCost, int length)>> costAndLengthPerTreatmentGroupPerYear,
            List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> simulationTreatments,
            Dictionary<TreatmentCategory, SortedDictionary<int, (decimal treatmentCost, int length)>> workTypeTotals,
            ChartRowsModel chartRowsModel
            )

        {
            FillFullDepthAsphaltTreatments(worksheet, currentCell, simulationYears, costAndLengthPerTreatmentPerYear, simulationTreatments);
            FillCompositeTreatments(worksheet, currentCell, simulationYears, costAndLengthPerTreatmentPerYear, simulationTreatments);
            FillConcreteTreatments(worksheet, currentCell, simulationYears, costAndLengthPerTreatmentPerYear, simulationTreatments);
            FillTreatmentGroups(worksheet, currentCell, simulationYears, costAndLengthPerTreatmentGroupPerYear);

            FillWorkTypeTotalsSection(worksheet, currentCell, simulationYears, workTypeTotals);

            return chartRowsModel;
        }

        private void FillFullDepthAsphaltTreatments(
            ExcelWorksheet worksheet,
            CurrentCell currentCell,
            List<int> simulationYears,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int length)>> lengthPerTreatmentPerYear,
            List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> simulationTreatments
            )
        {
            _pavementWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Segment Miles of Full Depth Asphalt Pavement Treatments", "PAMS Full Depth Asphalt Treatments");

            var asphaltTreatments = _pavementWorkSummaryCommon.GetNoTreatments(simulationTreatments).Concat(_pavementWorkSummaryCommon.GetAsphaltTreatments(simulationTreatments)).ToList();

            AddFullDepthAsphaltTreatmentSegmentMiles(worksheet, currentCell,
                lengthPerTreatmentPerYear,
                asphaltTreatments
                );
        }


        private void AddFullDepthAsphaltTreatmentSegmentMiles(ExcelWorksheet worksheet, CurrentCell currentCell,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int length)>> lengthPerTreatmentPerYear,
            List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> simulationTreatments
            )
        {
            int startRow, startColumn, row, column;
            _pavementWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            _pavementWorkSummaryCommon.SetPavementTreatmentExcelString(worksheet, simulationTreatments, ref row, ref column);
            worksheet.Cells[row++, column].Value = PAMSConstants.AsphaltTotal;
            column++;
            var fromColumn = column + 1;
            foreach (var yearlyValues in lengthPerTreatmentPerYear)
            {
                row = startRow;
                column = ++column;
                double totalLength = 0;
                foreach (var treatment in simulationTreatments)
                {
                    yearlyValues.Value.TryGetValue(treatment.Name, out var costAndLength);
                    var treatmentLength = costAndLength.length.FeetToMiles();
                    totalLength += treatmentLength;
                    worksheet.Cells[row, column].Value = treatmentLength;
                    row++;
                }
                worksheet.Cells[row, column].Value = totalLength;
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.FromArgb(180, 198, 231)); // treatment rows
            ExcelHelper.ApplyColor(worksheet.Cells[row, fromColumn, row, column], Color.FromArgb(132, 151, 176)); // total row

            _pavementWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
        }
 


    private void FillCompositeTreatments(
        ExcelWorksheet worksheet,
        CurrentCell currentCell,
        List<int> simulationYears,
        Dictionary<int, Dictionary<string, (decimal treatmentCost, int length)>> lengthPerTreatmentPerYear,
        List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> simulationTreatments
        )
    {
        _pavementWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Segment Miles of Composite Pavement Treatments", "PAMS Composite Treatments");

            var asphaltTreatments = _pavementWorkSummaryCommon.GetNoTreatments(simulationTreatments).Concat(_pavementWorkSummaryCommon.GetAsphaltTreatments(simulationTreatments)).ToList();


            AddCompositeTreatmentSegmentMiles(worksheet, currentCell,
            lengthPerTreatmentPerYear,
            asphaltTreatments
            );
    }


    private void AddCompositeTreatmentSegmentMiles(ExcelWorksheet worksheet, CurrentCell currentCell,
        Dictionary<int, Dictionary<string, (decimal treatmentCost, int length)>> lengthPerTreatmentPerYear,
        List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> simulationTreatments
        )
    {
        int startRow, startColumn, row, column;
        _pavementWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
        _pavementWorkSummaryCommon.SetPavementTreatmentExcelString(worksheet, simulationTreatments, ref row, ref column);
        worksheet.Cells[row++, column].Value = PAMSConstants.CompositeTotal;
        column++;
        var fromColumn = column + 1;
        foreach (var yearlyValues in lengthPerTreatmentPerYear)
        {
            row = startRow;
            column = ++column;
            double totalLength = 0;
            foreach (var treatment in simulationTreatments)
            {
                yearlyValues.Value.TryGetValue(treatment.Name, out var costAndLength);
                var treatmentLength = costAndLength.length.FeetToMiles();
                totalLength += treatmentLength;
                worksheet.Cells[row, column].Value = treatmentLength;
                row++;
            }
            worksheet.Cells[row, column].Value = totalLength;
        }
        ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
        ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.FromArgb(180, 198, 231));
            ExcelHelper.ApplyColor(worksheet.Cells[row, fromColumn, row, column], Color.FromArgb(132, 151, 176));
            _pavementWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
    }



        private void FillConcreteTreatments(
            ExcelWorksheet worksheet,
            CurrentCell currentCell,
            List<int> simulationYears,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int length)>> lengthPerTreatmentPerYear,
            List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> simulationTreatments
            )
        {
            var concreteTreatments = _pavementWorkSummaryCommon.GetNoTreatments(simulationTreatments).Concat(_pavementWorkSummaryCommon.GetConcreteTreatments(simulationTreatments)).ToList();

            _pavementWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Segment Miles of Concrete Pavement Treatments", "PAMS Concrete Treatments");
            AddConcreteTreatmentSegmentMiles(worksheet, currentCell,
                lengthPerTreatmentPerYear,
                concreteTreatments
                );
        }


        private void AddConcreteTreatmentSegmentMiles(ExcelWorksheet worksheet, CurrentCell currentCell,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int length)>> lengthPerTreatmentPerYear,
            List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> simulationTreatments
            )
        {
            int startRow, startColumn, row, column;
            _pavementWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            _pavementWorkSummaryCommon.SetPavementTreatmentExcelString(worksheet, simulationTreatments, ref row, ref column);
            worksheet.Cells[row++, column].Value = PAMSConstants.ConcreteTotal;
            column++;
            var fromColumn = column + 1;
            foreach (var yearlyValues in lengthPerTreatmentPerYear)
            {
                row = startRow;
                column = ++column;
                double totalLength = 0;
                foreach (var treatment in simulationTreatments)
                {
                    yearlyValues.Value.TryGetValue(treatment.Name, out var costAndLength);
                    var treatmentLength = costAndLength.length.FeetToMiles();
                    totalLength += treatmentLength;
                    worksheet.Cells[row, column].Value = treatmentLength;
                    row++;
                }
                worksheet.Cells[row, column].Value = totalLength;
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.FromArgb(180, 198, 231));
            ExcelHelper.ApplyColor(worksheet.Cells[row, fromColumn, row, column], Color.FromArgb(132, 151, 176));

            _pavementWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
        }

        private void FillTreatmentGroups(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears,
            Dictionary<int, Dictionary<PavementTreatmentHelper.TreatmentGroup, (decimal treatmentCost, int length)>> costAndLengthPerTreatmentGroupPerYear
            )
        {
            //var workTypeConcrete = new Dictionary<TreatmentCategory, SortedDictionary<int, decimal>>();
            if (simulationYears.Count <= 0)
            {
                return;// workTypeConcrete;
            }
            var headerRange = new Range(currentCell.Row, currentCell.Row + 1);
            _pavementWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Segment Miles of Treatment Groups", "PAMS Treatment Groups Totals");

            AddTreatmentGroupTotalDetails(worksheet, currentCell, costAndLengthPerTreatmentGroupPerYear, PavementTreatmentHelper.TreatmentGroupCategory.Bituminous);
            AddTreatmentGroupTotalDetails(worksheet, currentCell, costAndLengthPerTreatmentGroupPerYear, PavementTreatmentHelper.TreatmentGroupCategory.Concrete);
        }


        private void AddTreatmentGroupTotalDetails(ExcelWorksheet worksheet, CurrentCell currentCell,
            Dictionary<int, Dictionary<PavementTreatmentHelper.TreatmentGroup, (decimal treatmentCost, int length)>> costAndLengthPerTreatmentGroupPerYear,
            PavementTreatmentHelper.TreatmentGroupCategory treatmentGroupCategory)
        {
            int startRow, startColumn, row, column;
            _pavementWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);

            var treatmentGroups = PavementTreatmentHelper.GetListOfTreatmentGroupForCategory(treatmentGroupCategory);

            var prefix = PavementTreatmentHelper.GetTreatmentGroupString(treatmentGroupCategory) + " - ";
            var treatmentGroupTitles = treatmentGroups.Select(tg => prefix + tg.GroupDescription).ToList();

            _pavementWorkSummaryCommon.SetPavementTreatmentGroupsExcelString(worksheet, treatmentGroupTitles, ref row, ref column);

            column++;
            var fromColumn = column + 1;

            foreach (var yearlyValues in costAndLengthPerTreatmentGroupPerYear)
            {
                row = startRow;
                column = ++column;
                foreach (var treatmentGroup in treatmentGroups)
                {
                    yearlyValues.Value.TryGetValue(treatmentGroup, out var costAndLength);
                    worksheet.Cells[row, column].Value = costAndLength.length.FeetToMiles();
                    row++;
                }
            }
            row--;

            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.FromArgb(180, 198, 231));

            _pavementWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
        }



        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> FillWorkTypeTotalsSection(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears,
            Dictionary<TreatmentCategory, SortedDictionary<int, (decimal treatmentCost, int length)>> workTypeTotals
            )
        {
            var workTypesForReport = new List<TreatmentCategory> { TreatmentCategory.Maintenance, TreatmentCategory.Preservation, TreatmentCategory.Rehabilitation, TreatmentCategory.Replacement };
            var headerRange = new Range(currentCell.Row, currentCell.Row + 1);
            _pavementWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Total Number of Segment Miles", "Work Type Totals");

            int startRow, startColumn, row, column;
            _pavementWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);

            var workTypeTitles = workTypesForReport.Select(tc => tc.ToSpreadsheetString()).ToList();
            workTypeTitles.Add("Total");

            _pavementWorkSummaryCommon.SetPavementTreatmentGroupsExcelString(worksheet, workTypeTitles, ref row, ref column);

            column++;
            var fromColumn = column + 1;

            row = startRow;

            var columnTotals = new Dictionary<int, decimal>();

            foreach (var workType in workTypesForReport)
            {
                column = fromColumn;

                var workTypeTotalExists = workTypeTotals.TryGetValue(workType, out var workTypeTotal);

                foreach (var year in simulationYears)
                {
                    if (!columnTotals.ContainsKey(year))
                    {
                        columnTotals.Add(year, 0);
                    }
                    if (workTypeTotalExists && (workTypeTotal.TryGetValue(year, out var costAndLength)))
                    {
                        worksheet.Cells[row, column].Value = costAndLength.length;
                        columnTotals[year] += costAndLength.length;
                    }
                    else
                    {
                        worksheet.Cells[row, column].Value = 0.0;
                    }
                    column++;
                }

                row++;
            }

            // Add Total Row
            column = fromColumn;
            foreach (var year in simulationYears)
            {
                worksheet.Cells[row, column].Value = columnTotals[year];
                column++;
            }

            column = fromColumn + simulationYears.Count;

            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column - 1]);
            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column - 1], Color.FromArgb(132, 151, 176));

            ExcelHelper.ApplyColor(worksheet.Cells[row + 2, startColumn, row + 2, column - 1], Color.FromArgb(89, 89, 89));

            _pavementWorkSummaryCommon.UpdateCurrentCell(currentCell, row + 4, column);

            return null;
        }


    };




}
