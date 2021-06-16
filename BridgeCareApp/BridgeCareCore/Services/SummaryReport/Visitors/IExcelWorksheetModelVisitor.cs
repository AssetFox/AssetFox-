using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BridgeCareCore.Services.SummaryReport.Models;

namespace BridgeCareCore.Services.SummaryReport.Visitors
{
    public interface IExcelWorksheetModelVisitor<THelper, TOutput>
    {
        TOutput Visit(RowBasedExcelWorksheetContentModel model, THelper helper);
        TOutput Visit(AutoFitColumnsExcelWorksheetContentModel model, THelper helper);

    }
}
