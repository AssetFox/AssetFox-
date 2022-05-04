using System.Collections.Generic;
using System.Linq;
using BridgeCareCore.Helpers.Excel.Tables;

namespace BridgeCareCore.Helpers.Excel
{
    public static class ExcelTableRowModels
    {
        public static ExcelRowModel ContentRow<T>(
            IEnumerable<ExcelHeaderWithContentModel<T>> headerWithContentModels,
            T data)
        {
            var entries = headerWithContentModels.Select(m => m.Content(data)).ToList();
            var returnValue = ExcelRowModels.WithEntries(entries);
            return returnValue;
        }

        public static ExcelRowModel HeaderRow<T>(IEnumerable<ExcelHeaderWithContentModel<T>> headerWithContentModels, IExcelModel style)
        {
            var entries = headerWithContentModels.Select(m => m.Header).ToList();
            var returnValue = ExcelRowModels.WithEntries(entries);
            returnValue.EveryCell = style;
            return returnValue;
        }
    }
}
