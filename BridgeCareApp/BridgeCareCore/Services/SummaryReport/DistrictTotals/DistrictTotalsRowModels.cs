using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BridgeCareCore.Services.SummaryReport.Models;

namespace BridgeCareCore.Services.SummaryReport.DistrictTotals
{
    public static class DistrictTotalsRowModels
    {
        public static ExcelFormulaModel BridgeCountPlusSix
            => ExcelFormulaModels.Text(@"COUNT('Bridge Data'!C:C)+6");
     
        public static ExcelRowModel TopRow()
            => ExcelRowModels.WithEntries(BridgeCountPlusSix);
        public static ExcelRowModel IndexingRow(int numberOfYears)
        {
            var r = ExcelRowModels.WithEntries(
                BridgeCountPlusSix,
                ExcelIntegerValueModels.WithValue(103)
                );
            for (var i = 2; i < numberOfYears; i++)
            {
                var function = ExcelRangeFunctions.Plus(
                    ExcelRangeFunctions.Left,
                    ExcelRangeFunctions.Constant("17"));
                r.AddCell(ExcelFormulaModels.FromFunction(function));
            }
            return r;
        }
    }
}
