using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BridgeCareCore.Services.SummaryReport.Models;

namespace BridgeCareCore.Services.SummaryReport.DistrictTotals
{
    public class DistrictTotalsStyleModels
    {
        public static StackedExcelModel DarkGreenFill
            => StackedExcelModels.Stacked(
                ExcelStyleModels.BackgroundColor(DistrictTotalsColors.DarkGreen),
                ExcelStyleModels.WhiteText
                );

        public static ExcelFillModel LightGreenFill
            => ExcelStyleModels.BackgroundColor(DistrictTotalsColors.LightGreen);

        public static StackedExcelModel DarkGreenTotalsCells
            => StackedExcelModels.Stacked(
                ExcelStyleModels.Right,
                ExcelStyleModels.MediumBorder,
                DistrictTotalsStyleModels.DarkGreenFill);
    }
}
