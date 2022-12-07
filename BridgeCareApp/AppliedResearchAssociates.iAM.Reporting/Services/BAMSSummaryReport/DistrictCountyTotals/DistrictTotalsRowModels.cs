using System;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml;

using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.DistrictCountyTotals
{
    public static class DistrictTotalsRowModels
    {
        public static ExcelFormulaModel BridgeCountPlusSix
            => ExcelFormulaModels.Text(@"COUNT('Bridge Data'!C:C)+6");

        public static ExcelRowModel IndexingRow(int numberOfYears)
        {
            var returnValue = ExcelRowModels.WithEntries(
                BridgeCountPlusSix,
                ExcelValueModels.Integer(103)
                );
            for (var i = 1; i < numberOfYears; i++)
            {
                var function = ExcelRangeFunctions.Plus(
                    ExcelRangeFunctions.Left,
                    ExcelRangeFunctions.Constant("17"));
                returnValue.AddCells(ExcelFormulaModels.FromFunction(function));
            }
            return returnValue;
        }

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

        internal static List<ExcelRowModel> MpmsTableDistrict(SimulationOutput output, int districtNumber)
        {

            var district = ExcelValueModels.Integer(districtNumber);
            var counties = CountiesForDistrict(districtNumber);

            var rowModels = new List<ExcelRowModel>();
            foreach (var county in counties)
            {
                var values = new List<IExcelModel>();
                var countyLabel = ExcelValueModels.String(county.ToUpper());
                Func<AssetDetail, bool> predicate = detail => DistrictTotalsSectionDetailPredicates.IsNumberedDistrictMpmsTable(detail, districtNumber, county);
                values.AddRange(DistrictTotalsExcelModelEnumerables.TableContent(output, district, countyLabel, predicate)
                    .ToList());
                var excelRowModel = ExcelRowModels.WithEntries(values);
                rowModels.Add(excelRowModel);
            }

            return rowModels;
        }


        internal static List<ExcelRowModel> BamsTableDistrict(SimulationOutput output, int districtNumber)
        {
            var district = ExcelValueModels.Integer(districtNumber);
            var counties = CountiesForDistrict(districtNumber);

            var rowModels = new List<ExcelRowModel>();
            foreach (var county in counties)
            {
                var values = new List<IExcelModel>();
                var countyLabel = ExcelValueModels.String(county.ToUpper());
                Func<AssetDetail, bool> predicate = detail => DistrictTotalsSectionDetailPredicates.IsNumberedDistrictBamsTable(detail, districtNumber, county);
                values.AddRange(DistrictTotalsExcelModelEnumerables.TableContent(output, district, countyLabel, predicate)
                    .ToList());
                var excelRowModel = ExcelRowModels.WithEntries(values);
                rowModels.Add(excelRowModel);
            }

            return rowModels;
        }

        internal static List<ExcelRowModel> TotalsTableDistrict(SimulationOutput output, int districtNumber)
        {
            var district = ExcelValueModels.Integer(districtNumber);
            var counties = CountiesForDistrict(districtNumber);

            var rowModels = new List<ExcelRowModel>();
            foreach (var county in counties)
            {
                var values = new List<IExcelModel>();
                var countyLabel = ExcelValueModels.String(county.ToUpper());
                Func<AssetDetail, bool> predicate = section => DistrictTotalsSectionDetailPredicates.IsDistrictNotTurnpike(section, districtNumber) && DistrictTotalsSectionDetailPredicates.IsCounty(section, county);
                values.AddRange(DistrictTotalsExcelModelEnumerables.TableContent(output, district, countyLabel, predicate)
                    .ToList());
                var excelRowModel = ExcelRowModels.WithEntries(values);
                rowModels.Add(excelRowModel);
            }

            return rowModels;
        }

        internal static ExcelRowModel TotalsTableTurnpike(SimulationOutput output)
        {
            var totalRow = ExcelRowModels.RightHeader(0, "Turnpike", 2, 1);

            Func<AssetDetail, bool> predicate = DistrictTotalsSectionDetailPredicates.IsTurnpike;
            var values = DistrictTotalsExcelModelEnumerables.TableContentTotalsOrTurnpike(output, predicate)
                .ToList();

            totalRow.AddCells(values.ToArray());
            return totalRow;
        }

        public static ExcelRowModel FirstYearRow(SimulationOutput output)
        {
            var year = output.Years.FirstOrDefault()?.Year ?? 0;
            return ExcelRowModels.WithEntries(
                ExcelValueModels.Integer(year));
        }

        public static ExcelRowModel MpmsTableTurnpike(SimulationOutput output)
        {
            var totalRow = ExcelRowModels.RightHeader(0, "Turnpike", 2, 1);

            var values = DistrictTotalsExcelModelEnumerables.TableContentTotalsOrTurnpike(output, 
                DistrictTotalsSectionDetailPredicates.IsCommittedTurnpike)
               .ToList();

            totalRow.AddCells(values.ToArray());
            return totalRow;
        }


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

            totalRow.AddCells(entries.ToArray());

            return totalRow;
        }


        public static ExcelRowModel BamsTableTurnpike(SimulationOutput output)
        {
            var totalRow = ExcelRowModels.RightHeader(0, "Turnpike", 2, 1);

            var values = DistrictTotalsExcelModelEnumerables.TableContentTotalsOrTurnpike(output,
                DistrictTotalsSectionDetailPredicates.IsTurnpikeButNotCommitted)
               .ToList();

            totalRow.AddCells(values.ToArray());
            return totalRow;
        }

        public static ExcelRowModel PercentOverallDollarsTurnpike(SimulationOutput output, int stateTotalsRowOffset)
        {
            int numeratorOffset = 2; // Table Header + Years Header
            for (var i = 1; i <= 12; i++)
            {
                numeratorOffset += CountiesForDistrict(i).Count + 2; // District Header Row + County Rows + District Total Row
            }

            var turnpikeRow = ExcelRowModels.RightHeader(0, "Turnpike", 2, 1);

            var turnpikeDenominatorAddress = ExcelRangeFunctions.StartOffset(0, -stateTotalsRowOffset); // address of State Total row
            var turnpikeNumeratorAddress = ExcelRangeFunctions.StartOffset(0, -numeratorOffset, false, true);

            Func<ExcelRange, string> turnpikeQuotient = range =>
            {
                var turnpikeNumerator = turnpikeNumeratorAddress(range);
                var turnpikeDenominator = turnpikeDenominatorAddress(range);
                return $"IFERROR({turnpikeNumerator}/{turnpikeDenominator}, 0)";
            };
            var newTotalCell = StackedExcelModels.Stacked(
                ExcelFormulaModels.FromFunction(turnpikeQuotient),
                DistrictTotalsStyleModels.DarkBlueFill,
                ExcelStyleModels.HorizontalCenter,
                ExcelStyleModels.MediumBorder,
                ExcelStyleModels.PercentageFormat(0));
            turnpikeRow.AddRepeated(output.Years.Count, newTotalCell);

            return turnpikeRow;
        }


        public static ExcelRowModel TableBottomSumRow(SimulationOutput output, int districtNumber, int numberOfCounties)
        {
            var totalRow = ExcelRowModels.RightHeader(0, $"District {districtNumber:00} Total", 2, 1);

            var sumFormula = ExcelFormulaModels.StartOffsetRangeSum(0, -numberOfCounties, 0, -1);
            var styledFormula = StackedExcelModels.Stacked(sumFormula, DistrictTotalsStyleModels.DarkGreenTotalsCells);
            var entries = new List<IExcelModel>();

            for (int i = 0; i < output.Years.Count; i++)
            {
                entries.Add(styledFormula);
            }

            totalRow.AddCells(entries.ToArray());

            return totalRow;
        }


        public static List<ExcelRowModel> PercentOverallDollarsDistrictSubtable(SimulationOutput output, int districtNumber, int stateTotalsRowOffset)
        {
            var district = ExcelValueModels.Integer(districtNumber);
            var counties = CountiesForDistrict(districtNumber);

            int numeratorOffset = 2; // Table Header + Years Header
            for (var i = 1; i <= 12; i++)
            {
                numeratorOffset += CountiesForDistrict(i).Count + 2; // District Header Row + County Rows + District Total Row
            }

            var rowModels = new List<ExcelRowModel>();

            foreach (var county in counties)
            {
                var values = new List<IExcelModel>();
                var countyLabel = ExcelValueModels.String(county.ToUpper());

                values.Add(StackedExcelModels.Stacked(
                    district,
                    ExcelStyleModels.HorizontalCenter,
                    ExcelStyleModels.ThinBorder
                    ));
                values.Add(StackedExcelModels.Stacked(
                    countyLabel,
                    ExcelStyleModels.Left,
                    ExcelStyleModels.ThinBorder
                    ));
                var excelRowModel = ExcelRowModels.WithEntries(values);

                var denominatorAddress = ExcelRangeFunctions.StartOffset(0, -stateTotalsRowOffset); // address of State Total row
                var numeratorAddress = ExcelRangeFunctions.StartOffset(0, -numeratorOffset, false, true);

                Func<ExcelRange, string> quotient = range =>
                {
                    var numerator = numeratorAddress(range);
                    var denominator = denominatorAddress(range);
                    return $"IFERROR({numerator}/{denominator}, 0)";
                };
                var newCell = StackedExcelModels.Stacked(
                    ExcelFormulaModels.FromFunction(quotient),
                    DistrictTotalsStyleModels.LightBlueFill,
                    ExcelStyleModels.HorizontalCenter,
                    ExcelStyleModels.ThinBorder,
                    ExcelStyleModels.PercentageFormat(2));
                excelRowModel.AddRepeated(output.Years.Count, newCell);

                rowModels.Add(excelRowModel);
                stateTotalsRowOffset++;
            }

            var totalRow = ExcelRowModels.RightHeader(0, $"District {districtNumber:00} Total", 2, 1);

            var totalDenominatorAddress = ExcelRangeFunctions.StartOffset(0, -stateTotalsRowOffset); // address of State Total row
            var totalNumeratorAddress = ExcelRangeFunctions.StartOffset(0, -numeratorOffset, false, true);

            Func<ExcelRange, string> totalQuotient = range =>
            {
                var totalNumerator = totalNumeratorAddress(range);
                var totalDenominator = totalDenominatorAddress(range);
                return $"IFERROR({totalNumerator}/{totalDenominator}, 0)";
            };
            var newTotalCell = StackedExcelModels.Stacked(
                ExcelFormulaModels.FromFunction(totalQuotient),
                DistrictTotalsStyleModels.DarkBlueFill,
                ExcelStyleModels.HorizontalCenter,
                ExcelStyleModels.MediumBorder,
                ExcelStyleModels.PercentageFormat(0));
            totalRow.AddRepeated(output.Years.Count, newTotalCell);

            rowModels.Add(totalRow);

            return rowModels;
        }

        public static ExcelRowModel PercentOverallDollarsTotalsRow(SimulationOutput output)
        {
            var totalText = StackedExcelModels.BoldText("Total");
            var returnValue = ExcelRowModels.WithEntries(totalText);
            var sumFunction = ExcelRangeFunctions.StartOffsetRangeSum(0, -12, 0, -1);
            var sumEntry = StackedExcelModels.Stacked(
                ExcelFormulaModels.FromFunction(sumFunction),
                DistrictTotalsStyleModels.DarkBlueFill,
                ExcelStyleModels.HorizontalCenter,
                ExcelStyleModels.MediumBorder,
                ExcelStyleModels.PercentageFormat(0));
            returnValue.AddRepeated(output.Years.Count, sumEntry);
            return returnValue;
        }

    }
}
