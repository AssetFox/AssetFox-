using System.Collections.Generic;
using AppliedResearchAssociates.iAM.ExcelHelpers;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.ShortNameGlossary
{
    public static class ShortNameGlossaryModels
    {
        public static List<IExcelWorksheetContentModel> Content
            => new()
            {
                ConditionRangeRegion,
                GlossaryRegion,
                ColorKeyRegion,
                ExcelWorksheetContentModels.ColumnWidth(1, 20.5),
                ExcelWorksheetContentModels.ColumnWidth(2, 20.5),
                ExcelWorksheetContentModels.ColumnWidth(3, 20.5),
            };        

        public static AnchoredExcelRegionModel ConditionRangeRegion
            => new()
            {
                Region = ShortNameGlossaryConditionRangeModels.ConditionRangeContent(),
                StartRow = 1,
                StartColumn = 5
            };


        public static AnchoredExcelRegionModel GlossaryRegion
            => new()
            {
                Region = ShortNameGlossaryTabDefinitionsModels.GlossaryColumn(),
                StartRow = 1,
                StartColumn = 9
            };

        public static AnchoredExcelRegionModel ColorKeyRegion
            => new()
            {
                Region = ShortNameGlossaryColorKeyModels.ColorKeyRows(),
                StartRow = 1,
                StartColumn = 1
            };
    }
}
