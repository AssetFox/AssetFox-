using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BridgeCareCore.Services.SummaryReport.Models
{
    public static class ExcelRowModels
    {
        public static ExcelRowModel WithEntries(params IExcelModel[] entries)
            => new ExcelRowModel
            {
                Values = entries.ToList(),
            };
    }
}
