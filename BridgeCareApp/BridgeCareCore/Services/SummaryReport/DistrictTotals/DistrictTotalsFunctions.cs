using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BridgeCareCore.Services.SummaryReport.Models;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport.DistrictTotals
{
    public class DistrictTotalsFunctions
    {
        public static IEnumerable<Func<ExcelRange, string>> DistrictTotalsTableContentArguments()
        {
            yield return Row6ColumnLegendF12; 
        }
        private static Func<ExcelRange, string> Row6ColumnLegendF12
            => ExcelRangeFunctions.Indirect(
                ExcelRangeFunctions.Address(
                    ExcelRangeFunctions.Constant("6"),
                    ExcelRangeFunctions.Constant($"{SummaryReportTabNames.Legend}!$F$12"),
                    null,
                    null,
                    ExcelRangeFunctions.Constant(SummaryReportTabNames.BridgeData)
                    ));
        public static Func<ExcelRange, string> DistrictTotalsTableContent()
        {
            var arguments = DistrictTotalsTableContentArguments();
            var r = ExcelRangeFunctions.SumIfs(arguments);
            return r;
        }
    }
}
