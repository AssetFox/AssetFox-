using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BridgeCareCore.Services.SummaryReport.Models;

namespace BridgeCareCore.Services.SummaryReport.DistrictTotals
{
    public static class DistrectTotalsModels
    {
        public static ExcelTextFormulaModel BridgeCountPlusSix
            => ExcelFormulaModels.Text(@"COUNT('Bridge Data'!C:C)+6");
        public static ExcelRowModel IndexingRow(int numberOfYears)
        {
            var r = ExcelRowModels.WithEntries(
                BridgeCountPlusSix,
                ExcelIntegerValueModels.WithValue(103)
                );
            for (int i = 2; i< numberOfYears; i++)
            {
                var newFormula = ExcelFormulaModels.Plus(
                    ExcelFormulaModels.LeftNeighbor,
                    ExcelFormulaModels.Text("17"));
                r.Values.Add(newFormula);
            }
            return r;
        }
    }
}
