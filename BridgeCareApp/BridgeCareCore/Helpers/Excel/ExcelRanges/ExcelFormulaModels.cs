using System;
using OfficeOpenXml;

<<<<<<<< HEAD:BridgeCareApp/AppliedResearchAssociates.iAM.Reporting/Models/BAMSSummaryReport/ExcelRanges/ExcelFormulaModels.cs
namespace AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport.ExcelRanges
========
namespace BridgeCareCore.Helpers.Excel
>>>>>>>> faa0cabf (Moved a whole lot of files to a more-sensible namespace):BridgeCareApp/BridgeCareCore/Helpers/Excel/ExcelRanges/ExcelFormulaModels.cs
{
    public static class ExcelFormulaModels
    {
        public static ExcelFormulaModel Text(string text)
            => new ExcelFormulaModel
            {
                Formula = ExcelRangeFunctions.Constant(text),
            };
        internal static ExcelFormulaModel FromFunction(Func<ExcelRange, string> function)
            => new ExcelFormulaModel
            {
                Formula = function,
            };

        public static ExcelFormulaModel StartOffsetRangeSum(int dxStart, int dyStart, int dxEnd, int dyEnd)
        {
            var sumRange = ExcelRangeFunctions.StartOffsetRangeSum(dxStart, dyStart, dxEnd, dyEnd);
            return FromFunction(sumRange);
        }
    }
}
