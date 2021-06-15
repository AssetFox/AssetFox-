using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BridgeCareCore.Services.SummaryReport.Models
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
                Content = ExcelNothingModels.Nothing,
                Size = new ExcelRangeSize(width, height),
            };

        public static RelativeExcelRangeModel Text(string text, int width = 1, int height = 1)
            => new RelativeExcelRangeModel
            {
                Content = ExcelTextModels.Text(text),
                Size = new ExcelRangeSize(width, height),
            };

        public static RelativeExcelRangeModel BoldText(string text, int width = 1, int height = 1)
            => new RelativeExcelRangeModel
            {
                Content = StackedExcelModels.BoldText(text),
                Size = new ExcelRangeSize(width, height),
            };
    }
}
