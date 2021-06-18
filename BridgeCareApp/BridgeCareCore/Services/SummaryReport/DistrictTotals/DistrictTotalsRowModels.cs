using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using BridgeCareCore.Services.SummaryReport.Models;

namespace BridgeCareCore.Services.SummaryReport.DistrictTotals
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
                r.AddCell(ExcelFormulaModels.FromFunction(function));
            }
            return r;
        }

        internal static ExcelRowModel DistrictAndYearsHeaders(SimulationOutput output, params string[] additionalHeaders)
        {
            var values = new List<IExcelModel>
            {
                StackedExcelModels.Stacked(
                    ExcelValueModels.String("District"),
                    ExcelStyleModels.ThinBorder
                    ),
            };
            foreach (var year in output.Years)
            {
                values.Add(
                    StackedExcelModels.Stacked(
                        ExcelValueModels.Integer(year.Year),
                        ExcelStyleModels.HorizontalCenter,
                        ExcelStyleModels.ThinBorder));
            }
            foreach (var header in additionalHeaders)
            {
                values.Add(ExcelValueModels.String(header));
            }
            return ExcelRowModels.WithEntries(values, ExcelStyleModels.Bold, ExcelStyleModels.ThinBorder);
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
    }
}
