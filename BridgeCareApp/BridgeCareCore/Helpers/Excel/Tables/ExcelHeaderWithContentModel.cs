using System;

namespace BridgeCareCore.Helpers.Excel.Tables
{
    public class ExcelHeaderWithContentModel<TData>
    {
        public IExcelModel Header { get; set; }
        public Func<TData, IExcelModel> Content { get; set; }   
    }
}
