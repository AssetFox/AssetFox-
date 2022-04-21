using System;

namespace BridgeCareCore.Services.SummaryReport.Models.Tables
{
    public class ExcelHeaderWithContentModel<TData>
    {
        public IExcelModel Header { get; set; }
        public Func<TData, IExcelModel> Content { get; set; }   
    }
}
