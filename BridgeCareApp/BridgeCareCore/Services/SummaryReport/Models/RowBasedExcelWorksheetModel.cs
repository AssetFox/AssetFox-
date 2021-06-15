using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BridgeCareCore.Services.SummaryReport.Models
{
    public class RowBasedExcelWorksheetModel
    {
        public string TabName { get; set; }
        public List<ExcelRowModel> Rows { get; set; }
    }
}
