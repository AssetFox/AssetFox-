using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using BridgeCareCore.Services.SummaryReport.Models;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport.DistrictTotals
{
    public static class DistrictTotalsRegions
    {
        public static List<int> NumberedDistricts
            => new List<int> { 1, 2, 3, 4, 5, 6, 8, 9, 10, 11, 12 }; // Seven is indeed skipped.

        public static RowBasedExcelRegionModel MpmsTable(SimulationOutput output)
        {
            var rows = new List<ExcelRowModel>
            {
                ExcelRowModels.IndentedHeader(1, "Dollars Spent on MPMS Projects by District", output.Years.Count, 1),
                DistrictTotalsRowModels.DistrictAndYearsHeaders(output, DistrictTotalsStringConstants.DistrictTotal)
            };
            foreach (var districtNumber in NumberedDistricts)
            {
                rows.Add(DistrictTotalsRowModels.MpmsTableDistrict(output, districtNumber));
            }
            rows.Add(DistrictTotalsRowModels.MpmsTableTurnpike(output));
            rows.Add(DistrictTotalsRowModels.TableBottomSumRow(output));
            return RowBasedExcelRegionModels.WithRows(rows);
        }


        public static RowBasedExcelRegionModel BamsTable(SimulationOutput output)
        {
            var rows = new List<ExcelRowModel>
            {
                ExcelRowModels.IndentedHeader(1, "Dollars Spent on BAMS Projects by District", output.Years.Count, 1),
                DistrictTotalsRowModels.DistrictAndYearsHeaders(output, DistrictTotalsStringConstants.DistrictTotal),
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
                ExcelRowModels.IndentedHeader(1, "Overall Dollars Spent on Projects by District", output.Years.Count, 1),
                DistrictTotalsRowModels.DistrictAndYearsHeaders(output, DistrictTotalsStringConstants.DistrictTotal, "Yearly Average", "% Yearly Average"),
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
                ExcelRowModels.IndentedHeader(1, "% of Overall Dollars Spent on Projects by District", output.Years.Count, 1),
                DistrictTotalsRowModels.DistrictAndYearsHeaders(output)
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
