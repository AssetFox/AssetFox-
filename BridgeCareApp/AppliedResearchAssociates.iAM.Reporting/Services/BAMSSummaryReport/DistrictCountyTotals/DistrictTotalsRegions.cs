using System.Collections.Generic;
using System.Linq;

using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.ExcelHelpers;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.DistrictCountyTotals
{
    public static class DistrictTotalsRegions
    {
        public static List<int> NumberedDistricts
            => new List<int> { 1, 2, 3, 4, 5, 6, 8, 9, 10, 11, 12 }; // Seven is indeed skipped.

        public static RowBasedExcelRegionModel MpmsDistrictTable(SimulationOutput simulationOutput, int districtNumber)
        {
            // Header for District
            var rows = new List<ExcelRowModel>
            {
                ExcelRowModels.CenteredHeader(1, $"District: {districtNumber}", simulationOutput.Years.Count + 2, 1),
                DistrictTotalsRowModels.DistrictCountyAndYearsHeaders(simulationOutput, DistrictTotalsStringConstants.DistrictTotal)
            };

            var counties = new List<string> { "ONE", "TWO", "THREE", "FOUR", "FIVE" };

            foreach (var county in counties)
            {
                rows.Add(DistrictTotalsRowModels.MpmsTableDistrict(simulationOutput, districtNumber));
            }

            return RowBasedExcelRegionModels.WithRows(rows);
        }

        public static RowBasedExcelRegionModel MpmsTable(SimulationOutput simulationOutput)
        {
            // MPMS Projects By County global section header
            var rows = new List<ExcelRowModel>
            {
                ExcelRowModels.CenteredHeader(1, "Dollars Spent on MPMS Projects by County", simulationOutput.Years.Count, 1),
                DistrictTotalsRowModels.DistrictCountyAndYearsHeaders(simulationOutput)
            };

            // District sub-tables
            foreach (var districtNumber in NumberedDistricts)
            {
                MpmsDistrictTable(simulationOutput, districtNumber);
            }

            rows.Add(DistrictTotalsRowModels.MpmsTableTurnpike(simulationOutput));
            rows.Add(DistrictTotalsRowModels.TableBottomSumRow(simulationOutput));

            return RowBasedExcelRegionModels.WithRows(rows);

        }


        public static RowBasedExcelRegionModel BamsTable(SimulationOutput output)
        {
            var rows = new List<ExcelRowModel>
            {
                ExcelRowModels.CenteredHeader(1, "Dollars Spent on BAMS Projects by District", output.Years.Count, 1),
                DistrictTotalsRowModels.DistrictCountyAndYearsHeaders(output, DistrictTotalsStringConstants.DistrictTotal),
            };
            foreach (var districtNumber in NumberedDistricts)
            {
                rows.Add(DistrictTotalsRowModels.BamsTableDistrict(output, districtNumber));
            }
            rows.Add(DistrictTotalsRowModels.BamsTableTurnpike(output));
            rows.Add(DistrictTotalsRowModels.TableBottomSumRow(output));
            return RowBasedExcelRegionModels.WithRows(rows);
        }

        public static RowBasedExcelRegionModel TotalsTable(SimulationOutput output)
        {
            var rows = new List<ExcelRowModel>
            {
                ExcelRowModels.CenteredHeader(1, "Overall Dollars Spent on Projects by District", output.Years.Count, 1),
                DistrictTotalsRowModels.DistrictCountyAndYearsHeaders(output, DistrictTotalsStringConstants.DistrictTotal, "Yearly Average", "% Yearly Average"),
            };
            var additionalRows = new List<ExcelRowModel>();
            foreach (var districtNumber in NumberedDistricts)
            {
                additionalRows.Add(DistrictTotalsRowModels.TotalsTableDistrict(output, districtNumber));
            }
            additionalRows.Add(DistrictTotalsRowModels.TotalsTableTurnpike(output));
            additionalRows.Add(DistrictTotalsRowModels.TableBottomSumRow(output));
            rows.AddRange(additionalRows);
            for (var rowIndex = 0; rowIndex < additionalRows.Count; rowIndex++)
            {
                var yearlyAverage = DistrictTotalsExcelModels.YearlyAverage(output);
                var percentYearlyAverage = DistrictTotalsExcelModels.PercentYearlyAverage(additionalRows.Count - 1 - rowIndex);
                additionalRows[rowIndex].AddCells(yearlyAverage, percentYearlyAverage);
            }
            return RowBasedExcelRegionModels.WithRows(rows);
        }

        internal static RowBasedExcelRegionModel PercentOverallDollarsByDistrictTable(SimulationOutput output)
        {
            var rows = new List<ExcelRowModel>
            {
                ExcelRowModels.CenteredHeader(1, "% of Overall Dollars Spent on Projects by District", output.Years.Count, 1),
                DistrictTotalsRowModels.DistrictCountyAndYearsHeaders(output)
            };
            var titles = new List<IExcelModel>();
            titles.AddRange(NumberedDistricts.Select(x => ExcelValueModels.Integer(x)));
            titles.Add(ExcelValueModels.String("Turnpike"));
            var initialRowDelta = 4;
            for (int i=0; i<titles.Count; i++)
            {
                var newRow = DistrictTotalsRowModels.PercentOverallDollarsContentRow(output, titles, initialRowDelta, i);
                rows.Add(newRow);
            }
            rows.Add(DistrictTotalsRowModels.PercentOverallDollarsTotalsRow(output));
            return RowBasedExcelRegionModels.WithRows(rows);
        }

    }
}
