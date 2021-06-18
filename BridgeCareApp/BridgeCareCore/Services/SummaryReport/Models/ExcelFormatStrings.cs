using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BridgeCareCore.Services.SummaryReport.Models
{
    public static class ExcelFormatStrings
    {
        public const string Currency = "_-$* #,##0.00_-;_-$* #,##0.00_-;_-$* \"-\"??_-;_-@_-";
        public const string NegativeCurrency = "_-$* #,##0_-;$* (#,##0)_-;_-$* \"-\"??_-;_-@_-";
    }
}
