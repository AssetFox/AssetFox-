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
                ExcelIntegerValueModels.WithValue(103)
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
            var values = new List<IExcelModel>();
            values.Add(ExcelTextModels.Text("District"));
            foreach (var year in output.Years)
            {
                values.Add(ExcelIntegerValueModels.WithValue(year.Year));
            }
            foreach (var header in additionalHeaders)
            {
                values.Add(ExcelTextModels.Text(header));
            }
            return ExcelRowModels.WithEntries(values, ExcelBoldModels.Bold);
        }

        public static ExcelRowModel FirstYearRow(SimulationOutput output)
        {
            var year = output.Years.FirstOrDefault()?.Year ?? 0;
            return ExcelRowModels.WithEntries(ExcelIntegerValueModels.WithValue(year));
        }
    }
}
