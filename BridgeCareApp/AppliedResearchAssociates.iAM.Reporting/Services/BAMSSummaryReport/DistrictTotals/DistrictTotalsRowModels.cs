using System;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml;

using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.ExcelHelpers;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.DistrictTotals
{
    public static class DistrictTotalsRowModels
    {
        public static ExcelFormulaModel BridgeCountPlusSix
            => ExcelFormulaModels.Text(@"COUNT('Bridge Data'!C:C)+6");

        public static ExcelRowModel IndexingRow(int numberOfYears)
        {
            var r = ExcelRowModels.WithEntries(
                BridgeCountPlusSix,
                ExcelValueModels.Integer(103)
                );
            for (var i = 1; i < numberOfYears; i++)
            {
                var function = ExcelRangeFunctions.Plus(
                    ExcelRangeFunctions.Left,
                    ExcelRangeFunctions.Constant("17"));
                r.AddCells(ExcelFormulaModels.FromFunction(function));
            }
            return r;
        }

        internal static ExcelRowModel DistrictAndYearsHeaders(SimulationOutput output, params string[] additionalHeaders)
        {
            var values = new List<IExcelModel>
            {
                StackedExcelModels.LeftHeader("District"),
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

        internal static ExcelRowModel MpmsTableDistrict(SimulationOutput output, int districtNumber)
        {
            var title = ExcelValueModels.Integer(districtNumber);
            Func<SectionDetail, bool> predicate = detail => DistrictTotalsSectionDetailPredicates.IsNumberedDistrictMpmsTable(detail, districtNumber);
            var values = DistrictTotalsExcelModelEnumerables.TableContent(output, title, predicate)
                .ToList();
            return ExcelRowModels.WithEntries(values);
        }


        internal static ExcelRowModel BamsTableDistrict(SimulationOutput output, int districtNumber)
        {
            var title = ExcelValueModels.Integer(districtNumber);
            Func<SectionDetail, bool> predicate = detail => DistrictTotalsSectionDetailPredicates.IsNumberedDistrictBamsTable(detail, districtNumber);
            var values = DistrictTotalsExcelModelEnumerables.TableContent(output, title, predicate)
                         .ToList();
            return ExcelRowModels.WithEntries(values);
        }

        internal static ExcelRowModel TotalsTableDistrict(SimulationOutput output, int districtNumber)
        {
            var title = ExcelValueModels.Integer(districtNumber);
            Func<SectionDetail, bool> predicate = section => DistrictTotalsSectionDetailPredicates.IsDistrictNotTurnpike(section, districtNumber);
            var values = DistrictTotalsExcelModelEnumerables.TableContent(output, title, predicate)
                         .ToList();
            return ExcelRowModels.WithEntries(values);
        }

        internal static ExcelRowModel TotalsTableTurnpike(SimulationOutput output)
        {
            var title = ExcelValueModels.String("Turnpike");
            Func<SectionDetail, bool> predicate = DistrictTotalsSectionDetailPredicates.IsTurnpike;
            var values = DistrictTotalsExcelModelEnumerables.TableContent(output, title, predicate)
                .ToList();
            return ExcelRowModels.WithEntries(values);
        }

        public static ExcelRowModel FirstYearRow(SimulationOutput output)
        {
            var year = output.Years.FirstOrDefault()?.Year ?? 0;
            return ExcelRowModels.WithEntries(
                ExcelValueModels.Integer(year));
        }

        public static ExcelRowModel MpmsTableTurnpike(SimulationOutput output)
        {
            var title = ExcelValueModels.String("Turnpike");
            var values = DistrictTotalsExcelModelEnumerables.TableContent(output, title,
                DistrictTotalsSectionDetailPredicates.IsCommittedTurnpike)
               .ToList();
            return ExcelRowModels.WithEntries(values);
        }

        public static ExcelRowModel BamsTableTurnpike(SimulationOutput output)
        {
            var title = ExcelValueModels.String("Turnpike");
            var values = DistrictTotalsExcelModelEnumerables.TableContent(output, title,
                DistrictTotalsSectionDetailPredicates.IsTurnpikeButNotCommitted)
               .ToList();
            return ExcelRowModels.WithEntries(values);
        }

        public static ExcelRowModel TableBottomSumRow(SimulationOutput output)
        {
            var totalText = StackedExcelModels.BoldText("Total");
            var sumFormula = ExcelFormulaModels.StartOffsetRangeSum(0, -12, 0, -1);
            var styledFormula = StackedExcelModels.Stacked(sumFormula, DistrictTotalsStyleModels.DarkGreenFill);
            var entries = new List<IExcelModel> { totalText };
            for (int i = 0; i < output.Years.Count; i++)
            {
                entries.Add(styledFormula);
            }
            entries.Add(
                StackedExcelModels.Stacked(
                    ExcelFormulaModels.StartOffsetRangeSum(-output.Years.Count, 0, -1, 0),
                    ExcelStyleModels.MediumBorder,
                    ExcelStyleModels.Right,
                    DistrictTotalsStyleModels.DarkGreenFill
                ));
            return ExcelRowModels.WithEntries(entries);
        }
        public static ExcelRowModel PercentOverallDollarsContentRow(SimulationOutput output, List<IExcelModel> titles, int initialRowDelta, int i)
        {
            var newRow = ExcelRowModels.WithEntries(titles[i]);
            var numeratorAddress = ExcelRangeFunctions.StartOffset(0, -initialRowDelta - i, false, true);
            var denominatorAddress = ExcelRangeFunctions.StartOffset(0, -initialRowDelta - titles.Count);
            Func<ExcelRange, string> quotient = range =>
            {
                var numerator = numeratorAddress(range);
                var denominator = denominatorAddress(range);
                return $"IFERROR({numerator}/{denominator}, 0)";
            };
            var newCell = StackedExcelModels.Stacked(
                ExcelFormulaModels.FromFunction(quotient),
                DistrictTotalsStyleModels.LightBlueFill,
                ExcelStyleModels.Right,
                ExcelStyleModels.ThinBorder,
                ExcelStyleModels.PercentageFormat(0));
            newRow.AddRepeated(output.Years.Count, newCell);
            return newRow;
        }

        public static ExcelRowModel PercentOverallDollarsTotalsRow(SimulationOutput output)
        {
            var totalText = StackedExcelModels.BoldText("Total");
            var r = ExcelRowModels.WithEntries(totalText);
            var sumFunction = ExcelRangeFunctions.StartOffsetRangeSum(0, -12, 0, -1);
            var sumEntry = StackedExcelModels.Stacked(
                ExcelFormulaModels.FromFunction(sumFunction),
                DistrictTotalsStyleModels.DarkBlueFill,
                ExcelStyleModels.WhiteText,
                ExcelStyleModels.Right,
                ExcelStyleModels.MediumBorder,
                ExcelStyleModels.PercentageFormat(0));
            r.AddRepeated(output.Years.Count, sumEntry);
            return r;
        }

    }
}
