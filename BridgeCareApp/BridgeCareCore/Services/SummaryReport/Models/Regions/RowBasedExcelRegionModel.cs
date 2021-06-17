using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BridgeCareCore.Services.SummaryReport.Visitors;

namespace BridgeCareCore.Services.SummaryReport.Models
{
    public class RowBasedExcelRegionModel
    {
        public List<ExcelRowModel> Rows { get; set; }
    }
}
