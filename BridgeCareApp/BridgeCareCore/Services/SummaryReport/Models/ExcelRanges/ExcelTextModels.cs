using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BridgeCareCore.Services.SummaryReport.Models
{
    public static class ExcelTextModels
    {
        public static ExcelTextModel Text(string text)
            => new ExcelTextModel
            {
                Text = text
            };
    }
}
