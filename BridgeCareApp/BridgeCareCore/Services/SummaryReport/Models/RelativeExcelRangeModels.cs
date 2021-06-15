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
    }
}
