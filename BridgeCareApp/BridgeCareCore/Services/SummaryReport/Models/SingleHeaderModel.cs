using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml.Style;

namespace BridgeCareCore.Services.SummaryReport.Models
{
    public class ExcelGridValueModel<TDataSource>
    {
        public Func<TDataSource, int, int, IExcelModel> GetContent;
    }
}
