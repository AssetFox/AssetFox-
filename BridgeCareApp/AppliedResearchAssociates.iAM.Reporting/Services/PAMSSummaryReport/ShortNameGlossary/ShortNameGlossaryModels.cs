using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.ExcelHelpers;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.ShortNameGlossary
{
    public static class ShortNameGlossaryModels
    {
        public static List<IExcelWorksheetContentModel> Content
            => new List<IExcelWorksheetContentModel>
            {
                WorkTypesRegion,
                ExcelWorksheetContentModels.AutoFitColumns(70),
                ColorKeyRegion,
            };

        public static AnchoredExcelRegionModel WorkTypesRegion
            => new AnchoredExcelRegionModel
            {
                Region = ShortNameGlossaryWorkTypeModels.WorkTypesRows(),
                StartColumn = 1,
                StartRow = 1,
            };

        public static AnchoredExcelRegionModel ColorKeyRegion
            => new AnchoredExcelRegionModel
            {
                Region = ShortNameGlossaryColorKeyModels.ColorKeyRows(),
                StartRow = 82,
                StartColumn = 1
            };

    }
}
