using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport;

<<<<<<<< HEAD:BridgeCareApp/AppliedResearchAssociates.iAM.Reporting/Models/BAMSSummaryReport/ExcelRanges/RelativeExcelRangeModels.cs
namespace AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport.ExcelRanges
========
namespace BridgeCareCore.Helpers.Excel
>>>>>>>> faa0cabf (Moved a whole lot of files to a more-sensible namespace):BridgeCareApp/BridgeCareCore/Helpers/Excel/ExcelRanges/RelativeExcelRangeModels.cs
{
    public static class RelativeExcelRangeModels
    {
        public static RelativeExcelRangeModel OneByOne(IExcelModel content)
            => new RelativeExcelRangeModel
            {
                Content = content,
                Size = new ExcelRangeSize(1, 1),
            };

        public static RelativeExcelRangeModel Empty(int width = 1, int height = 1)
            => new RelativeExcelRangeModel
            {
                Content = ExcelValueModels.Nothing,
                Size = new ExcelRangeSize(width, height),
            };

        public static RelativeExcelRangeModel Text(string text, int width = 1, int height = 1)
            => new RelativeExcelRangeModel
            {
                Content = ExcelValueModels.String(text),
                Size = new ExcelRangeSize(width, height),
            };

        public static RelativeExcelRangeModel BoldText(string text, int width = 1, int height = 1)
            => new RelativeExcelRangeModel
            {
                Content = (IExcelModel)StackedExcelModels.BoldText(text),
                Size = new ExcelRangeSize(width, height),
            };
    }
}
