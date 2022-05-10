using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BridgeCareCore.Helpers.Excel
{
    /// <summary>For modelling situations where we know the content
    /// and size of our range but not its location.</summary>
    public class RelativeExcelRangeModel
    {
        public IExcelModel Content { get; set; }
        public ExcelRangeSize Size { get; set; } = new ExcelRangeSize();
    }
}
