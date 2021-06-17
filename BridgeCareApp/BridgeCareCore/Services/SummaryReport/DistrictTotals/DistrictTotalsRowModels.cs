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
            var values = new List<IExcelModel>();
            values.Add(ExcelValueModels.String("District"));
            foreach (var year in output.Years)
            {
                values.Add(ExcelValueModels.Integer(year.Year));
            }
            foreach (var header in additionalHeaders)
            {
                values.Add(ExcelValueModels.String(header));
            }
            return ExcelRowModels.WithEntries(values, ExcelValueModels.Bold, ExcelBorderModels.Thin);
        }

        internal static ExcelRowModel TopTableDistrict(SimulationOutput output, int districtNumber)
        {
            return new ExcelRowModel
            {
                Values = DistrictTotalsExcelModelEnumerables.TopTableDistrictContent(output, districtNumber)
                .Select(x => RelativeExcelRangeModels.OneByOne(x))
                .ToList()
            };
        }


        public static ExcelRowModel FirstYearRow(SimulationOutput output)
        {
            var year = output.Years.FirstOrDefault()?.Year ?? 0;
            return ExcelRowModels.WithEntries(
                ExcelValueModels.Integer(year));
        }

        public static ExcelRowModel TopTableTurnpike(SimulationOutput output)
        {
            var values = DistrictTotalsExcelModelEnumerables.TopTableTurnpikeContent(output)
               .ToList();
            return ExcelRowModels.WithEntries(values);
        }
    }
}
