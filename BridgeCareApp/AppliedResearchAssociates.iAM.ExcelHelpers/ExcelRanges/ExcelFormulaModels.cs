using System;
using OfficeOpenXml;

<<<<<<<< HEAD:BridgeCareApp/AppliedResearchAssociates.iAM.ExcelHelpers/ExcelRanges/ExcelFormulaModels.cs
namespace AppliedResearchAssociates.iAM.ExcelHelpers
========
namespace BridgeCareCore.Helpers.Excel
>>>>>>>> master:BridgeCareApp/BridgeCareCore/Helpers/Excel/ExcelRanges/ExcelFormulaModels.cs
{
    public static class ExcelFormulaModels
    {
        public static ExcelFormulaModel Text(string text)
            => new ExcelFormulaModel
            {
                Formula = ExcelRangeFunctions.Constant(text),
            };
        public static ExcelFormulaModel FromFunction(Func<ExcelRange, string> function)
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
