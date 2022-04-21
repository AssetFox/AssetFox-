using System.Collections.Generic;
using System.Linq;
using BridgeCareCore.Services.SummaryReport.Models.Tables;

namespace BridgeCareCore.Services.SummaryReport.Models
{
    public static class ExcelTableRowModels
    {
        public static ExcelRowModel ContentRow<T>(
            IEnumerable<ExcelHeaderWithContentModel<T>> headerWithContentModels,
            T data)
        {
            var entries = headerWithContentModels.Select(m => m.Content(data)).ToList();
            var r = ExcelRowModels.WithEntries(entries);
            return r;
        }

        public static ExcelRowModel HeaderRow<T>(IEnumerable<ExcelHeaderWithContentModel<T>> headerWithContentModels, IExcelModel style)
        {
            var entries = headerWithContentModels.Select(m => m.Header).ToList();
            var r = ExcelRowModels.WithEntries(entries);
            r.EveryCell = style;
            return r;
        }
    }
}
