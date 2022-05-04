using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BridgeCareCore.Helpers.Excel.Visitors;

namespace BridgeCareCore.Helpers.Excel
{
    public class RowBasedExcelRegionModel
    {
        public List<ExcelRowModel> Rows { get; set; }
    }
}
