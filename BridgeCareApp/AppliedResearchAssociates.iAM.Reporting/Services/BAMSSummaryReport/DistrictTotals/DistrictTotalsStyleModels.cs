﻿using AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport.ExcelRanges;
using AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport.ExcelStyles;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.DistrictTotals
{
    public class DistrictTotalsStyleModels
    {
        public static StackedExcelModel DarkGreenFill
            => StackedExcelModels.Stacked(
                ExcelStyleModels.BackgroundColor(DistrictTotalsColors.DarkGreen),
                ExcelStyleModels.WhiteText,
                ExcelStyleModels.CurrencyWithoutCentsFormat
                );

        public static ExcelFillModel LightGreenFill
            => ExcelStyleModels.BackgroundColor(DistrictTotalsColors.LightGreen);

        public static ExcelFillModel LightOrangeFill
            => ExcelStyleModels.BackgroundColor(DistrictTotalsColors.LightOrange);

        public static ExcelFillModel LightBlueFill
            => ExcelStyleModels.BackgroundColor(DistrictTotalsColors.LightBlue);

        public static ExcelFillModel DarkBlueFill
            => ExcelStyleModels.BackgroundColor(DistrictTotalsColors.DarkBlue);

        public static StackedExcelModel DarkGreenTotalsCells
            => StackedExcelModels.Stacked(
                ExcelStyleModels.Right,
                ExcelStyleModels.MediumBorder,
                ExcelStyleModels.CurrencyWithoutCentsFormat,
                DarkGreenFill);
    }
}
