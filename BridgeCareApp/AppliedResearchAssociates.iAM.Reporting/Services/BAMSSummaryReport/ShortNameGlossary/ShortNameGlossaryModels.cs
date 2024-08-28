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
            };        

        public static AnchoredExcelRegionModel ConditionRangeRegion
            => new()
            {
                Region = ShortNameGlossaryConditionRangeModels.ConditionRangeContent(),
                StartRow = 1,
                StartColumn = 1
            };


        public static AnchoredExcelRegionModel GlossaryRegion
            => new()
            {
                Region = ShortNameGlossaryTabDefinitionsModels.GlossaryColumn(),
                StartRow = 1,
                StartColumn = 5
            };

        public static AnchoredExcelRegionModel ColorKeyRegion
            => new()
            {
                Region = ShortNameGlossaryColorKeyModels.ColorKeyRows(),
                StartRow = 21,
                StartColumn = 1
            };

    }
}
