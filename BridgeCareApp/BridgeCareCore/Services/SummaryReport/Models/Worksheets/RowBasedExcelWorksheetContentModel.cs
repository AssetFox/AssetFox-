using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BridgeCareCore.Services.SummaryReport.Visitors;

namespace BridgeCareCore.Services.SummaryReport.Models
{
    public class RowBasedExcelWorksheetContentModel: IExcelWorksheetContentModel
    {
        public List<ExcelRowModel> Rows { get; set; }

        public TOutput Accept<TOutput, THelper>(IExcelWorksheetModelVisitor<THelper, TOutput> visitor, THelper helper)
            => visitor.Visit(this, helper);
    }
}
