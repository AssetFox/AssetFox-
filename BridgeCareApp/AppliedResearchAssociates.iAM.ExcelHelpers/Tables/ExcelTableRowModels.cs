using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.ExcelHelpers.Tables;

namespace AppliedResearchAssociates.iAM.ExcelHelpers
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
