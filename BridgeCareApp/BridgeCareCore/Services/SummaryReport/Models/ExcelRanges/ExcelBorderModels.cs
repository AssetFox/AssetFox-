using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml.Style;

namespace BridgeCareCore.Services.SummaryReport.Models
{
    public static class ExcelBorderModels
    {
        public static ExcelBorderModel Thin
            => new ExcelBorderModel
            {
                BorderStyle = ExcelBorderStyle.Thin,
            };
    }
}
