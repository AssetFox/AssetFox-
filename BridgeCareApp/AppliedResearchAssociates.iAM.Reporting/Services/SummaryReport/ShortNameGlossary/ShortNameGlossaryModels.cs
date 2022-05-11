using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using BridgeCareCore.Services.SummaryReport.BridgeData;
using BridgeCareCore.Services.SummaryReport.Models;
using BridgeCareCore.Services.SummaryReport.Models.Worksheets;

namespace BridgeCareCore.Services.SummaryReport.ShortNameGlossary
{
    public static class ShortNameGlossaryModels
    {
        public static List<IExcelWorksheetContentModel> Content
            => new List<IExcelWorksheetContentModel>
            {
                TreatmentsRegion,
                ExcelWorksheetContentModels.AutoFitColumns(70),
                ConditionRangeRegion,
                GlossaryRegion,
                ColorKeyRegion,
            };

        public static AnchoredExcelRegionModel TreatmentsRegion
            => new AnchoredExcelRegionModel
            {
                Region = ShortNameGlossaryTreatmentModels.TreatmentsRows(),
                StartColumn = 1,
                StartRow = 1,
            };

        public static AnchoredExcelRegionModel ConditionRangeRegion
            => new AnchoredExcelRegionModel
            {
                Region = ShortNameGlossaryConditionRangeModels.ConditionRangeContent(),
                StartRow = 1,
                StartColumn = 4
            };


        public static AnchoredExcelRegionModel GlossaryRegion
            => new AnchoredExcelRegionModel
            {
                Region = ShortNameGlossaryTabDefinitionsModels.GlossaryColumn(),
                StartRow = 1,
                StartColumn = 8,
            };

        public static AnchoredExcelRegionModel ColorKeyRegion
            => new AnchoredExcelRegionModel
            {
                Region = ShortNameGlossaryColorKeyModels.ColorKeyRows(),
                StartRow = 39,
                StartColumn = 1
            };

    }
}
