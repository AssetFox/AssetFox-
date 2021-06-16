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
            => range =>
            {
                var row = range.Start.Row;
                return $@"SUMIFS(INDIRECT(ADDRESS(6,B$1+1,,,""Bridge Data"")) :INDIRECT(ADDRESS($A$1,B$1+1,,,""Bridge Data"")),INDIRECT(ADDRESS(6,Legend!$F$10,,,""Bridge Data"")):INDIRECT(ADDRESS($A$1,Legend!$F$10,,,""Bridge Data"")),$A6,INDIRECT(ADDRESS(6,Legend!$F$12,,,""Bridge Data"")):INDIRECT(ADDRESS($A$1,Legend!$F$12,,,""Bridge Data"")),""<>31 - State Toll Authority"",INDIRECT(ADDRESS({row},B$1-2,,,""Bridge Data"")):INDIRECT(ADDRESS($A$1,B$1-2,,,""Bridge Data"")),""*MPMS*"")";
            };
    }
}
