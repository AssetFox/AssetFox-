using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BridgeCareCore.Services.SummaryReport.Models
{
    public static class ExcelBoldModels
    {
        public static ExcelBoldModel Bold
            => new ExcelBoldModel
            {
                Bold = true,
            };

        public static ExcelBoldModel NotBold
            => new ExcelBoldModel
            {
                Bold = false,
            };
    }
}
