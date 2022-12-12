using System;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml;

using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text;
using static System.Collections.Specialized.BitVector32;
using static Antlr4.Runtime.Atn.SemanticContext;
using static AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.DistrictCountyTotals.DistrictTotalsRowModels;
using static OfficeOpenXml.ExcelErrorValue;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.DistrictCountyTotals
{
    public static class DistrictTotalsRowModels
    {
        internal static ExcelRowModel DistrictCountyAndYearsHeaders(SimulationOutput output, params string[] additionalHeaders)
        {
            var values = new List<IExcelModel>
            {
                StackedExcelModels.LeftHeader("District"),
                StackedExcelModels.LeftHeader("County"),
            };
            foreach (var year in output.Years)
            {
                values.Add(
                    StackedExcelModels.Stacked(
                        ExcelValueModels.Integer(year.Year),
                        ExcelStyleModels.CenteredHeader));
            }
            foreach (var header in additionalHeaders)
            {
                values.Add(StackedExcelModels.LeftHeader(header));
            }
            return ExcelRowModels.WithEntries(values, ExcelStyleModels.ThinBorder);
        }

        internal static List<string> CountiesForDistrict(int districtNumber)
        {
            switch (districtNumber)
            {
                case 1: return new List<string> {
                    "Crawford",
                    "Erie",
                    "Forest",
                    "Mercer",
                    "Venango",
                    "Warren"
                };
                case 2: return new List<string> {
                    "Cameron",
                    "Centre",
                    "Clearfield",
                    "Clinton",
                    "Elk",
                    "Juniata",
                    "McKean",
                    "Mifflin",
                    "Potter"
                };
                case 3: return new List<string> {
                    "Bradford",
                    "Columbia",
                    "Lycoming",
                    "Montour",
                    "Northumberland",
                    "Snyder",
                    "Sullivan",
                    "Tioga",
                    "Union"
                };
                case 4: return new List<string> {
                    "Lackawanna",
                    "Luzerne",
                    "Pike",
                    "Susquehanna",
                    "Wayne",
                    "Wyoming"
                };
                case 5: return new List<string> {
                    "Berks",
                    "Carbon",
                    "Lehigh",
                    "Monroe",
                    "Northampton",
                    "Schuylkill"
                };
                case 6: return new List<string> {
                    "Bucks",
                    "Chester",
                    "Delaware",
                    "Montgomery",
                    "Philadelphia"
                };
                case 8: return new List<string> {
                    "Adams",
                    "Cumberland",
                    "Dauphin",
                    "Franklin",
                    "Lancaster",
                    "Lebanon",
                    "Perry",
                    "York"
                };
                case 9: return new List<string> {
                    "Bedford",
                    "Blair",
                    "Cambria",
                    "Fulton",
                    "Huntingdon",
                    "Somerset"
                };
                case 10: return new List<string> {
                    "Armstrong",
                    "Butler",
                    "Clarion",
                    "Indiana",
                    "Jefferson"
                };
                case 11: return new List<string> {
                    "Allegheny",
                    "Beaver",
                    "Lawrence"
                 };
                case 12: return new List<string> {
                    "Fayette",
                    "Greene",
                    "Washington",
                    "Westmoreland"
                };
                default: return new List<string> {
                    "Undefined"
                };
            }
        }

        public class CountyRow
        {
            public int District { get; internal set; }
            public string County { get; internal set; }
            public List<decimal> rowEntries { get; internal set; }
        }

        internal static List<CountyRow> DistrictCountyValues(SimulationOutput output, int district, Func<AssetDetail, int, string, bool> districtCountyFunction)
        {
            var counties = CountiesForDistrict(district);

            var countyRows = new List<CountyRow>();
            foreach (var county in counties)
            {
                Func<AssetDetail, bool> predicate = detail => districtCountyFunction(detail, district, county);
                var values = output.Years.Select(year => DistrictTotalsExcelModels.DistrictTableContentValue(year, predicate)).ToList();
                var rowModel = new CountyRow
                {
                    District = district,
                    County = county.ToUpper(),
                    rowEntries = values,
                };
                countyRows.Add(rowModel);
            }

            return countyRows;
        }

        internal static ExcelRowModel DistrictCountyRowToExcelRowModel(CountyRow countyRow)
        {
            var districtLabel = StackedExcelModels.Stacked(
                ExcelValueModels.Integer(countyRow.District),
                ExcelStyleModels.HorizontalCenter,
                ExcelStyleModels.ThinBorder
                );
            var countyLabel = StackedExcelModels.Stacked(
                ExcelValueModels.String(countyRow.County),
                ExcelStyleModels.Left,
                ExcelStyleModels.ThinBorder
                );
            var cellModels = new List<IExcelModel>
                {
                    districtLabel,
                    countyLabel
                };
            cellModels.AddRange(countyRow.rowEntries.Select(rowEntry => StackedExcelModels.Stacked(
                ExcelValueModels.Money(rowEntry),
                ExcelStyleModels.Right,
                ExcelStyleModels.ThinBorder,
                ExcelStyleModels.CurrencyWithoutCentsFormat,
                DistrictTotalsStyleModels.LightGreenFill
                )));
            var excelRowModel = ExcelRowModels.WithEntries(cellModels);

            return excelRowModel;
        }

        internal static List<ExcelRowModel> DistrictCountyRowsToExcelRowModels(List<CountyRow> countyRows) =>
            countyRows.Select(countyRow => DistrictCountyRowToExcelRowModel(countyRow)).ToList();

        internal static ExcelRowModel TurnpikeRowToExcelRowModel(CountyRow countyRow)
        {
            var excelRowModel = ExcelRowModels.RightHeader(0, "Turnpike", 2, 1);

            var cellModels = countyRow.rowEntries.Select(rowEntry => StackedExcelModels.Stacked(
                ExcelValueModels.Money(rowEntry),
                ExcelStyleModels.Right,
                ExcelStyleModels.ThinBorder,
                ExcelStyleModels.CurrencyWithoutCentsFormat,
                DistrictTotalsStyleModels.LightGreenFill
                )).ToList();
            excelRowModel.AddCells(cellModels);

            return excelRowModel;
        }

        internal static List<CountyRow> MpmsTableDistrictValues(SimulationOutput output, int district)
        {
            Func<AssetDetail, int, string, bool> predicate = (detail, district, county) =>
                    DistrictTotalsSectionDetailPredicates.IsNumberedDistrictMpmsTable(detail, district) &&
                    DistrictTotalsSectionDetailPredicates.IsCounty(detail, county);
            var countyRows = DistrictCountyValues(output, district, predicate);
            return countyRows;
         }

        internal static List<CountyRow> BamsTableDistrictValues(SimulationOutput output, int district)
        {
            Func<AssetDetail, int, string, bool> predicate = (detail, district, county) =>
                DistrictTotalsSectionDetailPredicates.IsNumberedDistrictBamsTable(detail, district) &&
                DistrictTotalsSectionDetailPredicates.IsCounty(detail, county);
            var countyRows = DistrictCountyValues(output, district, predicate);
            return countyRows;
        }

        internal static List<CountyRow> OverallDollarsTableDistrictValues(SimulationOutput output, int district)
        {
            Func<AssetDetail, int, string, bool> predicate = (detail, district, county ) =>
                DistrictTotalsSectionDetailPredicates.IsDistrictNotTurnpike(detail, district) &&
                DistrictTotalsSectionDetailPredicates.IsCounty(detail, county);
            var countyRows = DistrictCountyValues(output, district, predicate);
            return countyRows;
        }

        public static CountyRow TurnpikeRowValue(SimulationOutput output, Func<AssetDetail, bool> predicate)
        {
            var values = output.Years.Select(year => DistrictTotalsExcelModels.DistrictTableContentValue(year, predicate)).ToList();
            var turnpikeRow = new CountyRow
            {
                District = 0,
                County = "Turnpike",
                rowEntries = values,
            };
            return turnpikeRow;
        }

        public static CountyRow MpmsTurnpikeRowValue(SimulationOutput output) =>
            TurnpikeRowValue(output, DistrictTotalsSectionDetailPredicates.IsCommittedTurnpike);

        public static CountyRow BamsTurnpikeRowValue(SimulationOutput output) =>
            TurnpikeRowValue(output, DistrictTotalsSectionDetailPredicates.IsTurnpikeButNotCommitted);

        internal static CountyRow OverallDollarsTableTurnpike(SimulationOutput output) =>
            TurnpikeRowValue(output, DistrictTotalsSectionDetailPredicates.IsTurnpike);

        public static ExcelFormulaModel TotalSumFormula(int column, List<int> rowIndices)
        {
            var columnLetter = ExcelCellAddress.GetColumnLetter(column);
            var builder = new StringBuilder("SUM(");

            foreach (var rowIndex in rowIndices)
            {
                builder.Append(columnLetter);
                builder.Append(rowIndex.ToString());
                builder.Append(",");
            }
            builder.Remove(builder.Length - 1, 1);
            builder.Append(")");

            var formula = ExcelFormulaModels.Text(builder.ToString());
            return formula;
        }
            

        public static ExcelRowModel TableStateTotal(SimulationOutput output, List<int> districtTotalRowIndices)
        {
            var totalRow = ExcelRowModels.RightHeader(0, "State Total", 2, 1);
            var entries = new List<IExcelModel>();

            var turnpikeRow = districtTotalRowIndices.Last() + 1;
            districtTotalRowIndices.Add(turnpikeRow);

            for (int i = 0; i < output.Years.Count; i++)
            {
                var sumFormula = TotalSumFormula(i + 3, districtTotalRowIndices);

                var styledFormula = StackedExcelModels.Stacked(
                    sumFormula,
                    DistrictTotalsStyleModels.DarkGreenTotalsCells
                    );

                entries.Add(styledFormula);
            }
            totalRow.AddCells(entries);

            return totalRow;
        }

        public static ExcelRowModel TableStateTotal(List<decimal> totals)
        {
            var totalRow = ExcelRowModels.RightHeader(0, "State Total", 2, 1);

            var entries = totals.Select(total => StackedExcelModels.Stacked(
                    ExcelValueModels.Decimal(total),
                    DistrictTotalsStyleModels.DarkGreenTotalsCells
                    )).ToList();

            totalRow.AddCells(entries);
            return totalRow;
        }


        public static List<decimal> TableSumRowValue(List<CountyRow> countyRows)
        {
            var columnCount = countyRows.First().rowEntries.Count;
            var columnSums = new List<decimal>();
            for (int column = 0; column < columnCount; column++)
            {
                columnSums.Add(countyRows.Sum(row => row.rowEntries[column]));
            }
            return columnSums;
        }

        public static ExcelRowModel TableSumRowModel(List<CountyRow> countyRows)
        {
            var district = countyRows.First().District;
            var totals = TableSumRowValue(countyRows);

            var totalRow = ExcelRowModels.RightHeader(0, $"District {district:00} Total", 2, 1);
            var totalsCells = totals.Select(total => StackedExcelModels.Stacked(ExcelValueModels.Decimal(total), DistrictTotalsStyleModels.DarkGreenTotalsCells)).ToList();
            totalRow.AddCells(totalsCells);

            return totalRow;
        }


        internal static CountyRow CountyPercentageValues(CountyRow countyRow, List<decimal> stateTotalValues)
        {
            var yearColumnCount = stateTotalValues.Count;
            var percentageRow = new List<decimal>();

            for (var column = 0; column < yearColumnCount; column++)
            {
                var numerator = countyRow.rowEntries[column];
                var denominator = stateTotalValues[column];
                percentageRow.Add(denominator == 0 ? 0 : numerator / denominator);
            }

            return new CountyRow
            {
                County = countyRow.County,
                District = countyRow.District,
                rowEntries = percentageRow,
            };
        }

        public static List<CountyRow> PercentageOverallDollarsTableValues(SimulationOutput simulationOutput, List<int> districtList)
        {
            var allRows = districtList.SelectMany(district => OverallDollarsTableDistrictValues(simulationOutput, district)).ToList();
            var turnpikeRowValues = OverallDollarsTableTurnpike(simulationOutput);
            allRows.Add(turnpikeRowValues);

            var stateTotalValues = TableSumRowValue(allRows);
            var percentageRowsByDistrict = allRows.Select(countyRow => CountyPercentageValues(countyRow, stateTotalValues)).ToList();

            return percentageRowsByDistrict;
        }

        internal static ExcelRowModel DistrictCountyRowToPercentageExcelRowModel(CountyRow countyRow)
        {
            var districtLabel = StackedExcelModels.Stacked(
                ExcelValueModels.Integer(countyRow.District),
                ExcelStyleModels.HorizontalCenter,
                ExcelStyleModels.ThinBorder
                );
            var countyLabel = StackedExcelModels.Stacked(
                ExcelValueModels.String(countyRow.County.ToUpper()),
                ExcelStyleModels.Left,
                ExcelStyleModels.ThinBorder
                );
            var cellModels = new List<IExcelModel>
            {
                districtLabel,
                countyLabel
            };

            cellModels.AddRange(countyRow.rowEntries.Select(rowEntry => StackedExcelModels.Stacked(
                ExcelValueModels.Decimal(rowEntry),
                DistrictTotalsStyleModels.LightBlueFill,
                ExcelStyleModels.HorizontalCenter,
                ExcelStyleModels.ThinBorder,
                ExcelStyleModels.PercentageFormat(2)
                )));
            var excelRowModel = ExcelRowModels.WithEntries(cellModels);

            return excelRowModel;
        }

        internal static List<ExcelRowModel> DistrictCountyRowsToPercentageExcelRowModels(List<CountyRow> countyRows) =>
            countyRows.Select(countyRow => DistrictCountyRowToPercentageExcelRowModel(countyRow)).ToList();

        internal static ExcelRowModel TurnpikeRowToPercentageExcelRowModel(CountyRow turnpikeValueRow)
        {
            var excelRowModel = ExcelRowModels.RightHeader(0, "Turnpike", 2, 1);

            var cellModels = turnpikeValueRow.rowEntries.Select(rowEntry =>
                StackedExcelModels.Stacked(
                ExcelValueModels.Decimal(rowEntry),
                DistrictTotalsStyleModels.DarkBlueFill,
                ExcelStyleModels.HorizontalCenter,
                ExcelStyleModels.MediumBorder,
                ExcelStyleModels.PercentageFormat(0))).ToList();
            excelRowModel.AddCells(cellModels);

            return excelRowModel;
        }

        public static ExcelRowModel TableSumRowPercentageModel(List<CountyRow> countyRows)
        {
            var district = countyRows.First().District;
            var totals = TableSumRowValue(countyRows);

            var totalRow = ExcelRowModels.RightHeader(0, $"District {district:00} Total", 2, 1);

            var totalsCells = totals.Select(total =>
                StackedExcelModels.Stacked(
                ExcelValueModels.Decimal(total),
                DistrictTotalsStyleModels.DarkBlueFill,
                ExcelStyleModels.HorizontalCenter,
                ExcelStyleModels.MediumBorder,
                ExcelStyleModels.PercentageFormat(0))).ToList();
            totalRow.AddCells(totalsCells);

            return totalRow;
        }
    }
}
